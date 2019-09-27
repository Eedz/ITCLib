using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace ITCLib
{
    public enum CompareType { ByCharacter, ByWords }


    public static class ChangeHighlighter
    {
        enum FontModes { Original, Deleted, Added }

        //These constants are used as the font settings for the track changes
        //If you make modifications make sure they are valid, and that the
        //Opening and closing tags match.
        //const string FontOriginal = "<Font Color=Black>" ' no need to encode black text(EB)
        const string FontOriginal = "";
        const string FontDeleted = "<Font Color=Red>[s]";
        const string FontAdded = "<Font Color=Blue><u>";
        const string FontEnd = "</Font>";
        const string FontEndUnderline = "</u>";
        const string FontEndStrike = "[/s]";

        // These constants are used as placeholders in the text for the prfn_CustomEncode function
        const string FontPlaceHolderOriginal = "PlaceHolderFontOriginal";
        const string FontPlaceHolderAdded = "PlaceHolderFontAdded";
        const string FontPlaceHolderDeleted = "PlaceHolderFontDeleted";

        const string FontPlaceHolderEndFont = "PlaceHolderFontEndFont";
        const string FontPlaceHolderEndUnderline = "PlaceHolderFontEndUnderline";


        static public int[,] LCS(string[] s1, string[] s2)
        {
            int[,] c = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 1; i <= s1.Length; i++)
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                        c[i, j] = c[i - 1, j - 1] + 1;
                    else
                        c[i, j] = c[i - 1, j] > c[i, j - 1] ? c[i - 1, j] : c[i, j - 1];
                }
            return c;
        }

        static public int[,] LCS(string s1, string s2)
        {
            int[,] c = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 1; i <= s1.Length; i++)
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                        c[i, j] = c[i - 1, j - 1] + 1;
                    else
                        c[i, j] = c[i - 1, j] > c[i, j - 1] ? c[i - 1, j] : c[i, j - 1];
                }
            return c;
        }

        static public string BackTrack(int[,] c, string[] s1, string[] s2, int i, int j)
        {
            if (i == 0 || j == 0)
                return "";
            else if (s1[i - 1] == s2[j - 1])
                return BackTrack(c, s1, s2, i - 1, j - 1) + s1[i - 1];
            else if (c[i, j - 1] > c[i - 1, j])
                return BackTrack(c, s1, s2, i, j - 1);
            else
                return BackTrack(c, s1, s2, i - 1, j);
        }

        static public string BackTrack(int[,] c, string s1, string s2, int i, int j)
        {
            if (i == 0 || j == 0)
                return "";
            else if (s1[i - 1] == s2[j - 1])
                return BackTrack(c, s1, s2, i - 1, j - 1) + s1[i - 1];
            else if (c[i, j - 1] > c[i - 1, j])
                return BackTrack(c, s1, s2, i, j - 1);
            else
                return BackTrack(c, s1, s2, i - 1, j);
        }

        static public string PrintDiff(int[,] c, string[] s1, string[] s2, int i, int j)
        {
            var a = "";

            if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
            {
                a = PrintDiff(c, s1, s2, i - 1, j - 1);
                return a + " " + "" + s1[i - 1];
            }
            else if (j > 0 && (i == 0 || (c[i, j - 1] > c[i - 1, j])))
            {
                a = PrintDiff(c, s1, s2, i, j - 1);
                return a + " " + FontAdded + s2[j - 1] + FontEndUnderline + FontEnd;
            }
            else if (i > 0 && (j == 0 || (c[i, j - 1] <= c[i - 1, j])))
            {
                a = PrintDiff(c, s1, s2, i - 1, j);
                return a + " " + FontDeleted + s1[i - 1] + FontEndStrike + FontEnd;
            }
            return a;
        }

        static public string PrintDiff(int[,] c, string s1, string s2, int i, int j)
        {
            var a = "";

            if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
            {
                a = PrintDiff(c, s1, s2, i - 1, j - 1);
                return a + "" + s1[i - 1];
            }
            else if (j > 0 && (i == 0 || (c[i, j - 1] > c[i - 1, j])))
            {
                a = PrintDiff(c, s1, s2, i, j - 1);
                return a + FontAdded + s2[j - 1] + FontEndUnderline + FontEnd;
            }
            else if (i > 0 && (j == 0 || (c[i, j - 1] <= c[i - 1, j])))
            {
                a = PrintDiff(c, s1, s2, i - 1, j);
                return a + FontDeleted + s1[i - 1] + FontEndStrike + FontEnd;
            }
            return a;
        }

        static public string HighlightChanges(string strOld, string strNew, CompareType compareMethod)
        {
            //'Before starting the LCS comparisons we make some simpler comparisons,
            //'since the LCS is quite expensive to perform.

            //' Are the 2 strings equal?
            if (strNew.Equals(strOld))
            {
                return prfn_CustomEncode(strNew);

            }

            //' Are both strings empty?
            if (string.IsNullOrEmpty(strOld) && string.IsNullOrEmpty(strNew))
            {
                return "";
            }

            // TODO write better version of this section
            //'Optimization. Find what the strings have in common in the end and the beginning.
            //' This scales with the the number of chars, whereas the LCS scales with chars^2
            //' This can be VERY time optimizing under some conditions,
            //' especially for fields where you usually only append information
            //string strCommonStart;
            //strCommonStart = prfn_ExtractCommmonStart(ref strOld, ref strNew); // Compare);
            //string strCommonEnd;
            //strCommonEnd = prfn_ExtractCommmonEnd(ref strOld, ref strNew); //Compare);

            ////'Is one of the strings empty at this point?
            //if (string.IsNullOrEmpty(strNew))
            //{
            //    //'Everything else has been deleted
            //    return prfn_CustomEncode(strCommonStart + prfn_WriteModePrefix(FontModes.Deleted) + strOld + prfn_EndMode(FontModes.Deleted) + strCommonEnd);
            //}

            //if (string.IsNullOrEmpty(strOld))
            //{
            //    //'All the rest is new
            //    return prfn_CustomEncode(strCommonStart + prfn_WriteModePrefix(FontModes.Added) + strNew + prfn_EndMode(FontModes.Added) + strCommonEnd);

            //}

            //'Now we start the actual LCS comparison
            //'We need to divide the strings into arrays. We can do this in 2 ways.
            //'Either we create an array with 1 charecter per array spot, or
            //'we load the array with words*.
            //' * Technically its split on any non-alphanumeric entry. So Smith's become[Smith][']{s]

            
            int[,] LCS2; //'This array contains the LCS array. See wikipedia http://en.wikipedia.org/wiki/Longest_common_subsequence_problem
            string result; // the marked up text
            switch (compareMethod)
            {
                case CompareType.ByCharacter:
                    // already works on character basis
                    LCS2 = LCS(strOld, strNew);
                    result = PrintDiff(LCS2, strOld, strNew, strOld.Length, strNew.Length);
                    break;
                case CompareType.ByWords:
                    // convert to word array
                    string[] a1;
                    string[] a2;
                    a1 = strOld.Split(new char[] { ' ' }, StringSplitOptions.None); // prfn_StringToWordArray(strOld);
                    a2 = strNew.Split(new char[] { ' ' }, StringSplitOptions.None); // prfn_StringToWordArray(strNew);
                    LCS2 = LCS(a1, a2);
                    result = PrintDiff(LCS2, a1, a2, a1.Length, a2.Length);
                    break;
                default:
                    return "";
            }

            return result;
        }

        static private string prfn_ExtractCommmonStart(ref string strOld, ref string strNew)
        { 
            /*      
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_ExtractCommmonStart
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : This will determine what strOld and strNew has in common, when looked at
            '           : from the start. The common part will be returned as a string, and removed
            '           : from the 2 input strings.
            '           :
            ' Commments :
            '           :
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */

            long l;
            long lLast;

            l = 0;
            lLast = l;

            do
            {
                // 'Find next space
                l = 0;
                l = strNew.IndexOf(' ', (int)lLast + 1);
                if (l <= lLast)
                {
                    // 'No more spaces found
                    break;
                }

                // EB
                if (l >= strOld.Length)
                    break;

                //StrComp(Left$(strOld, l), Left$(strNew, l), Compare) = 0 Then
                if (strOld.Substring(0, (int)l).Equals(strNew.Substring(0, (int)l)))
                {
                    //'Update lLast
                    lLast = l;
                }
                else
                {
                    //'Exit
                    break;
                }
            } while (true);

            //'If any common string was found, iLast will be larger then 0.
            if (lLast != 0)
            {
                //'Remove same from the 2 input strings
                strNew = strNew.Substring((int)lLast + 1); // Mid$(strNew, lLast + 1)
                strOld = strOld.Substring((int)lLast + 1);// Mid$(strOld, lLast + 1)

                //'Return the common part
                if (string.IsNullOrEmpty(strNew))
                    return "";
                else if (lLast > strNew.Length)
                    return strNew;
                else
                    return strNew.Substring(0, (int)lLast);
            }
            else
            {
                //'Nothing was in commmon. Return empty string
                return "";
            }
        }

        static private string prfn_ExtractCommmonEnd(ref string strOld, ref string strNew)
        {//, Compare As VbCompareMethod) As String
            /*'---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : ExtractCommmonEnd
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : This will determine what strOld and strNew has in common, when looked at
            '           : from the end. The common part will be returned as a string, and removed
            '           : from the 2 input strings.
            '           :
            ' Commments : Might be possible to improve performance of this by splitting the strings
            '           : into arrays first.
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */
            long lSpace;
            long lRightLength;
            long lLast = 0;
            long lNew;


            lNew = strNew.Length;
            if (lNew > 1)
            { //'If iNew is 1 or less, it makes no sense to do the compare.
                do
                {
                    //'While looking from the end, Find the Last space
                    lSpace = strNew.LastIndexOf(' ', (int)lNew - (int)lLast - 1);// InStrRev(strNew, " ", lNew - lLast - 1)
                    if (lSpace == 0)
                    {
                        //'No spaces found.
                        break;
                    }
                    else
                    {
                        //'Get the Length of the right part of string
                        lRightLength = lNew - lSpace;
                    }

                    // EB
                    if (lRightLength > strOld.Length || lRightLength > strNew.Length)
                        break;

                    //'Now compare the rightmost part of both strings to each other
                    //StrComp(Right$(strOld, lRightLength), Right$(strNew, lRightLength), Compare) = 0 Then
                    if (strOld.Substring((int)lRightLength).Equals(strNew.Substring((int)lRightLength)))
                    {
                        //'Update iLast
                        lLast = lRightLength;
                        if (lLast + 1 == lNew) break; //'Special case, of starting with a space.
                    }
                    else
                    {
                        //'They are not equal
                        break;
                    }
                } while (true);
            }

            //'If any common string was found, iLast will be larger then 0.
            if (lLast != 0)
            {
                //'Remove same from the 2 input strings
                strNew = strNew.Substring((int)lNew - (int)lLast); // Left$(strNew, lNew - lLast)
                strOld = strOld.Substring(strOld.Length - (int)lLast); //Left$(strOld, Len(strOld) - lLast)

                //'Return the common part
                return strNew.Substring((int)lLast);
            }
            else
            {
                //'Nothing was in commmon. Return empty string
                return "";
            }

        }

        static private string[] prfn_StringToCharArray(ref string str)
        {
            /*'---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_stringToCharArray
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : Convert a string into an array of characters.
            '           :
            ' Commments : Original code by Thydzik
            '           :
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */

            string[] arr;
            arr = new string[str.Length - 1];

            for (int i = 0; i < str.Length; i++)
            {
                arr[i] = str.Substring(i, 1);
            }
            return arr;
        }

        static private string[] prfn_StringToWordArray(string strInput)
        {
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_SplitWordsIntoArray
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : Split a string into an array, using non-alphanumerics as seperators.
            '           : Note that each non-alphanumeric gets its own entry. (Unlike the built in
            '           : Split() function.
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */
            long l;
            long lLast;
            long i;
            string c;
            string[] arr;


            //'We initiliaze an array that can fit the entire string.
            //'It will be cut down before exiting.
            arr = new string[strInput.Length]; // ReDim arr(0 To Len(strInput))

            //'Initiate the last position to 1. lLast is used to keep track of the last
            //'   position in which a split occured.
            lLast = 0;
            i = 0; //EB

            //'Loop through the string, 1 charecter at a time
            for (l = 0; l < strInput.Length; l++)
            {
                //'Get the next charecter from the string
                c = strInput.Substring((int)l, 1); //Mid$(strInput, l, 1)


                //'Check for non-alphanumeric content
                if (!char.IsLetterOrDigit(Convert.ToChar(c)))
                {// Like "[!a-z!0-9!\-\:]" Then
                    //'It is a space or special char
                    //'Check lLast to see when we last performed a split.
                    if ((int)lLast == l)
                    {
                        //'Last split was 1 charecter ago. Thus only store the active charecter (the delimeter)
                        arr[i] = c;
                        i = i + 1;
                    }
                    else
                    {
                        //'Store everything between the last split and the current position into the array
                        arr[i] = strInput.Substring((int)lLast, (int)l - (int)lLast); //Mid$(strInput, lLast, l - lLast)
                        i = i + 1;
                        //'Store the current delimiter
                        arr[i] = c;
                        i = i + 1;
                    }
                    //'Store the last known split
                    lLast = l + 1;
                }
            }

            //'Get the last part of the string, in case string did not end on a delimiter
            if (lLast != l)
            {
                arr[i] = strInput.Substring((int)lLast, (int)l - (int)lLast); //Mid$(strInput, lLast, l - lLast)
                i = i + 1;
            }

            //'Resize the array to match the number of elements we put into it.
            Array.Resize(ref arr, (int)i - 1);

            //Return array
            return arr;

        }

        static private string prfn_WriteModePrefix(FontModes pMode)
        {
            // --------------------------------------------------------------------------------------
            //'---------------------------------------------------------------------------------------
            // Procedure : prfn_WriteModePrefix
            //' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            //' Date      : 2013-06-14
            //' Version   : 1.0
            //' Purpose   : Write the opening tag for the font about to be used. Note that any change
            //'           : to the opening tag likely needs to be reflected in the closing tag.
            //' Commments :
            //'           :
            //' Bugs?     : Email: SmileyCoderTools@gmail.com
            //'---------------------------------------------------------------------------------------
            //'---------------------------------------------------------------------------------------
            switch (pMode)
            {
                case FontModes.Original:
                    return FontOriginal;
                case FontModes.Added:
                    return FontAdded;
                case FontModes.Deleted:
                    return FontDeleted;
            }
            return "";
        }

        static private string prfn_EndMode(FontModes pMode)
        {
            //'---------------------------------------------------------------------------------------
            //'---------------------------------------------------------------------------------------
            //' Procedure : prfn_EndMode
            //' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            //' Date      : 2013-06-14
            //' Version   : 1.0
            //' Purpose   : This is used to return the closing tag for the active font
            //'           :
            //' Commments :
            //'           :
            //' Bugs?     : Email: SmileyCoderTools@gmail.com
            //'---------------------------------------------------------------------------------------
            //'---------------------------------------------------------------------------------------
            switch (pMode)
            {
                case FontModes.Original:
                    //return FontEnd;
                    return "";
                case FontModes.Added:
                    return FontEnd + FontEndUnderline;
                case FontModes.Deleted:
                    return FontEnd + FontEndStrike;
            }
            return "";
        }

        static private string prfn_CustomEncode(string strIn)
        {
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : CustomEncode
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-15
            ' Version   : 1.0
            ' Purpose   : Convert plain text to be used in a rich text field, without destroying
            '           : the programmed colour markings.
            '           :
            ' Commments : This is done by first replacing all our font markings with placeholders,
            '           : after which access get to do the rich text conversion, and then the
            '           : placeholders are replaced once again with the original value.
            '           :
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
             */

            if (string.IsNullOrEmpty(strIn))
                return "";

            //Replace the 3 starting types we have with placeholders
            //strIn = strIn.Replace(FontOriginal, FontPlaceHolderOriginal);
            strIn = strIn.Replace(FontAdded, FontPlaceHolderAdded);
            strIn = strIn.Replace(FontDeleted, FontPlaceHolderDeleted);

            //Replace the end font and end underline
            strIn = strIn.Replace(FontEnd, FontPlaceHolderEndFont);
            strIn = strIn.Replace(FontEndUnderline, FontPlaceHolderEndUnderline);

            //Now encode the string

            strIn = HttpUtility.HtmlEncode(strIn);
            strIn = strIn.Replace("&nbsp;", " ");
            //Now replace all the placeholders back to their original values
            strIn = strIn.Replace(FontPlaceHolderOriginal, FontOriginal);
            strIn = strIn.Replace(FontPlaceHolderAdded, FontAdded);
            strIn = strIn.Replace(FontPlaceHolderDeleted, FontDeleted);

            strIn = strIn.Replace(FontPlaceHolderEndFont, FontEnd);
            strIn = strIn.Replace(FontPlaceHolderEndUnderline, FontEndUnderline);

            //Return result
            return strIn;
        }

        #region VBA code converted to C# for reference
        // original public method used in VBA version of ISR
        //public string TSCfn_CompareAndDiff(string varNewText, string varOldText, CompareType compareMethod = CompareType.ByCharacter) // binary compare
        //{
        //    /*
        //    '---------------------------------------------------------------------------------------
        //    '---------------------------------------------------------------------------------------
        //    ' Procedure : TSCfn_CompareAndDiff
        //    ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
        //    ' Date      : 2013-06-14
        //    ' Version   : 1.0
        //    ' Purpose   : To compare 2 strings to each other, and output a Track Changes like
        //    '           : comparison with deletions marked in red+underline, and additions marked in blue.
        //    ' Commments :
        //    '           :
        //    ' Bugs?     : Email: SmileyCoderTools@gmail.com
        //    ' Converted to C# by Edward Bauer (Sep2019)
        //    '---------------------------------------------------------------------------------------
        //    '---------------------------------------------------------------------------------------
        //    */

        //    string strOld;
        //    string strNew;

        //    //'Convert the variants to strings. Variants are used since it makes it easy to use in queries,
        //    //'without having to take nulls into account.
        //    // EB note - variants not in C#
        //    //strOld = varOldText & ""
        //    //strNew = varNewText & ""
        //    strOld = varOldText;
        //    strNew = varNewText;


        //    //'Before starting the LCS comparisons we make some simpler comparisons,
        //    //'since the LCS is quite expensive to perform.

        //    //' Are the 2 strings equal?
        //    if (strNew.Equals(strOld))
        //    {
        //        return prfn_CustomEncode(prfn_WriteModePrefix(FontModes.Original) + strNew + prfn_EndMode(FontModes.Original));

        //    }

        //    //' Are both strings empty?
        //    if (string.IsNullOrEmpty(strOld) && string.IsNullOrEmpty(strNew))
        //    {
        //        return "";
        //    }

        //    //'Optimization. Find what the strings have in common in the end and the beginning.
        //    //' This scales with the the number of chars, whereas the LCS scales with chars^2
        //    //' This can be VERY time optimizing under some conditions,
        //    //' especially for fields where you usually only append information
        //    string strCommonStart;
        //    strCommonStart = prfn_ExtractCommmonStart(ref strOld, ref strNew); // Compare);
        //    string strCommonEnd;
        //    strCommonEnd = prfn_ExtractCommmonEnd(ref strOld, ref strNew); //Compare);

        //    //'Is one of the strings empty at this point?
        //    if (string.IsNullOrEmpty(strNew))
        //    {
        //        //'Everything else has been deleted
        //        return prfn_CustomEncode(strCommonStart + prfn_WriteModePrefix(FontModes.Deleted) + strOld + prfn_EndMode(FontModes.Deleted) + strCommonEnd);
        //    }

        //    if (string.IsNullOrEmpty(strOld)) {
        //        //'All the rest is new
        //        return prfn_CustomEncode(strCommonStart + prfn_WriteModePrefix(FontModes.Added) + strNew + prfn_EndMode(FontModes.Added) + strCommonEnd);

        //    }

        //    //'Now we start the actual LCS comparison
        //    //'We need to divide the strings into arrays. We can do this in 2 ways.
        //    //'Either we create an array with 1 charecter per array spot, or
        //    //'we load the array with words*.


        //    //' * Technically its split on any non-alphanumeric entry. So Smith's become[Smith][']{s]

        //    string[] a1;
        //    string[] a2;

        //    switch (compareMethod)
        //    {
        //        case CompareType.ByCharacter:
        //            a1 = prfn_StringToCharArray(ref strOld);
        //            a2 = prfn_StringToCharArray(ref strNew);
        //            break;
        //        case CompareType.ByWords:
        //            a1 = prfn_StringToWordArray(strOld);
        //            a2 = prfn_StringToWordArray(strNew);
        //            break; 
        //        default:
        //            return "";
        //            //err.Raise "No Comparemethod was supplied"

        //    }


        //    //'This array contains the LCS array. See wikipedia http://en.wikipedia.org/wiki/Longest_common_subsequence_problem

        //    long[,] LCS;

        //    LCS = prfn_LongestCommonSubsequenceArr(ref a1, ref a2); //, Compare);

        //    //'This array contains the 3xn array of elements that are either original, deleted or added.

        //    string[,,] dif;

        //    long i = a1.Length;
        //    long j = a2.Length;

        //    dif = prfn_getDiffArr(ref LCS, ref a1, ref a2, ref i, ref j); //, Compare)

        //    //'Replace deleted linebreaks with a ¶ sign
        //    //'   pr_FormatDeletedLineBreaks dif


        //    //'This will return the string formatted for a rich tect field

        //    return prfn_CustomEncode(strCommonStart + prfn_OutputComparedString(dif) + strCommonEnd);

        //}

        static private string prfn_OutputComparedString(string[,] dif)
        {
            return "";
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_OutputComparedString
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : This function takes the Dif array and converts it to a string with
            '           : red highlight+underline for deleted text, and blue for added text.
            '           :
            ' Commments :
            '           :
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */

            //    //int i;
            //    FontModes pMode;
            //    string strResult;
            //    pMode = prfn_GetNextMode(dif, 0);
            //    strResult = prfn_WriteModePrefix(pMode);
            //    for (int i = dif.GetLowerBound(2); i <= dif.GetUpperBound(2); i++) { 
            //        if (pMode != prfn_GetNextMode(dif, i)) {
            //            strResult = strResult + prfn_EndMode(pMode);
            //            pMode = prfn_GetNextMode(dif, i);
            //            strResult = strResult + prfn_WriteModePrefix(pMode);
            //        }

            //        strResult = strResult + dif[(int)pMode, i];

            //    }
            //return strResult + prfn_EndMode(pMode);
        }


        static private long[,] prfn_LongestCommonSubsequenceArr(ref string[] Array1, ref string[] Array2)
        {
            return null;
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_longestCommonSubsequenceArr
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : Return an array
            '           :
            ' Commments : Original code by Thydsik and can be found at
            '           : http://thydzik.com/longest-common-subsequence-implemented-in-vba-visual-basic-for-applications/
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */

            //if (UBound(Array1, 2) > 0 Or UBound(Array2, 2) > 0) { //'multidimensional arrays
            //    If Error = vbNullString Then
            //        Exit Function
            //    End If
            //}


            //if (UBound(Array1) < 0 Or UBound(Array2) < 0) { //'check if arrays are bounded
            //    If Error<> vbNullString Then
            //        Exit Function
            //    End If
            //}

            //long[,] num;


            ////'define the array, note rows of zeros get added to front automatically
            //num = new long[Array1.Length + 1, Array2.Length + 1];
            ////ReDim num(UBound(Array1) +1, UBound(Array2) + 1)

            ////'note, arrays must always start at indice zero.
            //for (int i = 0; i < Array1.Length; i++)
            //{
            //    for (int j = 0; i < Array2.Length; j++)
            //    {
            //        if (Array1[i].Equals(Array2[j]) && Array1[i] != " ")
            //        { //'Exclude spaces from comparison.
            //            num[i + 1, j + 1] = num[i, j] + 1;
            //        }
            //        else
            //        {
            //            num[i + 1, j + 1] = prfn_Max(num[i, j + 1], num[i + 1, j]);
            //        }
            //    }
            //}

            //return num;
        }

        static private FontModes prfn_GetNextMode(string[,] dif, int i)
        {
            return 0;
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_GetNextMode
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : By looking into the Dif array decide which mode should be used for the next
            '           : word/char to be printed.
            ' Commments :
            '           :
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */
            //if (!dif[0, i].Equals(""))
            //{
            //    return FontModes.Original;
            //}
            //else if (!dif[1, i].Equals(""))
            //{
            //    return FontModes.Deleted;
            //}
            //else if (!dif[2, i].Equals(""))
            //{
            //    return FontModes.Added;
            //}
            //else
            //{
            //    //' If this else statement is reached, its bacause you fucked up the code.
            //    // err.Raise 9999, "An error has occured while trying to do a text comparison."
            //}
            //return FontModes.Original;
        }

        static private long prfn_Max(long a, long b)
        {
            return 0;
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_Max
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : Return max of two longs.
            '           :
            ' Comments  : Kept private in order to not interfere with other Max methods.
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */
            //if (a >= b)
            //{
            //    return a;
            //}
            //else
            //{
            //    return b;
            //}
        }

        static private string[,,] prfn_getDiffArr(ref long[,] c, ref string[] arrayOld, ref string[] arrayNew, ref long i, ref long j)
        {
            return null;
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : prfn_getDiffArr
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-14
            ' Version   : 1.0
            ' Purpose   : Using the LCS array C to return a 3xn array where
            '           : Indice 0 entries are common to both strings
            '           : Indice 1 entries are only present in arrayOld (In other words deleted)
            '           : Indice 2 entries are only present in arrayNew (In other words added)
            '           :
            '           : This function will call itself recursively to fill the array.
            '           : It may be possible to improve the performance especially by looking at the
            '           : Error handling.
            '           :
            ' Commments : Original code made by Thydzik and can be found at
            '           : http://thydzik.com/longest-common-subsequence-implemented-in-vba-visual-basic-for-applications/
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */
            //string[] arr;
            //long bound;


            //if (i >= 0)
            //{
            //    if (j >= 0)
            //    { //'both are greater or equal to zero
            //      //'can only do the following comparison when i and j are greater or equal than zero


            //        //            if (arrayOld[i].Equals(arrayNew[j]) { //StrComp(arrayOld(i), arrayNew(j), Compare) = 0 Then
            //        //                arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j - 1); //, Compare)


            //        //                    bound = arr.GetUpperBound(2); //'check the bounding of arr

            //        //                If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(0, 0) = arrayOld(i)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(0, bound + 1) = arrayOld(i)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //            Else
            //        //        If i = 0 Then
            //        //            arr = prfn_getDiffArr(c, arrayOld, arrayNew, i, j - 1, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(2, 0) = arrayNew(j)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(2, bound + 1) = arrayNew(j)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                ElseIf c(i +1, j - 1 + 1) >= c(i - 1 + 1, j + 1) Then
            //        //           arr = prfn_getDiffArr(c, arrayOld, arrayNew, i, j - 1, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(2, 0) = arrayNew(j)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(2, bound + 1) = arrayNew(j)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                ElseIf j = 0 Then
            //        //            arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(1, 0) = arrayOld(i)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(1, bound + 1) = arrayOld(i)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                ElseIf c(i +1, j - 1 + 1) < c(i - 1 + 1, j + 1) Then
            //        //           arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(1, 0) = arrayOld(i)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(1, bound + 1) = arrayOld(i)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                Else
            //        //            prfn_getDiffArr = arr
            //        //                End If
            //        //            End If
            //        //        Else 'i is is greater or equal to zero
            //        //                If j = 0 Then
            //        //            arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(1, 0) = arrayOld(i)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(1, bound + 1) = arrayOld(i)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                ElseIf c(i +1, j - 1 + 1) < c(i - 1 + 1, j + 1) Then
            //        //           arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(1, 0) = arrayOld(i)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(1, bound + 1) = arrayOld(i)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                Else
            //        //            prfn_getDiffArr = arr
            //        //                End If
            //        //        End If
            //        //    Else
            //        //If j >= 0 Then 'j is  greater than zero
            //        //                If i = 0 Then
            //        //            arr = prfn_getDiffArr(c, arrayOld, arrayNew, i - 1, j, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(2, 0) = arrayNew(j)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(2, bound + 1) = arrayNew(j)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                ElseIf c(i +1, j - 1 + 1) >= c(i - 1 + 1, j + 1) Then
            //        //           arr = prfn_getDiffArr(c, arrayOld, arrayNew, i, j - 1, Compare)
            //        //                    bound = UBound(arr, 2) 'check the bounding of arr
            //        //                    If Error<> vbNullString Then
            //        //                err.Clear
            //        //                ReDim arr(2, 0)
            //        //                arr(2, 0) = arrayNew(j)
            //        //                    Else 'no error
            //        //                        ReDim Preserve arr(2, bound + 1)
            //        //                        arr(2, bound + 1) = arrayNew(j)
            //        //                    End If
            //        //                    prfn_getDiffArr = arr
            //        //                Else
            //        //            prfn_getDiffArr = arr
            //        //                End If
            //        //        Else 'none are greater than zero
            //        //                prfn_getDiffArr = arr
            //        //        End If
            //        //    End If
            //    }
            //}
            //return null;
        }



        //public void TSCs_ReportUnexpectedError(string strProcName = "", string strModuleName = "", string strCustomInfo = "") {
        //    /*'---------------------------------------------------------------------------------------
        //    '---------------------------------------------------------------------------------------
        //    ' Procedure : TSCs_ReportUnexpectedError
        //    ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
        //    ' Version   : X.X
        //    ' Purpose   : This is a much simplified version of my Err.LogAndMail, so that I don't
        //    '           : have to change all error handlers. For the real Crash Reporter visit
        //    '           : my blog www.TheSmileyCoder.com
        //    '           :
        //    '           : If you are allready using my crash reporter, you can just import this module
        //    '           : And then delete this sub.
        //    '           :
        //    ' Bugs?     : Email: SmileyCoderTools@gmail.com
        //    '---------------------------------------------------------------------------------------
        //    '---------------------------------------------------------------------------------------

        //    return "An error [" & err.number & "] has occured in " & vbNewLine & _
        //        "Procedure: " & "[" & strProcName & "]" & vbNewLine & _
        //        IIf(strModuleName <> "", "Module: " & "[" & strModuleName & "]" & vbNewLine, "") & _
        //        IIf(strCustomInfo <> "", vbNewLine & "Further details:" & vbNewLine & strCustomInfo, "") & _
        //        vbNewLine & err.Description, vbCritical, "Error in Application"
        //        */
        //}


        static public bool TSC_CompareByLCS(string varText1, string varText2, CompareType compareMethod = CompareType.ByCharacter, int MaxDif = 1)
        {
            return false;
            /*
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            ' Procedure : TSC_CompareByLCS
            ' Author    : AEC - Anders Ebro Christensen / TheSmileyCoder
            ' Date      : 2013-06-15
            ' Version   : 1.0
            ' Purpose   : Returns true if the difference between 2 strings is less then MaxDif
            '           :
            ' Commments : This can be used for fuzzy matching, such as matching Nancy to Nanci
            '           : It can also be used wordwise, to match texts where only a few words
            '           : have been replaced.
            ' Bugs?     : Email: SmileyCoderTools@gmail.com
            '---------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------
            */

            //long lLCS;
            //string strText1;
            //string strText2;

            //// EB - not needed in C# version
            ////'Convert input variants to text
            ////strText1 = varText1 & ""
            ////strText2 = varText2 & ""
            //strText1 = varText1;
            //strText2 = varText2;

            ////'If the fuzzy factor is less then the difference in lengths of the strings, it makes no sense to compare
            //if (Math.Abs(strText1.Length - strText2.Length) > MaxDif)
            //{
            //    return false;
            //}


            ////'Find how much they have in common
            //lLCS = TSC_LCS_Length(varText1, varText2, compareMethod);


            ////'Compare this to the length of the string and the provided MaxDif
            //switch (compareMethod)
            //{
            //    case CompareType.ByCharacter:
            //        if (Math.Abs(lLCS - prfn_Max(strText2.Length, strText1.Length)) <= MaxDif)
            //        {
            //            return true;
            //        }
            //        break;
            //    case CompareType.ByWords:
            //        if (Math.Abs(lLCS - prfn_StringToWordArray(strText1).Length + 1) <= MaxDif)
            //        {
            //            return true;
            //        }
            //        break;
            //}

            //return false;
        }

        static public long TSC_LCS_Length(string varOldText, string varNewText, CompareType compareMethod = CompareType.ByCharacter)
        {
            return 0;
            //string strOld;
            //string strNew;
            //strOld = varOldText;
            //strNew = varNewText;
            //string[] a1;
            //string[] a2;
            //switch (compareMethod)
            //{
            //    case CompareType.ByCharacter:
            //        a1 = prfn_StringToCharArray(ref strOld);
            //        a2 = prfn_StringToCharArray(ref strNew);
            //        break;
            //    case CompareType.ByWords:
            //        a1 = prfn_StringToWordArray(strOld);
            //        a2 = prfn_StringToWordArray(strNew);
            //        break;
            //    default:
            //        return 0;
            //        //err.Raise 5000, , "Invalid parameter passed as CompareMethod"
            //}


            //long[,] c;

            //c = prfn_LongestCommonSubsequenceArr(ref a1, ref a2); // Compare);
            //return c[c.GetUpperBound(1), c.GetUpperBound(2)];
        }

        #endregion  
    }
}

