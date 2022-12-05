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

namespace ITCLib
{
    public class QuestionReport
    {
        public Survey SelectedSurvey;
        public List<SurveyQuestion> Questions;

        public bool IncludeQuestion;
        public bool IncludeComments;
        public bool IncludeTranslation;
        public bool IncludeFilters;
        public bool IncludeSurvey;
        public bool IncludeVarLabel;
        public bool IncludeContent;
        public bool IncludeTopic;
        public bool IncludeDomain;
        public bool IncludeProduct;

        private List<string> Headings;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLet.dotx";



        public void CreateReport()
        {
            string title;
            if (SelectedSurvey != null)
                title = "Export from " + SelectedSurvey.SurveyCode;
            else
                title = "Question Export";

            filePath += title + " - " + DateTime.Now.DateTimeForFile() + ".docx";

            Word.Application appWord;
            appWord = new Word.Application() {
                Visible = false
            };
            Word.Document doc = appWord.Documents.Add(templateFile);
            doc.SaveAs2(filePath);
            doc.Close();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
            {

                Body body = new Body();
                wordDoc.MainDocumentPart.Document.Append(body);

                body.Append(XMLUtilities.NewParagraph(title, JustificationValues.Center, "32", "Verdana"));
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
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + title + 
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
            int columns = 3;

            

            if (IncludeSurvey) columns++;
            if (IncludeQuestion) columns++;
            if (IncludeComments) columns++;
            if (IncludeFilters) columns++;
            if (IncludeTranslation) columns = columns + 2;
            if (IncludeVarLabel) columns++;
            if (IncludeContent) columns++;
            if (IncludeTopic) columns++;
            if (IncludeDomain) columns++;
            if (IncludeProduct) columns++;

            Table table = XMLUtilities.NewTable(columns, TableLayoutValues.Autofit);


            Headings = new List<string>();
            Headings.Add("refVarName");
            Headings.Add("Qnum");
            Headings.Add("VarName");
            if (IncludeSurvey) Headings.Add("Survey");
            if (IncludeQuestion) Headings.Add("Question");
            if (IncludeTranslation)
            {
                Headings.Add("Translation");
                Headings.Add("Lang");
            }
            if (IncludeComments) Headings.Add("Comments");
            if (IncludeFilters) Headings.Add("Filters");
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

        private void AddQuestions(Table table)
        {
            int count = 1;
            foreach (SurveyQuestion q in Questions)
            {
                TableRow questionRow = new TableRow();

                questionRow.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new Text(q.VarName.RefVarName)))));

                questionRow.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new Text(q.Qnum)))));

                questionRow.Append(new TableCell(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                new Run(new Text(q.VarName.VarName)))));

                if (IncludeSurvey)
                {                    
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.SurveyCode)))));
                }

                if (IncludeQuestion)
                {
                    TableCell c = new TableCell();
                    c.Append(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false })));

                    c.SetCellText(q.GetQuestionText());

                    questionRow.Append(c);
                }

                if (IncludeTranslation)
                {
                    string translationText = "";
                    string langs = "";
                    foreach(Translation t in q.Translations)
                    {
                        translationText += t.TranslationText + "\r\n";
                        langs += t.Language + "\r\n";
                    }

                    

                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(translationText)))));

                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(langs)))));
                }

                if (IncludeComments)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.GetComments())))));
                }

                if (IncludeFilters)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.Filters)))));
                }

                if (IncludeVarLabel)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.VarName.VarLabel)))));
                }

                if (IncludeContent)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.VarName.Content.LabelText)))));
                }

                if (IncludeTopic)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.VarName.Topic.LabelText)))));
                }

                if (IncludeDomain)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.VarName.Domain.LabelText)))));
                }

                if (IncludeProduct)
                {
                    questionRow.Append(new TableCell(new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false }),
                    new Run(new Text(q.VarName.Product.LabelText)))));
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