using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace ITCSurveyReportLib
{
    public class ReportLayout
    {
        PaperSizes paperSize;
        FileFormats fileFormat;
        TableOfContents toc;
        bool coverPage;
        bool blankColumn;

        public ReportLayout()
        {
            paperSize = PaperSizes.Letter;
            fileFormat = FileFormats.DOC;
            toc = TableOfContents.None;
            coverPage = false;
            blankColumn = false;
        }

        // TODO test alot and fix spacing issues, some Clean and other string functions could be made into Utilities.
        public int FormatTables (Word.Document doc, bool useEnglish)
        {
            string qnum;
            string qnumNewRow;
            string varnameNewRow;
            string wording;
            string respOptionsText="";
            string litqNewRow;
            string mergedRow;
            string[] ROArray = null;
            int roStart;
            int roEnd;

            int qnumCol;
            int wordCol;
            int rows;
            int columns;
            int mainQnum = 0;
            double placeLeft;
            Word.Table surveyTable;
            surveyTable = doc.Tables[1];
            if (useEnglish)
            {
                surveyTable.Columns[3].Delete();
                surveyTable.Columns[3].Width = 9 * 72;
            }

            if (blankColumn)
            {
                surveyTable.Columns[3].Width = (float)7.6 * 72;
                surveyTable.Columns[4].Width = (float)1.4 * 72;
            }

            rows = surveyTable.Rows.Count;
            columns = surveyTable.Columns.Count;

            surveyTable.AllowAutoFit = false;
            //surveyTable.Select();

            qnumCol = 1;
            wordCol = 3;

            foreach (Word.Row row in surveyTable.Rows)
            {
                qnum = Utilities.StripChars(row.Cells[qnumCol].Range.Text);
                wording = row.Cells[wordCol].Range.Text;
                if (qnum.Equals("Q") || qnum.Equals("reghead") || qnum.Equals("subhead"))
                    continue;

                // check if its a series (ends in a letter)
                if (!Char.IsDigit(Char.Parse(qnum.Substring(qnum.Length - 1, 1))))
                {

                    // check if the TBLStart tag is in the question
                    if (wording.IndexOf("[TBLROS]") > 0 && wording.IndexOf("[TBLROE]") > 0 && wording.IndexOf("[LitQ]") > 0)
                    {
                        // get the qnum, VarName and question from the first member of the series
                        qnumNewRow = row.Cells[qnumCol].Range.Text;
                        varnameNewRow = row.Cells[qnumCol + 1].Range.Text;

                        roStart = wording.IndexOf("[TBLROS]") + "[TBLROS]".Length;
                        roEnd = wording.IndexOf("[TBLROE]") + "[TBLROE]".Length;

                        if (useEnglish)
                        {
                            respOptionsText = wording.Substring(roStart, roEnd - roStart);
                        }
                        else
                        {
                            respOptionsText = wording.Substring(roStart, roEnd - roStart); // need to get english? or just check if this has '*2*3*'
                        }

                        // now extract the LitQ from the first member of the series
                        //litqNewRow = wording.Substring(wording.IndexOf("[LitQ]") + "[LitQ]".Length, wording.IndexOf("[/LitQ]") - wording.IndexOf("[LitQ]") + "[LitQ]".Length);
                        litqNewRow = wording.Substring(wording.IndexOf("[LitQ]") + "[LitQ]".Length, wording.IndexOf("[/LitQ]") - wording.IndexOf("[LitQ]") - "[/LitQ]".Length + 1);
                        // get respOption numbers
                        ROArray = GetRespNums(wording, roStart, roEnd);

                        // and replace LitQ it with nothing
                        wording = wording.Replace("[LitQ]" + litqNewRow + "[/LitQ]", string.Empty);
                        wording = wording.Replace("[indent]", string.Empty);
                        wording = wording.Replace("[/indent]", string.Empty);
                        wording = wording.Replace("[TBLROS]", string.Empty);
                        wording = wording.Replace("[TBLROE]", string.Empty);


                        

                        while (EndsWithSpecial(wording)) {
                            wording = wording.Substring(0, wording.Length - 1);
                        }
                        wording = wording.Replace("\r\n\r\n", "\r\n");

                        row.Cells[wordCol].Range.Text = wording;

                        litqNewRow = Utilities.TrimString(litqNewRow, "<br>");

                        // put indents around 'new' litq
                        litqNewRow = "[indent]" + litqNewRow + "[/indent]";

                        // remove qnum and varname, then merge row
                        row.Cells[qnumCol].Range.Text = "";
                        row.Cells[qnumCol + 1].Range.Text = "";
                        row.Cells.Merge();

                        // add row above the next row, which will hold the VarName, Qnum, and LitQ of the 'removed' question
                        surveyTable.Rows.Add(surveyTable.Rows[row.Index+1]);
                        surveyTable.Rows[row.Index + 1].Cells[qnumCol].Range.Text = qnumNewRow;
                        surveyTable.Rows[row.Index + 1].Cells[qnumCol + 1].Range.Text = varnameNewRow;
                        surveyTable.Rows[row.Index + 1].Cells[wordCol].Range.Text = litqNewRow;

                        mainQnum = Int32.Parse(qnum.Substring(0,3));
                    } else if (Int32.Parse(qnum.Substring(0,3)) == mainQnum && Int32.Parse(qnum.Substring(0,3)) != 0) {
                        // member of the current series
                        // record qnum, varname and wording
                        qnumNewRow = Clean(row.Cells[qnumCol].Range.Text);
                        varnameNewRow = Clean(row.Cells[qnumCol + 1].Range.Text);
                        wording = Clean(row.Cells[wordCol].Range.Text);
                        wording = Utilities.TrimString(wording,"\r\n");
                        if (!wording.StartsWith("[indent]"))
                            wording = "[indent]" + wording + "[/indent]";

                        // split the row into Qnum, VarName, LitQ and a column for each response option
                        row.Cells.Split(1, ROArray.Length + 3, true);

                        // format each cell in the row, insert Qnum VarName, LitQ, response options
                        //TODO add another oclumn for blankColumn
                        placeLeft = 11 - 1 * 2;
                        for (int j = 1; j <= ROArray.Length + 3; j ++)
                        {
                            row.Cells[j].Range.ParagraphFormat.LeftIndent = 0;
                            row.Cells[j].Range.ParagraphFormat.SpaceAfter = 0;

                            switch (j)
                            {
                                case 1:
                                    row.Cells[j].Range.Text = qnumNewRow;
                                    row.Cells[j].Width = (float)0.57 * 72;
                                    placeLeft -= 0.57;
                                    break;
                                case 2:
                                    row.Cells[j].Range.Text = varnameNewRow;
                                    row.Cells[j].Width = (float)1.1 * 72;
                                    placeLeft -= 1.1;
                                    break;
                                case 3:
                                    row.Cells[j].Range.Text = wording;
                                    row.Cells[j].Width = (float)(placeLeft - (ROArray.Length  -1) * 0.7 ) * 72 ;
                                    placeLeft -= (placeLeft - (ROArray.Length - 1) * 0.7);
                                    break;
                                default:
                                    row.Cells[j].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    // clear response form 'o' variables unless they need responseoptions other than yes/no
                                    if (!varnameNewRow.EndsWith("o") || (varnameNewRow.EndsWith("o") && !respOptionsText.Contains("1%2%3%")))
                                        row.Cells[j].Range.Text = ROArray[j - 4];
                                    else
                                        row.Cells[j].Range.Text = "";

                                    row.Cells[j].Width = (float)0.7 * 72;
                                    placeLeft -= 0.7;
                                    break;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        // TODO
        private string[] GetRespNums (string wording, int roStart, int roEnd)
        {
            Regex rx = new Regex("[0-9]* ");
            MatchCollection m;
            string[] result;
            string[] respArray;
            string resps = wording.Substring(roStart, roEnd - roStart + 1);
            resps = Utilities.TrimString(resps, "\r\n");

            resps = resps.Replace("[indent3]", "");
            resps = resps.Replace("[/indent3]", "");
            resps = resps.Replace("[TBLROS]", "");
            resps = resps.Replace("[TBLROE]", "");
            resps = Utilities.TrimString(resps, "\r\n");
            resps = resps.Substring(0, resps.Length - 1);

            // break apart options
            if (resps.Contains("<br>"))
                respArray = resps.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            else
                respArray = resps.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);


            result = new string[respArray.Length];

            for (int i = 0; i < respArray.Length;i++)
            {
                m = rx.Matches(respArray[i]);
                if (m.Count > 0)
                    result[i] = m[0].Value;
            }

            return result;
        }

        // TODO
        private bool EndsWithSpecial (string input)
        {
            Regex rx = new Regex("[0-9A-Z]");
            return rx.IsMatch(input.Substring(input.Length-1,1)) || input.EndsWith("]") || input.EndsWith(")") || input.EndsWith (">") || input.EndsWith("_");
           

            
        }
        // TODO
        private string Clean(string input)
        {
            //Regex rx = new Regex("[0-9A-Z\\]\\>\\)_]");
            input = input.Replace("\a", string.Empty);
            while (!EndsWithSpecial(input.Substring(input.Length - 1, 1)) && !Utilities.ContainsNonLatin(input.Substring(input.Length - 1, 1))){
                input = input.Substring(0,input.Length - 1);
            }
            return input;
        }

        public override string ToString()
        {
            PropertyInfo[] _PropertyInfos = null;
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }

        

        public PaperSizes PaperSize { get => paperSize; set => paperSize = value; }
        public FileFormats FileFormat { get => fileFormat; set => fileFormat = value; }
        public TableOfContents ToC { get => toc; set => toc = value; }
        public bool CoverPage { get => coverPage; set => coverPage = value; }
        public bool BlankColumn { get => blankColumn; set => blankColumn = value; }
    }
}
