using System;
using System.Collections.Generic;
using System.Linq;
using OpenXMLHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    public class DraftReport
    {
        public Survey SelectedSurvey;
        public SurveyDraft DraftInfo;   // if this is a single draft report, this will not be null
        public List<DraftQuestion> Questions;
        

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLeg.dotx";

        public DraftReport()
        {
            
        }

        public void CreateReport()
        {
            filePath += SelectedSurvey.SurveyCode + " Draft Report - " + DateTime.Now.DateTimeForFile() + ".docx";

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

                string title = "";

                if (DraftInfo == null)
                    title = SelectedSurvey.SurveyCode + " Draft Report";
                else
                    title = DraftInfo.DraftTitle;

                body.Append(XMLUtilities.NewParagraph(title, JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));

                Paragraph colorKey = XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana");

                Run additions = new Run();
                additions.Append(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "32" }, new Color() { Val = "0000FF" }));
                additions.Append(new Text("Additions        ") { Space = SpaceProcessingModeValues.Preserve });

                Run deletions = new Run();
                deletions.Append(new RunProperties(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "32" }, new Color() { Val = "FF0000" }));
                deletions.Append(new Text("Deletions"));

                colorKey.Append(additions);
                colorKey.Append(deletions);

                body.Append(colorKey);

                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));

                Table table;
                if (DraftInfo == null)
                {
                    table = XMLUtilities.NewTable(9);
                    XMLUtilities.SetColumnWidths(table, new double[] { 0.92, 0.75, 4, 4, 1, 1, 1, 1, 1 });
                }
                else
                {
                    int efCount = DraftInfo.ExtraFields.Count;
                    table = XMLUtilities.NewTable(4 + efCount);
                    double[] widths = new double[] { 0.92, 0.75, 3, 3 };
                    if (efCount>0)
                        foreach(var ef in DraftInfo.ExtraFields)
                            widths = widths.Append((double)(6 / efCount)).ToArray();
                    XMLUtilities.SetColumnWidths(table, widths);
                }

                

                AddHeaderRow(table);

                AddRows(table);

                if (DraftInfo==null) DeleteUnusedExtraFields(table);

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
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + SelectedSurvey.SurveyCode + " Draft Report" +
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

        
        /// <summary>
        /// Add the report header row to the table.
        /// </summary>
        /// <param name="table"></param>
        private void AddHeaderRow(Table table)
        {
            TableRow header;
            if (DraftInfo == null)
                header = XMLUtilities.CreateHeaderRow(new string[] { "VarName", "Qnum", "Question", "Comment", "Extra1", "Extra2", "Extra3", "Extra4", "Extra5" });
            else
            {
                string[] headers = new string[] { "VarName", "Qnum", "Question", "Comment" };
                foreach (SurveyDraftExtraField ef in DraftInfo.ExtraFields)
                    headers = headers.Append(ef.Label).ToArray();
                header = XMLUtilities.CreateHeaderRow(headers);
            }

            IEnumerable<TableCell> cells = header.Elements<TableCell>();

            foreach (TableCell c in cells)
            {
                RunProperties rPr = c.Descendants<RunProperties>().First();
                rPr.Append(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" });
                rPr.Append(new FontSize() { Val = "20" });
            }

            table.Append(header);
        }

        
        /// <summary>
        /// Add the questions as rows to the table.
        /// </summary>
        /// <param name="table"></param>
        private void AddRows(Table table)
        {
            int count = 1;

            foreach (DraftQuestion dq in Questions)
            {
                TableRow issueRow = AddRow(dq);

                if (count % 2 == 0)
                    XMLUtilities.FillRow(issueRow, "D9D9D9");

                table.Append(issueRow);

                count++;
            }
        }

        /// <summary>
        /// Add a row to the report.
        /// </summary>
        /// <param name="dq"></param>
        /// <returns></returns>
        private TableRow AddRow(DraftQuestion dq)
        {
            TableRow row = new TableRow();

            row.Append(AddCell(dq.VarName));

            row.Append(AddCell(dq.Qnum));

            row.Append(AddRTFCell(dq.QuestionText));

            row.Append(AddRTFCell(dq.Comments));

            if (DraftInfo == null)
            {
                for (int i = 0; i < 5;i++)
                {
                    row.Append(AddRTFCell(dq.GetExtraFieldData(i)));
                }
            }
            else
            {
                foreach (SurveyDraftExtraField ef in DraftInfo.ExtraFields)
                {                    
                    row.Append(AddRTFCell(dq.GetExtraFieldData(ef.FieldNumber)));
                }
            }
            return row;
        }

        private void DeleteUnusedExtraFields(Table table)
        {
            bool keep1 = false, keep2 = false, keep3 = false, keep4 = false, keep5 = false;

            if (Questions.Any(x => !string.IsNullOrWhiteSpace(x.Extra1)))
                keep1 = true;
            if (Questions.Any(x => !string.IsNullOrWhiteSpace(x.Extra2)))
                keep2 = true;
            if (Questions.Any(x => !string.IsNullOrWhiteSpace(x.Extra3)))
                keep3 = true;
            if (Questions.Any(x => !string.IsNullOrWhiteSpace(x.Extra4)))
                keep4 = true;
            if (Questions.Any(x => !string.IsNullOrWhiteSpace(x.Extra5)))
                keep5 = true;

            if (!keep1) RemoveExtraField(table, "Extra1");
            if (!keep2) RemoveExtraField(table, "Extra2");
            if (!keep3) RemoveExtraField(table, "Extra3");
            if (!keep4) RemoveExtraField(table, "Extra4");
            if (!keep5) RemoveExtraField(table, "Extra5");
        }

        private void RemoveExtraField (Table table, string extrafieldName)
        {
            var rows = table.Elements<TableRow>();
            int index=-1;
            for (int i = 0; i < rows.ElementAt(0).Elements<TableCell>().Count(); i++)
            {
                if (rows.ElementAt(0).Elements<TableCell>().ElementAt(i).GetCellText().Equals(extrafieldName))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return;

            foreach (var row in rows)
            {
               row.RemoveChild(row.Elements<TableCell>().ElementAt(index));
            }
        }

        private TableCell AddRTFCell(string text)
        {
            
            text = XMLUtilities.FormatPlainText(text);

            TableCell c = new TableCell();
            c.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

            c.SetCellText(text);

            foreach (RunProperties rPr in c.Descendants<RunProperties>())
                rPr.Append(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" });

            return c;
        }

        private TableCell AddCell(string text)
        {

            TableCell c = new TableCell(new Paragraph(
                 new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(text))));

            return c;
        }



    }
}
