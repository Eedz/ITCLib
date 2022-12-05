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
using System.IO;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System.Web;

namespace ITCLib
{
    

    public class SurveyProcessingReport
    {
        public Survey SelectedSurvey;
        public List<SurveyProcessingRecord> Records;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLet.dotx";

        public SurveyProcessingReport()
        {
            SelectedSurvey = new Survey();
            Records = new List<SurveyProcessingRecord>();
        }

        public SurveyProcessingReport(Survey survey, List<SurveyProcessingRecord> records)
        {
            SelectedSurvey = survey;
            Records = records;
        }

        public void CreateReport()
        {
            filePath += "Survey Processing Report - " + SelectedSurvey.SurveyCode + " - " + DateTime.Now.DateTimeForFile() + ".docx";

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

               

                body.Append(XMLUtilities.NewParagraph("Survey Processing Report - " + SelectedSurvey.SurveyCode, JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
             
              
                Table table = XMLUtilities.NewTable(7);

                XMLUtilities.SetColumnWidths(table, new double[] { 1.5, 0.75, 0.75, 1.5, 1.5, 1.5, 2.8 });

                AddHeaderRow(table);

                AddRecords(table);

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
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\tSurvey Processing Report - " + SelectedSurvey.SurveyCode +
                        "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());

                doc.Save();

                appWord.Visible = true;
            }
            catch (Exception)
            {
                appWord.Quit();
            }
        }

        /// <summary>
        /// Add the report header row to the table.
        /// </summary>
        /// <param name="table"></param>
        private void AddHeaderRow(Table table)
        {
            TableRow header = XMLUtilities.CreateHeaderRow(new string[] { "Stage", "N/A", "Done?", "Date", "Entered By", "Contact", "Comments" });

            // now add the instructions
            IEnumerable<TableCell> cells = header.Elements<TableCell>();

            foreach (TableCell c in cells)
            {
                RunProperties rPr = c.Descendants<RunProperties>().First();
                rPr.Append(new RunFonts() { Ascii = "Verdana" });
                rPr.Append(new FontSize() { Val = "20" });
            }

            table.Append(header);
        }

        /// <summary>
        /// Add the issues to the table, 1 row per main issue, 1 row per response, with grey fill on alternating main issues.
        /// </summary>
        /// <param name="table"></param>
        private void AddRecords(Table table)
        {

            foreach (SurveyProcessingRecord pi in Records)
            {
                if (pi.StageDates.Count == 0)
                {
                    TableRow row = new TableRow();
                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.Stage.StageName)))));

                    string na = "";

                    if (pi.NotApplicable)
                        na = "N/A";

                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(na)))));

                    string done = "No";

                    if (pi.Done)
                        done = "Yes";

                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(done)))));

                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

                    row.Append(new TableCell(new Paragraph(
                       new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                       new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

                    row.Append(new TableCell(new Paragraph(
                       new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                       new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

                    row.Append(new TableCell(new Paragraph(
                       new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                       new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

                    table.Append(row);
                    continue;
                }

                foreach (SurveyProcessingDate spd in pi.StageDates)
                {

                    TableRow row = new TableRow();


                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.Stage.StageName)))));

                    string na = "";

                    if (pi.NotApplicable)
                        na = "N/A";

                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(na)))));

                    string done = "No";

                    if (pi.Done)
                        done = "Yes";

                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(done)))));

                    string date = "";
                    if (spd.StageDate != null)
                        date = spd.StageDate.Value.ToString("d");
                    
                    row.Append(new TableCell(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                        new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(date)))));

                    row.Append(new TableCell(new Paragraph(
                       new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                       new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(spd.EnteredBy.Name)))));

                    row.Append(new TableCell(new Paragraph(
                       new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                       new Run(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" }), new Text(spd.Contact.Name)))));

                    StringBuilder sb = new StringBuilder();
                    foreach(SurveyProcessingNote spn in spd.Notes)
                    {
                        if (spn.NoteDate == null)
                            sb.AppendLine(spn.Author.Name);
                        else 
                            sb.AppendLine(spn.NoteDate.Value.ToString("d") + ": " + spn.Author.Name);

                        sb.AppendLine(spn.Note);
                
                    }

                    TableCell c = new TableCell();
                    c.Append(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

                    c.SetCellText(sb.ToString());

                    foreach (RunProperties rPr in c.Descendants<RunProperties>())
                        rPr.Append(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" });

                    row.Append(c);

                    table.Append(row);
                }

            }
        }
    }
}
