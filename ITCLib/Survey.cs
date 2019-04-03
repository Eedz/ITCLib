﻿using System;
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
    // IDEA: add method for renumbering
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
        private int _sid;
        public int SID
        {
            get
            {
                return this._sid;
            }
            set
            {
                if (value!= this._sid)
                {
                    this._sid = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Survey code for the survey referenced by this object
        /// </summary>
        private string _surveycode;
        public string SurveyCode
        {
            get
            {
                return this._surveycode;
            }
            set
            {
                if (value != this._surveycode)
                {
                    this._surveycode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Full title of this survey.
        /// </summary>
        private string _title;
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (value != this._title)
                {
                    this._title = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Languages that this survey was translated into.
        /// </summary>
        private string _language;
        public string Languages
        {
            get
            {
                return this._language;
            }
            set
            {
                if (value != this._language)
                {
                    this._language = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// User group that this survey if meant for.
        /// </summary>
        private SurveyUserGroup _group;
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
        private SurveyCohort _cohort;
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
                }
            }
        }
        /// <summary>
        /// The survey mode. Telephone, web, or face to face.
        /// </summary>
        private SurveyMode _mode;
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
                }
            }
        }
        /// <summary>
        /// Country specific 2-digit code.
        /// </summary>
        private int _countrycode;
        public int CountryCode
        {
            get
            {
                return _countrycode;
            }
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
        private string _webname;
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
        private bool _englishrouting;
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
        private bool _locked;
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

        /// <summary>
        /// The wave that this survey belongs to.
        /// </summary>
        private int _waveid;
        public int WaveID
        {
            get
            {
                return _waveid;
            }
            set
            {
                if (value != _waveid)
                {
                    _waveid = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime? _creationdate;
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

        private bool _rerun;
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
        private bool _hidesurvey;
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
        private bool _nct;
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




        /// <summary>
        /// Comma-separated list of essential varnames (and their Qnums) in this survey.
        /// </summary>
        /// <remarks>Essential varnames are those that will exit the survey if not answered.</remarks>
        public string EssentialList { get; set; }                   

        // lists for this survey
        /// <summary>
        /// List of all SurveyQuestion objects for this Survey object. Each representing a single question in the survey.
        /// </summary>
        public BindingList<SurveyQuestion> Questions { get; set; }

        /// <summary>
        /// List of all SurveyQuestion objects for this Survey object which are designated as 'corrected.'
        /// </summary>
        /// <remarks>A corrected question is one that has content different than what appeared in the fieldwork.</remarks>
        public List<SurveyQuestion> CorrectedQuestions { get; set; }

        // this list contains any VarNames found in the survey wordings that are not questions themselves within the survey (TODO could be moved to SurveyQuestion object)
        public List<string> QNUlist;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

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

        #region Constructors
        // blank constructor

        /// <summary>
        /// Blank constructor.
        /// </summary>
        public Survey() {
            
            SurveyCode = "";
            WebName = "";

            EssentialList = "";

            Questions = new BindingList<SurveyQuestion>();
            QNUlist = new List<string>();
        }

        
        #endregion 

        #region Methods and Functions

        /// <summary>
        /// TODO make Questions list read only, so any adds have to go through this method
        /// </summary>
        /// <param name="newQ"></param>
        public void AddQuestion(SurveyQuestion newQ, int afterIndex)
        {
          //  Questions.Add(newQ);
            Questions.Insert(afterIndex, newQ);
          //  List<SurveyQuestion> sorted = new List<SurveyQuestion>(Questions);
           // sorted.Sort((x, y) => x.Qnum.CompareTo(y.Qnum));
           //Questions = new BindingList<SurveyQuestion>( sorted);
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
        /// Apply corrected wordings to the questions list. Overwrites the wording fields in the questions list with those found in the correctedQuestions list.
        /// </summary>        
        public void CorrectWordings()
        {
            foreach (SurveyQuestion cq in CorrectedQuestions)
            {
                try
                {
                    SurveyQuestion sq = Questions.Single(x => x.VarName == cq.VarName);

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

        // TODO also need to get non-standard vars
        // TODO check for response codes mentioned in filter and only show those
        /// <summary>
        /// Sets the Filter property for each SurveyQuestion that has a PreP containing a VarName.
        /// </summary>
        public void MakeFilterList()
        {
            
            string filterList = "";
            string filterVar = "";
            string filterRO;
            string filterNR;
            string filterLabel;
            
            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9]");

            // get any rows that contain a variable
            var refVars = from r in Questions.AsEnumerable()
                          where r.PreP != null && rx1.IsMatch(r.PreP)
                          select r;

            if (!refVars.Any())
                return;

            foreach (var item in refVars)
            {
                QuestionFilter qf = new QuestionFilter(item.PreP);

                for (int i = 0; i < qf.FilterVars.Count; i++)
                {
                    filterVar = qf.FilterVars[i].Varname;
                    filterRO = Questions.Single(x => x.refVarName == filterVar).RespOptions;
                    filterNR = Questions.Single(x => x.refVarName == filterVar).NRCodes; 
                    filterLabel = Questions.Single(x => x.refVarName == filterVar).VarLabel; 

                    filterList += "<strong>" + filterVar.Substring(0, 2) + "." + filterVar.Substring(2) + "</strong>\r\n<em>" +
                        filterLabel + "</em>\r\n" + filterRO + "\r\n" + filterNR + "\r\n";
                }

                item.Filters = filterList;

            }

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

        // TODO remove whitespace around each option before adding read out instruction
        // TODO check for tags before adding readOut string TEST THIS
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

            result = string.Join("\n\r", options);

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
        /// TODO: Could be optimized - exit if no match is found in the whole string, before looking at each word in the string.
        /// </summary>
        /// <remarks> Each word in the string is compared to the VarName pattern.</remarks>
        /// <param name="wording"></param>
        /// <param name="numbering"></param>
        /// <returns></returns>
        private string InsertQnums(string wording, Enumeration numbering = Enumeration.Qnum)
        {
            string newwording = wording;
            string[] words;
            string qnum;
            string varname;
            MatchCollection m;
            Regex rx = new Regex("[A-Z]{2}\\d{3}");
            // split the wording into words            
            words = newwording.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                // if words[i] contains a variable name, look up the qnum and place it before the variable
                if (rx.Match(words[i]).Success)
                {
                    m = rx.Matches(words[i]);
                    varname = m[0].Groups[0].Value;
                    switch (numbering)
                    {
                        case Enumeration.Both:
                        case Enumeration.Qnum:
                            qnum = Questions.Single(x => x.refVarName == varname).Qnum;
                            break;
                        case Enumeration.AltQnum:
                            qnum = Questions.Single(x => x.refVarName == varname).AltQnum;
                            break;
                        default:
                            qnum = Questions.Single(x => x.refVarName == varname).Qnum;
                            break;
                    }

                    if (qnum.Equals(""))
                    {
                        QNUlist.Add(varname);
                        qnum = "QNU";
                    }

                    words[i] = rx.Replace(words[i], qnum + "/" + varname);
                }
            }
            newwording = string.Join(" ", words);
            return newwording;
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

        // TODO
        private string InsertOddQnums(string wording, Enumeration numbering)
        {
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

        private string InsertCountryCodes(string wording)
        {
            string newwording = wording;
            string[] words;
            string varname;
            MatchCollection m;
            Regex rx = new Regex("[A-Z]{2}\\d{3}");
            // split the wording into words            
            words = newwording.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                // if words[i] contains a variable name, replace the variable with the country coded version
                if (rx.Match(words[i]).Success)
                {
                    m = rx.Matches(words[i]);
                    varname = m[0].Groups[0].Value;
                    varname = Utilities.ChangeCC(varname, CountryCode);

                    words[i] = rx.Replace(words[i], varname);
                }
            }
            newwording = string.Join(" ", words);
            return newwording;
        }

        /// <summary>
        /// Sets the 'essentialList' property by compiling a list of VarNames that contain the special routing instruction that only essential 
        /// questions have.
        /// </summary>
        public void GetEssentialQuestions()
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
        /// TODO what if a heading follows?
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSectionLowerBound(SurveyQuestion sq)
        {
            if (!sq.VarName.StartsWith("Z"))
                return sq.VarName;

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

            // if a heading is the next question return nothing
            if (Questions[index+1].VarName.StartsWith("Z"))
                return "";

            return Questions[index + 1].VarName;
        }

        /// <summary>
        /// Returns the variable name immediately preceding the heading that follows the provided heading question.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public string GetSectionUpperBound(SurveyQuestion sq)
        {
            if (!sq.VarName.StartsWith("Z"))
                return sq.VarName;

            int index = 0;
            bool inSection = false;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].VarName.Equals(sq.VarName))
                {
                    inSection = true;
                    continue;
                }

                if (Questions[i].VarName.StartsWith("Z") && inSection)
                {
                    index = i-1;
                    break;
                }


            }
            // next heading not found, so we must be looking for the end of the survey
            if (index == 0) 
                return Questions[Questions.Count-1].VarName;

            // if the next heading is the next question return nothing
            if (Questions[index].VarName.Equals(sq.VarName))
                return "";
            
            return Questions[index].VarName;
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

            return sb.ToString();
        }

        #endregion

    }
}