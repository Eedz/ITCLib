using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace ITCLib
{
    // TODO show order changes
    // TODO before after report

    // TODO hide identical questions (translation part)

    // TODO hide identical wordings and don't highlight NR are incompatible

    /// <summary>
    /// Compares two ReportSurvey objects. One survey is considered the 'primary' survey against which the other survey will be compared.
    /// </summary>
    public class Comparison
    {
        public ReportSurvey PrimarySurvey { get; set; }
        public ReportSurvey OtherSurvey { get; set; }       // this survey's question list will be altered

  
        private List<SurveyQuestion> Intersection { get; set; }
        private List<SurveyQuestion> RenamedMatchesOther { get; set; }
        private List<SurveyQuestion> RenamedMatchesPrimary { get; set; }
        private List<SurveyQuestion> PrimeOnly { get; set; }
        private List<SurveyQuestion> OtherOnly { get; set; }

        public string[][] SimilarWords { get; set; }

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
       

        public bool HideIdenticalQuestions { get ; set ; }  // true if identical questions should be removed from the report
        
        // IDEA implement
        //public List<string> fieldList; // this list contains fields that should be compared     

        public Comparison()
        {

            SimilarWords = new string[1][];

            ShowDeletedFields = true;
            ShowDeletedQuestions = true;
            ReInsertDeletions = true;

            IgnoreSimilarWords = true;

            Highlight = true;
            HighlightStyle = HStyle.Classic;
            HighlightScheme = HScheme.Sequential;
            HighlightNR = true;

           
            Intersection = new List<SurveyQuestion>();
            PrimeOnly = new List<SurveyQuestion>();
            OtherOnly = new List<SurveyQuestion>();
        }

        public Comparison(ReportSurvey p, ReportSurvey o)
        {
            PrimarySurvey = p;
            OtherSurvey = o;

            SimilarWords = new string[1][];

            ShowDeletedFields = true;
            ShowDeletedQuestions = true;
            ReInsertDeletions = true;

            IgnoreSimilarWords = true;

            Highlight = true;
            HighlightStyle = HStyle.Classic;
            HighlightScheme = HScheme.Sequential;
            HighlightNR = true;

         
            Intersection = new List<SurveyQuestion>();
            PrimeOnly = new List<SurveyQuestion>();
            OtherOnly = new List<SurveyQuestion>();
        }

        // Use VarName as the basis for comparison (actually uses refVarName)
        public void CompareByVarName()
        {
            // first check if this is a self comparison
            if (PrimarySurvey.SurveyCode == OtherSurvey.SurveyCode)
                SelfCompare = true;
            else
                SelfCompare = false;

            // Compare the English survey content
            if (Highlight)
                CompareSurveyTables();

            // Compare translation records if at least one language was selected for each survey TODO see if this can be included in regular comparison methods
            if (Highlight)
                if (PrimarySurvey.TransFields.Count >= 1 && OtherSurvey.TransFields.Count >= 1)
                    CompareTranslations();


        }

        /// <summary>
        /// Compares items in the Question lists of each survey. The non-primary questions will contain highlighting tags where it differs from the primary questions.
        /// For rows that exist in only one of the lists, either color the whole question (Sequential), or just the VarName field (Across Country).
        /// </summary>
        private void CompareSurveyTables()
        {
            // first remove any identical questions if needed
            if (HideIdenticalQuestions)
                ProcessExactMatches();

            // highlight any differences among the same refVarNames between the 2 surveys
            ProcessCommonQuestions();

            // highlight anything that only appears in the other survey
            ProcessOtherOnlyQuestions();

            // highlight anything that only appears in the primary survey 
            // if we want to include those questions that are only in the primary survey, process them now
            // they will either be included at the end of the report, or inserted into their proper places
            if (ShowDeletedQuestions)
            {
                ProcessPrimaryOnlyQuestions();

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
           
            Intersection = OtherSurvey.Questions.Intersect(PrimarySurvey.Questions, new SurveyQuestionComparer()).ToList();

            SurveyQuestion found;
            foreach (SurveyQuestion sq in Intersection)
            {
                found = PrimarySurvey.Questions.Single(x => x.VarName.RefVarName.ToLower().Equals(sq.VarName.RefVarName.ToLower())); // find the question in the primary survey

                if (HighlightStyle == HStyle.Classic)
                {
                    sq.PreP = CompareWordings(found.PreP, sq.PreP);
                    sq.PreI = CompareWordings(found.PreI, sq.PreI);
                    sq.PreA = CompareWordings(found.PreA, sq.PreA);
                    sq.LitQ = CompareWordings(found.LitQ, sq.LitQ);
                    sq.PstI = CompareWordings(found.PstI, sq.PstI);
                    sq.PstP = CompareWordings(found.PstP, sq.PstP);
                    sq.RespOptions = CompareWordings(found.RespOptions, sq.RespOptions);
                    if (HighlightNR) sq.NRCodes = CompareWordings(found.NRCodes, sq.NRCodes);
                }
                else
                {
                    sq.PreP = ChangeHighlighter.HighlightChanges(found.PreP, sq.PreP, CompareType.ByWords);
                    sq.PreI = ChangeHighlighter.HighlightChanges(found.PreI, sq.PreI, CompareType.ByWords);
                    sq.PreA = ChangeHighlighter.HighlightChanges(found.PreA, sq.PreA, CompareType.ByWords);
                    sq.LitQ = ChangeHighlighter.HighlightChanges(found.LitQ, sq.LitQ, CompareType.ByWords);
                    sq.PstI = ChangeHighlighter.HighlightChanges(found.PstI, sq.PstI, CompareType.ByWords);
                    sq.PstP = ChangeHighlighter.HighlightChanges(found.PstP, sq.PstP, CompareType.ByWords);
                    sq.RespOptions = ChangeHighlighter.HighlightChanges(found.RespOptions, sq.RespOptions, CompareType.ByWords);
                    if (HighlightNR) sq.NRCodes = ChangeHighlighter.HighlightChanges(found.NRCodes, sq.NRCodes, CompareType.ByWords);
                }
                
            }

            
        }

        /// <summary>
        /// Returns true if both the English and Translated wordings are identical. TODO make sure translations are identical
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns></returns>
        private bool AreIdenticalQuestions(SurveyQuestion sq1, SurveyQuestion sq2)
        {
            bool wordingsMatch = AreWordingsEqual(sq1, sq2);
            bool translationsMatch = AreTranslationsEqual(sq1, sq2);

            return wordingsMatch && translationsMatch;
        }

        /// <summary>
        /// Compares each wording field between 2 questions and determines if they are equal.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns>True if all wording fields are equal.</returns>
        private bool AreWordingsEqual(SurveyQuestion sq1, SurveyQuestion sq2)
        {
            return (sq1.PreP == sq2.PreP) &&
                    (sq1.PreI == sq2.PreI) &&
                    (sq1.PreA == sq2.PreA) &&
                    (sq1.LitQ == sq2.LitQ) &&
                    (sq1.PstI == sq2.PstI) &&
                    (sq1.PstP == sq2.PstP) &&
                    (sq1.NRCodes == sq2.NRCodes) &&
                    (sq1.RespOptions == sq2.RespOptions);
        }

        /// <summary>
        /// Compares each translation field of the same language between 2 questions and determines if they are equal.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns></returns>
        private bool AreTranslationsEqual(SurveyQuestion sq1, SurveyQuestion sq2)
        {
     
            foreach (Translation t1 in sq1.Translations)
            {
                Translation t2 = sq2.GetTranslation(t1.Language);
                
                if (t2 == null)
                {
                    return false;
                }
                else
                {
                    if (t1.TranslationText == t2.TranslationText)
                        return true;
                }
                
            }


            return false;
        }

        /// <summary>
        /// Removes any questions that are exactly the same in both English and Translation.
        /// </summary>
        private void ProcessExactMatches()
        {
            // for every refVarName shared by the 2 surveys, compare wordings
            Intersection = OtherSurvey.Questions.Intersect(PrimarySurvey.Questions, new SurveyQuestionComparer()).ToList();
            SurveyQuestion found; // the same question in the primary survey

            foreach (SurveyQuestion sq in Intersection)
            {
                found = PrimarySurvey.Questions.Single(x => x.VarName.RefVarName.ToLower().Equals(sq.VarName.RefVarName.ToLower())); // find the question in the primary survey

                if (AreIdenticalQuestions(sq, found))
                {
                    OtherSurvey.RemoveQuestion(sq);
                    PrimarySurvey.RemoveQuestion(found);
                }
            }
        }

        /// <summary>
        /// For any row that only appears in the primary survey, we need to either move it to the end of the survey, or re-insert it into its original position.
        /// </summary>
        private void ProcessPrimaryOnlyQuestions()
        {

            // for every refVarName in primary only, add to Qnum survey and highlight blue
            PrimeOnly = PrimarySurvey.Questions.Except(OtherSurvey.Questions, new SurveyQuestionComparer()).ToList();

            SurveyQuestion toAdd;
            string highlightStart;
            string highlightEnd;

            if (HighlightStyle == HStyle.Classic)
            {
                highlightStart = "[s][t]";
                highlightEnd = "[/t][/s]";
            }
            else
            {
                highlightStart = "[pinkfill]<Font Color=Red>";
                highlightEnd = "</Font>";
            }

            // if the primary survey is not the qnum survey, then the primary only questions need to be added to the Qnum survey and highlighted/struckout
            if (!PrimarySurvey.Qnum)
            {
                foreach (SurveyQuestion sq in PrimeOnly)
                {
                    toAdd = sq.Copy();

                    // highlight based on scheme
                    if (HighlightScheme == HScheme.Sequential)
                    {
                      //  toAdd.Qnum = "[s][t]" + toAdd.Qnum + "[/t][/s]";
                        toAdd.VarName.FullVarName = highlightStart + toAdd.VarName.FullVarName + highlightEnd;
       
                    }
                    else if (HighlightScheme == HScheme.AcrossCountry)
                    {
                        toAdd.VarName.FullVarName = highlightStart + toAdd.VarName.FullVarName + highlightEnd;
                        //toAdd.Qnum = "[s][t]" + toAdd.Qnum + "[/t][/s]";
                    }

                    // re-inserted questions are highlighted/struckout and renumbered, otherwise they are put at the bottom (Qnum starts with z)
                    if (ReInsertDeletions)
                    {

                        if (!string.IsNullOrEmpty(toAdd.PreP)) toAdd.PreP = highlightStart + toAdd.PreP + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.PreI)) toAdd.PreI = highlightStart + toAdd.PreI + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.PreA)) toAdd.PreA = highlightStart + toAdd.PreA + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.LitQ)) toAdd.LitQ = highlightStart + toAdd.LitQ + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.PstI)) toAdd.PstI = highlightStart + toAdd.PstI + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.PstP)) toAdd.PstP = highlightStart + toAdd.PstP + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.RespOptions)) toAdd.RespOptions = highlightStart + toAdd.RespOptions + highlightEnd;
                        if (!string.IsNullOrEmpty(toAdd.NRCodes)) toAdd.NRCodes = highlightStart + toAdd.NRCodes + highlightEnd;

                        RenumberDeletion(toAdd, PrimarySurvey, OtherSurvey);
                        OtherSurvey.AddQuestion(toAdd);
                    }
                    else
                    {
                        //toAdd.Qnum = "z" + toAdd.Qnum;
                        sq.Qnum = "z" + sq.Qnum;
                    }
                    

                }
            }
            else
            {

            }

        }

       

        /// <summary>
        /// Add highlighting to the non-primary survey.
        /// For any row not in the primary survey, color it yellow (classic) or blue (TC).
        /// </summary>
        private void ProcessOtherOnlyQuestions()
        {

            // for every refVarName in non-primary only, add to Qnum survey highlight yellow
            OtherOnly = OtherSurvey.Questions.Except(PrimarySurvey.Questions, new SurveyQuestionComparer()).ToList();

            string highlightStart;
            string highlightEnd;

            if (HighlightStyle == HStyle.Classic)
            {
                highlightStart = "[yellow]";
                highlightEnd = "[/yellow]";
            }
            else
            {
                highlightStart = "[bluefill]<Font Color=Blue>";
                highlightEnd = "</Font>";
            }

            if (PrimarySurvey.Qnum)
            {
                foreach (SurveyQuestion sq in OtherOnly)
                {
                    if (HighlightScheme == HScheme.Sequential)
                    {
                        sq.VarName.FullVarName = highlightStart + sq.VarName.FullVarName + highlightEnd;

                        if (!string.IsNullOrEmpty(sq.PreP)) sq.PreP = highlightStart + sq.PreP + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PreI)) sq.PreI = highlightStart + sq.PreI + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PreA)) sq.PreA = highlightStart + sq.PreA + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.LitQ)) sq.LitQ = highlightStart + sq.LitQ + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PstI)) sq.PstI = highlightStart + sq.PstI + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PstP)) sq.PstP = highlightStart + sq.PstP + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.RespOptions)) sq.RespOptions = highlightStart + sq.RespOptions + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.NRCodes)) sq.NRCodes = highlightStart + sq.NRCodes + highlightEnd;
                    }
                    else if (HighlightScheme == HScheme.AcrossCountry)
                    {
                        sq.VarName.FullVarName = highlightStart + sq.VarName.FullVarName + highlightEnd;
                    }

                    // need to find last common var and prepend its qnum to the var
                    RenumberDeletion(sq, OtherSurvey, PrimarySurvey);
                }
            }
            else
            {
                foreach (SurveyQuestion sq in OtherOnly)
                {
                    if (HighlightScheme == HScheme.Sequential)
                    {
                        sq.VarName.FullVarName = highlightStart + sq.VarName.FullVarName + highlightEnd;

                        if (!string.IsNullOrEmpty(sq.PreP)) sq.PreP = highlightStart + sq.PreP + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PreI)) sq.PreI = highlightStart + sq.PreI + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PreA)) sq.PreA = highlightStart + sq.PreA + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.LitQ)) sq.LitQ = highlightStart + sq.LitQ + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PstI)) sq.PstI = highlightStart + sq.PstI + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.PstP)) sq.PstP = highlightStart + sq.PstP + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.RespOptions)) sq.RespOptions = highlightStart + sq.RespOptions + highlightEnd;
                        if (!string.IsNullOrEmpty(sq.NRCodes)) sq.NRCodes = highlightStart + sq.NRCodes + highlightEnd;
                    }
                    else if (HighlightScheme == HScheme.AcrossCountry)
                    {
                        sq.VarName.FullVarName = highlightStart + sq.VarName.FullVarName + highlightEnd;
                    }
                }
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
                VarName = new VariableName("ZZ999"),
                Qnum = "z000",
                //PreP = new Wording(0,"Unmatched Questions"),
                PreP = "Umatched Questions",
                TableFormat = false,
                CorrectedFlag = false,
            });

            PrimarySurvey.Questions.Add(new SurveyQuestion
            {
                ID = -1,
                VarName = new VariableName("ZZ999"),
                Qnum = "z000",
                //PreP = new Wording(0,"Unmatched Questions"),
                PreP = "Umatched Questions",
                TableFormat = false,
                CorrectedFlag = false,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sq">The deleted question.</param>
        /// <param name="refSurvey">The survey that contains the deleted question.</param>
        public void RenumberDeletion(SurveyQuestion sq, ReportSurvey refSurvey, ReportSurvey otherSurvey)
        {
          
            // we need to find the last common varname between the 2 surveys, set the qnum of the row to that common row's qnum + the z-qnum
            
            string varname;
            string previousvar;
            string previousqnum = "";
           
            varname = sq.VarName.RefVarName;

            for (int i = 0; i < refSurvey.Questions.Count; i++)
            {
                
                if (refSurvey.Questions[i].VarName.RefVarName.Equals(varname))
                {
                    if (i == 0)
                    {
                        previousqnum = "000";
                    }
                    else
                    {
                        previousvar = refSurvey.Questions[i - 1].VarName.RefVarName;
                        previousqnum = GetPreviousCommonVar(varname, refSurvey, otherSurvey);
                    }
                    break;
                }
            }
            sq.Qnum = previousqnum + '^' + sq.Qnum;

            
        }


        

        /// <summary>
        /// Returns the VarName of the last common VarName between 2 datatables, starting from a specified VarName.
        /// </summary>
        /// <param name="varname">The starting point from which we will look back to find the first common VarName.</param>
        /// <returns>Returns the Qnum of the first common VarName that occurs before the specified VarName. If there are no common VarNmaes before this VarName, returns '000'.</returns>
        private string GetPreviousCommonVar(string varname, ReportSurvey refSurvey, ReportSurvey otherSurvey)
        {
            string previousQnum = "";
            string prev = "";
            string curr = "";

            refSurvey.Questions.ToList().Sort((x, y) => x.Qnum.CompareTo(y.Qnum));
            otherSurvey.Questions.ToList().Sort((x, y) => x.Qnum.CompareTo(y.Qnum));

            for (int i = refSurvey.Questions.Count - 1; i >= 0; i--)
            {
                curr = refSurvey.Questions[i].VarName.RefVarName;

                // get the previous var in refSurvey
                if (curr.Equals(varname))
                {
                    
                    if (i == 0)
                    {
                        prev = "";
                        break;
                    }
                    else
                    {
                        prev = refSurvey.Questions[i - 1].VarName.RefVarName;
                  
                        // check if it exists in otherSurvey
                        var foundRow = otherSurvey.Questions.SingleOrDefault(x => x.VarName.RefVarName == prev);


                        if (foundRow != null)
                        {
                            previousQnum = foundRow.Qnum;
                            break;
                        }
                        else
                        {
                            varname = refSurvey.Questions[i - 1].VarName.RefVarName;
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
                    otherWording = "[s][t]" + primaryWording + "[/t][/s]";
                }
                else if (primaryWording.Equals(""))
                {
                    otherWording = "[yellow]" + otherWording + "[/yellow]";
                }
                else
                {
                    if (HybridHighlight)
                    {
                        otherWording = "[brightgreen]" + ChangeHighlighter.HighlightChanges(primaryWording, otherWording, CompareType.ByWords) + "[/brightgreen]";
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
                    sqPrime = PrimarySurvey.Questions.SingleOrDefault(x => x.VarName.RefVarName.Equals(sqOther.VarName.RefVarName)); // find the question in the primary survey

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
        /// 
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="ignorePunctuation"></param>
        /// <param name="ignoreSimilarWords"></param>
        /// <returns></returns>
        private bool IsWordingEqual (string str1, string str2, bool ignorePunctuation = true, bool ignoreSimilarWords = true)
        {
          
         
            if (str1.Equals("") && str2.Equals(""))
                return true;

            // ignore similar words
            if (ignoreSimilarWords)
            {
                try
                {
                    for (int w = 0; w < SimilarWords.Length; w++)
                        for (int s = 0; s < SimilarWords[w].Length; s++)
                        {
                            str1 = str1.Replace(SimilarWords[w][s], SimilarWords[w][0]);
                            str2 = str2.Replace(SimilarWords[w][s], SimilarWords[w][0]);
                        }
                }catch (System.NullReferenceException)
                {
                    // SimilarWords not initialized
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
