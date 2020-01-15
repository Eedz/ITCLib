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

namespace ITCLib
{
    

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

        //public static int DTLookup(DataTable dt, string field, string criteria)
        //{
        //    int result = 0;
        //    return result;
        //}

        public static string ChangeCC (string varname, string cc = "00")
        {
            string result = "";
            VarNameFormat format = GetVarNameFormat(varname);
            
            

            if (varname.Equals("") || Convert.ToInt32(cc) < 0 || Convert.ToInt32(cc)>99) { result = ""; }

            if (format == VarNameFormat.NonStd) { result = varname; }

            if (cc.Equals("00"))
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
                    result = varname.Substring(1, 2) + varname.Substring(5);
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
            string wording = wordingText;
            wording = wording.Replace("<strong>", @"\b ");
            wording = wording.Replace("</strong>", @"\b0 ");
            wording = wording.Replace("<em>", @"\i ");
            wording = wording.Replace("</em>", @"\i0 ");
            wording = wording.Replace("<br>", @"\line ");
            wording = wording.Replace("\r\n", @"\line ");

            if (indents)
            {
                wording = wording.Replace("[indent]", @"\li360 ");
                wording = wording.Replace("[/indent]", @"\line \li0");
                wording = wording.Replace("[indent3]", @"\li720 ");
                wording = wording.Replace("[/indent3]", @"\line \li0");
            }
            wording = @"{\rtf1\ansi " + wording + "}";

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
            string varname = q.VarName.FullVarName;

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
    }
}
