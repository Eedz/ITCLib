using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public static class StringExtensionMethods
    {
        public static bool IsArabic(this string strCompare)
        {
            char[] chars = strCompare.ToCharArray();
            foreach (char ch in chars)
                if (ch >= '\u0627' && ch <= '\u0649') return true;
            return false;
        }

        public static bool IsHebrew(this string strCompare)
        {
            char[] chars = strCompare.ToCharArray();
            foreach (char ch in chars)
                if ((ch >= '\u0580' && ch <= '\u05ff') || (ch >= '\ufb1d' && ch <= '\ufb4f')) return true;
            return false;
        }

        public static int CountLines(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            int newLineLen = Environment.NewLine.Length;
            int numLines = input.Length - input.Replace(Environment.NewLine, string.Empty).Length;
            if (newLineLen != 0)
            {
                numLines /= newLineLen;
                numLines++;
            }
            return numLines;
        }

        public static int CountLinesHTML(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            int newLineLen = "<br>".Length;
            int numLines = input.Length - input.Replace("<br>", string.Empty).Length;
            if (newLineLen != 0)
            {
                numLines /= newLineLen;
                numLines++;
            }
            return numLines;
        }

        public static string TrimAndRemoveAll(this string str, string trimString)
        {
            // Remove leading and trailing whitespace
            string trimmedString = str.Trim();

            // Remove all occurrences of the specified string from both ends
            while (trimmedString.StartsWith(trimString))
            {
                trimmedString = trimmedString.Substring(trimString.Length);
                trimmedString = trimmedString.TrimStart();
            }

            while (trimmedString.EndsWith(trimString))
            {
                trimmedString = trimmedString.Substring(0, trimmedString.Length - trimString.Length);
                trimmedString = trimmedString.TrimEnd();
            }

            return trimmedString;
        }
    }
}
