using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RtfPipe;

namespace ITCLib
{
    public static class RTFUtilities
    {
        /// <summary>
        /// Converts a string from RTF to ITC RTF.
        /// </summary>
        /// <param name="wordingText"></param>
        /// <returns></returns>
        public static string ConvertRTF_To_ITC_HTML(string wordingText)
        {
            StringBuilder wording = new StringBuilder(wordingText);

            wording.Replace(@"{\colortbl;\red255\green255\blue0; }", "");
            wording.Replace(@"\highlight1\f0\fs18\lang1033", @"\f0\fs18\lang1033 [yellow]");

            wording.Replace("<", "&lt;");
            wording.Replace(">", "&gt;");

            wording.Replace(@"\bullet ", "[bullet]");
            wording.Replace(@"\b0 ", "</strong>");
            wording.Replace(@"\b0", "</strong>");
            wording.Replace(@"\b ", "<strong>");
            wording.Replace(@"\b", "<strong>");

            wording.Replace(@"\i0 ", "</em>");
            wording.Replace(@"\i0", "</em>");
            wording.Replace(@"\i ", "<em>");
            wording.Replace(@"\i", "<em>");

            wording.Replace(@"\ul0 ", "</u>");
            wording.Replace(@"\ul0", "</u>");
            wording.Replace(@"\ulnone ", "</u>");
            wording.Replace(@"\ulnone", "</u>");
            wording.Replace(@"\ul ", "<u>");
            wording.Replace(@"\ul", "<u>");

            wording.Replace(@"\strike ", "<s>");
            wording.Replace(@"\strike0 ", "</s>");
            wording.Replace(@"\highlight1 ", "[yellow]");
            wording.Replace(@"\highlight0", "[/yellow]");

            wording.Replace(@"\pntext•", "[bullet]");

            wording.Replace(@"\line ", "<br>");
            wording.Replace(@"\line", "<br>");
            wording.Replace(@"\pard", @"\drap");
            wording.Replace(@"\par", "<br>");
            wording.Replace(@"\drap", @"\pard");



            wording.Replace(@"{\rtf1\ansi ", "");
            while (wording[wording.Length - 1] == '}')
                wording.Remove(wording.Length - 1, 1);

            return wording.ToString();
        }

        // Convert RTF to HTML
        public static string ConvertRTFtoHTML(string rtfString)
        {
            string htmlString = string.Empty;

            var source = new RtfSource(new StringReader(rtfString));
            htmlString = Rtf.ToHtml(source);

            return htmlString;
        }

        private static string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ");
        }

        public static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        public static string FormatText(string wordingText)
        {
            string wording = GetRtfUnicodeEscapedString(FixElements(wordingText));

            wording = wording.Replace("<strong>", @"\b ");
            wording = wording.Replace("</strong>", @"\b0 ");
            wording = wording.Replace("<em>", @"\i ");
            wording = wording.Replace("</em>", @"\i0 ");
            wording = wording.Replace("<br>", @"\line ");
            wording = wording.Replace("\r\n", @"\line ");
            wording = wording.Replace("<u>", @"\ul ");
            wording = wording.Replace("</u>", @"\ul0 ");
            wording = wording.Replace("[bullet]", @"\bullet ");

            return wording;
        }

        /// <summary>
        /// Returns the complete question text for the given question in RTF format.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="colorLitQ"></param>
        /// <returns></returns>
        public static string QuestionToRTF(SurveyQuestion question, bool colorLitQ = false)
        {
            StringBuilder questionText = new StringBuilder();

            string prep = FormatText(question.PrePW.WordingText);
            string prei = FormatText(question.PreIW.WordingText);
            string prea = FormatText(question.PreAW.WordingText);
            string litq = FormatText(question.LitQW.WordingText);
            string ros = FormatText(question.RespOptionsS.RespList);
            string nrs = FormatText(question.NRCodesS.RespList);
            string psti = FormatText(question.PstIW.WordingText);
            string pstp = FormatText(question.PstPW.WordingText);

            if (colorLitQ)
                questionText.Append(@"{\colortbl;\red0\green0\blue255;}");

            questionText.Append(!string.IsNullOrEmpty(prep) ? @"{\pard\b " + prep + @"\b0\par}" : string.Empty);
            questionText.Append(!string.IsNullOrEmpty(prei) ? @"{\pard\i " + prei + @"\i0\par}" : string.Empty);
            questionText.Append(!string.IsNullOrEmpty(prea) ? @"{\pard " + prea + @"\par}" : string.Empty);
            

            if (!string.IsNullOrEmpty(litq))
            {
                if (colorLitQ)
                    questionText.Append(@"{\pard\li100\cf1 " + litq + @"\par}");
                else
                    questionText.Append(@"{\pard\li100 " + litq + @"\par}");
            }

            questionText.Append(!string.IsNullOrEmpty(ros) ? @"{\pard\li300 " + ros + @"\par}" : string.Empty);
            questionText.Append(!string.IsNullOrEmpty(nrs) ? @"{\pard\li300 ------------------\line " + nrs + @"\par}" : string.Empty);
            questionText.Append(!string.IsNullOrEmpty(psti) ? @"{\pard\i " + psti + @"\i0\par}" : string.Empty);
            questionText.Append(!string.IsNullOrEmpty(pstp) ? @"{\pard\b " + pstp + @"\b0\par}": string.Empty);
            
            if (question.Images.Count > 0)
            {
                questionText.Append(@"{\pard \par}");
                foreach (SurveyImage img in question.Images)
                    questionText.Append(@"{\pard " + img.Language + " Filename: " + img.ImageName + @"\par}");
            }

            questionText.Insert(0, @"{\rtf1\ansi ");
            questionText.Append("}");

            return questionText.ToString();
        }

        public static string FormatRTF_FromText(string wordingText)
        {
            return @"{\rtf1\ansi " + FormatText(wordingText) + "}";
        }
    }
}
