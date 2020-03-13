using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class ChangedSurveyQuestion
    {
        #region Properties

        public int ID; // question ID
        public string SurveyCode { get; set; }
        private string _varname;
        public string VarName
        {
            get { return _varname; }
            set
            {
                _varname = value;
                RefVarName = Utilities.RemoveHighlightTags(value);
                RefVarName = Utilities.ChangeCC(RefVarName, "00");

            }
        }
        public string RefVarName { get; private set; }
        public string Qnum { get; set; }
        public string AltQnum { get; set; }
        public string AltQnum2 { get; set; }
        public string AltQnum3 { get; set; }

        // wordings
        //public Wording PreP { get; set; }
        private string _prep;
        public string PreP { get { return _prep; } set { _prep = FixElements(value); PrepRTF = FormatText(value); } }
        public string PrepRTF { get; private set; }
        public int PrePNum { get; set; }

        private string _prei;
        public string PreI { get { return _prei; } set { _prei = FixElements(value); PreiRTF = FormatText(value);  } }
        public string PreiRTF { get; private set; }
        public int PreINum { get; set; }

        private string _prea;
        public string PreA { get { return _prea; } set { _prea = FixElements(value); PreaRTF = FormatText(value); } }
        public string PreaRTF { get; private set; }
        public int PreANum { get; set; }


        private string _litq;
        public string LitQ { get { return _litq; } set { _litq = FixElements(value); LitqRTF = FormatText(value); } }
        public string LitqRTF { get; private set; }
        public int LitQNum { get; set; }


        private string _psti;
        public string PstI { get { return _psti; } set { _psti = FixElements(value); PstiRTF = FormatText(value);  } }
        public string PstiRTF { get; private set; }
        public int PstINum { get; set; }


        private string _pstp;
        public string PstP { get { return _pstp; } set { _pstp = FixElements(value); PstpRTF = FormatText(value);  } }
        public string PstpRTF { get; private set; }

        public int PstPNum { get; set; }


        private string _respoptions;
        public string RespOptions { get { return _respoptions; } set { _respoptions = FixElements(value); RespOptionsRTF = FormatText(value);  } }
        public string RespOptionsRTF { get; private set; }

        public string RespName { get; set; }


        private string _nrcodes;
        public string NRCodes { get { return _nrcodes; } set { _nrcodes = FixElements(value); NRCodesRTF = FormatText(value); } }
        public string NRCodesRTF { get; private set; }
        public string NRName { get; set; }


        // field info

        public int NumCol { get; set; }


        public int NumDec { get; set; }


        public string NumFmt { get; set; }

   
        public string VarType { get; set; }


        public bool ScriptOnly { get; set; }
        
        

        public bool TableFormat { get; set; }
        
        public bool CorrectedFlag { get; set; }
     

        public DateTime UpdateDate { get; set; }
        public AuditEntryType ChangeType { get; set; }
        public string UserName { get; set; }
        #endregion

        public ChangedSurveyQuestion()
        {
            VarName = "";
            Qnum = "";

            PreP = "";
            PreI = "";
            PreA = "";
            LitQ = "";
            PstI = "";
            PstP = "";
            RespOptions = "";
            NRCodes = "";
            ChangeType = AuditEntryType.Undefined;
            UserName = "";


            //PreP = new Wording();
            //PreP.PropertyChanged += WordingChanged;
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

            wording = @"{\rtf1\ansi " + wording + "}";

            return wording;
        }

        public string GetQuestionTextRich()
        {
            string questionText = "";
            string newline = @"\line ";
            //string prep = PrepRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "");

            if (!string.IsNullOrEmpty(PreP))
                questionText += @"\b " + PrepRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\b0 " + newline;

            if (!string.IsNullOrEmpty(PreI))
                questionText += @"\i " + PreiRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\i0 " + newline;
            
            if (!string.IsNullOrEmpty(PreA))
                questionText += PreaRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + newline;
            if (!string.IsNullOrEmpty(LitQ))
                questionText += @"\li100 " + LitqRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + newline;
            if (!string.IsNullOrEmpty(RespOptions))
                questionText += @"\li300 " + RespOptionsRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + newline;

            if (!string.IsNullOrEmpty(NRCodes))
            {
                if (string.IsNullOrEmpty(RespOptions))
                    questionText += @"\li300";

                questionText += NRCodesRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + newline;
            }

            if (!string.IsNullOrEmpty(PstI))
                questionText += @"\li" + @"\i " + PstiRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\i0 " + newline;
            if (!string.IsNullOrEmpty(PstP))
                questionText += @"\li" + @"\b " + PstpRTF.Replace(@"{\rtf1\ansi ", "").Replace("}", "") + @"\b0 ";

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return @"{\rtf1\ansi " + questionText + "}";
        }
    }
}
