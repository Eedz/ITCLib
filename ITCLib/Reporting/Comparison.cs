using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace ITCLib
{
    public enum HScheme { Sequential = 1, AcrossCountry = 2 }
    public enum HStyle { Classic = 1, TrackedChanges = 2 }
    /// <summary>
    /// This class compares two Survey objects. One survey is considered the 'primary' survey against which the other survey will be compared.
    /// 
    /// TODO test with the primary and Qnum survey being the same survey
    /// </summary>
    public class Comparison
    {
        public ReportSurvey PrimarySurvey { get; set; }
        public ReportSurvey OtherSurvey { get; set; }       // this survey's question list will be altered


        public bool SelfCompare { get; set ; }              // true if we are comparing a survey to itself at a different date
        public bool HidePrimary { get ; set ; }             // true if the primary survey should be hidden in the final report
        public bool ShowDeletedFields { get; set; }         // true if fields that only exist in primary survey are colored blue
        public bool ShowDeletedQuestions { get ; set ; }    // true if primary only questions should be included in the report
        public bool ReInsertDeletions { get ; set ; }       // true if primary only questions should be inserted as close as possible to their original position
        public bool HideIdenticalWordings { get ; set ; }   // true if identical wordings should be hidden, resulting in only the differences being shown    
        public bool ShowOrderChanges { get ; set ; }        // true if varnames that have moved should be highlighted in their own color  
        public bool BeforeAfterReport { get ; set ; }       // true if the report should have 3 columns, Before, Marked and After
        public bool IgnoreSimilarWords { get ; set ; }      // true if similar words should not be considered different (this is for words like labour/labor)       
        public bool ConvertTrackedChanges { get ; set ; }   // true if artificial tracked changes should be converted into actual tracked changes in word
        public bool MatchOnRename { get; set ; }            // true if varnames should be matched up on previous names
        // highlight options
        public bool Highlight { get ; set ; }               // true if differences should be highlighted
        public HStyle HighlightStyle { get ; set ; }        // determines the type of highlighting to use (Classic or Tracked Changes)
        public HScheme HighlightScheme { get ; set ; }      // determines how deleted questions are highlighted
        public bool HighlightNR { get ; set ; }             // true if non-response options should be highlighted
        public bool HybridHighlight { get ; set ; }         // true if differences in wordings should have both Green and Tracked Changes coloring
        // order comparison
        public bool IncludeWordings { get ; set; }          // true if order comparison is to include wordings
        public bool BySection { get; set ; }                // true if order comparison should break the report into sections

        public bool HideIdenticalQuestions { get ; set ; }  // true if identical questions should be removed from the report
        

        // IDEA implement
        //public List<string> fieldList; // this list contains fields that should be compared     

        public Comparison()
        {
            ShowDeletedFields = true;
            ShowDeletedQuestions = true;
            ReInsertDeletions = true;

            IgnoreSimilarWords = true;

            Highlight = true;
            HighlightStyle = HStyle.Classic;
            HighlightScheme = HScheme.Sequential;
            HighlightNR = true;
           
        }

        public Comparison(ReportSurvey p, ReportSurvey o)
        {
            PrimarySurvey = p;
            OtherSurvey = o;
            
            ShowDeletedFields = true;
            ShowDeletedQuestions = true;
            ReInsertDeletions = true;

            IgnoreSimilarWords = true;

            Highlight = true;
            HighlightStyle = HStyle.Classic;
            HighlightScheme = HScheme.Sequential;
            HighlightNR = true;
           
        }

        // Use VarName as the basis for comparison (actually uses refVarName)
        public void CompareByVarName()
        {
            // Compare the English survey content
            if (Highlight)
                CompareSurveyTables();

            // Compare translation records if at least one language was selected for each survey TODO see if this can be included in regular comparison methods
            if (Highlight)
                if (PrimarySurvey.TransFields.Count >= 1 && OtherSurvey.TransFields.Count >= 1)
                    CompareTranslations();


        }

        /// <summary>
        /// Compares rows in this Surveys' DataTable objects. The non-primary data table will contain highlighting tags where it differs from the primary data table.
        /// For rows that exist in only one of the tables, either color the whole question (Sequential), or just the VarName field (Across Country).
        /// </summary>
        private void CompareSurveyTables()
        {
            
            ProcessCommonQuestions();

            ProcessPrimaryOnlyQuestions();

            ProcessOtherOnlyQuestions();
            

            //If we want to include those questions that are only in the primary survey, process them now
            //They will either be included at the end of the report, or inserted into their proper places
            if (ShowDeletedQuestions)
            {
                // if we are not re-inserting them, add heading for unmatched questions
                if (!ReInsertDeletions)
                    AddUnmatchedQuestionsHeading();
            }
            else
            {
                // need to take out primary only questions from the survey
            }

        }

        /// <summary>
        /// For any common rows, compare the wordings and color them if they are different or missing.
        /// </summary>
        private void ProcessCommonQuestions()
        {
            // for every refVarName shared by the 2 surveys, compare wordings
            List<SurveyQuestion> intersection = OtherSurvey.Questions.Intersect(PrimarySurvey.Questions, new SurveyQuestionComparer()).ToList();
            SurveyQuestion found;
            foreach (SurveyQuestion sq in intersection)
            {
                found = PrimarySurvey.Questions.Single(x => x.refVarName.Equals(sq.refVarName)); // find the question in the primary survey

                sq.PreP = CompareWordings(found.PreP, sq.PreP);
                sq.PreI = CompareWordings(found.PreI, sq.PreI);
                sq.PreA = CompareWordings(found.PreA, sq.PreA);
                sq.LitQ = CompareWordings(found.LitQ, sq.LitQ);
                sq.PstI = CompareWordings(found.PstI, sq.PstI);
                sq.PstP = CompareWordings(found.PstP, sq.PstP);
                sq.RespOptions = CompareWordings(found.RespOptions, sq.RespOptions);
                sq.NRCodes = CompareWordings(found.NRCodes, sq.NRCodes);
            }
        }

        /// <summary>
        /// For any row that only appears in the primary survey, we need to either move it to the end of the survey, or re-insert it into its original position.
        /// </summary>
        private void ProcessPrimaryOnlyQuestions()
        {
            if (ShowDeletedQuestions)
            {
                // for every refVarName in primary only, add to Qnum survey and highlight blue
                List<SurveyQuestion> primeOnly = PrimarySurvey.Questions.Except(OtherSurvey.Questions, new SurveyQuestionComparer()).ToList();
                SurveyQuestion toAdd;
                foreach (SurveyQuestion sq in primeOnly)
                {

                    if (PrimarySurvey.Qnum)
                        toAdd = sq;
                    else
                        toAdd = sq.Copy();

                    if (HighlightScheme == HScheme.Sequential)
                    {
                        toAdd.VarName = "[s][t]" + toAdd.VarName + "[/t][/s]";

                        if (ReInsertDeletions)
                        {
                            if (!string.IsNullOrEmpty(toAdd.PreP)) toAdd.PreP = "[s][t]" + toAdd.PreP + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.PreI)) toAdd.PreI = "[s][t]" + toAdd.PreI + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.PreA)) toAdd.PreA = "[s][t]" + toAdd.PreA + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.LitQ)) toAdd.LitQ = "[s][t]" + toAdd.LitQ + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.PstI)) toAdd.PstI = "[s][t]" + toAdd.PstI + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.PstP)) toAdd.PstP = "[s][t]" + toAdd.PstP + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.RespOptions)) toAdd.RespOptions = "[s][t]" + toAdd.RespOptions + "[/t][/s]";
                            if (!string.IsNullOrEmpty(toAdd.NRCodes)) toAdd.NRCodes = "[s][t]" + toAdd.NRCodes + "[/t][/s]";
                        }
                        else
                        {
                            toAdd.PreP = "";
                            toAdd.PreI = "";
                            toAdd.PreA = "";
                            toAdd.LitQ = "";
                            toAdd.PstI = "";
                            toAdd.PstP = "";
                            toAdd.RespOptions = "";
                            toAdd.NRCodes = "";
                        }
                    }
                    else if (HighlightScheme == HScheme.AcrossCountry)
                    {
                        toAdd.VarName = "[s][t]" + toAdd.VarName + "[/t][/s]";
                        toAdd.Qnum = "[s][t]" + toAdd.Qnum + "[/t][/s]";
                    }

                    if (ReInsertDeletions)
                    {
                        if (OtherSurvey.Qnum)
                            OtherSurvey.Questions.Add(toAdd);
                        // TODO find last common question, renumber etc. also color qnum blue here?
                    }
                    else
                    {

                        // add to bottom of Qnum survey (if Qnum<>Primary)
                        if (OtherSurvey.Qnum)
                        {
                            toAdd.Qnum = "z" + toAdd.Qnum;
                            OtherSurvey.Questions.Add(toAdd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add highlighting to the non-primary survey.
        /// For any row not in the primary survey, color it yellow.
        /// </summary>
        private void ProcessOtherOnlyQuestions()
        {

            // for every refVarName in non-primary only, add to Qnum survey highlight yellow
            List<SurveyQuestion> otherOnly = OtherSurvey.Questions.Except(PrimarySurvey.Questions, new SurveyQuestionComparer()).ToList();

            foreach (SurveyQuestion sq in otherOnly)
            {
                if (HighlightScheme == HScheme.Sequential)
                {
                    sq.VarName = "[yellow]" + sq.VarName + "[/yellow]";

                    if (!string.IsNullOrEmpty(sq.PreP)) sq.PreP = "[yellow]" + sq.PreP + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.PreI)) sq.PreI = "[yellow]" + sq.PreI + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.PreA)) sq.PreA = "[yellow]" + sq.PreA + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.LitQ)) sq.LitQ = "[yellow]" + sq.LitQ + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.PstI)) sq.PstI = "[yellow]" + sq.PstI + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.PstP)) sq.PstP = "[yellow]" + sq.PstP + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.RespOptions)) sq.RespOptions = "[yellow]" + sq.RespOptions + "[/yellow]";
                    if (!string.IsNullOrEmpty(sq.NRCodes)) sq.NRCodes = "[yellow]" + sq.NRCodes + "[/yellow]";
                }
                else if (HighlightScheme == HScheme.AcrossCountry)
                {
                    sq.VarName = "[yellow]" + sq.VarName + "[/yellow]";
                }



            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deletedQs"></param>
        public void RenumberDeletions(List<SurveyQuestion> deletedQs)
        {
            // for each row in deletedQs where the qnum starts with z, 
            // we need to find the last common varname between the 2 tables, set the qnum of the row to that common row's qnum + the z-qnum
            var foundQs = deletedQs.FindAll(x => x.Qnum.StartsWith("z"));
            string varname;
            string previousvar;
            string previousqnum = "";
            foreach (SurveyQuestion r in foundQs)
            {
                varname = r.VarName;
                varname = varname.Replace("[s][t]", "");
                varname = varname.Replace("[/t][/s]", "");

                for (int i = 0; i < PrimarySurvey.Questions.Count; i++)
                {
                    if (PrimarySurvey.Questions[i].VarName.Equals(varname))
                    {
                        if (i == 0)
                        {
                            previousqnum = "000";
                        }
                        else
                        {
                            previousvar = PrimarySurvey.Questions[i - 1].VarName;
                            previousqnum = GetPreviousCommonVar(varname);
                        }
                        break;
                    }
                }
                r.Qnum = previousqnum + r.Qnum;
                
            }
        }

        /// <summary>
        /// Add a row in both survey tables that acts as a heading for the unmatched questions. This heading will appear at the end of the list of matched questions, before the
        /// list of unmatched questions.
        /// </summary>
        private void AddUnmatchedQuestionsHeading()
        {
            OtherSurvey.Questions.Add(new SurveyQuestion
            {
                ID = -1,
                VarName = "ZZ999",
                Qnum = "z000",
                //PreP = new Wording(0,"Unmatched Questions"),
                TableFormat = false,
                CorrectedFlag = false,
            });

            PrimarySurvey.Questions.Add(new SurveyQuestion
            {
                ID = -1,
                VarName = "ZZ999",
                Qnum = "z000",
                //PreP = new Wording(0,"Unmatched Questions"),
                TableFormat = false,
                CorrectedFlag = false,
            });
        }

        /// <summary>
        /// Returns the VarName of the last common VarName between 2 datatables, starting from a specified VarName.
        /// </summary>
        /// <param name="varname">The starting point from which we will look back to find the first common VarName.</param>
        /// <returns>Returns the Qnum of the first common VarName that occurs before the specified VarName. If there are no common VarNmaes before this VarName, returns '000'.</returns>
        private string GetPreviousCommonVar(string varname)
        {
            string previousQnum = "";
            string prev = "";
            string curr = "";
           
            // TODO sort questions by Qnum
            //PrimarySurvey.Questions.Sort((x, y) => x.Qnum.CompareTo(y.Qnum));
           // OtherSurvey.Questions.Sort((x, y) => x.Qnum.CompareTo(y.Qnum));

            for (int i = PrimarySurvey.Questions.Count - 1; i >= 0; i--)
            {
                curr = PrimarySurvey.Questions[i].VarName;
                curr = curr.Replace("[s][t]", "");
                curr = curr.Replace("[/t][/s]", "");

                if (curr.Equals(varname))
                {
                    // get the previous var in dt1
                    if (i == 0)
                    {
                        prev = "";
                        break;
                    }
                    else
                    {
                        prev = PrimarySurvey.Questions[i - 1].VarName;
                        prev = prev.Replace("[s][t]", "");
                        prev = prev.Replace("[/t][/s]", "");
                        // check if it exists in dt2
                        var foundRow = OtherSurvey.Questions.Single(x => x.VarName == prev);


                        if (foundRow != null)
                        {
                            previousQnum = foundRow.Qnum;
                            break;
                        }
                        else
                        {
                            varname = PrimarySurvey.Questions[i - 1].VarName;
                        }
                    }
                }
            }

            if (prev.Equals(""))
                previousQnum = "000";


            return previousQnum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryWording"></param>
        /// <param name="otherWording"></param>
        /// <returns></returns>
        public string CompareWordings(string primaryWording, string otherWording)
        {
 
            if (IsWordingEqual(primaryWording, otherWording, true, IgnoreSimilarWords))
            {
                if (HideIdenticalWordings)  otherWording = ""; 
            }
            else
            {
                if (otherWording.Equals("") && ShowDeletedFields)
                {
                    otherWording = "[t][s]" + primaryWording + "[/t][/s]";
                }
                else if (primaryWording.Equals(""))
                {
                    otherWording = "[yellow]" + otherWording + "[/yellow]";
                }
                else
                {
                    if (HybridHighlight)
                    {
                        // tracked changes  and brightgreen
                        otherWording = "[brightgreen]" + otherWording + "[/brightgreen]";
                    }
                    else
                    {
                        otherWording = "[brightgreen]" + otherWording + "[/brightgreen]";
                    }
                }
            }
            return otherWording;
        }

        /// <summary>
        /// For each translation in the non-primary survey, compare it to the same language in the primary survey.
        /// </summary>
        private void CompareTranslations()
        {
            Translation p;

            SurveyQuestion sqPrime;
            foreach (string t in OtherSurvey.TransFields)
            {
                // continue to next language if primary does not contain this language
                if (!PrimarySurvey.TransFields.Contains(t))
                    continue;

                foreach (SurveyQuestion sqOther in OtherSurvey.Questions)
                {
                    sqPrime = PrimarySurvey.Questions.Single(x => x.refVarName.Equals(sqOther.refVarName)); // find the question in the primary survey

                    if (sqPrime == null)
                    {
                        // if no matching question is found in primary, this question is unique to other
                        if (HighlightScheme == HScheme.Sequential)
                        {

                            //sqOther.VarName = "[yellow]" + sqOther.VarName + "[/yellow]";
                            p = sqOther.GetTranslation(t);
                            if (p!=null)
                                p.TranslationText = "[yellow]" + p.TranslationText + "[/yellow]";



                        }
                        else if (HighlightScheme == HScheme.AcrossCountry)
                        {
                            //sqOther.VarName = "[yellow]" + sqOther.VarName + "[/yellow]";
                        }
                    }
                    else
                    {
                        p = sqOther.GetTranslation(t);
                        if (p!=null)
                            p.TranslationText = CompareWordings(sqPrime.GetTranslationText(t), sqOther.GetTranslationText(t));
                    }
                    
                }
            }
            
        }


        /// <summary>
        /// TODO create a list or dictionary for the alternate spelling words, remove need for database connection
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="ignorePunctuation"></param>
        /// <param name="ignoreSimilarWords"></param>
        /// <returns></returns>
        private bool IsWordingEqual (string str1, string str2, bool ignorePunctuation = true, bool ignoreSimilarWords = true)
        {
            DataTable similarWords;
            string[] words;
            if (str1.Equals("") && str2.Equals(""))
                return true;

            // ignore similar words
            if (ignoreSimilarWords)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
                {
                    using (SqlDataAdapter sql = new SqlDataAdapter("SELECT * FROM qryAlternateSpelling", conn))
                    {
                        similarWords = new DataTable();
                        sql.Fill(similarWords);

                        foreach (DataRow r in similarWords.Rows)
                        {
                            words = r["word"].ToString().Split(',');

                            for (int i = 0; i < words.Length; i++)
                            {
                                words[i] = words[i].Trim(' ');

                                str1 = str1.Replace(words[i], words[0]);
                                str2 = str2.Replace(words[i], words[0]);
                            }
                        }
                    }
                }
            }

            // remove tags
            str1 = Utilities.RemoveTags(str1);
            str2 = Utilities.RemoveTags(str2);

            // ignore punctuation
            if (ignorePunctuation)
            {
                str1 = str1.Replace("&lt;", "<");
                str2 = str2.Replace("&lt;", "<");

                str1 = str1.Replace("&gt;", ">");
                str2 = str2.Replace("&gt;", ">");

                str1 = Utilities.StripChars(str1, "0123456789 abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ <>=");
                str2 = Utilities.StripChars(str2, "0123456789 abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ <>=");
            }

            // remove line breaks
            str1 = str1.Replace("\r", string.Empty);
            str1 = str1.Replace("\n", string.Empty);

            str2 = str2.Replace("\r", string.Empty);
            str2 = str2.Replace("\n", string.Empty);


            // remove spaces
            while (str1.IndexOf(' ') > 0)
                str1 = str1.Replace(" ", string.Empty);

            while (str2.IndexOf(' ') > 0)
                str2 = str2.Replace(" ", string.Empty);

            if (!str1.Equals(str2))
                return false;
            else
                return true;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            PropertyInfo[] _PropertyInfos = null;
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }

        
    }
}
