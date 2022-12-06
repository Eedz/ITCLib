using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Word = Microsoft.Office.Interop.Word;
using System.ComponentModel;


using OpenXMLHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;



namespace ITCLib
{
    public class HarmonyReport : VarNameBasedReport
    {
        public List<Survey> surveyFilter;
        public List<string> matchFields; // wording fields used to group questions together

        

        /// <summary>
        /// True if each label should appear as a separate column in the report
        /// </summary>
        public bool SeparateLabels { get; set; }
        /// <summary>
        /// True if a column should be included displaying the last wave that used this wording
        /// </summary>
        public bool LastWaveOnly { get; set; }
        /// <summary>
        /// True if labels are present
        /// </summary>
        public bool HasLabels { get; set; }
        /// <summary>
        /// True if translations are present
        /// </summary>
        public bool HasLang { get; set; }

        private string lang;
        /// <summary>
        /// The language to use for this report. Setting this to a non-empty string also sets the HasLang property to true;
        /// </summary>
        /// 
        public string Lang {
            get
            {
                return lang;
            }
            set
            { 
                if (!string.IsNullOrEmpty(value))
                {
                    lang = value;
                    HasLang = true;
                }
                else
                {
                    lang = value;
                    HasLang = false;
                }
            }
        }

        public string CustomFileName { get; set; }

        string filePath = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Harmony\";
        string templateFile = @"\\psychfile\psych$\psych-lab-gfong\SMG\SDI\Reports\Templates\SMGLandLet.dotx";



        /// <summary>
        /// True if project (country plus wave) should be shown instead of survey
        /// </summary>
        public bool ShowProjects { get; set; } // display project (country wave) rather than survey

        public bool ShowGroupOn { get; set; }
        public bool ShowAllSurveys { get; set; }

        // TODO last wave only

        // TODO color differences
        public HarmonyReport()
        {
            VarNames = new BindingList<VariableName>();
            matchFields = new List<string>();
            LayoutOptions = new ReportLayout();

            HasLang = false;
            HasLabels = false;
            ShowGroupOn = false;
            LastWaveOnly = false;
            SeparateLabels = false;

            OpenFinalReport = true;
        }

        

        /// <summary>
        /// Generates a report using a list of refVarNames. Each unique (based on MatchFields) version of a refVarName will appear once in the report 
        /// with a list of surveys that use that version. Wordings, labels, and translations can be used to define what is unique.
        /// </summary>
        public void CreateHarmonyReport(DataTable table)
        {
            CreateHarmonyTable();

            // now for each row in table, add the refVarName, surveys and group on info, for any wording fields, contatencate them into the question field

            foreach (DataRow row in table.Rows)
            {
                DataRow newrow = ReportTable.NewRow();

                newrow["refVarName"] = row["refVarName"];
                newrow["Surveys"] = row["Surveys"];
                if (ShowGroupOn)
                    newrow["Group By Fields"] = row["GroupBy"];

                SurveyQuestion q = new SurveyQuestion();
                foreach (DataColumn c in table.Columns)
                {
                    switch (c.ColumnName)
                    {
                        case "PreP":
                            q.PreP = (string)row["PreP"];
                            break;
                        case "PreI":
                            q.PreI = (string)row["PreI"];
                            break;
                        case "PreA":
                            q.PreA = (string)row["PreA"];
                            break;
                        case "LitQ":
                            q.LitQ = (string)row["LitQ"];
                            break;
                        case "PstI":
                            q.PstI = (string)row["PstI"];
                            break;
                        case "PstP":
                            q.PstP = (string)row["PstP"];
                            break;
                        case "RespOptions":
                            q.RespOptions = (string)row["RespOptions"];
                            break;
                        case "NRCodes":
                            q.NRCodes = (string)row["NRCodes"];
                            break;

                    }
                }
                newrow["Question"] = q.GetQuestionText(true);

                if (HasLabels)
                {
                    if (SeparateLabels)
                    {
                        foreach (DataColumn c in table.Columns)
                        {
                            switch (c.ColumnName)
                            {
                                case "VarLabel":
                                    newrow["VarLabel"] = row["VarLabel"];
                                    break;
                                case "Content":
                                    newrow["Content"] = row["Content"];
                                    break;
                                case "Topic":
                                    newrow["Topic"] = row["Topic"];
                                    break;
                                case "Domain":
                                    newrow["Domain"] = row["Domain"];
                                    break;
                                case "Product":
                                    newrow["Product"] = row["Product"];
                                    break;
                            }
                        }
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataColumn c in table.Columns)
                        {
                            switch (c.ColumnName)
                            {
                                case "VarLabel":
                                    sb.AppendLine((string)row["VarLabel"]);
                                    break;
                                case "Content":
                                    sb.AppendLine((string)row["Content"]);
                                    break;
                                case "Topic":
                                    sb.AppendLine((string)row["Topic"]);
                                    break;
                                case "Domain":
                                    sb.AppendLine((string)row["Domain"]);
                                    break;
                                case "Product":
                                    sb.AppendLine((string)row["Product"]);
                                    break;
                            }
                        }
                        newrow["Labels"] = sb.ToString();
                    }
                }

                if (HasLang)
                {
                    newrow["Translation"] = row["TranslationWithRouting"];
                }

                ReportTable.Rows.Add(newrow);
            }

            if (ShowProjects)
                ReportTable.Columns["Surveys"].ColumnName = "Projects";

            if (ShowGroupOn)
            {
                ReportTable.Columns["Group By Fields"].ColumnName = string.Join(", ", matchFields);
            }

            CreateReport();
        }

        /// <summary>
        /// Generates a report using a list of refVarNames. Each unique (based on MatchFields) version of a refVarName will appear once in the report 
        /// with a list of surveys that use that version. Wordings, labels, and translations can be used to define what is unique.
        /// </summary>
        public void CreateHarmonyReportData(DataTable table)
        {
            CreateHarmonyTable();

            // now for each row in table, add the refVarName, surveys and group on info, for any wording fields, contatencate them into the question field

            foreach (DataRow row in table.Rows)
            {
                DataRow newrow = ReportTable.NewRow();

                newrow["refVarName"] = row["refVarName"];
                newrow["Surveys"] = row["Surveys"];
                if (ShowGroupOn)
                    newrow["Group By Fields"] = row["GroupBy"];

                newrow["Question"] = GetQuestion(table.Columns, row);

                if (HasLabels)
                {
                    FillLabels(table.Columns, newrow, row);
                }

                if (HasLang)
                {
                    newrow["Translation"] = row["Translation"];
                }

                ReportTable.Rows.Add(newrow);
            }

            if (ShowProjects)
                ReportTable.Columns["Surveys"].ColumnName = "Projects";

            if (ShowGroupOn)
            {
                ReportTable.Columns["Group By Fields"].ColumnName = string.Join(", ", matchFields);
            }
        }

        private string GetQuestion (DataColumnCollection columns, DataRow row)
        {
            SurveyQuestion q = new SurveyQuestion();
            foreach (DataColumn c in columns)
            {
                switch (c.ColumnName)
                {
                    case "PreP":
                        q.PreP = (string)row["PreP"];
                        break;
                    case "PreI":
                        q.PreI = (string)row["PreI"];
                        break;
                    case "PreA":
                        q.PreA = (string)row["PreA"];
                        break;
                    case "LitQ":
                        q.LitQ = (string)row["LitQ"];
                        break;
                    case "PstI":
                        q.PstI = (string)row["PstI"];
                        break;
                    case "PstP":
                        q.PstP = (string)row["PstP"];
                        break;
                    case "RespOptions":
                        q.RespOptions = (string)row["RespOptions"];
                        break;
                    case "NRCodes":
                        q.NRCodes = (string)row["NRCodes"];
                        break;

                }
            }

            return q.GetQuestionText();
        }

        private void FillLabels(DataColumnCollection columns, DataRow newrow, DataRow row)
        {
            if (SeparateLabels)
            {
                foreach (DataColumn c in columns)
                {
                    switch (c.ColumnName)
                    {
                        case "VarLabel":
                            newrow["VarLabel"] = row["VarLabel"];
                            break;
                        case "Content":
                            newrow["Content"] = row["Content"];
                            break;
                        case "Topic":
                            newrow["Topic"] = row["Topic"];
                            break;
                        case "Domain":
                            newrow["Domain"] = row["Domain"];
                            break;
                        case "Product":
                            newrow["Product"] = row["Product"];
                            break;
                    }
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn c in columns)
                {
                    switch (c.ColumnName)
                    {
                        case "VarLabel":
                            sb.AppendLine((string)row["VarLabel"]);
                            break;
                        case "Content":
                            sb.AppendLine((string)row["Content"]);
                            break;
                        case "Topic":
                            sb.AppendLine((string)row["Topic"]);
                            break;
                        case "Domain":
                            sb.AppendLine((string)row["Domain"]);
                            break;
                        case "Product":
                            sb.AppendLine((string)row["Product"]);
                            break;
                    }
                }
                newrow["Labels"] = sb.ToString();
            }
        }

        private void CreateHarmonyTable()
        {
            // create report table
            List<string> columns = new List<string>();
            columns.Add("refVarName");
            columns.Add("Question");
            
            if (matchFields.Contains("VarLabel") || matchFields.Contains("Domain") || matchFields.Contains("Topic") || matchFields.Contains("Content") || matchFields.Contains("Product"))
            {
                HasLabels = true;
            }

            if (HasLabels)
            {
                if (SeparateLabels)
                {
                    foreach (string s in matchFields)
                    {
                        if (s.Equals("VarLabel") || s.Equals("Domain") || s.Equals("Topic") || s.Equals("Content") || s.Equals("Product"))
                        {
                            columns.Add(s);
                        }
                    }
                }
                else
                {
                    columns.Add("Labels");
                }
            }

            if (HasLang)
                columns.Add("Translation");

            columns.Add("Surveys");

            if (LastWaveOnly)
                columns.Add("RecentWaves");

            if (ShowGroupOn)
                columns.Add("Group By Fields"); 

            List<string> colTypes = new List<string>();

            // column types are all string, so add a string item for each column name
            foreach (string s in columns)
                colTypes.Add("string");

            ReportTable = Utilities.CreateDataTable("Harmony Report", columns.ToArray(), colTypes.ToArray());
        }

        public void CreateReport()
        {
            string path;

            if (string.IsNullOrEmpty(CustomFileName))
                path = filePath + "Harmony Report - " + DateTime.Now.DateTimeForFile() + ".docx";
            else
                path = filePath + CustomFileName + " - " + DateTime.Now.DateTimeForFile() + ".docx";

            Word.Application appWord;
            appWord = new Word.Application();
            
            appWord.Visible = false;
            Word.Document doc = appWord.Documents.Add(templateFile);
            doc.SaveAs2(path);
            doc.Close();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
            {

                Body body = new Body();
                wordDoc.MainDocumentPart.Document.Append(body);

                body.Append(XMLUtilities.NewParagraph("Harmony Report", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));
                body.Append(XMLUtilities.NewParagraph("", JustificationValues.Center, "32", "Verdana"));


                Table table = XMLUtilities.NewTable(ReportTable.Columns.Count, TableLayoutValues.Autofit);

                AddHeaderRow(table);

                AddQuestions(table);

                //XMLUtilities.InterpretTags(table);

                body.Append(table);

            }

            try
            {
                doc = appWord.Documents.Open(path);

                // footer text                  
                foreach (Word.Section s in doc.Sections)
                    s.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\tHarmony Report" +
                        "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());

                ReportFormatting formatting = new ReportFormatting();

                formatting.FormatTags(appWord, doc, false);
                doc.Save();

                if (OpenFinalReport) appWord.Visible = true;
                else appWord.Quit();
            }
            catch (Exception)
            {
                appWord.Quit();
            }
        }

        private Table CreateTable(Body body)
        {         
            Table table = XMLUtilities.NewTable(ReportTable.Columns.Count, TableLayoutValues.Autofit);

            return table;
        }

        private void AddHeaderRow(Table table)
        {
            List<string> cols = new List<string>();
            foreach (DataColumn c in ReportTable.Columns)
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

        private void AddQuestions(Table table)
        {
            int count = 1;
            string refV = string.Empty;
            bool color = false;
            foreach (DataRow r in ReportTable.Rows)
            {
                string currentV = r[0].ToString();
                TableRow newRow = new TableRow();

                foreach (DataColumn c in ReportTable.Columns)
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

                if (!refV.Equals(currentV))
                {
                    color = !color;
                    refV = currentV;
                }

                if (color)
                {
                    var cells = newRow.Descendants<TableCell>();

                    foreach (TableCell c in cells)
                        c.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "F1F6AC" }));
                }

                table.Append(newRow);
                
                count++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OutputHarmony()
        {
            Word.Application appWord;   // instance of MSWord
            Word.Document docReport;    // the report document
            Word.Table surveyTable;     // the table in the document containing the survey(s)

            int rowCount = ReportTable.Rows.Count;          // number of rows in the survey table
            int columnCount = ReportTable.Columns.Count;    // number of columns in the survey table

            // create the instance of Word
            appWord = new Word.Application();
            appWord.Visible = false;
            // disable spelling and grammar checks (useful for foreign languages)
            appWord.Options.CheckSpellingAsYouType = false;
            appWord.Options.CheckGrammarAsYouType = false;

            // create the document
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter:
                    docReport = appWord.Documents.Add(Properties.Resources.TemplateLetter); 
                    break;
                case PaperSizes.Legal:
                    docReport = appWord.Documents.Add(Properties.Resources.TemplateLegal); 
                    break;
                case PaperSizes.Eleven17:
                    docReport = appWord.Documents.Add(Properties.Resources.Template11x17);  
                    break;
                case PaperSizes.A4:
                    docReport = appWord.Documents.Add(Properties.Resources.TemplateA4); 
                    break;
                default:
                    docReport = appWord.Documents.Add(Properties.Resources.TemplateLetter);
                    break;
            }
            // add a table
            surveyTable = docReport.Tables.Add(docReport.Range(0, 0), rowCount + 1, columnCount);

            // fill header row
            for (int c = 1; c <= columnCount; c++)
            {
                surveyTable.Cell(1, c).Range.Text =ReportTable.Columns[c - 1].Caption;
            }

            // fill the rest of the rows
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < columnCount; c++)
                {
                    surveyTable.Cell(r + 2, c + 1).Range.Text = ReportTable.Rows[r][c].ToString();
                }
            }

            // table style
            surveyTable.Rows.AllowBreakAcrossPages = -1;
            surveyTable.Rows.Alignment = 0;
            surveyTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
            surveyTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            surveyTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            surveyTable.Borders.OutsideColor = Word.WdColor.wdColorGray25;
            surveyTable.Borders.InsideColor = Word.WdColor.wdColorGray25;
            surveyTable.Select();
            docReport.Application.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;

            //header row style
            surveyTable.Rows[1].Range.Bold = 1;
            surveyTable.Rows[1].Shading.ForegroundPatternColor = Word.WdColor.wdColorRose;
            surveyTable.Rows[1].Borders.OutsideColor = Word.WdColor.wdColorBlack;
            surveyTable.Rows[1].Borders.InsideColor = Word.WdColor.wdColorBlack;
            surveyTable.Rows[1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
            surveyTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            // repeat heading row
            if (RepeatedHeadings)
                surveyTable.Rows[1].HeadingFormat = -1;
            else
                surveyTable.Rows[1].HeadingFormat = 0;

            //header text
            docReport.Range(0, 0).Select();
            docReport.Application.Selection.Range.ParagraphFormat.SpaceAfter = 0;
            docReport.Application.Selection.SplitTable();
            docReport.Application.Selection.TypeParagraph();
            docReport.Application.Selection.Font.Bold = 0;
            docReport.Application.Selection.Font.Size = 12;
            docReport.Application.Selection.Font.Name = "Arial";
            docReport.Application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            docReport.Application.Selection.Text = "Harmony Report";

            docReport.Application.Selection.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // footer text
            docReport.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + "Harmony Report" +
                "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());

            //
            docReport.Paragraphs.SpaceAfter = 0;

            // format column names and widths
            FormatColumns(docReport);



            Formatting.FormatTags(appWord, docReport, false);

            FileName += "Harmony Report" + ", " + DateTime.Now.DateTimeForFile();
            FileName += ".doc";

            //save the file
            docReport.SaveAs2(FileName);

            if (LayoutOptions.FileFormat == FileFormats.PDF)
            {
                try
                {
                    docReport.ExportAsFixedFormat(FileName.Replace(".doc", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, true,
                        Word.WdExportOptimizeFor.wdExportOptimizeForPrint, Word.WdExportRange.wdExportAllDocument, 1, 1,
                        Word.WdExportItem.wdExportDocumentContent, true, true, Word.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false);
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    docReport.Close();
                    appWord.Quit();
                }
            }
            else
            {
                appWord.Visible = true;
            }


        }

        #region Object based Harmony Report (not used right now)
        /// <summary>
        /// Generates a report using a list of refVarNames. Each unique (based on MatchFields) version of a refVarName will appear once in the report 
        /// with a list of surveys that use that version. Wordings, labels, and translations can be used to define what is unique.
        /// </summary>
        private void CreateHarmonyReport()
        {
            CreateHarmonyTable();

            FillHarmonyTable();

            OutputHarmony();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FillHarmonyTable()
        {

            DataRow newrow;

            // reduce list of possible questions to the unqiue refVarNames and wordings
            List<SurveyQuestion> questionsCombined = UniqueQuestions();

            // add each question to the table
            foreach (SurveyQuestion sq in questionsCombined)
            {
                newrow = ReportTable.NewRow();

                newrow["refVarName"] = sq.VarName.RefVarName;

                newrow["Question"] = sq.GetQuestionText(matchFields, true);

                newrow["Surveys"] = GetSurveyList(sq);

                if (HasLabels)
                {
                    if (SeparateLabels)
                    {
                        foreach (string s in matchFields)
                        {

                            if (s.Equals("Domain"))
                                newrow["Domain"] = sq.VarName.Domain.LabelText;
                            else if (s.Equals("Topic"))
                                newrow["Topic"] = sq.VarName.Topic.LabelText;
                            else if (s.Equals("Content"))
                                newrow["Content"] = sq.VarName.Content.LabelText;
                            else if (s.Equals("Product"))
                                newrow["Product"] = sq.VarName.Product.LabelText;
                            else if (s.Equals("VarLabel"))
                                newrow["VarLabel"] = sq.VarName.VarLabel;
                        }
                    }
                    else
                    {
                        string labels = "";
                        foreach (string s in matchFields)
                        {

                            if (s.Equals("Domain"))
                                labels += sq.VarName.Domain.LabelText + "\r\n";
                            else if (s.Equals("Topic"))
                                labels += sq.VarName.Topic.LabelText + "\r\n";
                            else if (s.Equals("Content"))
                                labels += sq.VarName.Content.LabelText + "\r\n";
                            else if (s.Equals("Product"))
                                labels += sq.VarName.Product.LabelText + "\r\n";
                            else if (s.Equals("VarLabel"))
                                labels += sq.VarName.VarLabel;
                        }
                        newrow["Labels"] = Utilities.TrimString(labels, "\r\n");
                    }
                }

                if (HasLang)
                {
                    newrow["Translation"] = sq.GetTranslationText(Lang);
                }

                if (ShowGroupOn)
                    newrow["Group By Fields"] = GetGroupByFields(sq);

                ReportTable.Rows.Add(newrow);
            }


        }

        /// <summary>
        /// Returns a list of SurveyQuestion objects that represent the unique members of the original question list. Uniqueness depends on refVarName and the fields listed in the matching field list.
        /// </summary>
        /// <returns></returns>
        private List<SurveyQuestion> UniqueQuestions()
        {
            List<SurveyQuestion> questionsCombined = new List<SurveyQuestion>(); // this will be the list of questions that will appear in the report

            bool toAdd = true;
            foreach (SurveyQuestion sq in questions)
            {
                // if we are including translations, but this question has no translation, skip it
                if (sq.Translations == null && HasLang)
                    continue;



                // if there are no questions in the list, add this one right away
                if (questionsCombined.Count == 0)
                {
                    toAdd = true;
                }
                else
                {
                    for (int i = 0; i < questionsCombined.Count; i++)
                    {
                        if (sq.VarName.RefVarName == questionsCombined[i].VarName.RefVarName)
                        {
                            if (HarmonyMatch(sq, questionsCombined[i]))
                            {
                                toAdd = false;
                                break;

                            }
                        }
                    }
                }
                if (toAdd) questionsCombined.Add(sq);
                toAdd = true;
            }
            return questionsCombined;
        }


        /// <summary>
        /// Returns a string that contains all the Survey Codes that match the provided SurveyQuestion.
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        private string GetSurveyList(SurveyQuestion cq)
        {
            //string list = "";
            List<string> list = new List<string>();
            foreach (SurveyQuestion sq in questions)
            {
                if (HarmonyMatch(sq, cq))
                {
                    list.Add(sq.SurveyCode);
                }
            }


            list.Sort();

            return String.Join(", ", list.ToArray());
        }

        /// <summary>
        /// Return a string that contains the values of the match fields.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        private string GetGroupByFields(SurveyQuestion sq)
        {
            List<string> matchFieldValues = new List<string>();
            foreach (string s in matchFields)
            {
                if (s.Equals("PreP"))
                    matchFieldValues.Add(Convert.ToString(sq.PrePNum));

                if (s.Equals("PreI"))
                    matchFieldValues.Add(Convert.ToString(sq.PreINum));

                if (s.Equals("PreA"))
                    matchFieldValues.Add(Convert.ToString(sq.PreANum));

                if (s.Equals("LitQ"))
                    matchFieldValues.Add(Convert.ToString(sq.LitQNum));

                if (s.Equals("PstI"))
                    matchFieldValues.Add(Convert.ToString(sq.PstINum));

                if (s.Equals("PstP"))
                    matchFieldValues.Add(Convert.ToString(sq.PstPNum));

                if (s.Equals("RespOptions"))
                    matchFieldValues.Add(sq.RespName);

                if (s.Equals("NRCodes"))
                    matchFieldValues.Add(sq.NRName);

                if (s.Equals("Domain"))
                    matchFieldValues.Add(sq.VarName.Domain.LabelText);

                if (s.Equals("Topic"))
                    matchFieldValues.Add(sq.VarName.Topic.LabelText);

                if (s.Equals("Content"))
                    matchFieldValues.Add(sq.VarName.Content.LabelText);

                if (s.Equals("Product"))
                    matchFieldValues.Add(sq.VarName.Product.LabelText);

                if (s.Equals("VarLabel"))
                    matchFieldValues.Add(sq.VarName.VarLabel);

                if (s.Equals("Translation"))
                    matchFieldValues.Add(Lang);

            }

            return String.Join(", ", matchFieldValues);
        }

        /// <summary>
        /// Returns true if the provided SurveyQuestion objects are equal in terms of refVarName and the match fields.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns></returns>
        public bool HarmonyMatch(SurveyQuestion sq1, SurveyQuestion sq2)
        {
            bool prepMatch = false, preiMatch = false, preaMatch = false, litqMatch = false, pstiMatch = false, pstpMatch = false, roMatch = false, nrMatch = false;
            bool tranMatch = false;

            if (!matchFields.Contains("PreP")) prepMatch = true;
            if (!matchFields.Contains("PreI")) preiMatch = true;
            if (!matchFields.Contains("PreA")) preaMatch = true;
            if (!matchFields.Contains("LitQ")) litqMatch = true;
            if (!matchFields.Contains("PstI")) pstiMatch = true;
            if (!matchFields.Contains("PstP")) pstpMatch = true;
            if (!matchFields.Contains("RespOptions")) roMatch = true;
            if (!matchFields.Contains("NRCodes")) nrMatch = true;
            if (!matchFields.Contains("Translation")) tranMatch = true;

            foreach (string s in matchFields)
            {

                if (s.Equals("PreP"))
                    prepMatch = (sq1.PreP == sq2.PreP);

                if (s.Equals("PreI"))
                    preiMatch = (sq1.PreI == sq2.PreI);

                if (s.Equals("PreA"))
                    preaMatch = (sq1.PreA == sq2.PreA);

                if (s.Equals("LitQ"))
                    litqMatch = (sq1.LitQ == sq2.LitQ);

                if (s.Equals("PstI"))
                    pstiMatch = (sq1.PstI == sq2.PstI);

                if (s.Equals("PstP"))
                    pstpMatch = (sq1.PstP == sq2.PstP);

                if (s.Equals("RespOptions"))
                    roMatch = (sq1.RespOptions == sq2.RespOptions);

                if (s.Equals("NRCodes"))
                    nrMatch = (sq1.NRCodes == sq2.NRCodes);

                if (s.Equals("Translation"))
                    tranMatch = (sq1.GetTranslationText(Lang) == sq2.GetTranslationText(Lang));

            }

            return prepMatch && preiMatch && preaMatch && litqMatch && pstiMatch & pstpMatch && roMatch && nrMatch;
        }
        #endregion  
    }
}
