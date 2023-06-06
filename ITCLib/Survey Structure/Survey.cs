using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ITCLib
{
    /// <summary>
    /// Represents an ITC Survey. A survey may have a list of SurveyQuestions representing its content.
    /// </summary>
    public class Survey : INotifyPropertyChanged
    {
        #region Survey Properties

        // properties from database
        /// <summary>
        /// Unique ID for the survey referenced by this object.
        /// </summary>
        public int SID
        {
            get { return this._sid; }
            set { if (value != this._sid) { this._sid = value; NotifyPropertyChanged(); } }
        }
        /// <summary>
        /// Survey code for the survey referenced by this object
        /// </summary>
        public string SurveyCode
        {
            get { return this._surveycode; }
            set { if (value != this._surveycode) { this._surveycode = value; NotifyPropertyChanged(); WebName = UpdateWebName(); } }
        }
        
        public string SurveyCodePrefix { get; set; }

        /// <summary>
        /// Full title of this survey.
        /// </summary>
        public string Title
        {
            get { return this._title; }
            set { if (value != this._title) { this._title = value; NotifyPropertyChanged(); } }
        }

        /// <summary>
        /// Languages that this survey was translated into.
        /// </summary>
        public List<SurveyLanguage> LanguageList { get; set; }
        public string LanguagesList
        {
            get
            {
                return string.Join(",", LanguageList.Select(m => m.SurvLanguage.LanguageName).ToArray());
            }
        }

        public List<SurveyScreenedProduct> ScreenedProducts { get; set; }
        public string ProductList
        {
            get
            {
                return string.Join(",", ScreenedProducts.Select(m => m.Product.ProductName).ToArray());
            }
        }

        public List<SurveyUserState> UserStates { get; set; }
        public string UserStateList {
            get
            {
                return string.Join(", ", UserStates.Select(m => m.State.UserStateName).ToArray());
            }
        }

        /// <summary>
        /// User group that this survey if meant for.
        /// </summary>
        public SurveyUserGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                if (value != _group)
                {
                    _group = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Cohort name for this survey. Recontact, replenishment, recruitment or some combination.
        /// </summary>
        public SurveyCohort Cohort
        {
            get
            {
                return _cohort;
            }
            set
            {
                if (value != _cohort)
                {
                    _cohort = value;
                    
                    NotifyPropertyChanged();
                    WebName = UpdateWebName();
                }
            }
        }
        /// <summary>
        /// The survey mode. Telephone, web, or face to face.
        /// </summary>
        public SurveyMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    NotifyPropertyChanged();
                    WebName = UpdateWebName();
                }
            }
        }
        /// <summary>
        /// Country specific 2-digit code.
        /// </summary>
        public string CountryCode
        {
            get { return _countrycode; }
            set
            {
                if (value != _countrycode)
                {
                    _countrycode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// File name to be used when uploading this survey to the website.
        /// </summary>
        public string WebName
        {
            get
            {
                return _webname;
            }
            set
            {
                if (value != _webname)
                {
                    _webname = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// True if this survey utilizes English Routing.
        /// </summary>
        /// <remarks>English Routing means that the translated version may have filters and routing taken from the English version.</remarks>
        public bool EnglishRouting
        {
            get
            {
                return _englishrouting;
            }
            set
            {
                if (value != _englishrouting)
                {
                    _englishrouting = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// True if this survey cannot be edited until unlocked.
        /// </summary>
        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                if (value != _locked)
                {
                    _locked = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public DateTime? CreationDate
        {
            get
            {
                return _creationdate;
            }
            set
            {
                if (value != _creationdate)
                {
                    _creationdate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool ReRun
        {
            get
            {
                return _rerun;
            }
            set
            {
                if (value != _rerun)
                {
                    _rerun = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool HideSurvey
        {
            get
            {
                return _hidesurvey;
            }
            set
            {
                if (value != _hidesurvey)
                {
                    _hidesurvey = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool NCT
        {
            get
            {
                return _nct;
            }
            set
            {
                if (value != _nct)
                {
                    _nct = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool ITCSurvey
        {
            get
            {
                return _itcsurvey;
            }
            set
            {
                if (value != _itcsurvey)
                {
                    _itcsurvey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double Wave { get; set; }

        public bool HasCorrectedWordings { get; set; }

        /// <summary>
        /// Comma-separated list of essential varnames (and their Qnums) in this survey.
        /// </summary>
        /// <remarks>Essential varnames are those that will exit the survey if not answered.</remarks>
        public string EssentialList { get; set; }

        // lists for this survey
        /// <summary>
        /// List of all SurveyQuestion objects for this Survey object. Each representing a single question in the survey.
        /// </summary>
        public List<SurveyQuestion> Questions 
        {
            get { return _questions; }
            private set {
                _questions = value;
                UpdateEssentialQuestions();
                }
        }

        /// <summary>
        /// List of all SurveyQuestion objects for this Survey object which are designated as 'corrected.'
        /// </summary>
        /// <remarks>A corrected question is one that has content different than what appeared in the fieldwork.</remarks>
        public List<SurveyQuestion> CorrectedQuestions { get; set; }

        // this list contains any VarNames found in the survey wordings that are not questions themselves within the survey
        public List<string> QNUlist;

        public List<SurveyComment> SurveyNotes { get; set; }

        #endregion



        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        // blank constructor

        /// <summary>
        /// Blank constructor.
        /// </summary>
        public Survey() {

            Cohort = new SurveyCohort();
            Mode = new SurveyMode();
            Group = new SurveyUserGroup();
            Title = string.Empty;
            SurveyCode = "";
            WebName = "";
            
            

            CreationDate = DateTime.Today;

            EssentialList = "";

            Questions = new List<SurveyQuestion>();
            
            CorrectedQuestions = new List<SurveyQuestion>();
            QNUlist = new List<string>();
            SurveyNotes = new List<SurveyComment>();

            LanguageList = new List<SurveyLanguage>();
            UserStates = new List<SurveyUserState>();
            ScreenedProducts = new List<SurveyScreenedProduct>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Survey(string surveyCode)
        {

            SurveyCode = surveyCode;
            WebName = "";

            Cohort = new SurveyCohort();
            Mode = new SurveyMode(0, "", "");
            Group = new SurveyUserGroup();

            EssentialList = "";


            Questions = new List<SurveyQuestion>();
            
            CorrectedQuestions = new List<SurveyQuestion>();
            QNUlist = new List<string>();
            SurveyNotes = new List<SurveyComment>();

            LanguageList = new List<SurveyLanguage>();
            UserStates = new List<SurveyUserState>();
            ScreenedProducts = new List<SurveyScreenedProduct>();
        }

        #endregion

        #region Methods and Functions

        public string UpdateWebName()
        {
            //ITC_[strAbbrev][strWave]_[strWebCohort]_[strMode]_ENG
            

            StringBuilder webname = new StringBuilder();
            webname.Append("ITC_");

            webname.Append(SurveyCode ?? "" );

            if (Cohort != null)
            {
                webname.Append("_");
                webname.Append(Cohort.WebName ?? "");
            }

            if (Mode != null)
            {
                webname.Append("_");
                webname.Append(Mode.ModeAbbrev);
            }
            webname.Append("_ENG");

            while (webname.ToString().Contains("__"))
                webname.Replace("__", "_");

            return webname.ToString();
        }

        /// <summary>
        /// Adds a question to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestion(SurveyQuestion newQ)
        {
            if (!Questions.Contains(newQ, new SurveyQuestionComparer()))
            {
                Questions.Add(newQ);
                UpdateEssentialQuestions();
            }
        }

        /// <summary>
        /// Adds a question to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestion(SurveyQuestion newQ, bool withRenumber)
        {
            Questions.Add(newQ);
            if (withRenumber) Renumber(0);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Adds a question to the survey's question list at the specified location.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestion(SurveyQuestion newQ, int afterIndex)
        {
            Questions.Insert(afterIndex, newQ);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Adds a question to the survey's question list at the specified location.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestion(SurveyQuestion newQ, int afterIndex, bool withRenumber)
        {
            Questions.Insert(afterIndex, newQ);
            if (withRenumber) Renumber(0);
            UpdateEssentialQuestions();
        }



        /// <summary>
        /// Adds each question in the list to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestions(BindingList<SurveyQuestion> questions)
        {
            foreach(SurveyQuestion sq in questions)
                Questions.Add(sq);

            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Adds each question in the list to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestions(List<SurveyQuestion> questions)
        {
            foreach (SurveyQuestion sq in questions)
                Questions.Add(sq);

            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Adds each question in the list to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestions(BindingList<SurveyQuestion> questions, bool withRenumber)
        {
            foreach (SurveyQuestion sq in questions)
                Questions.Add(sq);

            if (withRenumber) Renumber(0);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Adds each question in the list to the survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestions(BindingList<SurveyQuestion> questions, int afterIndex, bool withRenumber)
        {
            foreach (SurveyQuestion sq in questions)
                Questions.Insert(afterIndex, sq);

            if (withRenumber) Renumber(0);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Removes the question from the Survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void RemoveQuestion(SurveyQuestion q)
        {
            Questions.Remove(q);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Removes the question from the Survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void RemoveQuestion(SurveyQuestion q, bool withRenumber)
        {
            Questions.Remove(q);
            if (withRenumber) Renumber(0);
            UpdateEssentialQuestions();
        }

        /// <summary>
        /// Removes all questions from the Survey's question list.
        /// </summary>
        /// <param name="newQ"></param>
        public void RemoveAllQuestions()
        {
            Questions.Clear();
            EssentialList = "";
        }

        public List<RefVariableName> GetRefVars()
        {
            List<RefVariableName> refVars = new List<RefVariableName>();

            foreach (SurveyQuestion q in Questions)
            {
                refVars.Add(new RefVariableName()
                {
                    RefVarName = q.VarName.RefVarName
                });
            }

            return refVars;
        }

        public bool IsOtherSpecify(SurveyQuestion q)
        {
            if (!q.VarName.VarName.EndsWith("o"))
                return false;

            return Questions.Any(x => x.VarName.VarName.Equals(q.VarName.Prefix + q.VarName.NumberInt().ToString("000")));
        }

        public bool HasOtherSpecify(SurveyQuestion q)
        {
            if (q.VarName.VarName.EndsWith("o"))
                return false;

            return Questions.Any(x => x.VarName.VarName.Equals(q.VarName.Prefix + q.VarName.NumberInt().ToString("000") + "o"));
        }

        /// <summary>
        /// Returns true is this question could be formatted as a table
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public bool IsTableFormatSeries(SurveyQuestion q)
        {
            // standalone question is not for table format
            if (q.Qnum.Length == 3)
                return false;

            if (q.IsDerived())
                return false;

            var seriesvars = Questions.Where(x => x.Qnum.StartsWith(q.Qnum.Substring(0, 3))).ToList();
            var responses = seriesvars.GroupBy(r => r.RespName).Select(group => new
            {
                RespName = group.Key
            }).ToList();

            // if not in a series, false
            if (seriesvars.Count() == 1)
                return false;

            // if no responses, false
            if (responses.Count() == 1 && responses[0].RespName.Equals("0"))
                return false;

            // if there are 2 response sets and neither is empty, false
            if (responses.Count() == 2 && !responses.Any(x=>x.RespName.Equals("0")))
                return false;

            // if there are more than 2 responses, false
            if (responses.Count() >= 3)
                return false;

            // if there are 2 members of the series, and one is 'other specify', false
            if (seriesvars.Count() == 2 && HasOtherSpecify(q)) 
                return false;

            // if there is a unique response set for each member, false
            if (seriesvars.Count() == responses.Count())
                return false;

            // if there are 2 response sets but the last one is different, true
            if (responses.Count() == 2 && responses[1].RespName.Equals("0"))
                return true;
            // if there is only one response set, true
            else if (responses.Count() == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// </summary>
        protected virtual void Renumber(int start)
        {
            int qLet = 0;
            int hcount = 0;
            int i;
            int counter = 0;
            QuestionType qType;

            int currQnum;
            string newQnum;

            currQnum = start;

            foreach (SurveyQuestion sq in Questions)
            {
                qType = Utilities.GetQuestionType(sq);

                // increment either the letter or the number, count headings
                switch (qType)
                {
                    case QuestionType.Series:
                        qLet++;
                        hcount = 0;
                        break;
                    case QuestionType.Standalone:
                        currQnum++;
                        qLet = 1;
                        hcount = 0;
                        break;
                    case QuestionType.Heading:
                        hcount++;
                        break;
                    case QuestionType.Subheading:
                        hcount++;
                        break;
                }

                newQnum = currQnum.ToString("000");

                if (qType != QuestionType.Standalone)
                {
                    newQnum += new string('z', (qLet - 1) / 26);
                    newQnum += Char.ConvertFromUtf32(96 + qLet - 26 * ((qLet - 1) / 26));

                }

                if (hcount > 0)
                    newQnum += "!" + hcount.ToString("000");

                sq.Qnum = newQnum;

                // add 'a' to series starters
                if (qType == QuestionType.Standalone)
                {
                    i = counter;

                    do
                    {
                        if (i < Questions.Count - 1)
                            i++;
                        else
                            break;

                    } while (Utilities.GetQuestionType(Questions[i]) == QuestionType.Heading || Utilities.GetQuestionType(Questions[i]) == QuestionType.InterviewerNote);

                    if (Utilities.GetQuestionType(Questions[i]) == QuestionType.Series)
                        sq.Qnum += "a";
                }
                counter++;
            }
        }

        /// <summary>
        /// Gets a specific question by it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyQuestion object matching the supplied ID. Returns null if one is not found.</returns>
        public SurveyQuestion QuestionByID (int id)
        {
            foreach (SurveyQuestion sq in Questions)
            {
                if (sq.ID == id)
                    return sq;
            }
            return null;
        }

        /// <summary>
        /// Gets a specific question by it's refVarName.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyQuestion object matching the supplied refVarName. Returns null if one is not found.</returns>
        public SurveyQuestion QuestionByRefVar(string refVar)
        {
            foreach (SurveyQuestion sq in Questions)
            {
                if (sq.VarName.RefVarName.Equals(refVar))
                    return sq;
            }
            return null;
        }



        /// <summary>
        /// Apply corrected wordings to the questions list. Overwrites the wording fields in the questions list with those found in the correctedQuestions list.
        /// </summary>        
        public void CorrectWordings()
        {
            foreach (SurveyQuestion cq in CorrectedQuestions)
            {
                try
                {
                    SurveyQuestion sq = Questions.Single(x => x.ID == cq.ID);

                    sq.PreP = cq.PreP;
                    sq.PreI = cq.PreI;
                    sq.PreA = cq.PreA;
                    sq.LitQ = cq.LitQ;
                    sq.PstI = cq.PstI;
                    sq.PstP = cq.PstP;
                    sq.RespOptions = cq.RespOptions;
                    sq.NRCodes = cq.NRCodes;
                }
                catch (ArgumentNullException )
                {

                }
            }
        }

        // TODO check for response codes mentioned in filter and only show those
        /// <summary>
        /// Sets the Filter property for each SurveyQuestion that has a PreP containing a VarName.
        /// </summary>
        private void PopulateFilters()
        {
            
            string filterList = "";
            string filterVar = "";
            string filterRO;
            string filterNR;
            string filterLabel;
            
            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // get any rows that contain a variable
            var refVars = from r in Questions.AsEnumerable()
                          where r.PreP != null && rx1.IsMatch(r.PreP)
                          select r;

            if (!refVars.Any())
                return;

            foreach (var item in refVars)
            {
                QuestionFilter qf = new QuestionFilter(item.PreP);
                filterList = "";
                for (int i = 0; i < qf.FilterVars.Count; i++)
                {
                    filterVar = qf.FilterVars[i].Varname;
                    var found = Questions.FirstOrDefault(x => x.VarName.RefVarName.Equals(filterVar));

                    if (found != null)
                    {
                        filterRO = found.RespOptions;
                        filterNR = found.NRCodes;
                        filterLabel = "<em>" + found.VarName.VarLabel + "</em>";

                        filterList += "<strong>" + filterVar.Substring(0, 2) + "." + filterVar.Substring(2) + "</strong>\r\n" +
                        filterLabel + "\r\n" + filterRO + "\r\n" + filterNR + "\r\n";
                    }
                    else
                    {
                        filterRO = "";
                        filterNR = "";
                        filterLabel = "";

                        filterList += "<strong>" + filterVar.Substring(0, 2) + "." + filterVar.Substring(2) + "</strong>\r\n";
                    }
                    
                }

                item.Filters += filterList;

            }

        }

        // TODO check for response codes mentioned in filter and only show those
        /// <summary>
        /// Sets the Filter property for each SurveyQuestion that has a PreP containing a VarName.
        /// </summary>
        private void PopulateOddFilters()
        {

            string filterList = "";
            string filterVar = "";
            string filterRO;
            string filterNR;
            string filterLabel;


            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9]");

            // get any rows that contain a variable
            var refVars = from r in Questions.AsEnumerable()
                            where !string.IsNullOrEmpty(r.PreP) && !r.VarName.VarName.StartsWith("Z")
                            select r;

            // get the variables that are not in standard form
            var oddVars = from r in Questions.AsEnumerable()
                          where !rx1.IsMatch(r.VarName.RefVarName)
                          select r;

            if (!refVars.Any())
                return;

            foreach (var item in refVars)
            {
                QuestionFilter qf = new QuestionFilter(item.PreP, oddVars.ToList());
                filterList = "";
                if (qf.FilterVars == null)
                    continue;

                for (int i = 0; i < qf.FilterVars.Count; i++)
                {
                    filterVar = qf.FilterVars[i].Varname;
                    var found = Questions.FirstOrDefault(x => x.VarName.RefVarName.Equals(filterVar));

                    if (found != null)
                    {
                        filterRO = found.RespOptions;
                        filterNR = found.NRCodes;
                        filterLabel = "<em>" + found.VarName.VarLabel + "</em>";

                        filterList += "<strong>" + filterVar.Substring(0, 2) + "." + filterVar.Substring(2) + "</strong>\r\n" +
                            filterLabel + "\r\n" + filterRO + "\r\n" + filterNR + "\r\n";
                    }
                    else
                    {
                        filterRO = "";
                        filterNR = "";
                        filterLabel = "";

                        filterList += "<strong>" + filterVar.Substring(0, 2) + "." + filterVar.Substring(2) + "</strong>\r\n";
                    }

                }

                item.Filters += filterList;

            }
            

        }

        public void MakeFilterList()
        {
            PopulateFilters();
            PopulateOddFilters();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sq"></param>
        public void FormatRouting(SurveyQuestion sq)
        {
            QuestionRouting qr;

            // format the response options
            qr = new QuestionRouting(sq.PstP, sq.RespOptions);
            sq.RespOptions = qr.ToString();

            // format the non-response options
            qr = new QuestionRouting(sq.PstP, sq.NRCodes);
            sq.NRCodes = qr.ToString();

            // format the PstP
            sq.PstP = string.Join("\r\n", qr.RoutingText);
           

            
        }

        /// <summary>
        /// Adds a Don't Read or Don't Read Out instruction to the end of each line in the wording.
        /// </summary>
        /// <param name="wording"></param>
        /// <param name="nrFormat"></param>
        /// <returns></returns>
        public string FormatNR(string wording, ReadOutOptions nrFormat)
        {
            string[] options;
            string result = wording;
            string readOut = new string(' ', 3); // the read out instruction will be 3 spaces after the response option
            int tagEnd = -1; // location of an end tag like [/yellow] or [/t][/s]
            string tagText; // the actual end tag

            options = wording.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            switch (nrFormat)
            {
                case ReadOutOptions.DontRead:
                    readOut += "(Don't read)";
                    break;
                case ReadOutOptions.DontReadOut:
                    readOut += "(Don't read out)";
                    break;
                case ReadOutOptions.Neither:
                    break;
            }

            for (int i = 0; i < options.Length; i++)
            {
                tagEnd = options[i].IndexOf("[/");
                if (tagEnd > -1)
                {
                    tagText = options[i].Substring(tagEnd);
                    options[i] = options[i].Substring(0, tagEnd) + readOut + tagText;
                }
                else
                {
                    options[i] += readOut;
                }
            }

            result = string.Join("\r\n", options);

            return result;
        }

        /// <summary>
        /// Modifies each wording field in the Survey object's question list to include Qnums before any standard variable name found in the wording.
        /// </summary>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        public void InsertQnums(Enumeration numbering)
        {
            foreach (SurveyQuestion q in Questions)
            {
                q.PreP = InsertQnums(q.PreP, numbering);
                q.PreI = InsertQnums(q.PreI, numbering);
                q.PreA = InsertQnums(q.PreA, numbering);
                q.LitQ = InsertQnums(q.LitQ, numbering);
                q.PstI = InsertQnums(q.PstI, numbering);
                q.PstP = InsertQnums(q.PstP, numbering);
                q.RespOptions = InsertQnums(q.RespOptions, numbering);
                q.NRCodes = InsertQnums(q.NRCodes, numbering);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include Qnums before any standard variable name found in the wording.
        /// </summary>
        /// <param name="sq">The question to modify.</param>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        public void InsertQnums(SurveyQuestion sq, Enumeration numbering)
        {
            
            sq.PreP = InsertQnums(sq.PreP, numbering);
            sq.PreI = InsertQnums(sq.PreI, numbering);
            sq.PreA = InsertQnums(sq.PreA, numbering);
            sq.LitQ = InsertQnums(sq.LitQ, numbering);
            sq.PstI = InsertQnums(sq.PstI, numbering);
            sq.PstP = InsertQnums(sq.PstP, numbering);
            sq.RespOptions = InsertQnums(sq.RespOptions, numbering);
            sq.NRCodes = InsertQnums(sq.NRCodes, numbering);
        }

        /// <summary>
        /// Searches a string for a VarName pattern and, if found, looks up and inserts the Qnum right before the VarName. If a Qnum can not be found,
        /// "QNU" (Qnum Unknown) is used instead.
        /// </summary>
        /// <remarks> Each word in the string is compared to the VarName pattern.</remarks>
        /// <param name="wording"></param>
        /// <param name="numbering"></param>
        /// <returns></returns>
        private string InsertQnums(string wording, Enumeration numbering)
        {
            int offset = 0; 
            string qnum = "";
            string foundVarname = "";
            MatchCollection matches;
            Regex rx = new Regex("\\b[A-Z]{2}[0-9]{3}[a-z]*", RegexOptions.IgnoreCase);

            // if wording contains a variable name, look up the qnum and place it before the variable
            if (rx.Match(wording).Success)
            {
                matches = rx.Matches(wording);
                foreach (Match match in matches)
                {
                    // for each found pattern, replace the found variable once, starting at the position in the string where it was found
                    // an offset is added to this position to account for previous insertions
                    foundVarname = match.Groups[0].Value;
                   
                    SurveyQuestion foundQ = Questions.SingleOrDefault(x => x.VarName.RefVarName == foundVarname);

                    if (foundQ == null)
                    {
                        
                        qnum = "QNU";
                    }
                    else
                        switch (numbering)
                        {
                            case Enumeration.Both:
                            case Enumeration.Qnum:
                                qnum = foundQ.Qnum;
                                break;
                            case Enumeration.AltQnum:
                                if (string.IsNullOrEmpty(foundQ.AltQnum))
                                    qnum = string.Empty;
                                else
                                    qnum = foundQ.AltQnum;
                                
                                break;
                            default:
                                qnum = foundQ.Qnum;
                                break;
                        }
                    

                    wording = rx.Replace(wording, qnum + "/" + foundVarname, 1, match.Index + offset );
                    offset += qnum.Length + 1; // offset the starting point but the length of the qnum and slash just inserted
                }

                if (qnum.Equals("QNU"))
                {
                    QNUlist.Add(foundVarname);

                }
                offset = 0;
            }
            
            return wording;
        }

        /// <summary>
        /// Modifies each wording field in the Survey object to include Qnums before any non-standard variable name found in the wording.
        /// </summary>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        public void InsertOddQnums(Enumeration numbering)
        {
            foreach (SurveyQuestion q in Questions)
            {
                q.PreP = InsertOddQnums(q.PreP, numbering);
                q.PreI = InsertOddQnums(q.PreI, numbering);
                q.PreA = InsertOddQnums(q.PreA, numbering);
                q.LitQ = InsertOddQnums(q.LitQ, numbering);
                q.PstI = InsertOddQnums(q.PstI, numbering);
                q.PstP = InsertOddQnums(q.PstP, numbering);
                q.RespOptions = InsertOddQnums(q.RespOptions, numbering);
                q.NRCodes = InsertOddQnums(q.NRCodes, numbering);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include Qnums before any non-standard variable name found in the wording.
        /// </summary>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        /// <param name="sq">The question to modify.</param>
        public void InsertOddQnums(SurveyQuestion sq, Enumeration numbering)
        {
            sq.PreP = InsertOddQnums(sq.PreP, numbering);
            sq.PreI = InsertOddQnums(sq.PreI, numbering);
            sq.PreA = InsertOddQnums(sq.PreA, numbering);
            sq.LitQ = InsertOddQnums(sq.LitQ, numbering);
            sq.PstI = InsertOddQnums(sq.PstI, numbering);
            sq.PstP = InsertOddQnums(sq.PstP, numbering);
            sq.RespOptions = InsertOddQnums(sq.RespOptions, numbering);
            sq.NRCodes = InsertOddQnums(sq.NRCodes, numbering);
        }

        /// <summary>
        /// Searches a wording for any non-standard VarName and places it's Qnum (or AltQnum) in front of it.
        /// </summary>
        /// <param name="wording"></param>
        /// <param name="numbering"></param>
        /// <returns></returns>
        private string InsertOddQnums(string wording, Enumeration numbering)
        {
            Regex rx = new Regex("[A-Z]{2}[0-9]{3}[a-z]*", RegexOptions.IgnoreCase);

            // get the non standard Qnums
            List<SurveyQuestion> oddVars = Questions.Where(x => !rx.IsMatch(x.VarName.RefVarName)).ToList();

            // for every oddVar, search the wording for a match
            foreach (SurveyQuestion sq in oddVars)
            {
                Regex rxReplace = new Regex("\\b" + sq.VarName.RefVarName, RegexOptions.IgnoreCase);
                // if a match is found, replace it with [Qnum]/[refVarName]
                if (rxReplace.Match(wording).Success)
                {
                    
                    switch (numbering)
                    {
                        case Enumeration.Both:
                        case Enumeration.Qnum:
                            wording = rxReplace.Replace(wording, sq.Qnum + "/" + sq.VarName.RefVarName);
                            break;
                        case Enumeration.AltQnum:
                            wording = rxReplace.Replace(wording, sq.AltQnum + "/" + sq.VarName.RefVarName);
                            break;
                        default:
                            wording = rxReplace.Replace(wording, sq.Qnum + "/" + sq.VarName.RefVarName);
                            break;
                    }                    
                }
            }
            
            return wording;
        }

        /// <summary>
        /// Looks for any refVarName in the wording and places it's Qnum or AltQnum before the refVarName. This method does not detect unknown VarNames.
        /// </summary>
        /// <param name="wording"></param>
        /// <param name="numbering"></param>
        /// <returns></returns>
        private string InsertAllQnums(string wording, Enumeration numbering)
        {
            foreach (SurveyQuestion sq in Questions)
            {
                Regex rxReplace = new Regex("\\b" + sq.VarName.RefVarName + "\\b", RegexOptions.IgnoreCase);

                // if words[i] contains a variable name, look up the qnum and place it before the variable
                if (rxReplace.Match(wording).Success)
                {
                    
                    SurveyQuestion foundQ = Questions.SingleOrDefault(x => x.VarName.RefVarName == sq.VarName.RefVarName);
                    string qnum;
                    switch (numbering)
                    {
                        case Enumeration.Both:
                        case Enumeration.Qnum:
                            qnum = foundQ.Qnum;
                            break;
                        case Enumeration.AltQnum:
                            qnum = foundQ.AltQnum;
                            break;
                        default:
                            qnum = foundQ.Qnum;
                            break;
                    }

                    if (qnum.Equals(""))
                    {
                        QNUlist.Add(sq.VarName.RefVarName);
                        qnum = "QNU";
                    }

                    wording = rxReplace.Replace(wording, qnum + "/" + sq.VarName.RefVarName);
                }
                
            }
           
            return wording;
        }

        /// <summary>
        /// Modifies each wording field in the Survey object to include the country code for any standard variable name found in the wording.
        /// </summary>
        public void InsertCountryCodes()
        {
            foreach (SurveyQuestion q in Questions)
            {
                q.PreP = InsertCountryCodes(q.PreP);
                q.PreI = InsertCountryCodes(q.PreI);
                q.PreA = InsertCountryCodes(q.PreA);
                q.LitQ = InsertCountryCodes(q.LitQ);
                q.PstI = InsertCountryCodes(q.PstI);
                q.PstP = InsertCountryCodes(q.PstP);
                q.RespOptions = InsertCountryCodes(q.RespOptions);
                q.NRCodes = InsertCountryCodes(q.NRCodes);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include the country code for any standard variable name found in the wording.
        /// </summary>
        /// <param name="sq">The question to modify.</param>
        public void InsertCountryCodes(SurveyQuestion sq)
        {
            sq.PreP = InsertCountryCodes(sq.PreP);
            sq.PreI = InsertCountryCodes(sq.PreI);
            sq.PreA = InsertCountryCodes(sq.PreA);
            sq.LitQ = InsertCountryCodes(sq.LitQ);
            sq.PstI = InsertCountryCodes(sq.PstI);
            sq.PstP = InsertCountryCodes(sq.PstP);
            sq.RespOptions = InsertCountryCodes(sq.RespOptions);
            sq.NRCodes = InsertCountryCodes(sq.NRCodes);
        }

        /// <summary>
        /// Adds this survey's country code to any standard Var found in the wording.
        /// </summary>
        /// <param name="wording"></param>
        /// <returns></returns>
        private string InsertCountryCodes(string wording)
        {

            string varname;
            string foundVarName;
            MatchCollection matches;
            Regex rx = new Regex("\\b[A-Z]{2}\\d{3}", RegexOptions.IgnoreCase);
          
            // if wording contains a variable name, replace the variable with the country coded version
            if (rx.Match(wording).Success)
            {
                matches = rx.Matches(wording);
                foreach (Match match in matches )
                {
                    foundVarName = match.Groups[0].Value;
                    Regex rxReplace = new Regex("\\b" + foundVarName, RegexOptions.IgnoreCase);
                    varname = Utilities.ChangeCC(foundVarName, CountryCode);
                    wording = rxReplace.Replace(wording, varname);
                }
            }
            return wording;
        }

        /// <summary>
        /// Sets the 'essentialList' property by compiling a list of VarNames that contain the special routing instruction that only essential 
        /// questions have.
        /// </summary>
        public void UpdateEssentialQuestions()
        {
            if (Questions == null)
                return;

            string varlist = "";
            Regex rx = new Regex("go to [A-Z][A-Z][0-9][0-9][0-9], then BI9");

            var query = from r in Questions
                        where r.PstP != null && rx.IsMatch(r.PstP)
                        select r;

            // if there are any variables with the special PstP instruction, create a list of them
            if (query.Any())
            {
                foreach (var item in query)
                {
                    varlist += item.VarName + " (" + item.Qnum + "), ";
                }

                varlist = varlist.Substring(0, varlist.Length - 2);
            }
            EssentialList = varlist;
        }

        /// <summary>
        /// Returns the variable name immediately following the provided heading question.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSectionLowerBound(SurveyQuestion sq)
        {
            if (!sq.VarName.VarName.StartsWith("Z"))
                return sq.VarName.VarName;

            int index = 0;

            for(int i = 0; i < Questions.Count; i ++)
            {
                if (Questions[i].VarName.Equals(sq.VarName))
                {
                    index = i;
                    break;
                }

            }

            // if this heading is the last question, return nothing
            if (index == Questions.Count)
                return "";

            // if a heading is the next question return this Varname
            if (Questions[index+1].VarName.VarName.StartsWith("Z"))
                return sq.VarName.VarName;

            return Questions[index + 1].VarName.VarName;
        }

        /// <summary>
        /// Returns the variable name immediately preceding the heading that follows the provided heading question.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSectionUpperBound(SurveyQuestion sq)
        {
            if (!sq.VarName.VarName.StartsWith("Z"))
                return sq.VarName.VarName;

            int index = 0;
            bool inSection = false;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].VarName.Equals(sq.VarName))
                {
                    inSection = true;
                    continue;
                }

                if (Questions[i].VarName.VarName.StartsWith("Z") && inSection)
                {
                    index = i-1;
                    break;
                }


            }
            // next heading not found, so we must be looking for the end of the survey
            if (index == 0) 
                return Questions[Questions.Count-1].VarName.VarName;

            // if the next heading is the next question return nothing
            if (Questions[index].VarName.Equals(sq.VarName))
                return "";
            
            return Questions[index].VarName.VarName;
        }

        /// <summary>
        /// Returns the previous heading varname's PreP.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSectionName(SurveyQuestion sq)
        {
            if (QuestionByID(sq.ID) == null)
                return "VarName not part of this survey.";

            // if there are no headings, it is part of the "Main" section
            if (!Questions.Any(x => x.IsHeading()))
                return "Main";
         
            int index = 0;
            for (int i =0;i <Questions.Count;i++)
            {
                if (Questions[i].VarName.Equals(sq.VarName))
                {
                    index = i;
                    break;
                }
            }
       
            for (int i = index; i > 0; i--)
            {
                if (Questions[i].IsHeading())
                    return Questions[i].PreP;
            }

            // if we are in the part of the survey before any headings, leave it blank
            return string.Empty;
            
        }

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

            return SurveyCode;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override bool Equals(object obj)
        {
            var survey = obj as Survey;
            return survey != null &&
                   SID == survey.SID &&
                   SurveyCode == survey.SurveyCode;
        }

        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + SID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SurveyCode);
            return hashCode;
        }
        #endregion

        #region Private Backing Variables
        private int _sid;
        private string _surveycode;
        private string _title;
        private string _language;
        private SurveyUserGroup _group;
        private SurveyCohort _cohort;
        private SurveyMode _mode;
        private string _countrycode;
        private string _webname;
        private bool _englishrouting;
        private bool _locked;
        private DateTime? _creationdate;
        private bool _rerun;
        private bool _hidesurvey;
        private bool _nct;
        private bool _itcsurvey;
        private List<SurveyQuestion> _questions;
        
        #endregion

        #region Unused Code - Space for old versions of methods and stuff, just in case
        
        #endregion  
    }

    public class SurveyImage : ITCImage
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string VarName { get; set; }
        public string Description { get; set; }
        public string GetDescription()
        {
            return ImageName.Substring(ImageName.LastIndexOf('_') + 1);
        }
    }

    public class ITCImage
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
