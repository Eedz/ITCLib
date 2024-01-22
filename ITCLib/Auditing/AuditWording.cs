using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class AuditWording
    {
        public string WordingType { get; set; }
        public int ID { get; set; }
        private string _wording;
        public string Wording { get { return _wording; } set { _wording = FixElements(value); WordingRTF = FormatText(value); } }
        public string WordingRTF { get; private set; }
        public DateTime UpdateDate { get; set; }
        public AuditEntryType ChangeType { get; set; }
        public string UserName { get; set; }

        private string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&nbsp;", " ");
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
    }

    public class AuditResponse
    {
        public string WordingType { get; set; }
        public string ID { get; set; }
        private string _wording;
        public string Wording { get { return _wording; } set { _wording = FixElements(value); WordingRTF = FormatText(value); } }
        public string WordingRTF { get; private set; }
        public DateTime UpdateDate { get; set; }
        public AuditEntryType ChangeType { get; set; }
        public string UserName { get; set; }

        private string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&nbsp;", " ");
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
    }
}
