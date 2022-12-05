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
    public enum ReportMatching { None, RefVarName }
    public class HeadingReport
    {

        public List<Survey> SelectedSurveys;
        public List<List<Heading>> HeadingLists;

        public ReportMatching MatchStyle;

        public bool IncludeQnum;
        public bool IncludeFirstVarName;
        public bool IncludeLastVarName;

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\External\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLeg.dotx";

        public HeadingReport(List<List<Heading>> lists)
        {
            HeadingLists = lists;
            MatchStyle = ReportMatching.None;
        }

        public void CreateReport()
        {
            string surveyList = string.Join(", ", SelectedSurveys.Select(x => x.SurveyCode));
            filePath += surveyList + " Heading Report - " + DateTime.Now.DateTimeForFile() + ".docx";

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

                
                title = surveyList + " Heading Report";
                

                body.Append(XMLUtilities.NewParagraph(title, JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));

                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));

                Table table;


                int headingCount = 0;// 1 + SelectedSurveys.Count;
                foreach (Survey s in SelectedSurveys)
                {
                    headingCount++; // varname
                    headingCount++; // heading text
                    if (IncludeQnum) headingCount++;
                    if (IncludeFirstVarName) headingCount++;
                    if (IncludeLastVarName) headingCount++;
                }

                table = XMLUtilities.NewTable(headingCount, TableLayoutValues.Autofit);

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
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + surveyList + " Heading Report" +
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

            string[] headers = new string[0];
            
            foreach (Survey survey in SelectedSurveys)
            {
                headers = headers.Append(survey.SurveyCode + " VarName").ToArray();
                headers = headers.Append(survey.SurveyCode).ToArray();
                if (IncludeQnum) headers = headers.Append(survey.SurveyCode + " Qnum").ToArray();
                if (IncludeFirstVarName) headers = headers.Append(survey.SurveyCode + " First VarName").ToArray();
                if (IncludeLastVarName) headers = headers.Append(survey.SurveyCode + " Last VarName").ToArray();
            }
            header = XMLUtilities.CreateHeaderRow(headers);
            

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
            if (MatchStyle == ReportMatching.None)
            {
                // loop from 0 to longest list count
                // add items 
                int longest = HeadingLists[0].Count;
                foreach (List<Heading> list in HeadingLists)
                    if (list.Count > longest)
                        longest = list.Count;

                
                CreateRows(table, longest);
                
                foreach (List<Heading> list in HeadingLists)
                {
                    for (int i = 1; i <= longest; i++)
                    {
                        TableRow currentRow = table.Elements<TableRow>().ElementAt(i);
                        if (i > list.Count)
                        {
                            currentRow.Append(AddCell(string.Empty));
                            if (IncludeQnum) currentRow.Append(AddCell(string.Empty));
                            currentRow.Append(AddCell(string.Empty));
                            if (IncludeFirstVarName) currentRow.Append(AddCell(string.Empty));
                            if (IncludeLastVarName) currentRow.Append(AddCell(string.Empty));
                        }
                        else
                        {
                            currentRow.Append(AddCell(list[i - 1].VarName.VarName));
                            if (IncludeQnum) currentRow.Append(AddCell(list[i - 1].Qnum));
                            currentRow.Append(AddCell(list[i - 1].PreP));
                            if (IncludeFirstVarName) currentRow.Append(AddCell(list[i - 1].FirstVarName));
                            if (IncludeLastVarName) currentRow.Append(AddCell(list[i - 1].LastVarName));
                        }
                    }                    
                }
            }
            else if (MatchStyle == ReportMatching.RefVarName)
            {
                // add item from first list
                // for each additional list, search for the added item, if found add it to other columns, remove it from other list
            }
        }

        private void CreateRows(Table table, int number)
        {
            for (int i = 0; i < number; i++)
            {
                table.Append(new TableRow());
            }
        }

        /// <summary>
        /// Add a row to the report.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        private TableRow AddRow(Heading sq)
        {
            TableRow row = new TableRow();

            row.Append(AddCell(sq.VarName.VarName));

            row.Append(AddRTFCell(sq.PreP));

            if (IncludeQnum) row.Append(AddCell(sq.Qnum));
            if (IncludeFirstVarName) row.Append(AddCell(sq.FirstVarName));
            if (IncludeLastVarName) row.Append(AddCell(sq.LastVarName));


            
            
            return row;
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
