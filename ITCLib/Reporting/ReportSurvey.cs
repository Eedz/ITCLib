using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ITCLib
{
    /// <summary>
    /// Represents a Survey that is to be used in a report. Additional properties are used to specify which parts of the survey are to be included.
    /// </summary>
    public class ReportSurvey : Survey
    {
        // report properties
        public int ID { get; set; }                         // unique id for report, NOT the database ID
        public List<SurveyQuestion> reportQuestions;        // formatted version of question list (contains highlighting etc.)

        public DateTime Backend { get ; set; }               // date of backup

        // question filters
        public int QRangeLow { get; set; }
        public int QRangeHigh { get; set; }
        public List<string> Prefixes { get; set; }
        public List<Heading> Headings { get ; set; }
        public List<VariableName> Varnames { get; set; }
        public List<ProductLabel> Products { get; set; }
        public bool RemoveOtherSpecify { get; set; }

        // comment filters
        public DateTime? CommentDate { get; set; }
        public List<Person> CommentAuthors { get; set; }
        public List<string> CommentSources { get; set; }
        public List<string> CommentFields { get; set; }     // comment types
        public string CommentText { get; set; }

        // translation languages
        public List<string> TransFields { get; set; }

        // standard field options
        public List<string> StdFields { get; }
        public List<string> StdFieldsChosen { get; set; }
        public List<string> RepeatedFields { get; set; }
        public RoutingStyle RoutingFormat { get; set; }
        public RoutingStyle TranslationRoutingFormat { get; set; }

        // additional info 
        public bool VarLabelCol { get; set; }
        public bool DomainLabelCol { get; set; }
        public bool TopicLabelCol { get; set; }
        public bool ContentLabelCol { get; set; }
        public bool ProductLabelCol { get; set; }
        public bool FilterCol { get; set; }
        public bool AltQnum2Col { get; set; }
        public bool AltQnum3Col { get; set; }
        public bool ShowQuestion { get; set; }

        // report attributes
        public bool Primary { get; set; }                   // true if this is the primary survey
        public bool Qnum { get; set; }                      // true if this is the qnum-defining survey
        public bool Corrected { get; set; }                 // true if this uses corrected wordings
        public bool Marked { get; set; }                    // true if the survey contains tracked changes (for 3-way report)

        public List<VarNameChange> VarChanges = new List<VarNameChange>();

        #region Constructors

        public ReportSurvey() :base()
        {
            reportQuestions = new List<SurveyQuestion>();
            Backend = DateTime.Today;
            
            Prefixes = new List<string>();
            Varnames = new List<VariableName>();
            Headings = new List<Heading>();
            Products = new List<ProductLabel>();

            //CommentDate = new DateTime(2000, 1, 1);
            CommentAuthors = new List<Person>();
            CommentSources = new List<string>();

            RepeatedFields = new List<string>();
            CommentFields = new List<string>();
            TransFields = new List<string>();

            StdFields = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            StdFieldsChosen = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            ShowQuestion = true;

            RoutingFormat = RoutingStyle.Normal;

            VarChanges = new List<VarNameChange>();
        }

        public ReportSurvey(string surveyCode) : base()
        {
            SurveyCode = surveyCode;

            reportQuestions = new List<SurveyQuestion>();
            Backend = DateTime.Today;

            Prefixes = new List<string>();
            Varnames = new List<VariableName>();
            Headings = new List<Heading>();
            Products = new List<ProductLabel>();

            //CommentDate = new DateTime(2000, 1, 1);
            CommentAuthors = new List<Person>();
            CommentSources = new List<string>();

            RepeatedFields = new List<string>();
            CommentFields = new List<string>();
            TransFields = new List<string>();

            ShowQuestion = true;

            StdFields = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            StdFieldsChosen = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            RoutingFormat = RoutingStyle.Normal;

            VarChanges = new List<VarNameChange>();
        }

        /// <summary>
        /// Constructor for copying a base version of a survey.
        /// </summary>
        /// <param name="s"></param>
        public ReportSurvey(Survey s)
        {
            // copy values from base 
            SID = s.SID;
            SurveyCode = s.SurveyCode;
            Title = s.Title;
            Languages = s.Languages;
            Group = s.Group;
            Mode = s.Mode;
            CountryCode = s.CountryCode;
            WebName = s.WebName;
            EnglishRouting = s.EnglishRouting;

            EssentialList = s.EssentialList;
            HasCorrectedWordings = s.HasCorrectedWordings;
            AddQuestions(s.Questions);
            CorrectedQuestions = s.CorrectedQuestions;
            reportQuestions = new List<SurveyQuestion>(s.Questions);

            ShowQuestion = true;

            // initialize derived properties
            Backend = DateTime.Today;

            Prefixes = new List<string>();
            Varnames = new List<VariableName>();
            Headings = new List<Heading>();
            Products = new List<ProductLabel>();

            //CommentDate =  new DateTime(2000, 1, 1);
            CommentAuthors = new List<Person>();
            CommentSources = new List<string>();

            RepeatedFields = new List<string>();
            CommentFields = new List<string>();
            TransFields = new List<string>();

            StdFields = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            StdFieldsChosen = new List<string>
            {
                "PreP",
                "PreI",
                "PreA",
                "LitQ",
                "RespOptions",
                "NRCodes",
                "PstI",
                "PstP"
            };

            RoutingFormat = RoutingStyle.Normal;

            VarChanges = new List<VarNameChange>();
        }

        #endregion

        #region Methods and Functions

        

        /// <summary>
        /// Returns a WHERE clause restricting records to selected question range, prefix list and/or varname list.
        /// </summary>
        /// <returns>String to be used right after the WHERE keyword.</returns>
        public string GetQuestionFilter()
        {
            string filter = "";

            filter = GetQRangeFilter();

            if (Prefixes != null && Prefixes.Count != 0) { filter += " AND SUBSTRING(VarName,1,2) IN ('" + string.Join("','", Prefixes) + "')"; }
            if (Varnames != null && Varnames.Count != 0) { filter += " AND (" + GetVarNameFilter() + ")"; }
            if (Headings != null && Headings.Count != 0) { filter += " AND (" + GetHeadingFilter() + ")"; }
            filter = Utilities.TrimString(filter, " AND ");
            return filter;
        }

        /// <summary>
        /// Returns a filter expression restricting the range of Qnums.
        /// </summary>
        /// <returns></returns>
        private string GetQRangeFilter()
        {
            string filter = "";
            if (QRangeLow == 0 && QRangeHigh == 0) { return ""; }
            if (QRangeLow <= QRangeHigh)
            {
                filter = "Qnum >= '" + QRangeLow.ToString().PadLeft(3, '0') + "' AND Qnum <= '" + QRangeHigh.ToString().PadLeft(3, '0') + "'";
            }
            return filter;
        }

        /// <summary>
        /// Returns a WHERE condition based on the chosen headings. TEST use first 3 digits of heading
        /// </summary>
        /// <returns></returns>
        private string GetHeadingFilter()
        {
            List<SurveyQuestion> raw;
            string currentVar;
            string filter = "";
            foreach (Heading h in Headings)
            {
                raw = Questions.Where(x => Int32.Parse(x.Qnum.Substring(0, 3)) > Int32.Parse(h.Qnum.Substring(0, 3))).ToList();

                filter += " OR Qnum = '" + h.Qnum + "' OR (Qnum >= '" + h.Qnum + "'";
                foreach (SurveyQuestion r in raw)
                {
                    currentVar = r.VarName.RefVarName;
                    // when we reach the next heading, add its qnum to the end of the filter expression
                    if (currentVar.StartsWith("Z"))
                    {
                        filter += " AND Qnum < '" + r.Qnum + "')";
                        break;
                    }
                }

            }
            filter = Utilities.TrimString(filter, " OR ");
            return filter;
        }

        private string GetVarNameFilter()
        {
            if (Varnames == null || Varnames.Count == 0)
                return "1=1";

            string filter = "VarName IN ('";
            foreach(VariableName v in Varnames)
            {
                filter += v.VarName + "','";
            }
            filter = Utilities.TrimString(filter, "','");
            filter += "')";
            return filter;
        }

        /// <summary>
        /// Remove repeated values from the wording fields (PreP, PreI, PreA, LitQ, PstI, Pstp, RespOptions, NRCodes) unless they are requested. 
        /// This applies only to series questions, which are questions whose Qnum ends in a letter.
        /// </summary>
        public void RemoveRepeats()
        {
            int mainQnum = 0;
            string currQnum = "";
            int currQnumInt = 0;
            bool firstRow = true;
            bool removeAll = false;
            SurveyQuestion refQ = null; // this object is the 'a' question

            // only try to remove repeats if there are more than 0 rows
            if (Questions.Count == 0) return;

            foreach (SurveyQuestion sq in Questions)
            {
                currQnum = sq.Qnum;

                if (currQnum.Contains("z") && currQnum.Length >= 6)
                {
                    int zPos = currQnum.IndexOf("z");

                    currQnum = currQnum.Substring(zPos + 1);
                }

                if (currQnum.Length != 4 && currQnum.Length != 5) { continue; }

                // get the integer value of the current qnum
                int.TryParse(currQnum.Substring(0, 3), out currQnumInt);

                // if this row is in table format, we need to remove all repeats, regardless of repeated designations
                if (sq.TableFormat)
                    removeAll = true;
                else
                    removeAll = false;

                // if this is a non-series row, the first member of a series, the first row in the report, or a new Qnum, make this row the reference row
                if (currQnum.Length == 3 || (currQnum.Length == 3 && currQnum.EndsWith("a")) || firstRow || currQnumInt != mainQnum)
                {
                    mainQnum = currQnumInt;
                    // copy the current question's contents into a new object for reference
                    refQ = new SurveyQuestion
                    {
                        PreP = sq.PreP,
                        PreI = sq.PreI,
                        PreA = sq.PreA,
                        LitQ = sq.LitQ,
                        PstI = sq.PstI,
                        PstP = sq.PstP,
                        RespOptions = sq.RespOptions,
                        NRCodes = sq.NRCodes,
                        Filters = sq.Filters
                    };
                }
                else
                {
                    // if we are inside a series, compare the wording fields to the reference question
                    // if the current column is a standard wording column and has not been designated as a repeated field, compare wordings
                    if ((StdFields.Contains("PreP") && !RepeatedFields.Contains("PreP")) || removeAll)
                    {
                        // if the current question's wording field matches the reference question's, clear it. 
                        // otherwise, set the reference question's field to the current question's field
                        // this will cause a new reference point for that particular field, but not the fields that were identical to the original reference question
                        if (Utilities.RemoveTags(sq.PreP).Equals(Utilities.RemoveTags(refQ.PreP)))
                            sq.PreP = "";
                        else
                            refQ.PreP = sq.PreP;
                    }
                    // PreI
                    if ((StdFields.Contains("PreI") && !RepeatedFields.Contains("PreI")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.PreI).Equals(Utilities.RemoveTags(refQ.PreI)))
                            sq.PreI = "";
                        else
                            refQ.PreI = sq.PreI;
                    }
                    // PreA
                    if ((StdFields.Contains("PreA") && !RepeatedFields.Contains("PreA")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.PreA).Equals(Utilities.RemoveTags(refQ.PreA)))
                            sq.PreA = "";
                        else
                            refQ.PreA = sq.PreA;
                    }
                    // LitQ
                    if ((StdFields.Contains("LitQ") && !RepeatedFields.Contains("LitQ")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.LitQ).Equals(Utilities.RemoveTags(refQ.LitQ)))
                            sq.LitQ = "";
                        else
                            refQ.LitQ = sq.LitQ;
                    }
                    // PstI
                    if ((StdFields.Contains("PstI") && !RepeatedFields.Contains("PstI")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.PstI).Equals(Utilities.RemoveTags(refQ.PstI)))
                            sq.PstI = "";
                        else
                            refQ.PstI = sq.PstI;
                    }
                    // PstP
                    if ((StdFields.Contains("PstP") && !RepeatedFields.Contains("PstP")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.PstP).Equals(Utilities.RemoveTags(refQ.PstP)))
                            sq.PstP = "";
                        else
                            refQ.PstP = sq.PstP;
                    }
                    // RespOptions
                    if ((StdFields.Contains("RespOptions") && !RepeatedFields.Contains("RespOptions")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.RespOptions).Equals(Utilities.RemoveTags(refQ.RespOptions)))
                            sq.RespOptions = "";
                        else
                            refQ.RespOptions = sq.RespOptions;
                    }
                    // NRCodes
                    if ((StdFields.Contains("NRCodes") && !RepeatedFields.Contains("NRCodes")) || removeAll)
                    {
                        if (Utilities.RemoveTags(sq.NRCodes).Equals(Utilities.RemoveTags(refQ.NRCodes)))
                            sq.NRCodes = "";
                        else
                            refQ.NRCodes = sq.NRCodes;
                    }

                    // Filter column, remove repeated filters if the PreP field is also being removed
                    if (!string.IsNullOrEmpty(sq.Filters) && ((StdFields.Contains("PreP") && !RepeatedFields.Contains("PreP")) || removeAll))
                        if (sq.Filters.Equals(refQ.Filters))
                            sq.Filters = "";
                    

                }

                firstRow = false; // after once through the loop, we are no longer on the first row
            }
        }

        /// <summary>
        /// Remove repeated values from the wording fields (PreP, PreI, PreA, LitQ, PstI, Pstp, RespOptions, NRCodes) unless they are requested. 
        /// This applies only to series questions, which are questions whose Qnum ends in a letter.
        /// </summary>
        public void RemoveRepeatsTC()
        {
            string mainTopic = "";
            string mainContent = "";
            string currTopic = "";
            string currContent = "";

            bool firstRow = true;
            SurveyQuestion refQ = null;// this will hold the 'a' question's fields

            // only try to remove repeats if there are more than 0 rows
            if (Questions.Count == 0) return;

            // sort questions by their topic and then content labels
            var sorted = Questions.OrderBy(q => q.VarName.Topic.LabelText).ThenBy(q => q.VarName.Content.LabelText).ToList();
            Questions.Clear();
            AddQuestions(new BindingList<SurveyQuestion>(sorted));
     
            sorted = null;

            foreach (SurveyQuestion sq in Questions)
            {
                currTopic = sq.VarName.Topic.LabelText;
                currContent = sq.VarName.Content.LabelText;

                // if this is a non-series row, the first member of a series, the first row in the report, or a new Qnum, make this row the reference row
                if (!currTopic.Equals(mainTopic) || (!currContent.Equals(mainContent)) || firstRow)
                {
                    mainTopic = currTopic;
                    mainContent = currContent;
                    // copy the row's contents into an array
                    refQ = new SurveyQuestion
                    {
                        PreP = sq.PreP,
                        PreI = sq.PreI,
                        PreA = sq.PreA,
                        LitQ = sq.LitQ,
                        PstI = sq.PstI,
                        PstP = sq.PstP,
                        RespOptions = sq.RespOptions,
                        NRCodes = sq.NRCodes
                    };
                }
                else
                {
                    // if we are inside a series, compare the wording fields to the reference question
                    // if the current column is a standard wording column and has not been designated as a repeated field, compare wordings
                    if ((StdFields.Contains("PreP") && !RepeatedFields.Contains("PreP")))
                    {
                        // if the current question's wording field matches the reference question's, clear it. 
                        // otherwise, set the reference question's field to the current question's field
                        // this will cause a new reference point for that particular field, but not the fields that were identical to the original reference question
                        if (Utilities.RemoveTags(sq.PreP).Equals(Utilities.RemoveTags(refQ.PreP)))
                            sq.PreP = "";
                        else
                            refQ.PreP = sq.PreP;
                    }
                    // PreI
                    if ((StdFields.Contains("PreI") && !RepeatedFields.Contains("PreI")))
                    {
                        if (Utilities.RemoveTags(sq.PreI).Equals(Utilities.RemoveTags(refQ.PreI)))
                            sq.PreI = "";
                        else
                            refQ.PreI = sq.PreI;
                    }
                    // PreA
                    if ((StdFields.Contains("PreA") && !RepeatedFields.Contains("PreA")))
                    {
                        if (Utilities.RemoveTags(sq.PreA).Equals(Utilities.RemoveTags(refQ.PreA)))
                            sq.PreA = "";
                        else
                            refQ.PreA = sq.PreA;
                    }
                    // LitQ
                    if ((StdFields.Contains("LitQ") && !RepeatedFields.Contains("LitQ")))
                    {
                        if (Utilities.RemoveTags(sq.LitQ).Equals(Utilities.RemoveTags(refQ.LitQ)))
                            sq.LitQ = "";
                        else
                            refQ.LitQ = sq.LitQ;
                    }
                    // PstI
                    if ((StdFields.Contains("PstI") && !RepeatedFields.Contains("PstI")))
                    {
                        if (Utilities.RemoveTags(sq.PstI).Equals(Utilities.RemoveTags(refQ.PstI)))
                            sq.PstI = "";
                        else
                            refQ.PstI = sq.PstI;
                    }
                    // RespOptions
                    if ((StdFields.Contains("RespOptions") && !RepeatedFields.Contains("RespOptions")))
                    {
                        if (Utilities.RemoveTags(sq.RespOptions).Equals(Utilities.RemoveTags(refQ.RespOptions)))
                            sq.RespOptions = "";
                        else
                            refQ.RespOptions = sq.RespOptions;
                    }
                    // NRCodes
                    if ((StdFields.Contains("NRCodes") && !RepeatedFields.Contains("NRCodes")))
                    {
                        if (Utilities.RemoveTags(sq.NRCodes).Equals(Utilities.RemoveTags(refQ.NRCodes)))
                            sq.NRCodes = "";
                        else
                            refQ.NRCodes = sq.NRCodes;
                    }
                }
                firstRow = false; // after once through the loop, we are no longer on the first row
            }
        }

        /// <summary>
        /// Inserts LITQ and TBLROS/E tags around the LitQ and Response sets in the translation text. Only works on series starters (Qnum ends in 'a')
        /// </summary>
        /// <param name="sq"></param>
        public void InsertTranslationTableTags(SurveyQuestion sq)
        {
            if (!sq.Qnum.EndsWith("a"))
                return;

            Regex rx = new Regex("[0-9][0-9]*  ");
            MatchCollection matches;
            string litq, currentChar;
            int litqPos=0, index=0;

            foreach (Translation t in sq.Translations)
            {
                matches = rx.Matches(t.TranslationText);
                if (matches.Count == 1)
                {
                    t.TranslationText = t.TranslationText.Substring(0, matches[0].Index) + "[/LitQ][TBLROS]" + t.TranslationText.Substring(matches[0].Index + 1) + "[TBLROE]";
                }

                if (string.IsNullOrEmpty(t.LitQ))
                    litq = sq.LitQ;
                else
                    litq = t.LitQ;

                index = 0;
                do
                {
                    if (index > litq.Length)
                        break;

                    currentChar = litq.Substring(index, 1);
                    if (currentChar.Equals(".") || currentChar.Equals("/") || currentChar.Equals("\r") || currentChar.Equals("\n") || currentChar.Equals(":") || currentChar.Equals("?")
                        || string.IsNullOrEmpty(currentChar))
                        break;

                    index++;
                }
                while (true);

                litq = litq.Substring(0, index-1);

                litqPos = t.TranslationText.IndexOf(litq,StringComparison.OrdinalIgnoreCase);

                if (litqPos > 0)
                {
                    t.TranslationText = t.TranslationText.Substring(0, litqPos - 1) + "[LitQ]" + t.TranslationText.Substring(litqPos);
                }

            }
        }

      
        public override string ToString()
        {
            string description = SurveyCode;
            if (Corrected)
                description += " (Corrected) ";

            description += " from " + Backend.ToString("d");

            return description;
        }

        public string ToString(bool detailed)
        {
            if (!detailed)
                return ToString();

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
        #endregion
    }
}
