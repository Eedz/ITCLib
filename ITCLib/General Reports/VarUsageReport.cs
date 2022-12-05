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
    public class VarUsageReport : DataTableReport
    {
        
        public VarUsageReport(DataTable data, string title) : base(data, title)
        {
            
        }

        public override void CreateReport()
        {
            filePath += ReportTitle + " " + DateTime.Now.DateTimeForFile() + ".docx";

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

                foreach (Text t in body.Descendants<Text>())
                {
                    t.Text = XMLUtilities.ReplaceHexadecimalSymbols(t.Text);
                }

            }

            try
            {
                doc = appWord.Documents.Open(filePath);

                // footer text                  
                foreach (Word.Section s in doc.Sections)
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle +
                        "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());

                // interpret formatting tags
                ReportFormatting formatting = new ReportFormatting();
                formatting.FormatTags(appWord, doc, true);

                doc.Save();

                appWord.Visible = true;
            }
            catch (Exception)
            {
                appWord.Quit();
            }
        }

        protected override void AddRows(Table table)
        {
            
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

                
                var cells = newRow.Descendants<TableCell>();
                int count = 0;
                foreach (TableCell c in cells)
                {
                    if (!string.IsNullOrEmpty(c.GetCellText()))
                        count++;
                }
                
                if (count == 1 && !string.IsNullOrEmpty(cells.ElementAt(0).GetCellText()))
                {
                    foreach (TableCell c in cells)
                    {

                        c.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "CDE1FF" }));
                    }
                }
                else if (count == 1 && !string.IsNullOrEmpty(cells.ElementAt(1).GetCellText()))
                {
                    foreach (TableCell c in cells)
                    {

                        c.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "000000" }));
                        foreach (Run run in c.Descendants<Run>())
                            run.PrependChild(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "28" }, new Color() { Val = "FFFFFF" }));
                    }
                    
                }
                

                table.Append(newRow);

                
            }
        }
    

        



    }
}
