using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyQuestion
    {

        // IDEA create a wording object which has (int, string, string) for (W#, field, wording), then a question has a collection of wordings

        public int ID; // question ID
        public string SurveyCode;
        public string VarName { get; set; }
        public string refVarName { get; set; }
        public string Qnum { get; set; }
        public string AltQnum;
        public string AltQnum2;
        public string AltQnum3;
        public string PreviousNames;

        // wordings
        private string _prep;
        public string PrepRTF { get; private set; }
        public int PrePNum { get; set; }
        public string PreP { get { return _prep; } set { _prep = FixElements(value); PrepRTF = FormatText(value); } }
        private string _prei;
        public string PreiRTF { get; private set; }
        public int PreINum { get; set; }
        public string PreI { get { return _prei; } set { _prei = FixElements(value); PreiRTF = FormatText(value); } }
        private string _prea;
        public string PreaRTF { get; private set; }
        public int PreANum { get; set; }
        public string PreA { get { return _prea; } set { _prea = FixElements(value); PreaRTF = FormatText(value); } }
        private string _litq;
        public string LitqRTF { get; private set; }
        public int LitQNum { get; set; }
        public string LitQ { get { return _litq; } set { _litq = FixElements(value); LitqRTF = FormatText(value); } }
        private string _psti;
        public string PstiRTF { get; private set; }
        public int PstINum { get; set; }
        public string PstI { get { return _psti; } set { _psti = FixElements(value); PstiRTF = FormatText(value); } }
        private string _pstp;
        public string PstpRTF { get; private set; }
        public int PstPNum { get; set; }
        public string PstP { get { return _pstp; } set { _pstp = FixElements(value); PstpRTF = FormatText(value); } }
        private string _respoptions;
        public string RespOptionsRTF { get; private set; }
        public string RespName { get; set; }
        public string RespOptions { get { return _respoptions; } set { _respoptions = FixElements(value); RespOptionsRTF = FormatText(value); } }
        private string _nrcodes;
        public string NRCodesRTF { get; private set; }
        public string NRName { get; set; }
        public string NRCodes { get { return _nrcodes; } set { _nrcodes = FixElements(value); NRCodesRTF = FormatText(value); } }

        // labels
        public DomainLabel Domain { get; set; }
        public TopicLabel Topic { get; set; }
        public ContentLabel Content { get; set; }
        public ProductLabel Product { get; set; }
        public string VarLabel { get; set; }
        public string ContentLabel;
        public string TopicLabel;
        public string DomainLabel;
        public string ProductLabel;

        // field info
        public int NumCol;
        public int NumDec;
        public string NumFmt;
        public string VarType;
        public bool ScriptOnly;

        public bool TableFormat;
        public bool CorrectedFlag;

        public List<Translation> Translations { get; set; }

        public List<Comment> Comments { get; set; }

        public string Filters { get; set; }

        public SurveyQuestion()
        {
            Translations = new List<Translation>();
            Comments = new List<Comment>();
        }

        private string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ");
        }

        private string FormatText(string wordingText)
        {
            string wording = wordingText;
            wording = wording.Replace("<strong>", @"{\b ");
            wording = wording.Replace("</strong>", @"}");
            wording = wording.Replace("<em>", @"{\i");
            wording = wording.Replace("</em>", @"}");
            wording = wording.Replace("<br>", @"{\line}");

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
                ContentLabel = ContentLabel,
                TopicLabel = TopicLabel,
                DomainLabel = DomainLabel,
                ProductLabel = ProductLabel,
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
