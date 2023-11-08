using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ITCLib
{
    public class SurveyQuestion : INotifyPropertyChanged // VariableName too?
    {
        // IDEA create a wording object which has (int, string, string) for (W#, field, wording), then a question has a collection of wordings

        #region Properties

        public int ID; // question ID
        public string SurveyCode { get; set; }

        public VariableName VarName { get; set; }
        
        public string Qnum
        {
            get { return _qnum; }
            set
            {
                if (value != _qnum)
                {
                    string old = _qnum;
                    _qnum = value;
                    NotifyPropertyChanged(old,value);
                }
            }
        }
        public string AltQnum
        {
            get
            {
                return _altqnum;
            }
            set {
                if (value != _altqnum)
                {
                    string old = _altqnum;
                    _altqnum = value;
                    NotifyPropertyChanged(old, value, "AltQnum");
                }
            }
        }   
        public string AltQnum2 {
            get
            {
                return _altqnum2;
            }
            set
            {
                if (value != _altqnum2)
                {
                    string old = _altqnum2;
                    _altqnum2 = value;
                    NotifyPropertyChanged(old, value, "AltQnum2");
                }
            }
        }
        public string AltQnum3 {
            get
            {
                return _altqnum3;
            }
            set
            {
                if (value != _altqnum3)
                {
                    string old = _altqnum3;
                    _altqnum3 = value;
                    NotifyPropertyChanged(old, value, "AltQnum3");
                }
            }
        }

        // wordings
        //public Wording PreP { get; set; }


        public string PreP { get { return _prep; } set { string old = _prep; _prep = FixElements(value); PrepRTF = FormatText(value); NotifyPropertyChanged(old, value, "PreP"); } }
        public string PrepRTF { get; private set; }        
        public int PrePNum
        {
            get { return _prepnum; }
            set
            {
                if (value != _prepnum)
                {
                    int old = _prepnum;
                    _prepnum = value;
                     NotifyPropertyChanged(old, value, "PrePNum");
                }
            }
        }

        public string PreI { get { return _prei; } set { _prei = FixElements(value); PreiRTF = FormatText(value); } }//NotifyPropertyChanged(); } }
        public string PreiRTF { get; private set; }
        public int PreINum {
            get
            {
                return _preinum;
            }
            set
            {
                if (value != _preinum)
                {
                    int old = _preinum;
                    _preinum = value;
                    NotifyPropertyChanged(old, value, "PreINum");
                }
            }
        }
              
        public string PreA { get { return _prea; } set { _prea = FixElements(value); PreaRTF = FormatText(value); } } //NotifyPropertyChanged(); } }
        public string PreaRTF { get; private set; }
        public int PreANum {
            get
            {
                return _preanum;
            }
            set
            {
                if (value != _preanum)
                {
                    int old = _preanum;
                    _preanum = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }
        
        public string LitQ { get { return _litq; } set { _litq = FixElements(value); LitqRTF = FormatText(value); } }// NotifyPropertyChanged(); } }
        public string LitqRTF { get; private set; }
        public int LitQNum {
            get
            {
                return _litqnum;
            }
            set
            {
                if (value != _litqnum)
                {
                    int old = _litqnum;
                    _litqnum = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }
        
        public string PstI { get { return _psti; } set { _psti = FixElements(value); PstiRTF = FormatText(value); } }// NotifyPropertyChanged(); } }
        public string PstiRTF { get; private set; }
        public int PstINum {
            get
            {
                return _pstinum;
            }
            set
            {
                if (value != _pstinum)
                {
                    int old = _pstinum;
                    _pstinum = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }       
        
        public string PstP { get { return _pstp; } set { _pstp = FixElements(value); PstpRTF = FormatText(value); } }// NotifyPropertyChanged(); } }
        public string PstpRTF { get; private set; }
        public int PstPNum {
            get
            {
                return _pstpnum;
            }
            set
            {
                if (value != _pstpnum)
                {
                    int old = _pstpnum;
                    _pstpnum = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }
        
        public string RespOptions { get { return _respoptions; } set { _respoptions = FixElements(value); RespOptionsRTF = FormatText(value); } }// NotifyPropertyChanged(); } }
        public string RespOptionsRTF { get; private set; }
        public string RespName {
            get
            {
                return _respname;
            }
            set
            {
                if (value != _respname)
                {
                    string old = _respname;
                    _respname = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }
        public string RespNameLower { get { return _respname.ToLower(); } }
        
        
        public string NRCodes { get { return _nrcodes; } set { _nrcodes = FixElements(value); NRCodesRTF = FormatText(value); } }// NotifyPropertyChanged(); } }
        public string NRCodesRTF { get; private set; }
        public string NRName
        {
            get
            {
                return _nrname;
            }
            set
            {
                if (value != _nrname)
                {
                    string old = _nrname;
                    _nrname = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }
        public string NRNameLower { get { return _nrname.ToLower(); } }


        // field info

        public bool ProgrammerOnly { get; set; }

        public bool TableFormat { get; set; }
        public bool ScriptOnly { get; set; }
        public string NumFmt { get; set; }

        private bool _correctedflag;
        public bool CorrectedFlag {
            get
            {
                return _correctedflag;
            }
            set
            {
                if (value != _correctedflag)
                {
                    bool old = _correctedflag;
                    _correctedflag = value;
                    NotifyPropertyChanged(old, value);
                }
            }
        }

        public List<Translation> Translations { get; set; }

        public List<QuestionComment> Comments { get; set; }

        private string _filters;
        public string Filters {
            get
            {
                return _filters;
            }
            set
            {
                if (value != _filters)
                {
                    _filters = value;
                    //NotifyPropertyChanged();
                }
            }
        }

        public bool Internal { get; set; } // dervied variables, programmer notes, routing screens, headings?

        public List<VariableName> PreviousNameList { get; set; }

        public List<QuestionTimeFrame> TimeFrames { get; set; }

        public string FilterDescription
        {
            get { return _plainfilter; }
            set
            {
                if (value != _plainfilter)
                {
                    string old = _plainfilter;
                    
                    _plainfilter = FixElements(value);
                    FilterDescriptionRTF = FormatText(value);
                    NotifyPropertyChanged(old, value);
                }

            }
        }

        public string FilterDescriptionRTF { get; private set; }

        public string QuestionText
        {
            get { return GetQuestionTextWPF(); }
        }
        #endregion

        #region Events
        public virtual event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public SurveyQuestion()
        {
            VarName = new VariableName("");
            Qnum = "";

            PreP = "";
            PreI = "";
            PreA = "";
            LitQ = "";
            PstI = "";
            PstP = "";
            RespName = "0";
            RespOptions = "";
            NRName = "0";
            NRCodes = "";
            FilterDescription = string.Empty;
            Filters = string.Empty;

            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();

            PreviousNameList = new List<VariableName>();
            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;

            TimeFrames = new List<QuestionTimeFrame>();
        }

        public SurveyQuestion(string var)
        {
            VarName = new VariableName(var);
            Qnum = "";

            PreP = "";
            PreI = "";
            PreA = "";
            LitQ = "";
            PstI = "";
            PstP = "";
            RespName = "0";
            RespOptions = "";
            NRName = "0";
            NRCodes = "";
            FilterDescription = string.Empty;
            Filters = string.Empty;

            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();

            PreviousNameList = new List<VariableName>();
            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;
            TimeFrames = new List<QuestionTimeFrame>();
        }

        public SurveyQuestion(string var, string qnum)
        {
            VarName = new VariableName(var);
            Qnum = qnum;

            PreP = "";
            PreI = "";
            PreA = "";
            LitQ = "";
            PstI = "";
            PstP = "";
            RespName = "0";
            RespOptions = "";
            NRName = "0";
            NRCodes = "";
            FilterDescription = string.Empty;
            Filters = string.Empty;

            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();

            PreviousNameList = new List<VariableName>();
            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;
            TimeFrames = new List<QuestionTimeFrame>();
        }

        public SurveyQuestion(string surveyCode, string var, ProductLabel product)
        {
            SurveyCode = surveyCode;
            VarName = new VariableName(var);
            VarName.Product = product;

            PreP = string.Empty;
            PreI = string.Empty;
            PreA = string.Empty;
            LitQ = string.Empty;
            PstI = string.Empty;
            PstP = string.Empty;
            RespName = "0";
            RespOptions = string.Empty;
            NRName = "0";
            NRCodes = string.Empty;
            FilterDescription = string.Empty;
            Filters = string.Empty;

            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();

            PreviousNameList = new List<VariableName>();
            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;
            TimeFrames = new List<QuestionTimeFrame>();
        }

        public SurveyQuestion DeepCopyWordings()
        {
            SurveyQuestion copy = new SurveyQuestion();
            copy.Qnum = string.Copy(Qnum);
            copy.VarName = new VariableName(VarName.VarName);
            copy.PrePNum = PrePNum; copy.PreINum = PreINum; 
            copy.PreANum = PreANum; copy.LitQNum = LitQNum;
            copy.RespName = string.Copy(RespName);
            copy.NRName = string.Copy(NRName);

            copy.PreP = string.Copy(PreP);
            copy.PreI = string.Copy(PreI);
            copy.PreA = string.Copy(PreA);
            copy.LitQ = string.Copy(LitQ);
            copy.PstI = string.Copy(PstI);
            copy.PstP = string.Copy(PstP);
            copy.RespOptions = string.Copy(RespOptions);
            copy.NRCodes = string.Copy(NRCodes);
            copy.Filters = string.Copy(Filters);
            copy.FilterDescription = string.Copy(FilterDescription);

            foreach (Translation t in Translations)
            {
                copy.Translations.Add(new Translation
                {
                    ID = t.ID,
                    QID = t.QID,
                    LitQ = string.Copy(t.LitQ ?? string.Empty),
                    LanguageName = t.LanguageName,
                    TranslationText = string.Copy(t.TranslationText ?? string.Empty),
                    Bilingual = t.Bilingual
                });
            }
            return copy;
        }

        public SurveyQuestion Copy()
        {
            SurveyQuestion sq;

            sq = new SurveyQuestion
            {
                
                VarName = VarName,
                
                Qnum = Qnum,
                AltQnum = AltQnum,
                AltQnum2 = AltQnum2,
                AltQnum3 = AltQnum3,
                PreviousNameList = PreviousNameList,
                //PreP = new Wording(this.PreP.ID, this.PreP.WordingText),
                PrePNum = PrePNum,
                PreP = PreP,
                PreINum = PreINum,
                PreI = PreI,
                PreANum = PreANum,
                PreA = PreA,
                LitQNum = LitQNum,
                LitQ = LitQ,
                PstINum = PstINum,
                PstI = PstI,
                PstPNum = PstPNum,
                PstP = PstP,
                RespName = RespName,
                RespOptions = RespOptions,
                NRName = NRName,
                NRCodes = NRCodes,
               
                CorrectedFlag = CorrectedFlag,

                TimeFrames = TimeFrames,
                FilterDescription = FilterDescription,
                Filters = Filters
            };

            return sq;
        }

        //private void WordingChanged(object o, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        //    }
        //}

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected virtual void NotifyPropertyChanged<T>(T oldValue, T newValue, [CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
            }
        }
        


        private string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ");
        }

        private string FormatText(string wordingText)
        {
            string wording = FixElements(wordingText);

            wording = wording.Replace("<strong>", @"\b ");
            wording = wording.Replace("</strong>", @"\b0 ");
            wording = wording.Replace("<em>", @"\i ");
            wording = wording.Replace("</em>", @"\i0 ");
            wording = wording.Replace("<br>", @"\line ");
            wording = wording.Replace("\r\n", @"\line ");
            wording = wording.Replace("<u>", @"\ul ");
            wording = wording.Replace("</u>", @"\ul0 ");
            wording = wording.Replace("[bullet]", @"\bullet ");
            wording = @"{\rtf1\ansi " + wording + "}";

            return wording;
        }

        public string GetFullVarLabel()
        {
            if (TimeFrames.Count == 0)
                return VarName.VarLabel;

            // concatenate the list of time frames
            string timeframeList = string.Join(", ", TimeFrames.Select(x => x.TimeFrame));

            // insert in between {} if present
            if (VarName.VarLabel.Contains("{") && VarName.VarLabel.Contains("}"))
            {
                return VarName.VarLabel.Replace("{}", "{" +  timeframeList + "}");
            }
            // append if {} is not present
            return VarName.VarLabel + " - {" + timeframeList + "}";

        }

        public string GetQuestionText(List<string> stdFieldsChosen, bool colorLitQ = false, string newline = "\r\n")
        {
            string questionText = "";

            if (stdFieldsChosen.Contains("PreP") && !string.IsNullOrEmpty(PreP)) { questionText += "<strong>" + PreP + "</strong>" + newline; }
            if (stdFieldsChosen.Contains("PreI") && !string.IsNullOrEmpty(PreI)) { questionText += "<em>" + PreI + "</em>" + newline; }
            if (stdFieldsChosen.Contains("PreA") && !string.IsNullOrEmpty(PreA)) { questionText += PreA + newline; }

            if (stdFieldsChosen.Contains("LitQ") && !string.IsNullOrEmpty(LitQ)) {
                if (colorLitQ)
                    questionText += "[indent][lblue]" + LitQ + "[/lblue][/indent]" + newline;
                else
                    questionText += "[indent]" + LitQ + "[/indent]" + newline;
            }

            if (stdFieldsChosen.Contains("RespOptions") && !string.IsNullOrEmpty(RespOptions)) { questionText += "[indent3]" + RespOptions + "[/indent3]" + newline; }
            if (stdFieldsChosen.Contains("NRCodes") && !string.IsNullOrEmpty(NRCodes)) { questionText += "[indent3]" + NRCodes + "[/indent3]" + newline; }
            if (stdFieldsChosen.Contains("PstI") && !string.IsNullOrEmpty(PstI)) { questionText += "<em>" + PstI + "</em>" + newline; }
            if (stdFieldsChosen.Contains("PstP") && !string.IsNullOrEmpty(PstP)) { questionText += "<strong>" + PstP + "</strong>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);
            

            return questionText;
        }

        public string GetQuestionText(string newline = "\r\n")
        {
            string questionText = "";

            if (!string.IsNullOrEmpty(PreP)) { questionText += "<strong>" + PreP + "</strong>" + newline; }
            if (!string.IsNullOrEmpty(PreI)) { questionText += "<em>" + PreI + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PreA)) { questionText += PreA + newline; }

            if (!string.IsNullOrEmpty(LitQ)) { questionText += "[indent]" + LitQ + "[/indent]" + newline; }

            if (!string.IsNullOrEmpty(RespOptions)) { questionText += "[indent3]" + RespOptions + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(NRCodes)) { questionText += "[indent3]" + NRCodes + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(PstI)) { questionText += "<em>" + PstI + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PstP)) { questionText += "<strong>" + PstP + "</strong>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionTextWPF(string newline = "\r\n")
        {
            string questionText = "";

            if (!string.IsNullOrEmpty(PreP)) { questionText += "<Bold>" + PreP + "</Bold>" + newline; }
            if (!string.IsNullOrEmpty(PreI)) { questionText += "<Italic>" + PreI + "</Italic>" + newline; }
            if (!string.IsNullOrEmpty(PreA)) { questionText += PreA + newline; }

            if (!string.IsNullOrEmpty(LitQ)) { questionText += "<TextBlock TextIndent=\"20\">" + LitQ + "</TextBlock>" + newline; }

            if (!string.IsNullOrEmpty(RespOptions)) { questionText += "<TextBlock TextIndent=\"50\">" + RespOptions + "</TextBlock>" + newline; }
            if (!string.IsNullOrEmpty(NRCodes)) { questionText += "<TextBlock TextIndent=\"50\">" + NRCodes + "</TextBlock>" + newline; }
            if (!string.IsNullOrEmpty(PstI)) { questionText += "<Italic>" + PstI + "</Italic>" + newline; }
            if (!string.IsNullOrEmpty(PstP)) { questionText += "<Bold>" + PstP + "</Bold>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionTextPlain(string newline = "\r\n")
        {
            string questionText = "";

            if (!string.IsNullOrEmpty(PreP)) { questionText += PreP + newline; }
            if (!string.IsNullOrEmpty(PreI)) { questionText += PreI + newline; }
            if (!string.IsNullOrEmpty(PreA)) { questionText += PreA + newline; }

            if (!string.IsNullOrEmpty(LitQ)) { questionText += LitQ + newline; }

            if (!string.IsNullOrEmpty(RespOptions)) { questionText += RespOptions + newline; }
            if (!string.IsNullOrEmpty(NRCodes)) { questionText += NRCodes + newline; }
            if (!string.IsNullOrEmpty(PstI)) { questionText += PstI + newline; }
            if (!string.IsNullOrEmpty(PstP)) { questionText += PstP; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionText(bool colorLitQ)
        {
            string questionText = "";
            string newline = "\r\n";

            if (!string.IsNullOrEmpty(PreP)) { questionText += "<strong>" + PreP + "</strong>" + newline; }
            if (!string.IsNullOrEmpty(PreI)) { questionText += "<em>" + PreI + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PreA)) { questionText += PreA + newline; }

            if (!string.IsNullOrEmpty(LitQ))
            {
                if (colorLitQ)
                    questionText += "[indent][lblue]" + LitQ + "[/lblue][/indent]" + newline;
                else
                    questionText += "[indent]" + LitQ + "[/indent]" + newline;
            }

            if (!string.IsNullOrEmpty(RespOptions)) { questionText += "[indent3]" + RespOptions + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(NRCodes)) { questionText += "[indent3]" + NRCodes + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(PstI)) { questionText += "<em>" + PstI + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PstP)) { questionText += "<strong>" + PstP + "</strong>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionTextRich(bool colorLitQ = false)
        {
            string questionText = "";
            string newline = @"\line ";

            if (colorLitQ)
                questionText += @"{\colortbl;\red0\green0\blue255;}";

            if (!string.IsNullOrEmpty(PreP))
                questionText += @"{\pard\b " + PrepRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\b0\par}";

            if (!string.IsNullOrEmpty(PreI))
                questionText += @"{\pard\i " + PreiRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\i0\par}";

            if (!string.IsNullOrEmpty(PreA))
                questionText += @"{\pard " + PreaRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\par}";

            if (!string.IsNullOrEmpty(LitQ))
            {
                if (colorLitQ)
                    questionText += @"{\pard\li100\cf1 " + LitqRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\par}";
                else
                    questionText += @"{\pard\li100 " + LitqRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\par}";
            }

            if (!string.IsNullOrEmpty(RespOptions))
                questionText += @"{\pard\li300 " + RespOptionsRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\par}";

            
            if (!string.IsNullOrEmpty(NRCodes))
                questionText += @"{\pard\li300 ------------------\line " + NRCodesRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\par}";

            if (!string.IsNullOrEmpty(PstI))
                questionText += @"{\pard\i " + PstiRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\i0\par}" ;
            if (!string.IsNullOrEmpty(PstP))
                questionText += @"{\pard\b " + PstpRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\b0\par}";

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);
            if (questionText.StartsWith(@"\par"))
                questionText = questionText.Substring(4);

            return @"{\rtf1\ansi " + questionText + "}";
        }

        public string GetEnglishRoutingTranslation(string lang, RoutingStyle routingStyle = RoutingStyle.Normal)
        {
            if (Translations == null || Translations.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (Translation t in Translations)
            {
                if (t.Language.Equals(lang))
                {
                    sb.Append(t.TranslationText);
                    // if heading and there is a translation do not proceed to English Routing
                    if (VarName.VarName.StartsWith("Z") && !string.IsNullOrEmpty(sb.ToString()))
                        continue;
                    // if not a heading and there is no translation, do not add English Routing
                    if (!VarName.VarName.StartsWith("Z") && string.IsNullOrEmpty(t.TranslationText))
                        continue;


                    if (!string.IsNullOrEmpty(PreP))
                    {
                        // insert PreP in the desired style
                        switch (routingStyle)
                        {
                            case RoutingStyle.Normal:
                                sb.Insert(0, "<strong>" + PreP + "</strong>\r\n");
                                break;
                            case RoutingStyle.Grey:
                                sb.Insert(0, "<strong><Font Color=#a6a6a6>" + PreP + "</Font></strong>\r\n");
                                break;
                            case RoutingStyle.None:
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(PstP))
                    {
                        switch (routingStyle)
                        {
                            case RoutingStyle.Normal:
                                sb.Append("\r\n<strong>" + PstP + "</strong>");
                                break;
                            case RoutingStyle.Grey:
                                sb.Append("\r\n<strong><Font Color=#a6a6a6>" + PstP + "</Font></strong>");
                                break;
                            case RoutingStyle.None:
                                break;
                        }
                    }

                    break;
                }
            }
            return sb.ToString();
        }

        public string GetTranslationText(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return string.Empty;

            foreach (Translation t in Translations)
            {
                if (t.LanguageName.LanguageName.Equals(lang))
                    return t.TranslationText;
            }

            return string.Empty;
        }


        public Translation GetTranslation(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return null;

            foreach (Translation t in Translations)
            {
                if (t.LanguageName.LanguageName.Equals(lang))
                    return t;
            }

            return null;
        }

        public string GetComments()
        {
            string comments = "";
            foreach (QuestionComment qc in Comments)
            {
                comments += qc.GetComments() + "\r\n";
            }

            return Utilities.TrimString(comments, "\r\n");
        }

        /// <summary>
        /// Returns the Qnum formatted as a normal Qnum. If the Qnum contains 2 qnums, with a ^, this returns the 2nd qnum.
        /// </summary>
        /// <returns></returns>
        public string GetQnum()
        {
            // if contains ^ (re-inserted mark) get the 2nd qnum
            // else return the whole qnum
            if (Qnum.Contains ("^"))
                return Qnum.Substring(Qnum.LastIndexOf("^") + 1);
            else
                return Qnum;
        }

        /// <summary>
        /// Returns the integer value of the Qnum
        /// </summary>
        /// <returns></returns>
        public int GetQnumValue()
        {
            return Int32.Parse(Qnum.Substring(0, 3));
        }

        public List<string> GetFilterVars()
        {
            List<string> filterVars = new List<string>();

            if (string.IsNullOrEmpty(PreP))
                return filterVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(PreP);

            if (matches.Count == 0)
                return filterVars;

            foreach (Match m in matches)
            {
                filterVars.Add(m.Value);
            }

            return filterVars;
        }

        /// <summary>
        /// Return a list of FilterInstruction objects found in this question's PreP. 
        /// </summary>
        /// <returns></returns>
        public List<FilterInstruction> GetFilterInstructions()
        {
            List<FilterInstruction> filterVars = new List<FilterInstruction>();

            if (string.IsNullOrEmpty(PreP))
                return filterVars;

            if (!PreP.Contains("Ask if"))
                return filterVars;

            // check if "any of" list exists

            if (PreP.StartsWith("Ask if any of ("))
            {
                filterVars.AddRange(GetAnyOfList());
            }
                

            Regex rx1 = new Regex("([A-Z][A-Z][0-9][0-9][0-9][a-z]*)" +
                                "(=|<|>|<>)" +
                                "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                "|([0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+)" +
                                "|([0-9]+\\sand\\s<[0-9]+)" +
                                "|([0-9]+\\sand\\s>[0-9]+)" +
                                "|([0-9]+))");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(PreP);

            if (matches.Count == 0)
                return filterVars;
            
            // get the varname, operation and numbers from the groups in the match

            foreach (Match m in matches)
            {
                string match = m.Groups[0].Value;
                string var = m.Groups[1].Value;
                string condition = m.Groups[2].Value;
                Operation op = Operation.Equals;
                switch(condition)
                {
                    case "=":
                        op = Operation.Equals;
                        break;
                    case "<>":
                        op = Operation.NotEquals;
                        break;
                    case "<":
                        op = Operation.LessThan;
                        break;
                    case ">":
                        op = Operation.GreaterThan;
                        break;
                }
                string number = m.Groups[3].Value;

                if (string.IsNullOrEmpty(number))
                    continue;

                // determine number range if present
                List<int> numbers = GetNumberRange(number);
                List<string> numbersStr;
                if (numbers.Count() < 100)
                {
                    numbersStr = GetNumberRangeStr(number);
                }
                else
                {
                    numbersStr = new List<string> { numbers[0].ToString(), numbers[numbers.Count-1].ToString() };
                }

                bool range = number.Contains("-");

                FilterInstruction fi = new FilterInstruction();
                fi.VarName = var;
                fi.Oper = op;
                fi.Values = numbers;
                fi.ValuesStr = numbersStr;
                fi.Range = range;
                fi.FilterExpression = match;

                filterVars.Add(fi);
            }
            return filterVars;
        }

        private List<FilterInstruction> GetAnyOfList()
        {
            List<FilterInstruction> list = new List<FilterInstruction>();

            
            Regex rx1 = new Regex("([A-Z][A-Z][0-9][0-9][0-9][a-z]*(,\\s*[A-Z][A-Z][0-9][0-9][0-9][a-z]*)*)\\)" +
                                "(=|<|>|<>)" +
                                "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                "|([0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+)" +
                                "|([0-9]+))");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(PreP);

            if (matches.Count == 0)
                return list;


           
            // get the varname, operation and numbers from the groups in the match

            foreach (Match m in matches)
            {
                string match = m.Groups[0].Value;
                string vars = m.Groups[1].Value;
                string condition = m.Groups[3].Value;
                Operation op = Operation.Equals;
                switch (condition)
                {
                    case "=":
                        op = Operation.Equals;
                        break;
                    case "<>":
                        op = Operation.NotEquals;
                        break;
                    case "<":
                        op = Operation.LessThan;
                        break;
                    case ">":
                        op = Operation.GreaterThan;
                        break;
                }
                string number = m.Groups[4].Value;

                if (string.IsNullOrEmpty(number))
                    continue;

                // determine number range if present
                List<int> numbers = GetNumberRange(number);
                List<string> numbersStr = GetNumberRangeStr(number);

                bool range = number.Contains("-");
                
                foreach (string v in vars.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {

                    FilterInstruction fi = new FilterInstruction();
                    fi.VarName = v;
                    fi.Oper = op;
                    fi.Values = numbers;
                    fi.ValuesStr = numbersStr;
                    fi.Range = range;
                    fi.FilterExpression = match;
                    fi.AnyOf = true;

                    list.Add(fi);
                }
            }

            return list;
        }

        

        private List<int> GetNumberRange(string nums)
        {
            List<int> numbers = new List<int>();
            string[] words = nums.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Replace(",", "");
                bool s = Int32.TryParse(words[i], out int n);
                if (words[i].Contains("-"))
                {
                    int lower = Int32.Parse(words[i].Substring(0, words[i].IndexOf("-")));
                    int upper = Int32.Parse(words[i].Substring(words[i].IndexOf("-") + 1, words[i].Length - words[i].IndexOf("-") - 1));
                    for (int j = lower; j <= upper; j++)
                        numbers.Add(j);
                }
                else
                {

                }
                if (s) numbers.Add(n);
            }
            return numbers;
        }

        private List<string> GetNumberRangeStr(string nums)
        {
            List<string> numbers = new List<string>();
            string[] words = nums.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Replace(",", "");
                int n = 0;
                bool s = Int32.TryParse(words[i], out n) || words[i].Equals("P") || words[i].Equals("C") ;
                if (words[i].Contains("-"))
                {
                    
                    int lower = Int32.Parse(words[i].Substring(0, words[i].IndexOf("-")));
                    int upper = Int32.Parse(words[i].Substring(words[i].IndexOf("-") + 1, words[i].Length - words[i].IndexOf("-") - 1));
                    for (int j = lower; j <= upper; j++)
                        numbers.Add(j.ToString());

                }
                else
                {
                    if (s) numbers.Add(words[i]);
                    
                }
            }
            return numbers;
        }


        public List<FilterInstruction> GetFilterInstructions(List<string> NonStdVars)
        {
            List<FilterInstruction> filterVars = new List<FilterInstruction>();

            if (string.IsNullOrEmpty(PreP))
                return filterVars;

            if (!PreP.Contains("Ask if"))
                return filterVars;

            foreach (string v in NonStdVars)
            {
                Regex rx1 = new Regex("(" + v + ")" +
                                    "(=|<|>|<>)" +
                                    "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                    "|([0-9]+\\sor\\s[0-9]+)" +
                                    "|([0-9]+\\-[0-9]+)" +
                                    "|([0-9]+)" +
                                    "|([A-Z]))", RegexOptions.IgnoreCase);
                
                // find all VarNames in the prep
                MatchCollection matches = rx1.Matches(PreP);

                if (matches.Count == 0)
                    continue;


                // get the varname, operation and numbers from the groups in the match

                foreach (Match m in matches)
                {
                    string match = m.Groups[0].Value;
                    string var = m.Groups[1].Value;
                    string condition = m.Groups[2].Value;
                    Operation op = Operation.Equals;
                    switch (condition)
                    {
                        case "=":
                            op = Operation.Equals;
                            break;
                        case "<>":
                            op = Operation.NotEquals;
                            break;
                        case "<":
                            op = Operation.LessThan;
                            break;
                        case ">":
                            op = Operation.GreaterThan;
                            break;
                    }
                    string number = m.Groups[3].Value;

                    if (string.IsNullOrEmpty(number))
                        continue;

                    // determine number range if present
                    List<int> numbers = GetNumberRange(number);
                    List<string> numbersStr = GetNumberRangeStr(number);

                    bool range = number.Contains("-");

                    FilterInstruction fi = new FilterInstruction();
                    fi.VarName = var;
                    fi.Oper = op;
                    fi.Values = numbers;
                    fi.ValuesStr = numbersStr;
                    fi.Range = range;
                    fi.FilterExpression = match;

                    filterVars.Add(fi);
                }
            }
            return filterVars;
        }

        public List<string> GetRoutingVars()
        {
            List<string> routingVars = new List<string>();

            if (string.IsNullOrEmpty(PstP))
                return routingVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the pstp
            MatchCollection matches = rx1.Matches(PstP);

            if (matches.Count == 0)
                return routingVars;

            foreach (Match m in matches)
            {
                routingVars.Add(m.Value);
            }

            return routingVars;
        }

        // TODO finish
        public Dictionary<int, string> GetRoutingInstructions()
        {
            Dictionary<int, string> routingVars = new Dictionary<int, string>();

            if (string.IsNullOrEmpty(PstP))
                return routingVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the pstp
            MatchCollection matches = rx1.Matches(PstP);

            if (matches.Count == 0)
                return routingVars;

            foreach (Match m in matches)
            {
                //routingVars.Add(m.Value);
            }

            return routingVars;
        }

        public List<string> GetRespNumbers(bool both = false)
        {

            List<string> responseList;
            int space;
            responseList = RespOptions.Split(new string [] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (both)
                responseList.AddRange(NRCodes.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList());

            for (int s = 0; s<responseList.Count; s ++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (char.IsWhiteSpace(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(0, i);
                        break;

                    }
                }
                
            }

            return responseList;
        }

        public List<string> GetRespLabels(bool both = false)
        {

            List<string> responseList;
            int space;
            responseList = RespOptions.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (both)
                responseList.AddRange(NRCodes.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList());

            for (int s = 0; s < responseList.Count; s++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (char.IsWhiteSpace(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(i);
                        responseList[s] = responseList[s].TrimStart(new char[] { ' ' });
                        break;

                    }
                }

            }

            return responseList;
        }

        public List<string> GetNonRespNumbers()
        {

            List<string> responseList;
            int space;
            
            responseList = NRCodes.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int s = 0; s < responseList.Count; s++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (!char.IsNumber(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(0, i);
                        break;

                    }
                }

            }

            return responseList;
        }

        /// <summary>
        /// Returns altered versions of the PreI, LitQ and Response Options to conform to Semi-tel specifications.
        /// </summary>
        /// <param name="ChangedPreI"></param>
        /// <param name="ChangedLitQ"></param>
        /// <param name="ChangedResponseOptions"></param>
        public void FormatSemiTel(out string ChangedPreI, out string ChangedLitQ, out string ChangedResponseOptions)
        {
            // replace "flash card" with "read out response options" in PreI
            // add ", or" to second last line of response options
            if (!PreI.Contains("flash card") && !PreI.Contains("response options"))
            {
                ChangedPreI = this.PreI;
                ChangedLitQ = this.LitQ;
                ChangedResponseOptions = this.RespOptions;
                return;
            }
            else if (PreI.ToLower().Contains("don't read out response options") || PreI.ToLower().Contains("do not read out response options"))
            {
                ChangedPreI = this.PreI;
                ChangedLitQ = this.LitQ;
                ChangedResponseOptions = this.RespOptions;
                return;
            }

            ChangedPreI = PreI.Replace("flash card", "Read out response options");
            ChangedLitQ = this.LitQ;
            ChangedResponseOptions = this.RespOptions;

            if (IsSeries())
            {
                if (!Qnum.EndsWith("a"))
                {
                    ChangedResponseOptions = RespOptionsWithOr();
                    if (!LitQ.Contains("Would you say"))
                        ChangedLitQ = this.LitQ + " Would you say...?";
                }
            }
            else
            {
                ChangedResponseOptions = RespOptionsWithOr();
                if (!LitQ.Contains("Would you say"))
                    ChangedLitQ = this.LitQ + " Would you say...?";
            }
        }

        public string RespOptionsWithOr()
        {
            string[] options = RespOptions.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            options[options.Length - 2] = options[options.Length - 2] + ", or";
            return string.Join("\r\n", options );
        }

        public bool IsSeries()
        {
            return char.IsLetter(Qnum[Qnum.Length-1]);
        }

        public bool IsDerived()
        {
            return this.VarName.RefVarName.EndsWith("v") ||
                this.PreP.ToLower().StartsWith("programmer:") || 
                this.PreP.ToLower().StartsWith("programming instructions:") ||
                this.PreP.ToLower().Contains("derived variable") ||            
                this.PreI.ToLower().Contains("derived variable") ||
                this.PreA.ToLower().Contains("derived variable") ||
                this.LitQ.ToLower().Contains("derived variable") ||
                this.PstI.ToLower().Contains("derived variable") ||
                this.PstP.ToLower().Contains("derived variable");
        }

        public bool IsProgramming()
        {
            return this.PreP.ToLower().StartsWith("programmer:") || this.PreP.StartsWith("programming instructions:") || 
                this.PreA.Replace("<strong>", "").Replace("<u>", "").ToLower().StartsWith("programmer:") || this.PreA.Replace("<strong>","").Replace("<u>", "").StartsWith("programming instructions:") ||
                this.LitQ.Replace("<strong>", "").Replace("<u>", "").ToLower().StartsWith("programmer:") || this.LitQ.Replace("<strong>", "").Replace("<u>", "").StartsWith("programming instructions:");
        }

        public bool IsTermination()
        {
            return this.VarName.RefVarName.StartsWith("BI9");
        }

        public bool IsHeading()
        {
            return this.VarName.RefVarName.StartsWith("Z") && !this.VarName.RefVarName.EndsWith("s");
        }

        public bool IsSubHeading()
        {
            return this.VarName.RefVarName.StartsWith("Z") && this.VarName.RefVarName.EndsWith("s");
        }

        public bool IsBlank()
        {
            return PrePNum == 0 && PreINum == 0 && PreANum == 0 && LitQNum == 0 && PstINum == 0 && PstPNum == 0 && 
                RespName.Equals("0") && NRName.Equals("0");
        }


        public int GetNumCols()
        {
            if (RespName.Equals("0") && NRName.Equals("0"))
                return 0;

            return GetRespNumbers(true)[0].Length;
            
        }

        public string GetVarType()
        {
            string type = string.Empty;

            if (VarName.RefVarName.EndsWith("hdg") || VarName.RefVarName.StartsWith("Z") || VarName.RefVarName.StartsWith("HG"))
                type = string.Empty;
            else if (VarName.RefVarName.EndsWith("o"))
                type = "string";
            else
            {
                if (string.IsNullOrEmpty(RespOptions) && string.IsNullOrEmpty(NRCodes))
                    type = string.Empty; // derived or other instruction
                else
                    type = "numeric";
            }

            return type;
        }
        

        public void ChangeRefVarName(string newRefVarName)
        {
            VarName.RefVarName = newRefVarName;
        }

        public string GetRefVarName()
        {
            return VarName.RefVarName;
        }

        public bool ContainsString(string searchFor, int startAt = 0)
        {
            string fullText = GetQuestionTextPlain();

            if (startAt > fullText.Length)
                return false;

            return fullText.IndexOf(searchFor,startAt)>-1;
        }

        public bool ContainsString(string field, string searchFor, int startAt = 0)
        {
            switch (field)
            {
                case "PreP":
                    if (startAt < PreP.Length)
                        return PreP.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;

                case "PreI":
                    if (startAt < PreI.Length)
                        return PreI.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PreA":
                    if (startAt < PreA.Length)
                        return PreA.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "LitQ":
                    if (startAt < LitQ.Length)
                        return LitQ.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PstI":
                    if (startAt < PstI.Length)
                        return PstI.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PstP":
                    if (startAt < PstP.Length)
                        return PstP.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "RespOptions":
                    if (startAt < RespOptions.Length)
                        return RespOptions.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "NRCodes":
                    if (startAt < NRCodes.Length)
                        return NRCodes.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "VarLabel":
                    if (startAt < VarName.VarLabel.Length)
                        return VarName.VarLabel.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Content":
                    if (startAt < VarName.Content.LabelText.Length)
                        return VarName.Content.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Topic":
                    if (startAt < VarName.Topic.LabelText.Length)
                        return VarName.Topic.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Domain":
                    if (startAt < VarName.Domain.LabelText.Length)
                        return VarName.Domain.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Product":
                    if (startAt < VarName.Product.LabelText.Length)
                        return VarName.Product.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
            }

            return false;
        }

        public override string ToString()
        {
            return VarName.RefVarName;
        }

        #region Private backing variables
        private string _qnum;
        public string _altqnum;
        public string _altqnum2;
        public string _altqnum3;
        private string _prep;
        private int _prepnum;
        private string _prei;
        private int _preinum;
        private string _prea;
        private int _preanum;
        private string _litq;
        private int _litqnum;
        private string _psti;
        private int _pstinum;
        private string _pstp;
        private int _pstpnum;
        private string _respoptions;
        private string _respname;
        private string _nrcodes;
        private string _nrname;
        private string _plainfilter;
        #endregion
    }

    public class QuestionUsage : SurveyQuestion
    {
        public string SurveyList { get; set; }

        public QuestionUsage() : base()
        {
            SurveyList = string.Empty;
        }
    }

    public class QuestionTimeFrame
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string TimeFrame { get; set; }

        public QuestionTimeFrame()
        {
            TimeFrame = string.Empty;
        }
    }

}
