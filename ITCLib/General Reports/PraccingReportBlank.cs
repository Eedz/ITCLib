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
    public class PraccingReportBlank
    {
        
       
        public Survey SelectedSurvey;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\Praccing\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\Templates\SMGLandLet.dotx";

        public void CreateReport()
        {
            filePath += "Praccing Issues Form - " + DateTime.Now.ToString("g").Replace(":", ",") + ".docx";
            
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

                body.Append(XMLUtilities.NewParagraph("Praccing Issues Form", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));

                Table table = XMLUtilities.NewTable(7);

                XMLUtilities.SetColumnWidths(table, new double[] { 0.75, 0.92, 4.3, 1.22, 0.85, 0.85, 1.2 });


                TableRow header = XMLUtilities.CreateHeaderRow(new string[] { "#", "Var Name", "Description/Response", "Date", "From", "To", "Category" });

                // now add the instructions
                IEnumerable<TableCell> cells = header.Elements<TableCell>();

                foreach (TableCell c in cells)
                {
                    RunProperties rPr = c.Descendants<RunProperties>().First();
                    rPr.Append(new RunFonts() { Ascii = "Verdana" });
                    rPr.Append(new FontSize() { Val = "20" });
                }

                AddInstructions(cells.ElementAt(0), "Leave blank");
                AddInstructions(cells.ElementAt(1), "Omit country code");
                AddInstructions(cells.ElementAt(3), "dd-mmm-yyyy");
                AddInstructions(cells.ElementAt(3), "e.g. 21-Apr-2021");
                AddInstructions(cells.ElementAt(4), "First name & initial, or firm");
                AddInstructions(cells.ElementAt(5), "First name & initial, or firm");
                AddInstructions(cells.ElementAt(6), "Leave blank");


                table.Append(header);

                AddBlankRows(body, table);

            }

            try
            {
                doc = appWord.Documents.Open(filePath);

                // footer text                  
                foreach (Word.Section s in doc.Sections)
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + "Praccing Issues Form" +
                        "\t\t" + "Generated on " + DateTime.Today.ToString("d"));

                appWord.Visible = true;
            }
            catch (Exception)
            {
                appWord.Quit();
            }
        }

        private void AddInstructions(TableCell cell, string instructionText)
        {
            Paragraph instructions = new Paragraph();

            ParagraphProperties pPr = new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            instructions.Append(pPr);
            instructions.Append(new Run(new Text(instructionText)));
            RunProperties rPr = new RunProperties();
            rPr.Append(new RunFonts() { Ascii = "Verdana" });
            rPr.Append(new FontSize() { Val = "14" });
            instructions.Descendants<Run>().First().PrependChild(rPr);

            cell.Append(instructions);
        }

        private void AddBlankRows(Body body, Table table)
        {
            for (int i = 0; i < 10; i++)
            {
                TableRow row = new TableRow();
                for (int c = 0; c < 7; c++)
                {
                    ParagraphProperties pPr = new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
                    row.Append(new TableCell(new Paragraph(pPr, new Run(new Text()))));
                }
                table.Append(row);
            }

            foreach (TableRow row in table.Descendants<TableRow>())
            {
                var cell1 = row.Descendants<TableCell>().ElementAt(0);
                TableCellProperties tcPr = new TableCellProperties();
                tcPr.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9" });
                cell1.PrependChild(tcPr);

                var cell2 = row.Descendants<TableCell>().ElementAt(6);
                TableCellProperties tcPr2 = new TableCellProperties();
                tcPr2.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9" });
                cell2.PrependChild(tcPr2);

            }

            body.Append(table);
        }

        

       

    }
}
