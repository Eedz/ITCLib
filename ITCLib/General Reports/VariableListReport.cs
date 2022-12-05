using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenXMLHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Word = Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Bibliography;

using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Shading = DocumentFormat.OpenXml.Wordprocessing.Shading;

namespace ITCLib
{
    public class VariableListReport : VarNameBasedReport
    {

        public bool IncludeVarLabel;
        public bool IncludeContent;
        public bool IncludeTopic;
        public bool IncludeDomain;
        public bool IncludeProduct;

        private List<string> Headings;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\External\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLet.dotx";


        public void CreateReport()
        {
            filePath += "Variable List Report - " + DateTime.Now.DateTimeForFile() + ".docx";

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

                body.Append(XMLUtilities.NewParagraph("Variable List Report", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));


                Table table = CreateTable(body);

                AddHeaderRow(table);

                AddQuestions(table);

                //XMLUtilities.InterpretTags(table);

                body.Append(table);

            }

            try
            {
                doc = appWord.Documents.Open(filePath);

                // footer text                  
                foreach (Word.Section s in doc.Sections)
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\tVariable List Report" +
                        "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());

                ReportFormatting formatting = new ReportFormatting();

                formatting.FormatTags(appWord, doc, false);
                doc.Save();

                appWord.Visible = true;
            }
            catch (Exception)
            {
                appWord.Quit();
            }
        }

        private Table CreateTable(Body body)
        {
            int columns = 2;

            if (IncludeVarLabel) columns++;
            if (IncludeContent) columns++;
            if (IncludeTopic) columns++;
            if (IncludeDomain) columns++;
            if (IncludeProduct) columns++;

            Table table = XMLUtilities.NewTable(columns, TableLayoutValues.Autofit);

            Headings = new List<string>();
            Headings.Add("refVarName");
            Headings.Add("VarName");
            if (IncludeVarLabel) Headings.Add("VarLabel");
            if (IncludeContent) Headings.Add("Content");
            if (IncludeTopic) Headings.Add("Topic");
            if (IncludeDomain) Headings.Add("Domain");
            if (IncludeProduct) Headings.Add("Product");

            return table;
        }

        private void AddHeaderRow(Table table)
        {
            TableRow header = XMLUtilities.CreateHeaderRow(Headings.ToArray<string>());

            IEnumerable<TableCell> cells = header.Elements<TableCell>();

            foreach (TableCell c in cells)
            {
                RunProperties rPr = c.Descendants<RunProperties>().First();
                rPr.Append(new RunFonts() { Ascii = "Verdana" });
                rPr.Append(new FontSize() { Val = "20" });
            }


            table.Append(header);
        }

        private void AddQuestions(Table table)
        {
            int count = 1;
            foreach (VariableName v in VarNames)
            {
                TableRow questionRow = new TableRow();

                questionRow.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new Text(v.VarName)))));

                questionRow.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new Text(v.RefVarName)))));

                if (IncludeVarLabel)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(v.VarLabel)))));
                }

                if (IncludeContent)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(v.Content.LabelText)))));
                }

                if (IncludeTopic)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(v.Topic.LabelText)))));
                }

                if (IncludeDomain)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(v.Domain.LabelText)))));
                }

                if (IncludeProduct)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(v.Product.LabelText)))));
                }


                if (count % 2 == 0)
                {
                    var cells = questionRow.Descendants<TableCell>();

                    foreach (TableCell c in cells)
                        c.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9" }));
                }




                table.Append(questionRow);





                count++;
            }
        }
    }
}
