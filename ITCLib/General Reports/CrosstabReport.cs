using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OpenXMLHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    public class CrosstabReport
    {
        DataTable SourceTable;
        public string ReportTitle;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\Templates\SMGLandLet.dotx";


        public CrosstabReport(DataTable table)
        {
            SourceTable = table;
            ReportTitle = "Crosstab Report";
        }

        public CrosstabReport(DataTable table, string title)
        {
            SourceTable = table;
            ReportTitle = title;
        }

        public void CreateReport()
        {
            filePath += ReportTitle + " - " + DateTime.Now.ToString("G").Replace(":", ",") + ".docx";

            Word.Application appWord;
            appWord = new Word.Application();
            appWord.Visible = false;
            Word.Document doc = appWord.Documents.Add(templateFile);
            doc.SaveAs2(filePath);
            doc.Close();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
            {

                Body body = new Body();
                wordDoc.MainDocumentPart.Document.Append(body);

                body.Append(XMLUtilities.NewParagraph(ReportTitle, JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));


                Table table = XMLUtilities.NewTable(SourceTable.Columns.Count, TableLayoutValues.Autofit);

                AddHeaderRow(table);

                AddRows(table);

                //XMLUtilities.InterpretTags(table);

                body.Append(table);

            }

            try
            {
                doc = appWord.Documents.Open(filePath);

                ReportFormatting formatting = new ReportFormatting();

                formatting.FormatTags(appWord, doc, false);
                doc.Save();

                appWord.Visible = true;
            }
            catch (Exception)
            {
                appWord.Quit();
            }

            GC.Collect();

        }

        private void AddHeaderRow(Table table)
        {
            List<string> cols = new List<string>();
            foreach (DataColumn c in SourceTable.Columns)
            {
                cols.Add(c.ColumnName);
            }
            TableRow header = XMLUtilities.CreateHeaderRow(cols.ToArray());

            IEnumerable<TableCell> cells = header.Elements<TableCell>();

            foreach (TableCell c in cells)
            {
                RunProperties rPr = c.Descendants<RunProperties>().First();
                rPr.Append(new RunFonts() { Ascii = "Verdana" });
                rPr.Append(new FontSize() { Val = "20" });
            }


            table.Append(header);
        }

        private void AddRows(Table table)
        {
            int count = 1;
            foreach (DataRow r in SourceTable.Rows)
            {
                TableRow newRow = new TableRow();

                foreach (DataColumn c in SourceTable.Columns)
                {
                    string v = r[c].ToString();

                    TableCell cell = new TableCell();
                    cell.Append(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

                    cell.SetCellText(v);

                    foreach (RunProperties rPr in cell.Descendants<RunProperties>())
                        rPr.Append(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" });

                    newRow.Append(cell);
                }
                
                if (count % 2 == 0)
                {
                    var cells = newRow.Descendants<TableCell>();

                    foreach (TableCell c in cells)
                        c.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9" }));
                }

                table.Append(newRow);

                count++;
            }
        }
    }
}
