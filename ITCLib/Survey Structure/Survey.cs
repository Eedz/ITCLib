using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace ITCLib
{
    /// <summary>
    /// Represents an ITC Survey. A survey may have a list of SurveyQuestions representing its content.
    /// </summary>
    public class Survey : ObservableObject
    {
        #region Properties

        public int SID
        {
            get => _sid;
            set => SetProperty(ref _sid, value);
        }

        public int WaveID 
        { 
            get => _waveid; 
            set => SetProperty(ref _waveid, value); 
        }

        /// <summary>
        /// Survey code for the survey referenced by this object
        /// </summary>
        public string SurveyCode
        {
            get => _surveycode;
            set => SetProperty(ref _surveycode, value);
        }
        
        public string SurveyCodePrefix { 
            get => _surveycodeprefix; 
            set => SetProperty(ref _surveycodeprefix, value); 
        }

        /// <summary>
        /// Full title of this survey.
        /// </summary>
        public string Title
        {
            get =>_title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Languages that this survey was translated into.
        /// </summary>
        public List<SurveyLanguage> LanguageList { get; set; }
        public string LanguagesList
        {
            get
            {
                return string.Join(", ", LanguageList.Select(m => m.SurvLanguage.LanguageName).ToArray());
            }
        }

        public string[] ListLanguages
        {
            get
            {
                return LanguageList.Select(m => m.SurvLanguage.LanguageName).ToArray();
            }
        }

        public List<SurveyScreenedProduct> ScreenedProducts { get; set; }
        public string ProductList
        {
            get
            {
                return string.Join(", ", ScreenedProducts.Select(m => m.Product.ProductName).ToArray());
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
            get => _group;
            set => SetProperty(ref _group, value);
        }
        /// <summary>
        /// Cohort name for this survey. Recontact, replenishment, recruitment or some combination.
        /// </summary>
        public SurveyCohort Cohort
        {
            get => _cohort;
            set => SetProperty(ref _cohort, value);
        }
        /// <summary>
        /// The survey mode. Telephone, web, or face to face.
        /// </summary>
        public SurveyMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }
        /// <summary>
        /// Country specific 2-digit code.
        /// </summary>
        public string CountryCode
        {
            get => _countrycode;
            set => SetProperty(ref _countrycode, value);
        }
        /// <summary>
        /// File name to be used when uploading this survey to the website.
        /// </summary>
        public string WebName
        {
            get => _webname;
            set => SetProperty(ref _webname, value);
        }
        /// <summary>
        /// True if this survey utilizes English Routing.
        /// </summary>
        /// <remarks>English Routing means that the translated version may have filters and routing taken from the English version.</remarks>
        public bool EnglishRouting
        {
            get => _englishrouting;
            set => SetProperty(ref _englishrouting, value);
        }
        /// <summary>
        /// True if this survey cannot be edited until unlocked.
        /// </summary>
        public bool Locked
        {
            get => _locked;
            set => SetProperty(ref _locked, value);
        }
        
        public DateTime? CreationDate
        {
            get => _creationdate;
            set => SetProperty(ref _creationdate, value);
        }
        public bool ReRun
        {
            get => _rerun;
            set => SetProperty(ref _rerun, value);
        }
        public bool HideSurvey
        {
            get => _hidesurvey;
            set => SetProperty(ref _hidesurvey, value);
        }
        public bool NCT
        {
            get => _nct;
            set => SetProperty(ref _nct, value);
        }
        public bool ITCSurvey
        {
            get => _itcsurvey;
            set => SetProperty(ref _itcsurvey, value);
        }

        public double Wave { 
            get => _wave; 
            set => SetProperty(ref _wave, value); 
        }

        public bool HasCorrectedWordings { get => CorrectedQuestions != null & CorrectedQuestions.Count > 0; }

        /// <summary>
        /// Comma-separated list of essential varnames (and their Qnums) in this survey.
        /// </summary>
        /// <remarks>Essential varnames are those that will exit the survey if not answered.</remarks>
        public string EssentialList { get; set; }

        /// <summary>
        /// Comma-separated list of essential VarNames (and their Qnums) in this survey.
        /// </summary>
        /// <remarks>Essential varnames are those that will exit the survey if not answered (i.e. contains text in the form 'go to [varname], then BI9XX').</remarks>
        public string EssentialQuestions 
        { 
            get
            {
                if (Questions == null || Questions.Count==0)
                    return string.Empty;

                Regex rx = new Regex("go to [A-Z][A-Z][0-9][0-9][0-9], then BI9");

                var query = Questions.Where(x => x.PstPW.WordingText != null && rx.IsMatch(x.PstPW.WordingText));

                // if there are any variables with the special PstP instruction, create a list of them
                if (query.Any())
                {
                    return string.Join(", ", query.Select(x => x.VarName + " (" + x.Qnum + ")"));
                }
                else
                    return string.Empty;
                
            } 
        }

        // lists for this survey
        /// <summary>
        /// List of all SurveyQuestion objects for this Survey object. Each representing a single question in the survey.
        /// </summary>
        public List<SurveyQuestion> Questions 
        {
            get => _questions; 
            private set 
            {
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

        public DateTime? LastUpdate { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Blank constructor.
        /// </summary>
        public Survey() {

            Cohort = new SurveyCohort();
            Mode = new SurveyMode();
            Group = new SurveyUserGroup();
            Title = string.Empty;
            SurveyCode = string.Empty;
            WebName = string.Empty;

            CreationDate = DateTime.Today;

            EssentialList = string.Empty;

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
            WebName = string.Empty;

            Cohort = new SurveyCohort();
            Mode = new SurveyMode();
            Group = new SurveyUserGroup();

            EssentialList = string.Empty;


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

        public string GetWebName()
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
        public virtual void AddQuestions(List<SurveyQuestion> questions)
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
            var responses = seriesvars.GroupBy(r => r.RespOptionsS.RespSetName).Select(group => new
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
                qType = sq.QuestionType;

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

                    } while (Questions[i].QuestionType == QuestionType.Heading || Questions[i].QuestionType == QuestionType.InterviewerNote);

                    if (Questions[i].QuestionType == QuestionType.Series)
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
        /// Gets a specific question by it's VarName.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyQuestion object matching the supplied VarName. Returns null if one is not found.</returns>
        public SurveyQuestion QuestionByVar(string varname)
        {
            foreach (SurveyQuestion sq in Questions)
            {
                if (sq.VarName.VarName.Equals(varname))
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

                    sq.PrePW = cq.PrePW;
                    sq.PreIW = cq.PreIW;
                    sq.PreAW = cq.PreAW;
                    sq.LitQW = cq.LitQW;
                    sq.PstIW = cq.PstIW;
                    sq.PstPW = cq.PstPW;
                    sq.RespOptionsS = cq.RespOptionsS;
                    sq.NRCodesS = cq.NRCodesS;
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
                          where r.PrePW.WordingText != null && rx1.IsMatch(r.PrePW.WordingText)
                          select r;

            if (!refVars.Any())
                return;

            foreach (var item in refVars)
            {
                QuestionFilter qf = new QuestionFilter(item.PrePW.WordingText);
                filterList = "";
                for (int i = 0; i < qf.FilterVars.Count; i++)
                {
                    filterVar = qf.FilterVars[i].Varname;
                    var found = Questions.FirstOrDefault(x => x.VarName.RefVarName.Equals(filterVar));

                    if (found != null)
                    {
                        filterRO = found.RespOptionsS.RespList;
                        filterNR = found.NRCodesS.RespList;
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
                            where !string.IsNullOrEmpty(r.PrePW.WordingText) && !r.VarName.VarName.StartsWith("Z")
                            select r;

            // get the variables that are not in standard form
            var oddVars = from r in Questions.AsEnumerable()
                          where !rx1.IsMatch(r.VarName.RefVarName)
                          select r;

            if (!refVars.Any())
                return;

            foreach (var item in refVars)
            {
                QuestionFilter qf = new QuestionFilter(item.PrePW.WordingText, oddVars.ToList());
                filterList = "";
                if (qf.FilterVars == null)
                    continue;

                for (int i = 0; i < qf.FilterVars.Count; i++)
                {
                    filterVar = qf.FilterVars[i].Varname;

                    if (filterVar.Length < 3)
                        continue;

                    var found = Questions.FirstOrDefault(x => x.VarName.RefVarName.Equals(filterVar));

                    if (found != null)
                    {
                        filterRO = found.RespOptionsS.RespList;
                        filterNR = found.NRCodesS.RespList;
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
            qr = new QuestionRouting(sq.PstPW.WordingText, sq.RespOptionsS.RespList);
            sq.RespOptionsS.RespList = qr.ToString();

            // format the non-response options
            qr = new QuestionRouting(sq.PstPW.WordingText, sq.NRCodesS.RespList);
            sq.NRCodesS.RespList = qr.ToString();

            // format the PstP
            sq.PstPW.WordingText = string.Join("\r\n", qr.RoutingText);
        }

        

        /// <summary>
        /// Modifies each wording field in the Survey object's question list to include Qnums before any standard variable name found in the wording.
        /// </summary>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        public void InsertQnums(Enumeration numbering)
        {
            foreach (SurveyQuestion q in Questions)
            {
                q.PrePW.WordingText = InsertQnums(q.PrePW.WordingText, numbering);
                q.PreIW.WordingText = InsertQnums(q.PreIW.WordingText, numbering);
                q.PreAW.WordingText = InsertQnums(q.PreAW.WordingText, numbering);
                q.LitQW.WordingText = InsertQnums(q.LitQW.WordingText, numbering);
                q.PstIW.WordingText = InsertQnums(q.PstIW.WordingText, numbering);
                q.PstPW.WordingText = InsertQnums(q.PstPW.WordingText, numbering);
                q.RespOptionsS.RespList = InsertQnums(q.RespOptionsS.RespList, numbering);
                q.NRCodesS.RespList = InsertQnums(q.NRCodesS.RespList, numbering);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include Qnums before any standard variable name found in the wording.
        /// </summary>
        /// <param name="sq">The question to modify.</param>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        public void InsertQnums(SurveyQuestion sq, Enumeration numbering)
        {
            
            sq.PrePW.WordingText = InsertQnums(sq.PrePW.WordingText, numbering);
            sq.PreIW.WordingText = InsertQnums(sq.PreIW.WordingText, numbering);
            sq.PreAW.WordingText = InsertQnums(sq.PreAW.WordingText, numbering);
            sq.LitQW.WordingText = InsertQnums(sq.LitQW.WordingText, numbering);
            sq.PstIW.WordingText = InsertQnums(sq.PstIW.WordingText, numbering);
            sq.PstPW.WordingText = InsertQnums(sq.PstPW.WordingText, numbering);
            sq.RespOptionsS.RespList = InsertQnums(sq.RespOptionsS.RespList, numbering);
            sq.NRCodesS.RespList = InsertQnums(sq.NRCodesS.RespList, numbering);
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
                q.PrePW.WordingText = InsertOddQnums(q.PrePW.WordingText, numbering);
                q.PreIW.WordingText = InsertOddQnums(q.PreIW.WordingText, numbering);
                q.PreAW.WordingText = InsertOddQnums(q.PreAW.WordingText, numbering);
                q.LitQW.WordingText = InsertOddQnums(q.LitQW.WordingText, numbering);
                q.PstIW.WordingText = InsertOddQnums(q.PstIW.WordingText, numbering);
                q.PstPW.WordingText = InsertOddQnums(q.PstPW.WordingText, numbering);
                q.RespOptionsS.RespList = InsertOddQnums(q.RespOptionsS.RespList, numbering);
                q.NRCodesS.RespList = InsertOddQnums(q.NRCodesS.RespList, numbering);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include Qnums before any non-standard variable name found in the wording.
        /// </summary>
        /// <param name="numbering">Determines whether Qnum or AltQnum is inserted.</param>
        /// <param name="sq">The question to modify.</param>
        public void InsertOddQnums(SurveyQuestion sq, Enumeration numbering)
        {
            sq.PrePW.WordingText = InsertOddQnums(sq.PrePW.WordingText, numbering);
            sq.PreIW.WordingText = InsertOddQnums(sq.PreIW.WordingText, numbering);
            sq.PreAW.WordingText = InsertOddQnums(sq.PreAW.WordingText, numbering);
            sq.LitQW.WordingText = InsertOddQnums(sq.LitQW.WordingText, numbering);
            sq.PstIW.WordingText = InsertOddQnums(sq.PstIW.WordingText, numbering);
            sq.PstPW.WordingText = InsertOddQnums(sq.PstPW.WordingText, numbering);
            sq.RespOptionsS.RespList = InsertOddQnums(sq.RespOptionsS.RespList, numbering);
            sq.NRCodesS.RespList = InsertOddQnums(sq.NRCodesS.RespList, numbering);
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
                Regex rxReplace = new Regex("\\b" + sq.VarName.RefVarName + "\\b", RegexOptions.IgnoreCase);
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
                q.PrePW.WordingText = InsertCountryCodes(q.PrePW.WordingText);
                q.PreIW.WordingText = InsertCountryCodes(q.PreIW.WordingText);
                q.PreAW.WordingText = InsertCountryCodes(q.PreAW.WordingText);
                q.LitQW.WordingText = InsertCountryCodes(q.LitQW.WordingText);
                q.PstIW.WordingText = InsertCountryCodes(q.PstIW.WordingText);
                q.PstPW.WordingText = InsertCountryCodes(q.PstPW.WordingText);
                q.RespOptionsS.RespList = InsertCountryCodes(q.RespOptionsS.RespList);
                q.NRCodesS.RespList = InsertCountryCodes(q.NRCodesS.RespList);
            }
        }

        /// <summary>
        /// Modifies each wording field in a single SurveyQuestion object to include the country code for any standard variable name found in the wording.
        /// </summary>
        /// <param name="sq">The question to modify.</param>
        public void InsertCountryCodes(SurveyQuestion sq)
        {
            sq.PrePW.WordingText = InsertCountryCodes(sq.PrePW.WordingText);
            sq.PreIW.WordingText = InsertCountryCodes(sq.PreIW.WordingText);
            sq.PreAW.WordingText = InsertCountryCodes(sq.PreAW.WordingText);
            sq.LitQW.WordingText = InsertCountryCodes(sq.LitQW.WordingText);
            sq.PstIW.WordingText = InsertCountryCodes(sq.PstIW.WordingText);
            sq.PstPW.WordingText = InsertCountryCodes(sq.PstPW.WordingText);
            sq.RespOptionsS.RespList = InsertCountryCodes(sq.RespOptionsS.RespList);
            sq.NRCodesS.RespList = InsertCountryCodes(sq.NRCodesS.RespList);
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
                        where r.PstPW.WordingText != null && rx.IsMatch(r.PstPW.WordingText)
                        select r;

            // if there are any variables with the special PstP instruction, create a list of them
            if (query.Any())
            {
                foreach (var item in query)
                {
                    if (item.Qnum.Contains("^"))
                        varlist += item.VarName + " (" + item.Qnum.Substring(item.Qnum.LastIndexOf("^")+1) + "), ";
                    else 
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
        /// Returns the variable name immediately following the provided heading question.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSubSectionLowerBound(SurveyQuestion sq)
        {
            if (!sq.IsSubHeading())
                return sq.VarName.VarName;

            int index = 0;

            for (int i = 0; i < Questions.Count; i++)
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
            if (Questions[index + 1].VarName.VarName.StartsWith("Z"))
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
            if (!sq.IsHeading())
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

                if (Questions[i].IsHeading() && inSection)
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
        /// Returns the variable name immediately preceding the heading that follows the provided heading question.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSubSectionUpperBound(SurveyQuestion sq)
        {
            if (!sq.IsSubHeading())
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

                if ((Questions[i].IsHeading() || Questions[i].IsSubHeading()) && inSection)
                {
                    index = i - 1;
                    break;
                }


            }
            // next heading not found, so we must be looking for the end of the survey
            if (index == 0)
                return Questions[Questions.Count - 1].VarName.VarName;

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
       
            for (int i = index; i >= 0; i--)
            {
                if (Questions[i].IsHeading())
                    return Questions[i].PrePW.WordingText.Replace("&amp;", "&");
            }

            // if we are in the part of the survey before any headings, leave it blank
            return string.Empty;
            
        }

        public int GetSectionCount(SurveyQuestion sq)
        {
            if (QuestionByID(sq.ID) == null)
                return 0;

            // if there are no headings, the count is the whole question list
            if (!Questions.Any(x => x.IsHeading()))
                return Questions.Count;

            if (sq.IsHeading())
                return QuestionsUntilNextSection(sq);

            int before = QuestionsSinceLastSection(sq);

            int after = QuestionsUntilNextSection(sq);

            int total = before + after + 1;

            return total;
        }

        private int IndexOf(SurveyQuestion question)
        {
            int index = 0;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].VarName.Equals(question.VarName))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private int QuestionsUntilNextSection(SurveyQuestion surveyQuestion)
        {
            int index = IndexOf(surveyQuestion);
            int after = 0;
            for (int i = index+1; i < Questions.Count; i++)
            {
                if (Questions[i].IsHeading())
                    break;
                after++;
            }
            return after;
        }

        private int QuestionsSinceLastSection(SurveyQuestion surveyQuestion)
        {
            int index = IndexOf(surveyQuestion);
            int before = 0;
            for (int i = index-1; i > 0; i--)
            {
                if (Questions[i].IsHeading())
                    break;
                before++;
            }
            return before;
        }

        public string GetParallelVars(SurveyQuestion q)
        {
            var questionList = Questions.Where(x => x.VarName.Topic.ID == q.VarName.Topic.ID && x.VarName.Content.ID == q.VarName.Content.ID && x.ID != q.ID);

            StringBuilder list = new StringBuilder();


            foreach (SurveyQuestion sq in questionList) {
                if (sq.VarName.Product.ID != q.VarName.Product.ID)
                    list.Append("<strong><u>" + sq.GetRefVarName() + "</u></strong> (" + sq.VarName.Product.LabelText + ")<br>" + sq.GetQuestionTextHTML_Shorter() + "<br>");
                else
                    list.Append("<strong><u>" + sq.GetRefVarName() + "</u></strong> (" + sq.VarName.Product.LabelText + ")<br>");
                
            }

            return list.ToString();
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

        public Survey Clone()
        {
            return new Survey
            {
                SID = this.SID,
                WaveID = this.WaveID,
                SurveyCode = this.SurveyCode,
                SurveyCodePrefix = this.SurveyCodePrefix,
                Title = this.Title,
                
                Cohort = new SurveyCohort()
                {
                    ID = this.Cohort.ID,
                    Cohort = this.Cohort.Cohort,
                    WebName = this.Cohort.WebName,
                },
                Mode = new SurveyMode()
                {
                    ID = this.Mode.ID,
                    Mode = this.Mode.Mode,
                    ModeAbbrev = this.Mode.ModeAbbrev
                },
                CountryCode = this.CountryCode,
                WebName = this.WebName,
                EnglishRouting = this.EnglishRouting,
                Locked = this.Locked,
                CreationDate = this.CreationDate,
                ReRun = this.ReRun,
                HideSurvey = this.HideSurvey,
                NCT = this.NCT,
                ITCSurvey = this.ITCSurvey,
                Wave = this.Wave,
                UserStates = this.UserStates.Select(us => new SurveyUserState() { ID = us.ID, SurvID = us.SurvID, State = us.State }).ToList(),
                ScreenedProducts = this.ScreenedProducts.Select(sp => new SurveyScreenedProduct() { ID = sp.ID, SurvID = sp.SurvID, Product = sp.Product }).ToList(),
                LanguageList = this.LanguageList.Select(l => new SurveyLanguage() { ID = l.ID, SurvID = l.SurvID, SurvLanguage = l.SurvLanguage }).ToList(),


            };
        }

        #region Private Backing Variables
        private int _sid;
        private int _waveid;
        private string _surveycode;
        private string _surveycodeprefix;
        private string _title;
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
        private double _wave;
        private List<SurveyQuestion> _questions;
        
        #endregion
    }
}
