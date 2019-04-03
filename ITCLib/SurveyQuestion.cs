using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ITCLib
{
    public class SurveyQuestion : INotifyPropertyChanged
    {

        // IDEA create a wording object which has (int, string, string) for (W#, field, wording), then a question has a collection of wordings

        #region Properties

        public int ID; // question ID
        public string SurveyCode { get; set; }
        private string _varname;
        public string VarName { get { return _varname; } set { _varname = value; refVarName = Utilities.ChangeCC(value, 0); } }
        public string refVarName { get; private set; }
        public string Qnum { get; set; }
        public string AltQnum { get; set; }
        public string AltQnum2 { get; set; }
        public string AltQnum3 { get; set; }
        public string PreviousNames;

        // wordings
        //public Wording PreP { get; set; }
        private string _prep;
        public string PreP { get { return _prep; } set { _prep = FixElements(value); PrepRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string PrepRTF { get; private set; }
        private int _prepnum;
        public int PrePNum
        {
            get
            {
                return _prepnum;
            }
            set
            {
                if (value != _prepnum)
                {
                    _prepnum = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _prei;
        public string PreI { get { return _prei; } set { _prei = FixElements(value); PreiRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string PreiRTF { get; private set; }
        private int _preinum;
        public int PreINum {
            get
            {
                return _preinum;
            }
            set
            {
                if (value != _preinum)
                {
                    _preinum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _prea;
        public string PreA { get { return _prea; } set { _prea = FixElements(value); PreaRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string PreaRTF { get; private set; }
        private int _preanum;
        public int PreANum {
            get
            {
                return _preanum;
            }
            set
            {
                if (value != _preanum)
                {
                    _preanum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _litq;
        public string LitQ { get { return _litq; } set { _litq = FixElements(value); LitqRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string LitqRTF { get; private set; }
        private int _litqnum;
        public int LitQNum {
            get
            {
                return _litqnum;
            }
            set
            {
                if (value != _litqnum)
                {
                    _litqnum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _psti;
        public string PstI { get { return _psti; } set { _psti = FixElements(value); PstiRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string PstiRTF { get; private set; }
        private int _pstinum;
        public int PstINum {
            get
            {
                return _pstinum;
            }
            set
            {
                if (value != _pstinum)
                {
                    _pstinum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _pstp;
        public string PstP { get { return _pstp; } set { _pstp = FixElements(value); PstpRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string PstpRTF { get; private set; }
        private int _pstpnum;
        public int PstPNum {
            get
            {
                return _pstpnum;
            }
            set
            {
                if (value != _pstpnum)
                {
                    _pstpnum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _respoptions;
        public string RespOptions { get { return _respoptions; } set { _respoptions = FixElements(value); RespOptionsRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string RespOptionsRTF { get; private set; }
        private string _respname;
        public string RespName {
            get
            {
                return _respname;
            }
            set
            {
                if (value != _respname)
                {
                    _respname = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private string _nrcodes;
        public string NRCodes { get { return _nrcodes; } set { _nrcodes = FixElements(value); NRCodesRTF = FormatText(value); NotifyPropertyChanged(); } }
        public string NRCodesRTF { get; private set; }
        private string _nrname;
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
                    _nrname = value;
                    NotifyPropertyChanged();
                }
            }
        }
        

        // labels
        private DomainLabel _domain;
        public DomainLabel Domain
        { get { return _domain; }
            set
            {
                if (value != _domain)
                {
                    _domain = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private TopicLabel _topic;
        public TopicLabel Topic { get { return _topic; }
            set
            {
                if (value != _topic)
                {
                    _topic = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ContentLabel _content;
        public ContentLabel Content { get { return _content; }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ProductLabel _product;
        public ProductLabel Product { get { return _product; }
            set
            {
                if (value != _product)
                {
                    _product = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _varlabel;
        public string VarLabel {
            get { return _varlabel; }
            set
            {
                if (value != _varlabel)
                {
                    _varlabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // field info
        private int _numcol;
        public int NumCol
        {
            get
            {
                return _numcol;
            }
            set
            {
                if (value != _numcol)
                {
                    _numcol = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _numdec;
        public int NumDec
        {
            get
            {
                return _numdec;
            }
            set
            {
                if (value != _numdec)
                {
                    _numdec = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _numfmt;
        public string NumFmt
        {
            get
            {
                return _numfmt;
            }
            set
            {
                if (value != _numfmt)
                {
                    _numfmt = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _vartype;
        public string VarType
        {
            get
            {
                return _vartype;
            }
            set
            {
                if (value != _vartype)
                {
                    _vartype = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _scriptonly;
        public bool ScriptOnly {
            get
            {
                return _scriptonly;
            }
            set
            {
                if (value != _scriptonly)
                {
                    _scriptonly = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _tableformat;
        public bool TableFormat {
            get
            {
                return _tableformat;
            }
            set
            {
                if (value != _tableformat)
                {
                    _tableformat = value;
                    NotifyPropertyChanged();
                }
            }
        }
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
                    _correctedflag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<Translation> Translations { get; set; }

        public List<Comment> Comments { get; set; }

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
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        

        public SurveyQuestion()
        {
            Translations = new List<Translation>();
            Comments = new List<Comment>();
            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;
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
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

            wording = @"{\rtf1\ansi " + wording + "}";

            return wording;
        }

        public string GetQuestionText(List<string> stdFieldsChosen, bool withQnumVar = false, bool colorLitQ = false, string newline = "\r\n")
        {
            string questionText = "";

            if (withQnumVar)
                questionText += "<strong>" + Qnum + "</strong> (" + VarName + ")" + newline;
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

        /// <summary>
        /// Surround the translation text with the PreP and PstP from the English.
        /// </summary>
        public void InsertEnglishRouting()
        {
            string existing;
            foreach (Translation t in Translations)
            {
                existing = t.TranslationText;
                if (!string.IsNullOrEmpty(PreP))
                    t.TranslationText = PreP + "\r\n";


                t.TranslationText += existing;

                if (!string.IsNullOrEmpty(PstP))
                    t.TranslationText = t.TranslationText + "\r\n" + PstP;
                
            }
        }

        public string GetTranslationText(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return "";

            foreach (Translation t in Translations)
            {
                if (t.Language.Equals(lang))
                    return t.TranslationText;
            }

            return "";
        }

        public Translation GetTranslation(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return null;

            foreach (Translation t in Translations)
            {
                if (t.Language.Equals(lang))
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Returns the Qnum formatted as a normal Qnum. If the Qnum contains 2 qnums, with a z, this returns the 2nd qnum.
        /// </summary>
        /// <returns></returns>
        public string GetQnum()
        {

            if (Qnum.Length > 7)
                return Qnum.Substring(Qnum.LastIndexOf("z") + 1);
            else
                return Qnum;
        }

        public SurveyQuestion Copy()
        {
            SurveyQuestion sq; ;

            sq = new SurveyQuestion
            {
                VarName = VarName,
                refVarName = refVarName,
                Qnum = Qnum,
                AltQnum = AltQnum,
                AltQnum2 = AltQnum2,
                AltQnum3 = AltQnum3,
                PreviousNames = PreviousNames,
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
                NRName= NRName,
                NRCodes = NRCodes,
                VarLabel = VarLabel,
                Content = new ContentLabel ( this.Content.ID,  this.Content.LabelText ),
                Topic = new TopicLabel ( this.Topic.ID,  this.Topic.LabelText ),
                Domain = new DomainLabel ( this.Domain.ID,  this.Domain.LabelText ),
                Product = new ProductLabel (this.Product.ID,  this.Product.LabelText ),
                NumCol = NumCol,
                NumDec = NumDec,
                NumFmt = NumFmt,
                VarType = VarType,
                ScriptOnly = ScriptOnly,
                TableFormat = TableFormat,
                CorrectedFlag = CorrectedFlag


            };

            return sq;
        }

        public List<string> GetRespNumbers()
        {

            List<string> responseList;
            int space;
            responseList = RespOptions.Split(new string [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            responseList.AddRange(NRCodes.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList());

            for (int s = 0; s<responseList.Count; s ++)
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
    }
}
