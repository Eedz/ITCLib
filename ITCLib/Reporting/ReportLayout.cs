using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXMLExtensions;

namespace ITCLib
{
    public class ReportLayout
    {

        public PaperSizes PaperSize { get; set; }
        public FileFormats FileFormat { get; set; }
        public TableOfContents ToC { get; set ; }
        public bool CoverPage { get ; set; }
        public bool BlankColumn { get ; set ; }


        public ReportLayout()
        {
            PaperSize = PaperSizes.Letter;
            FileFormat = FileFormats.DOC;
            ToC = TableOfContents.None;
            CoverPage = false;
            BlankColumn = false;
        }

        /// <summary>
        /// Format any row in the table that contains [LitQ] tags
        /// </summary>
        /// <param name="table"></param>
        public void FormatSubTables(Table table, int qnumCol, int varCol, int wordCol)
        {
            string qnum;
            string varname;
            string wording;
            int mainQnum = 0;
            string litqNewRow;

            string[] ROArray = null;
            int roStart;
            int roEnd;
            string respOptionsText = "";
            double placeLeft;

            var rows = table.Elements<TableRow>();

            // for each row in the table, examine the qnum for series questions
            for (int r = 1; r < rows.Count(); r++)
            {
                TableRow currentRow = rows.ElementAt(r);

                // get this row's cells
                var cells = rows.ElementAt(r).Elements<TableCell>();

                // get the cell's containing wordings, qnum and varname
                TableCell wordCell = cells.ElementAt(wordCol);
                TableCell varCell = cells.ElementAt(varCol);
                TableCell qnumCell = cells.ElementAt(qnumCol);

                varname = varCell.GetCellText();

                // skip headings
                if (varname.StartsWith("Z"))
                    continue;

                qnum = qnumCell.GetCellText();

                // skip non-series questions
                if (Char.IsDigit(Char.Parse(qnum.Substring(qnum.Length - 1, 1))))
                    continue;

                wording = wordCell.GetCellText();

                // check for [LitQ] and table tags
                if (wording.Contains("[LitQ]") && wording.Contains("[TBLROS]") && wording.Contains("[TBLROE"))
                {
                    roStart = wording.IndexOf("[TBLROS]") + "[TBLROS]".Length;
                    roEnd = wording.IndexOf("[TBLROE]") + "[TBLROE]".Length;

                    respOptionsText = wording.Substring(roStart, roEnd - roStart);

                    // now extract the LitQ from the first member of the series
                    litqNewRow = wording.Substring(wording.IndexOf("[LitQ]") + "[LitQ]".Length, wording.IndexOf("[/LitQ]") - wording.IndexOf("[LitQ]") - "[/LitQ]".Length + 1);

                    // get respOption numbers
                    ROArray = GetRespNums(wording, roStart, roEnd);

                    // and replace LitQ it with nothing
                    wording = wording.Replace("[LitQ]" + litqNewRow + "[/LitQ]", string.Empty);
                    wording = wording.Replace("[indent]", string.Empty);
                    wording = wording.Replace("[/indent]", string.Empty);
                    wording = wording.Replace("[TBLROS]", string.Empty);
                    wording = wording.Replace("[TBLROE]", string.Empty);
                    wording = wording.Replace("\r\n\r\n", "\r\n");

                    // set this cell to the question wording minus the LitQ
                    wordCell.SetCellText(wording);

                    litqNewRow = Utilities.TrimString(litqNewRow, "<br>");

                    // put indents around 'new' litq
                    litqNewRow = "[indent]" + litqNewRow + "[/indent]";

                    // merge row into a single cell, set the qnum cell to contain the wording so that the resulting merged cell contains the wording
                    qnumCell.SetCellText(wording);
                    //Utilities.SetCellText((TableCell)cells.ElementAt(varCol), string.Empty);

                    TableCellProperties firstCellProps = new TableCellProperties();
                    firstCellProps.Append(new HorizontalMerge() { Val = MergedCellValues.Restart });

                    cells.ElementAt(qnumCol).Append(firstCellProps);

                    TableCellProperties otherCellProps = new TableCellProperties();
                    otherCellProps.Append(new HorizontalMerge() { Val = MergedCellValues.Continue });

                    cells.ElementAt(varCol).Append(otherCellProps);

                    otherCellProps = new TableCellProperties();
                    otherCellProps.Append(new HorizontalMerge() { Val = MergedCellValues.Continue });

                    cells.ElementAt(wordCol).Append(otherCellProps);

                    // now add a new row after this one for the LitQ
                    TableRow newrow = new TableRow();
                    TableCell newQnum = new TableCell();
                    newQnum.SetCellText(qnum);
                    TableCell newVar = new TableCell();
                    newVar.SetCellText(varname);
                    TableCell newWording = new TableCell();
                    newWording.SetCellText(litqNewRow);

                    newrow.Append(newQnum);
                    newrow.Append(newVar);
                    newrow.Append(newWording);

                    table.InsertAfter(newrow, rows.ElementAt(r));

                    mainQnum = Int32.Parse(qnum.Substring(0, 3));
                }
                else if (Int32.Parse(qnum.Substring(0, 3)) == mainQnum && Int32.Parse(qnum.Substring(0, 3)) != 0)
                {
                    // member of the current series
                    // record qnum, varname and wording
                    if (!wording.StartsWith("[indent]"))
                        wording = "[indent]" + wording + "[/indent]";

                    placeLeft = 11 - 1 * 2;

                    TableCellProperties cellProps = new TableCellProperties();

                    TableCell currentCell;

                    // qnum
                    currentCell = currentRow.Elements<TableCell>().ElementAt(qnumCol);
                    currentCell.PrependChild(new TableCellProperties(new TableCellWidth()
                    {
                        Width = Convert.ToString(0.57 * 1440),
                        Type = TableWidthUnitValues.Dxa
                    }));
                    placeLeft -= 0.57;

                    // varname
                    currentCell = currentRow.Elements<TableCell>().ElementAt(varCol);
                    currentCell.PrependChild(new TableCellProperties(new TableCellWidth()
                    {
                        Width = Convert.ToString(1.1 * 1440),
                        Type = TableWidthUnitValues.Dxa
                    }));
                    placeLeft -= 0.57;

                    // question
                    currentCell = currentRow.Elements<TableCell>().ElementAt(wordCol);
                    currentCell.PrependChild(new TableCellProperties(new TableCellWidth()
                    {
                        Width = Convert.ToString((placeLeft - (ROArray.Length * 0.7)) * 1440),
                        Type = TableWidthUnitValues.Dxa
                    }));

                    placeLeft -= (placeLeft - (ROArray.Length - 1) * 0.7);

                    for (int i = 0; i < ROArray.Length; i++)
                    {
                        currentCell = new TableCell();
                        currentCell.SetCellText(ROArray[i]);
                        currentCell.PrependChild(new TableCellProperties(new TableCellWidth()
                        {
                            Width = Convert.ToString(0.7 * 1440),
                            Type = TableWidthUnitValues.Dxa
                        }));
                        placeLeft -= 0.7;

                        currentRow.Append(currentCell);
                    }
                }
            }

            // for each regular row in the table, set the gridSpan for the word column to be the span of the word column to the column count
            for (int r = 0; r < rows.Count(); r++)
            {
                var currentRow = table.Elements<TableRow>().ElementAt(r);

                var rowCells = currentRow.Elements<TableCell>();
                int columns = table.Elements<TableGrid>().ElementAt(0).Elements<GridColumn>().Count();

                TableCell questionCell;

                if (rowCells.Count() == wordCol + 1)
                {
                    questionCell = rowCells.ElementAt(wordCol);
                    questionCell.Append(new TableCellProperties(new GridSpan() { Val = columns - wordCol }));

                }
                else if (rowCells.Count() > 1)
                {
                    questionCell = rowCells.ElementAt(wordCol);
                    questionCell.Append(new TableCellProperties(new GridSpan() { Val = (columns - wordCol) - (rowCells.Count() - wordCol + 1) }));
                }
            }
        }
        
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

        
        private bool EndsWithSpecial (string input)
        {
            Regex rx = new Regex("[0-9A-Z]");
            return rx.IsMatch(input.Substring(input.Length-1,1)) || input.EndsWith("]") || input.EndsWith(")") || input.EndsWith (">") || input.EndsWith("_");
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

        

        
    }
}
