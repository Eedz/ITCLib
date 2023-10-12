using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;

namespace ITCLib
{

    /* 
            // Environment.GetFolderPath
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Current User's Application Data
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); // All User's Application Data
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles); // Program Files
            Environment.GetFolderPath(Environment.SpecialFolder.Cookies); // Internet Cookie
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Logical Desktop
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); // Physical Desktop
            Environment.GetFolderPath(Environment.SpecialFolder.Favorites); // Favorites
            Environment.GetFolderPath(Environment.SpecialFolder.History); // Internet History
            Environment.GetFolderPath(Environment.SpecialFolder.InternetCache); // Internet Cache
            Environment.GetFolderPath(Environment.SpecialFolder.MyComputer); // "My Computer" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // "My Documents" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); // "My Music" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); // "My Pictures" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.Personal); // "My Document" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); // Program files Folder
            Environment.GetFolderPath(Environment.SpecialFolder.Programs); // Programs Folder
            Environment.GetFolderPath(Environment.SpecialFolder.Recent); // Recent Folder
            Environment.GetFolderPath(Environment.SpecialFolder.SendTo); // "Sent to" Folder
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu); // Start Menu
            Environment.GetFolderPath(Environment.SpecialFolder.Startup); // Startup
            Environment.GetFolderPath(Environment.SpecialFolder.System); // System Folder
            Environment.GetFolderPath(Environment.SpecialFolder.Templates); // Document Templates
            */
    public static class Utilities
    {
        public static DataTable CreateDataTable(string name, string[] fields, string[] types)   
        {
            DataTable dt;
            System.Type dataType = null;
            dt = new DataTable(name);

            if (fields.Length != types.Length)
            {
                throw new System.ArgumentException("fields and types must have the same number of elements.");
            }

            for (int i = 0; i < fields.Length; i ++)
            {
                switch (types[i])
                {
                    case "string":
                        dataType = System.Type.GetType("System.String");
                        break;
                    case "int":
                        dataType = System.Type.GetType("System.Int32");
                        break;
                    default:
                        dataType = System.Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(new DataColumn(fields[i], dataType));

            }
            return dt;
        }

        public static void Export_Data_To_Word(DataGridView DGV, string filename)
        {
            if (DGV.Rows.Count != 0)
            {
                int RowCount = DGV.Rows.Count;
                int ColumnCount = DGV.Columns.Count;
                Object[,] DataArray = new object[RowCount + 1, ColumnCount + 1];

                //add rows
                int r = 0;
                for (int c = 0; c <= ColumnCount - 1; c++)
                {
                    for (r = 0; r <= RowCount - 1; r++)
                    {
                        DataArray[r, c] = DGV.Rows[r].Cells[c].Value;
                    } //end row loop
                } //end column loop

                Word.Document oDoc = new Word.Document();
                oDoc.Application.Visible = true;

                //page orintation
                oDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;


                dynamic oRange = oDoc.Content.Application.Selection.Range;
                string oTemp = "";
                for (r = 0; r <= RowCount - 1; r++)
                {
                    for (int c = 0; c <= ColumnCount - 1; c++)
                    {
                        oTemp = oTemp + DataArray[r, c] + "\t";

                    }
                }

                //table format
                oRange.Text = oTemp;

                object Separator = Word.WdTableFieldSeparator.wdSeparateByTabs;
                object ApplyBorders = true;
                object AutoFit = true;
                object AutoFitBehavior = Word.WdAutoFitBehavior.wdAutoFitContent;

                oRange.ConvertToTable(ref Separator, ref RowCount, ref ColumnCount,
                                      Type.Missing, Type.Missing, ref ApplyBorders,
                                      Type.Missing, Type.Missing, Type.Missing,
                                      Type.Missing, Type.Missing, Type.Missing,
                                      Type.Missing, ref AutoFit, ref AutoFitBehavior, Type.Missing);

                oRange.Select();

                oDoc.Application.Selection.Tables[1].Select();
                oDoc.Application.Selection.Tables[1].Rows.AllowBreakAcrossPages = 0;
                oDoc.Application.Selection.Tables[1].Rows.Alignment = 0;
                oDoc.Application.Selection.Tables[1].Rows[1].Select();
                oDoc.Application.Selection.InsertRowsAbove(1);
                oDoc.Application.Selection.Tables[1].Rows[1].Select();

                //header row style
                oDoc.Application.Selection.Tables[1].Rows[1].Range.Bold = 1;
                oDoc.Application.Selection.Tables[1].Rows[1].Range.Font.Name = "Tahoma";
                oDoc.Application.Selection.Tables[1].Rows[1].Range.Font.Size = 14;

                //add header row manually
                for (int c = 0; c <= ColumnCount - 1; c++)
                {
                    oDoc.Application.Selection.Tables[1].Cell(1, c + 1).Range.Text = DGV.Columns[c].HeaderText;
                }

                //table style 
                oDoc.Application.Selection.Tables[1].set_Style("Grid Table 4 - Accent 5");
                oDoc.Application.Selection.Tables[1].Rows[1].Select();
                oDoc.Application.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //header text
                foreach (Word.Section section in oDoc.Application.ActiveDocument.Sections)
                {
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Word.WdFieldType.wdFieldPage);
                    headerRange.Text = "your header text";
                    headerRange.Font.Size = 16;
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }

                //save the file
                oDoc.SaveAs2(filename);

                //NASSIM LOUCHANI
            }
        }

        public static DateTime PreviousWorkDay(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            }
            while (IsWeekend(date));
                
            return date;
            
        }

        private static bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static string TrimString (string input, string totrim)
        {
            while (input.EndsWith(totrim))
            {
                input = input.Substring(0, input.Length - totrim.Length);
            }
            while (input.StartsWith(totrim))
            {
                input = input.Substring(totrim.Length);
            }
            return input;
        }

        public static int CountLines(string input)
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

        // returns the first record matching the criteria
        public static string DTLookup(DataTable dt, string field, string criteria)
        {

            DataRow[] dr = dt.Select(criteria);
            string result = "";
            if (dr.Length == 0)
            {
                result = "";
            }
            else
            { 
                result = (string)dr[0][field];
            }
            

            return result;
        }

       

        public static string ChangeCC (string varname, string cc = "00")
        {
            string result = "";
            VarNameFormat format = GetVarNameFormat(varname);
            
            

            if (varname.Equals("") || Convert.ToInt32(cc) < 0 || Convert.ToInt32(cc)>99) { result = ""; }

            if (format == VarNameFormat.NonStd) { result = varname; }

            if (cc.Equals("00") || cc.Equals("0"))
            {
                if (format == VarNameFormat.NoCC)
                {
                    result = varname;
                }else if (format== VarNameFormat.WithCC)
                {
                    result = varname.Substring(0, 2) + varname.Substring(4);
                }
                
            } else
            {
                if (format == VarNameFormat.NoCC)
                {
                    result = varname.Substring(0,2) + cc + varname.Substring(2);
                }
                else if (format == VarNameFormat.WithCC)
                {
                    result = varname.Substring(0, 2) + cc + varname.Substring(4);
                }
            }

            return result;
            

        }
        
        public static VarNameFormat GetVarNameFormat (string varname)
        {
            VarNameFormat result;
            Regex rx;

            rx = new Regex("[A-Z]{2}\\d{5}");

            if (rx.Match(varname).Success)
            {
                result = VarNameFormat.WithCC;
            }
            else
            {
                rx = new Regex("[A-Z]{2}\\d{3}");
                if (rx.Match(varname).Success)
                {
                    result = VarNameFormat.NoCC;
                }
                else
                {
                    result = VarNameFormat.NonStd;
                }
            }

            return result;
        }

        public static string ExtractVarName (string input)
        {
            string var = "";
            Regex rx = new Regex("[A-Z]{2}\\d{3}[a-z]*");

            if (rx.Match(input).Success)
            {
                var = rx.Matches(input)[0].Value;
            }
            return var;
        }

        // TODO add more tags, RTF, HTML, see VBA version
        public static string RemoveTags (string input)
        {
            if (input == null)
                return "";

            string output = input;
            output = output.Replace("[yellow]", "");
            output = output.Replace("[/yellow]", "");
            output = output.Replace("[brightgreen]", "");
            output = output.Replace("[/brightgreen]", "");
            output = output.Replace("[t]", "");
            output = output.Replace("[s]", "");
            output = output.Replace("[/t]", "");
            output = output.Replace("[/s]", "");
            output = output.Replace("<strong>", "");
            output = output.Replace("</strong>", "");
            output = output.Replace("<em>", "");
            output = output.Replace("</em>", "");
            output = output.Replace("<s>", "");
            output = output.Replace("</s>", "");
            output = output.Replace("<u>", "");
            output = output.Replace("</u>", "");
            output = Regex.Replace(output, "<font style=\"BACKGROUND-COLOR:#[0-9A-F]{6}\">", "");
            output = Regex.Replace(output, "</font>", "");


            return output;
        }

        public static string PrepareTextCompare(string input)
        {
            if (input == null)
                return "";

            string output = input;
            output = output.Replace("[yellow]", "");
            output = output.Replace("[/yellow]", "");
            output = output.Replace("[brightgreen]", "");
            output = output.Replace("[/brightgreen]", "");
            output = output.Replace("[t]", "");
            output = output.Replace("[s]", "");
            output = output.Replace("[/t]", "");
            output = output.Replace("[/s]", "");
            output = output.Replace("<strong>", "");
            output = output.Replace("</strong>", "");
            output = output.Replace("<em>", "");
            output = output.Replace("</em>", "");
            output = output.Replace("<s>", "");
            output = output.Replace("</s>", "");
            output = output.Replace("<u>", "");
            output = output.Replace("</u>", "");
            output = output.Replace("<br>", "");
            output = Regex.Replace(output, "<font style=\"BACKGROUND-COLOR:#[0-9A-F]{6}\">", "");
            output = output.Replace("</font>", "");
            output = output.Replace("\n", "");
            output = output.Replace("\r", "");
            output = output.Replace(" ", "");
            



            return output;
        }


        public static string RemoveHighlightTags(string input)
        {
            if (input == null)
                return "";

            string output = input;
            output = output.Replace("[yellow]", "");
            output = output.Replace("[/yellow]", "");
            output = output.Replace("[brightgreen]", "");
            output = output.Replace("[/brightgreen]", "");
            output = output.Replace("[t]", "");
            output = output.Replace("[s]", "");
            output = output.Replace("[/t]", "");
            output = output.Replace("[/s]", "");
            output = output.Replace("[pinkfill]", "");
            output = output.Replace("[bluefill]", "");
            output = output.Replace("<Font Color=Red>", "");
            output = output.Replace("<Font Color=Blue>", "");
            output = output.Replace("</Font>", "");


            return output;
        }

        public static string StripChars (string input)
        {
            Regex rx = new Regex("[^A-Za-z0-9 ]");

            input = rx.Replace(input, string.Empty);

            return input;
        }

        public static string StripChars(string input, string pattern)
        {
            Regex rx = new Regex("[^" + pattern + "]");

            input = rx.Replace(input, string.Empty);

            return input;
        }

        // TODO test both UTF8 and ASCII
        public static bool ContainsNonLatin(string input)
        {
            byte[] b;
            b = Encoding.UTF8.GetBytes(input);
            //b = Encoding.ASCII.GetBytes(input);
            for (int i = 0; i < b.Length; i += 2)
            {
                if (b[i] > 0)
                    return true;

                break;
            }
            return false;
        }

        /// <summary>
        /// Returns the Qnum without the suffix.
        /// </summary>
        /// <param name="qnum"></param>
        public static string GetSeriesQnum (string qnum)
        {
            int letterPosition = 0;
            for (int i = 0; i < qnum.Length; i++)
            {
                if (char.IsLetter(qnum[i]) || qnum[i] == '`')
                {
                    letterPosition = i;
                    break;
                }
            }

            if (letterPosition == 0)
            {
                return qnum;
            }
            else
            {
                return qnum.Substring(0, letterPosition);
            }
        }

        public static string GetQnumSuffix(string qnum)
        {
            string suffix = "";
            if (char.IsLetter(qnum[qnum.Length-1]))
            {
                for (int i = 0; i< qnum.Length;  i++)
                {
                    if (char.IsLetter(qnum[i]))
                        suffix += qnum[i];
                }
            }
            else
            {
                return "";
            }
            return suffix;
        }

        public static string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ");
        }


        // TODO eliminate double line breaks after indent tags
        /// <summary>
        /// Formats a given string in RTF format.
        /// </summary>
        /// <param name="wordingText"></param>
        /// <param name="indents"></param>
        /// <returns></returns>
        public static string FormatText(string wordingText, bool indents = false)
        {
            string colorTbl = @"{\colortbl;";

            if (wordingText.Contains("<font style=\"BACKGROUND-COLOR:#FFFF00\">") || wordingText.Contains("[yellow]"))
                colorTbl += @"\red255\green255\blue0;";

            //if (wordingText.Contains("<font style=\"BACKGROUND-COLOR:#00FF00\">") || wordingText.Contains("[yellow]"))
            //    colorTbl += @"\red0\green255\blue0;";

            colorTbl += "}";

            string wording = wordingText;
            wording = wording.Replace("<strong>", @"\b ");
            wording = wording.Replace("</strong>", @"\b0 ");
            wording = wording.Replace("<em>", @"\i ");
            wording = wording.Replace("</em>", @"\i0 ");
            wording = wording.Replace("<u>", @"\ul ");
            wording = wording.Replace("</u>", @"\ul0 ");
            wording = wording.Replace("<s>", @"\strike ");
            wording = wording.Replace("</s>", @"\strike0 ");
            wording = wording.Replace("<br>", @"\line ");
            wording = wording.Replace("\r\n", @"\line ");
            wording = wording.Replace("<font style=\"BACKGROUND-COLOR:#FFFF00\">", @"\highlight1 ");
            wording = wording.Replace("[yellow]", @"\highlight1 ");
            wording = wording.Replace("[/yellow]", @"\highlight0 ");
            wording = wording.Replace("</font>", @"\highlight0 ");
            wording = wording.Replace("&nbsp;", " ");


            if (indents)
            {
                wording = wording.Replace("[indent]", @"\li360 ");
                wording = wording.Replace("[/indent]", @"\line \li0");
                wording = wording.Replace("[indent3]", @"\li720 ");
                wording = wording.Replace("[/indent3]", @"\line \li0");
            }
            wording = @"{\rtf1\ansi " + colorTbl + wording + "}";

            return wording;
        }

        // TODO eliminate double line breaks after indent tags
        /// <summary>
        /// Converts a string from RTF to Access RTF.
        /// </summary>
        /// <param name="wordingText"></param>
        /// <param name="indents"></param>
        /// <returns></returns>
        public static string FormatRTF(string wordingText, bool indents = false)
        {
            string wording = wordingText;
            
            wording = wording.Replace(@"{\colortbl;\red255\green255\blue0; }", "");
            wording = wording.Replace(@"\highlight1\f0\fs18\lang1033", @"\f0\fs18\lang1033 [yellow]");

            wording = wording.Replace(@"\bullet ", "[bullet]");
            wording = wording.Replace(@"\b0 ", "</strong>");
            wording = wording.Replace(@"\b0", "</strong>");
            wording = wording.Replace(@"\b ", "<strong>");
            wording = wording.Replace(@"\b", "<strong>");

            wording = wording.Replace(@"\i0 ", "</em>");
            wording = wording.Replace(@"\i0", "</em>");
            wording = wording.Replace(@"\i ", "<em>");
            wording = wording.Replace(@"\i", "<em>");

            wording = wording.Replace(@"\ul0 ", "</u>");
            wording = wording.Replace(@"\ul0", "</u>");
            wording = wording.Replace(@"\ulnone ", "</u>");
            wording = wording.Replace(@"\ulnone", "</u>");
            wording = wording.Replace(@"\ul ", "<u>");
            wording = wording.Replace(@"\ul", "<u>");
            
            wording = wording.Replace(@"\strike ", "<s>");
            wording = wording.Replace(@"\strike0 ", "</s>");
            wording = wording.Replace(@"\highlight1 ", "[yellow]");
            wording = wording.Replace(@"\highlight0", "[/yellow]");

            wording = wording.Replace(@"\pntext•", "[bullet]");
            
            wording = wording.Replace(@"\line ", "<br>");
            wording = wording.Replace(@"\line", "<br>");
            wording = wording.Replace(@"\pard", @"\drap");
            wording = wording.Replace(@"\par", "<br>");
            wording = wording.Replace(@"\drap", @"\pard");

            
            wording = wording.Replace(@"{\rtf1\ansi ", "");
            wording = Utilities.TrimString(wording, "}");

            return wording;
        }

        

        /// <summary>
        /// Converts a string from RTF to plain text.
        /// </summary>
        /// <param name="wordingText"></param>
        /// <param name="indents"></param>
        /// <returns></returns>
        public static string FormatPlainText(string wordingText, bool indents = false)
        {
            string wording = wordingText;

            wording = wording.Replace("<br>", "\r\n");
            wording = wording.Replace("&nbsp;", " ");

            return wording;
        }

        /// <summary>
        /// Determines the type of questions for the given row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>QuestionType enum based on the Qnum and VarName.</returns>
        public static QuestionType GetQuestionType(SurveyQuestion q)
        {
            string qnum = q.Qnum;
            string varname = q.VarName.VarName;

            int head = Int32.Parse(GetSeriesQnum(qnum));
            string tail = GetQnumSuffix(qnum);

            QuestionType qType;

            // get Question Type
            if (varname.StartsWith("Z"))
            {
                if (varname.EndsWith("s"))
                    qType = QuestionType.Subheading; // subheading
                else
                    qType = QuestionType.Heading; // heading
            }
            else if (varname.StartsWith("HG"))
            {
                qType = QuestionType.Standalone; // QuestionType.InterviewerNote; // interviewer note
            }
            else
            {
                if ((tail == "" || tail == "a") && (head != 0))
                    qType = QuestionType.Standalone; // standalone or first in series
                else
                    qType = QuestionType.Series; // series
            }
            return qType;
        }

        /// <summary>
        /// Remove duplicate records from data table
        /// </summary>
        /// <param name="table">DataTable for removing duplicate records</param>
        /// <param name="DistinctColumn">Column to check for duplicate values or records</param>
        /// <returns></returns>
        public static DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                ArrayList UniqueRecords = new ArrayList();
                ArrayList DuplicateRecords = new ArrayList();

                // Check if records is already added to UniqueRecords otherwise,
                // Add the records to DuplicateRecords
                foreach (DataRow dRow in table.Rows)
                {
                    if (UniqueRecords.Contains(dRow[DistinctColumn]))
                        DuplicateRecords.Add(dRow);
                    else
                        UniqueRecords.Add(dRow[DistinctColumn]);
                }

                // Remove duplicate rows from DataTable added to DuplicateRecords
                foreach (DataRow dRow in DuplicateRecords)
                {
                    table.Rows.Remove(dRow);
                }

                // Return the clean DataTable which contains unique records.
                return table;
            }
            catch (Exception)
            {
                return null;
            }
        }

       

        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        public static string SafeGetString(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        public static int SafeGetInt(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return 0;
        }

        public static DateTime? SafeGetDate(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);
            if (!reader.IsDBNull(colIndex))
                return reader.GetDateTime(colIndex);
            return null;
        }

        public static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F]"; //\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            string result = Source;
            if (Place>-1)
                result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.LastIndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static bool ContainsSubsequence<T>(List<T> sequence, List<T> subsequence)
        {
            return
                Enumerable
                    .Range(0, sequence.Count - subsequence.Count + 1)
                    .Any(n => sequence.Skip(n).Take(subsequence.Count).SequenceEqual(subsequence));
        }

        public static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Name";

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public static decimal GetMedian(IEnumerable<double> source)
        {
            // Create a copy of the input, and sort the copy
            double[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[count / 2 - 1];
                double b = temp[count / 2];
                return (decimal)(a + b) / 2m;
            }
            else
            {
                // count is odd, return the middle element
                return (decimal)temp[count / 2];
            }
        }

        public static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
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

        public static string FixBiDirectionalString(string textToFix)
        {
            try
            {
                char RLE = '\u202B';
                char PDF = '\u202C';
                char LRM = '\u200E';
                char RLM = '\u200F';

                StringBuilder sb = new StringBuilder(textToFix.Replace("\r", "").Replace("\n", string.Format("{0}", '\u000A')));

                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[A-Za-z0-9-+ ]+");
                System.Text.RegularExpressions.MatchCollection mc = r.Matches(sb.ToString());
                foreach (System.Text.RegularExpressions.Match m in mc)
                {
                    double tmp;
                    if (m.Value == " ")
                        continue;
                    if (double.TryParse(RemoveAcceptedChars(m.Value), out tmp))
                        continue;

                    string mTrim = m.Value.Trim();
                    sb.Replace(m.Value, " " + LRM + mTrim + " " + RLM);
                    // above 2 lines instead of this one
                    //sb.Replace(m.Value, LRM + m.Value + RLM);
                }

                return RLE + sb.ToString() + PDF;
            }
            catch { return textToFix; }

        }

        private static string RemoveAcceptedChars(string p)
        {
            return p.Replace("+", "").Replace("-", "").Replace("*", "").Replace("/", "");
        }

        public static bool IsArabic(char ch)
        {
            if (ch >= '\u0627' && ch <= '\u0649') return true;
            return false;
        }

        public static bool IsArabic(string strCompare)
        {
            char[] chars = strCompare.ToCharArray();
            foreach (char ch in chars)
                if (ch >= '\u0627' && ch <= '\u0649') return true;
            return false;
        }

        public static bool IsHebrew(string strCompare)
        {
            char[] chars = strCompare.ToCharArray();
            foreach (char ch in chars)
                if ((ch >= '\u0580' && ch <= '\u05ff') || (ch >= '\ufb1d' && ch <= '\ufb4f')) return true;
            return false;
        }

        public static bool IsHebrew(char ch)
        {
            if ((ch >= '\u0580' && ch <= '\u05ff') || (ch >= '\ufb1d' && ch <= '\ufb4f')) return true;
            return false;


        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                if (!dataTable.Columns.Contains(prop.Name))
                    dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                //var values = new object[Props.Length];
                var values = new object[dataTable.Columns.Count];
                for (int i = 0; i < dataTable.Columns.Count; i++) //; Props.Count;
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


    }

    public class ListtoDataTableConverter
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
