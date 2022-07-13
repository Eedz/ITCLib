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

namespace ITCLib
{
    // TODO interpret tags
    public class PraccingReport
    {
        public Survey SelectedSurvey;
        public List<PraccingIssue> Issues;
        public Dictionary<string, string> VarQnums;
        public Dictionary<string, string> PrevNames;
        public Dictionary<string, string> imageIds;

        public bool IncludeToPracc;
        public bool AddBlankLine;
        public bool IncludePraccInstructions;
        public bool IncludeQnums;
        public bool IncludePrevNames;
        public List<string> Recipients;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\Praccing\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\Access\Reports\Templates\SMGLandLet.dotx";

        public PraccingReport()
        {
            imageIds = new Dictionary<string, string>();
        }

        public void CreateReport()
        {
            filePath += SelectedSurvey.SurveyCode + " Praccing Issues Report - " + DateTime.Now.ToString("G").Replace(":", ",") + ".docx";

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

                AddImageParts(wordDoc);

                string title = SelectedSurvey.SurveyCode + " Praccing Issues Report";
                if (Recipients.Count > 0)
                    title += " (for " + string.Join(", ", Recipients) + ")";

                body.Append(XMLUtilities.NewParagraph(title, JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph(Issues.Count() + " Issue(s) Total.", JustificationValues.Center, "32", "Verdana"));

                Table table = XMLUtilities.NewTable(7);

                XMLUtilities.SetColumnWidths(table, new double[] { 0.75, 0.92, 4.3, 1.22, 0.85, 0.85, 1.2 });

                AddHeaderRow(table);

                AddIssues(table);

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
                        s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + SelectedSurvey.SurveyCode + " Praccing Issues Report" +
                            "\t\t" + "Generated on " + DateTime.Today.ToString("d"));

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
        /// For each image to be included in the report, add an image part to the document. Record the relationship ID in the image dictionary.
        /// </summary>
        /// <param name="wordDoc"></param>
        private void AddImageParts(WordprocessingDocument wordDoc)
        {
            var images = Issues.Where(x => x.Images.Count > 0);

            foreach (PraccingIssue i in images)
            {
                foreach (PraccingImage p in i.Images)
                {
                    var imagePart = wordDoc.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                    string id = wordDoc.MainDocumentPart.GetIdOfPart(imagePart);
                    if (!imageIds.ContainsKey(p.Path)) imageIds.Add(p.Path, id);
                    using (var stream = new FileStream(p.Path, FileMode.Open))
                    {
                        imagePart.FeedData(stream);
                    }
                }
            }

            var responses = Issues.Where(x => x.Responses.Count > 0).ToList();

            foreach(PraccingIssue i in responses)
            { 
                var responseimages = i.Responses.Where(x => x.Images.Count > 0);

                foreach (PraccingResponse r in responseimages)
                {
                    foreach (PraccingImage p in r.Images)
                    {
                        var imagePart = wordDoc.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                        string id = wordDoc.MainDocumentPart.GetIdOfPart(imagePart);
                        imageIds.Add(p.Path, id);
                        using (var stream = new FileStream(p.Path, FileMode.Open))
                        {
                            imagePart.FeedData(stream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add the report header row to the table.
        /// </summary>
        /// <param name="table"></param>
        private void AddHeaderRow(Table table)
        {
            TableRow header = XMLUtilities.CreateHeaderRow(new string[] { "#", "Var Name", "Description/Response", "Date", "From", "To", "Category" });

            // now add the instructions
            IEnumerable<TableCell> cells = header.Elements<TableCell>();

            foreach (TableCell c in cells)
            {
                RunProperties rPr = c.Descendants<RunProperties>().First();
                rPr.Append(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" });
                rPr.Append(new FontSize() { Val = "20" });
            }

            if (IncludePraccInstructions)
            {
                AddInstructions(cells.ElementAt(0), "Leave blank");
                AddInstructions(cells.ElementAt(1), "Omit country code");
                AddInstructions(cells.ElementAt(3), "dd-mmm-yyyy");
                AddInstructions(cells.ElementAt(3), "e.g. 21-Apr-2021");
                AddInstructions(cells.ElementAt(4), "First name & initial, or firm");
                AddInstructions(cells.ElementAt(5), "First name & initial, or firm");
                AddInstructions(cells.ElementAt(6), "Leave blank");
            }

            table.Append(header);
        }

        /// <summary>
        /// Add and format the instructions that will appear under each heading.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="instructionText"></param>
        private void AddInstructions(TableCell cell, string instructionText)
        {
            Paragraph instructions = new Paragraph();

            ParagraphProperties pPr = new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            instructions.Append(pPr);
            instructions.Append(new Run(new Text(instructionText)));
            RunProperties rPr = new RunProperties();
            rPr.Append(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" });
            rPr.Append(new FontSize() { Val = "14" });
            instructions.Descendants<Run>().First().PrependChild(rPr);

            cell.Append(instructions);
        }

        /// <summary>
        /// Add the issues to the table, 1 row per main issue, 1 row per response, with grey fill on alternating main issues.
        /// </summary>
        /// <param name="table"></param>
        private void AddIssues(Table table)
        {
            int count = 1;

            var mainPart = table.Parent;

            foreach (PraccingIssue pi in Issues)
            {
                TableRow issueRow = AddIssueRow(pi);

                if (count % 2 == 0)
                    XMLUtilities.FillRow(issueRow, "D9D9D9");

                table.Append(issueRow);

                int responseNum = 2;
                foreach(PraccingResponse pr in pi.Responses)
                {
                    if (!IncludeToPracc && pr.ResponseTo.Name.Equals("pracc"))
                        continue;

                    TableRow responseRow = AddResponseRow(pr, pi.IssueNo + "-" + responseNum);

                    if (count % 2 == 0)
                        XMLUtilities.FillRow(responseRow, "D9D9D9");

                    table.Append(responseRow);
                    responseNum++;
                }

                if (AddBlankLine)
                {
                    TableRow blankRow = AddResponseRow(new PraccingResponse(){ ResponseDate = null }, pi.IssueNo + "-" + responseNum);

                    if (count % 2 == 0)
                        XMLUtilities.FillRow(blankRow, "D9D9D9");

                    table.Append(blankRow);
                }

                count++;
            }
        }

        /// <summary>
        /// Add a response as a row.
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="responseNumber"></param>
        /// <returns></returns>
        private TableRow AddResponseRow (PraccingResponse pr, string responseNumber)
        {

            TableRow row = new TableRow();

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(responseNumber)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

           

            string response = pr.Response;
            response = XMLUtilities.FormatPlainText(response);

            TableCell c = new TableCell();
            c.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

            c.SetCellText(response);

            foreach(RunProperties rPr in c.Descendants<RunProperties>())
                rPr.Append(new RunFonts() { Ascii = "Verdana" }, new FontSize() { Val = "20" });

            foreach (PraccingImage img in pr.Images)
            {

                

                if (imageIds.TryGetValue(img.Path, out string relID))
                    c.Append(new Paragraph(new Run(XMLUtilities.AddImage(relID, 914400 * 3, 792000 * 3))));
            }

            row.Append(c);


            string date = "";
            if (pr.ResponseDate != null)
                date = pr.ResponseDate.Value.ToString("d");

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(date)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pr.ResponseFrom.Name)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pr.ResponseTo.Name)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text()))));

            return row;
        }

        /// <summary>
        /// Add a main issue as a row.
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private TableRow AddIssueRow(PraccingIssue pi)
        {
            TableRow row = new TableRow();

            row.Append(new TableCell(new Paragraph(
                 new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts(){ Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.IssueNo + "-1")))));

            string varnames = pi.VarNames;

            if (IncludeQnums || IncludePrevNames)
            {
                string[] varnamesList = pi.VarNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in varnamesList)
                {
                    if (IncludePrevNames && PrevNames.TryGetValue(s, out string prev))
                        varnames = varnames.Replace(s, s + " (Prev. " + prev + ")");

                    if (IncludeQnums && VarQnums.TryGetValue(s, out string qnum))
                        varnames = varnames.Replace(s, s + " ("+ qnum + ")");
                }
                        
            }

            row.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(varnames)))));

            string description = pi.Description;            
            description = XMLUtilities.FormatPlainText(description);

            TableCell c = new TableCell();
            c.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

            c.SetCellText(description);

            foreach (RunProperties rPr in c.Descendants<RunProperties>())
                rPr.Append(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" });

            foreach (PraccingImage img in pi.Images)
            {
                if (imageIds.TryGetValue(img.Path, out string relID))
                    c.Append(new Paragraph(new Run(XMLUtilities.AddImage(relID, 914400 * 3, 792000*3))));
            }

            

            row.Append(c);

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.IssueDate.ToString("d"))))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.IssueFrom.Name)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.IssueTo.Name)))));

            row.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new RunProperties(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" }, new FontSize() { Val = "20" }), new Text(pi.Category.Category)))));

            return row;
        }

        

        
    }
}
