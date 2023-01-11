using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using OpenXMLHelper;


namespace ITCLib
{
    /// <summary>
    /// This class represents a report outputting one or more surveys to Word.
    /// </summary>
    /// <remarks>If 2+ surveys, they are matched up by refVarName, with one survey's Qnums defining the order.</remarks>
    public class SurveyReport : SurveyBasedReport
    {
        #region Survey Report Properties

        public List<DataTable> FinalSurveyTables { get; set; }
        public DataTable qnumSurveyTable;

        // comparison class
        public Comparison SurveyCompare { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SurveyReport object.
        /// </summary>
        public SurveyReport() : base() {


            FinalSurveyTables = new List<DataTable>();

            // default settings

            ReportType = ReportTypes.Standard;


            // intialize the column order collection with the default columns
            ColumnOrder = new List<ReportColumn>
            {
                new ReportColumn("Qnum", 1),
                new ReportColumn("VarName", 2)
            };

            // comparison options
            SurveyCompare = new Comparison();

        }

        public SurveyReport(SurveyBasedReport sbr)
        {
            FinalSurveyTables = new List<DataTable>();

            // comparison options
            SurveyCompare = new Comparison();

            this.Surveys = sbr.Surveys;

            this.TranslatorInstructions = sbr.TranslatorInstructions;
            this.CompareWordings = sbr.CompareWordings;

            this.VarChangesCol = sbr.VarChangesCol;
            this.SurvNotes = sbr.SurvNotes;
            this.VarChangesApp = sbr.VarChangesApp;
            this.ExcludeTempChanges = sbr.ExcludeTempChanges;

            // formatting options
            this.SemiTel = sbr.SemiTel;
            this.SubsetTables = sbr.SubsetTables;
            this.SubsetTablesTranslation = sbr.SubsetTablesTranslation;
            this.ShowAllQnums = sbr.ShowAllQnums;
            this.ShowAllVarNames = sbr.ShowAllVarNames;
            this.ShowQuestion = sbr.ShowQuestion;
            this.ShowSectionBounds = sbr.ShowSectionBounds; // true if, for each heading question, we should include the first and last question in that section

            this.ReportTable = sbr.ReportTable; // the final report table, which will be output to Word

            this.ReportType = sbr.ReportType;
            this.Batch = sbr.Batch;

            this.FileName = sbr.FileName; // this value will initially contain the path up to the file name, which will be added in the Output step

            // formatting and layout options
            this.Formatting = sbr.Formatting;
            this.LayoutOptions = sbr.LayoutOptions;

            this.ColorSubs = sbr.ColorSubs;

            this.InlineRouting = sbr.InlineRouting;
            this.ShowLongLists = sbr.ShowLongLists;
            this.QNInsertion = sbr.QNInsertion;
            this.AQNInsertion = sbr.AQNInsertion;
            this.CCInsertion = sbr.CCInsertion;

            this.ColumnOrder = sbr.ColumnOrder;

            this.NrFormat = sbr.NrFormat;
            this.Numbering = sbr.Numbering;


            this.Details = sbr.Details;
            this.OpenFinalReport = sbr.OpenFinalReport;
            // other details        
            this.Web = sbr.Web;
        }

        #endregion

        /// <summary>
        /// Returns this object's helper objects back to their original state, so that a new report can be run.
        /// </summary>
        public void Reset()
        {
            FinalSurveyTables = new List<DataTable>();
            // comparison options
            SurveyCompare = new Comparison();
        }

        // after surveys have been selected, this can be used to auto select other options
        public void LoadTemplateSettings(ReportTemplate t)
        {

            switch (t)
            {
                case ReportTemplate.Standard:

                    // default settings


                    LayoutOptions.BlankColumn = true;

                    // comparison options
                    CompareWordings = true;

                    break;
                case ReportTemplate.StandardTranslation:
                    break;
                case ReportTemplate.Website:
                    break;
                case ReportTemplate.WebsiteTranslation:
                    break;
                case ReportTemplate.Automatic:
                    break;

            }
        }

        #region Methods and Functions

        

        #region Report Creation

        /// <summary>
        /// Creates a survey report in standard form. Standard form displays one or more surveys in Qnum order. Additional surveys are matched up
        /// based on refVarName and have their wordings compared to each other.
        /// </summary>
        /// <returns>0 for success, 1 for failure.</returns>
        public int GenerateReport()
        {
            FinalSurveyTables.Clear();

            if (Surveys.Count > 1 && CompareWordings)
            {
                ReportStatus = "Running comparisons...";
                DoComparisons();
            }

            ReportStatus = "Creating survey table(s)...";
            CreateFinalSurveyTables();

            ReportStatus = "Creating final report table...";
            if (CreateFinalReportTable() == 1)
                return 1;

            return 0;
        }

        /// <summary>
        /// Perform comparisons by comparing each non-primary survey to the primary survey.
        /// </summary>
        private void DoComparisons()
        {
            foreach (ReportSurvey s in Surveys)
            {
                if (!s.Primary)
                {
                    SurveyCompare.PrimarySurvey = PrimarySurvey();
                    SurveyCompare.OtherSurvey = s;
                    SurveyCompare.CompareByVarName();
                }
            }
        }

        /// <summary>
        /// Create a data table for each Survey in the report.
        /// </summary>
        private void CreateFinalSurveyTables()
        {
            DataTable dt;

            // create final tables
            foreach (ReportSurvey s in Surveys)
            {
                s.RemoveRepeats();
                dt = MakeFinalTable(s);
                if (dt != null)
                {
                    FinalSurveyTables.Add(dt);

                    if (s.Qnum)
                    {
                        qnumSurveyTable = dt;
                        qnumSurveyTable.TableName = "Qnum Survey";
                    }
                }
                else
                {
                    // this survey has no records
                }
            }
        }

        /// <summary>
        /// Merges all final tables into one final report table, ensures that the order of columns is correct, and then finally sorts the report by the SortBy column.
        /// </summary>
        private int CreateFinalReportTable()
        {

            // start with the QnumSurvey and then merge others into it
            // in order to add the extra columns as they appear in the final table, preserveChanges must be false
            // this causes the QNum and SortBy columns from the QnumSurvey to be overwritten 
            // to fix this, we copy the Qnum and SortBy columns from the QnumSurvey's original table
            if (qnumSurveyTable == null)
                return 1;

            ReportTable = qnumSurveyTable.Copy();
            ReportTable.AcceptChanges();

            if (Surveys.Count > 1)
                MergeTables();

            

            // remove primary if chosen
            if (SurveyCompare.HidePrimary)
            {
                foreach (ReportSurvey s in Surveys)
                {
                    if (s.Primary)
                    {
                        string columnName;
                        if (s.Backend == DateTime.Today)
                            columnName = s.SurveyCode;
                        else
                            columnName = s.SurveyCode + " " + s.Backend.ShortDate();

                        ReportTable.Columns.Remove(columnName);

                        for (int i = 0; i < ColumnOrder.Count; i++)
                        {
                            if (ColumnOrder[i].ColumnName.Equals(columnName))
                            {
                                ColumnOrder.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            ReportTable.PrimaryKey = null; // new DataColumn[] { ReportTable.Columns["VarName"] };
            ReportTable.Columns.Remove("refVarName");

            // add a blank column
            if (LayoutOptions.BlankColumn)
                ReportTable.Columns.Add(new DataColumn("Comment", Type.GetType("System.String")));

            // set the column order 
            try
            {
                foreach (ReportColumn rc in ColumnOrder)
                {
                    ReportTable.Columns[rc.ColumnName].SetOrdinal(rc.Ordinal - 1);
                }
            }
            catch
            {

            }

            // sort the report
            DataView dv = ReportTable.DefaultView;
            dv.Sort = "SortBy ASC";
            ReportTable = dv.ToTable();
            ReportTable.Columns.Remove("SortBy");

            // at this point the reportTable should be exactly how we want it to appear, minus interpreting tags

            return 0;
        }

        /// <summary>
        /// Compile all the survey data tables into a single ReportTable data table. The Qnum Survey's SortBy and Qnum columns will be copied again after the merge so that they are the 
        /// order-defining columns.
        /// </summary>
        private void MergeTables()
        {
            foreach (DataTable s in FinalSurveyTables)
            {
                if (!s.TableName.Equals("Qnum Survey"))
                {

                    ReportTable.Merge(s, false, MissingSchemaAction.Add);
                    // update the qnum and sortby columns to the original found in the qnum survey
                    for (int i = 0; i < ReportTable.Rows.Count; i++)
                    {
                        try
                        {
                            ReportTable.Rows[i]["SortBy"] = qnumSurveyTable.Rows[i]["SortBy"];
                            ReportTable.Rows[i]["Qnum"] = qnumSurveyTable.Rows[i]["Qnum"];
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }
                }
            }
        }


        #region XML Table methods

        /// <summary>
        /// Output the ReportTable data table as a DOCX file using the OpenXML library.
        /// </summary>
        public void OutputReportTableXML(string customFileName = "")
        {
            Word.Application appWord;   // instance of MSWord
            Word.Document docReport;

            string templatePath;

            ReportStatus = "Outputting report...";

            if (string.IsNullOrEmpty(customFileName))
            {
                if (Batch)
                    if (Surveys[0].FilterCol)
                        FileName += ReportFileName() + ", with filters, " + DateTime.Today.ShortDate();
                    else
                        FileName += ReportFileName() + ", " + DateTime.Today.ShortDate();
                else
                    FileName += ReportFileName() + ", " + DateTime.Now.DateTimeForFile();
            }
            else
            {
                FileName += customFileName + ", " + DateTime.Now.DateTimeForFile();
            }
            FileName += ".docx";

            // get the template path
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter:
                    templatePath = Properties.Resources.TemplateLetter;
                    break;
                case PaperSizes.Legal:
                    templatePath = Properties.Resources.TemplateLegal;
                    break;
                case PaperSizes.Eleven17:
                    templatePath = Properties.Resources.Template11x17;
                    break;
                case PaperSizes.A4:
                    templatePath = Properties.Resources.TemplateA4;
                    break;
                default:
                    templatePath = Properties.Resources.TemplateLetter;
                    break;
            }


            // create a new, blank document from the appropriate template
            appWord = new Word.Application
            {
                Visible = false
            };

            docReport = appWord.Documents.Add(templatePath);
           
            // save it to the destination folder and close it
            docReport.SaveAs2(FileName);
            docReport.Close();

            // now open the file and add content to it using the OpenXML library
            CreateXMLDoc(FileName);

            // open it again using Word Interop to set all the formatting options
            docReport = appWord.Documents.Open(FileName);
            docReport.Range(0, 0).Delete(); // delete the extra paragraph that word will insert upon opening the file (bookmark related?)
            
            ReportStatus = "Formatting report...";

            // disable spelling and grammar checks (useful for foreign languages)
            appWord.Options.CheckSpellingAsYouType = false;
            appWord.Options.CheckGrammarAsYouType = false;

            // footer text            
            if (Surveys.Count == 1 || !CompareWordings)
            {
                docReport.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                    "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());
            }
            else
            {
                docReport.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                    "\t\t" + "Generated on " + DateTime.Today.ShortDateDash() + "\r\n" + HighlightingKey().Replace("Highlighting key: ", "").Replace("\t", "  "));
            }

            // add highlight key to footer
            if (Surveys.Count > 1 && CompareWordings)
            {
                Word.Range rng = docReport.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                rng.Paragraphs[rng.Paragraphs.Count - 1].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;               
            }

            // create title page
            if (LayoutOptions.CoverPage)
            {
                // inserting a picture with Word interop is infinitely easier than with openXML
                // Table on cover page is always table number 1
                docReport.Tables[1].Rows[1].Cells[1].Range.Text = "";
                docReport.Tables[1].Rows[1].Cells[1].Range.InlineShapes.AddPicture(Properties.Resources.logoPath, false, true);
            }
            
            ReportStatus = "Interpreting formatting tags...";
            // interpret formatting tags
            Formatting.FormatTags(appWord, docReport, SurveyCompare.Highlight);

            // TODO convert TC tags into real tracked changes
            if (SurveyCompare.ConvertTrackedChanges)
            {
                ReportStatus = "Converting to real tracked changes...";
                Formatting.ConvertTC(docReport);
            }

            // TODO format shading for order comparisons
            if (ReportType == ReportTypes.Order)
            {
                ReportStatus = "Interpreting shading tags...";
                Formatting.FormatShading(docReport);
            }

            // update ToC since headings are formatting after ToC is created
            if (LayoutOptions.ToC == TableOfContents.PageNums && docReport.TablesOfContents.Count > 0)
            {
                docReport.TablesOfContents[1].UpdatePageNumbers();
                docReport.TablesOfContents[1].Update();
            }

            ReportStatus = "Saving...";
            //save the file
            docReport.Save();

            // close the document and word if this is an automatic survey
            if (Batch)
            {
                if (LayoutOptions.FileFormat == FileFormats.PDF)
                {
                    docReport.ExportAsFixedFormat(FileName.Replace(".docx", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, true,
                        Word.WdExportOptimizeFor.wdExportOptimizeForPrint, Word.WdExportRange.wdExportAllDocument, 1, 1,
                        Word.WdExportItem.wdExportDocumentContent, true, true, Word.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false);
                }
                docReport.Close();
                appWord.Quit();
            }
            else
            {
                if (LayoutOptions.FileFormat == FileFormats.PDF)
                {
                    try
                    {
                        docReport.ExportAsFixedFormat(FileName.Replace(".docx", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, true,
                            Word.WdExportOptimizeFor.wdExportOptimizeForPrint, Word.WdExportRange.wdExportAllDocument, 1, 1,
                            Word.WdExportItem.wdExportDocumentContent, true, true, Word.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false);
                    }
                    catch (Exception)
                    {
                        ReportStatus = "Error outputing report...";
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

            ReportStatus = "Report generated successfully...";

        }

        /// <summary>
        /// Create an OpenXML document and add content and formatting.
        /// </summary>
        /// <param name="filePath"></param>
        private void CreateXMLDoc(string filePath)
        {
            WordDocumentMaker wd = new WordDocumentMaker(filePath);

            Document document = (Document)wd.body.Parent;

            var settingsPart = document.MainDocumentPart.DocumentSettingsPart;//  AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings { BordersDoNotSurroundFooter = new BordersDoNotSurroundFooter() { Val = true } };

            settingsPart.Settings.Append(new UpdateFieldsOnOpen() { Val = true });

           
            if (LayoutOptions.CoverPage)
            {
                ReportStatus = "Creating title page...";
                MakeTitlePageXML(wd.body);
            }

            if (LayoutOptions.ToC != TableOfContents.None)
            {
                ReportStatus = "Creating Table of Contents...";
                MakeToCXML(wd.body);
            }

            // create title of report
            string titleText = ReportTitle();
            
            // include highlight key if needed
            if (Surveys.Count > 1 && CompareWordings)
                titleText += "\r\n" + HighlightingKey();

            // include filter info if specified
            titleText += "\r\n" + FilterLegend();

            titleText = Utilities.TrimString(titleText, "\r\n");

            wd.AddTitleParagraph(titleText);

            // add survey content
            MakeSurveyContentTable(wd);

            // add appendices
            if (SurvNotes) {
                ReportStatus = "Creating survey notes appendix...";
                MakeSurveyNotesAppendixXML(wd.body);
            }

            if (VarChangesApp)
            {
                ReportStatus = "Creating VarName changes appendix...";
                MakeVarChangesAppendixXML(wd.body);
            }

            

            wd.Close();

        }

        private void FixTextDirection (Table table)
        {
            foreach (TableCell c in table.Descendants<TableCell>())
            {
                foreach (Text t in c.Descendants<Text>())
                {
                    if (Utilities.IsArabic(t.Text) || Utilities.IsHebrew(t.Text))
                    {
                        foreach (Paragraph p in c.Descendants<Paragraph>())
                        {

                            ParagraphProperties pPr = p.Elements<ParagraphProperties>().FirstOrDefault();

                            if (pPr != null)
                            {
                                pPr.Append(new BiDi(), new TextDirection()
                                {
                                    Val = TextDirectionValues.TopToBottomRightToLeft
                                });
                            }
                            foreach (RunProperties rPr in p.Descendants<RunProperties>())
                                rPr.Append(new RightToLeftText());
                        }
                        break;
                    }
                }
            }
        }

        private void MakeSurveyContentTable(WordDocumentMaker wd)
        {
            // add any instructions first
            if (TranslatorInstructions)
                InsertTranslatorInstructions(wd.body);

            // create the table with the main content of the report
            Table table = wd.AddTable(ReportTable);

            FixTextDirection(table);

            // adjust the columns for table format if needed
            if (SubsetTables && Numbering == Enumeration.Qnum && ReportType == ReportTypes.Standard)
                AddTableFormatColumns(table);

            // format header row
            FormatHeaderRow(table);

            // set cell alignment for all cells
            foreach (TableCell cell in table.Descendants<TableCell>())
            {
                TableCellProperties tcPr = new TableCellProperties();
                TableCellVerticalAlignment tcva = new TableCellVerticalAlignment()
                {
                    Val = TableVerticalAlignmentValues.Top
                };

                cell.Append(tcPr);
            }

            // set table cell text font to Verdana 10 (20 in half-size points)
            foreach (Run r in table.Descendants<Run>())
            {
                RunProperties rPr = new RunProperties();

                rPr.PrependChild(new FontSize() { Val = "20" });
                rPr.PrependChild(new RunFonts() { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" });
                r.PrependChild(rPr);
            }

            // set any filter columns to Verdana 9
            foreach (ReportSurvey rs in Surveys)
                if (rs.FilterCol)
                {
                    ChangeFilterColumnFont(table);
                    break;
                }

            // format the column widths
            ReportStatus = "Formatting column widths...";
            FormatColumnsXML(table);

            // insert subset tables
            if (SubsetTables && Numbering == Enumeration.Qnum && ReportType == ReportTypes.Standard)
            {
                FormatSubsetTables(table);
            }

            ReportStatus = "Formatting section headings...";
            if (ReportType == ReportTypes.Standard)
                FormatSectionHeadings(table, ShowAllVarNames, ShowAllQnums, ColorSubs);

            

            // remove space after paragraphs
            foreach (ParagraphProperties pPr in table.Descendants<ParagraphProperties>())
            {
                pPr.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            }

            Paragraph lastPara = new Paragraph();
            lastPara.Append(new ParagraphProperties(new SectionProperties())); 
            wd.body.Append(lastPara);
        }

        private void InsertTranslatorInstructions(Body body)
        {

            
            Paragraph instructionsPara = new Paragraph();

            Run runHeader = new Run(new RunProperties(new FontSize() { Val = "32" }, new RunFonts() { Ascii = "Arial" }), new Text("Translation Instructions:"));
            instructionsPara.Append(runHeader);
            body.Append(instructionsPara);
            if (Surveys.Count == 1)
            {
                string language = Surveys[0].TransFields[0];
                body.Append(NumberedListItem("Any English text that is greyed out does not need to be translated.", 1));
                body.Append(NumberedListItem("The English has <u>programming instructions</u> that do not appear in the " + language + ", like '<strong>ask if...</strong>' at the start of each " + 
                                                "question and often '<strong>if response=???, go to...</strong>' at the end.  They are greyed out to indicate that they do not need to be translated.  These fields will automatically be " +
                                                "inserted into the final " + language + " draft.", 1));

                body.Append(NumberedListItem("<u>Headings</u>, sometimes pink and sometimes blue, should stay English, so you do not need to translate them.  The respondent never sees them; they are for survey development only.", 1));
                body.Append(NumberedListItem("Any [grey]<u>grey cells</u>[/grey] in the " + language +
                                                " column do not require translation.  This is text that does not appear to the respondent.", 1));

                body.Append(NumberedListItem("In the English, to <u>emphasize certain words</u>, we sometimes use " +
                                                "bold font and sometimes upper-case font; this is just because of past technical limitations.  Please use bold in all such cases in " + language + ".", 1));

                body.Append(NumberedListItem("Important: When translating new text, <u>use the same " + language + " wording and terminology</u> as in the rest of the survey, where relevant.", 1));
                body.Append(XMLUtilities.NewParagraph(""));
            }
            else
            {
                string language = Surveys[1].TransFields[0];
                body.Append(NumberedListItem("If the English has changed, the first column is <u>highlighted</u>.  That shows that changes are needed to the " + language + ".  " +
                        "Any English that is not highlighted does not need to be edited in the " + language + ".", 1));
                body.Append(NumberedListItem("[brightgreen]Green[/brightgreen] <u>highlighting</u> means the wording has changed. [yellow]Yellow[/yellow] highlighting means a new wording, or a whole new question, has been added.", 1));
                body.Append(NumberedListItem("[t]Blue[/t] highlighting means the text was in " + language + " but not in the current survey.  It should be deleted from the " + language + ".", 1));
                body.Append(NumberedListItem("Any English text that is greyed out does not need to be translated.", 1));
                body.Append(NumberedListItem("The English has <u>programming instructions</u> that do not appear in the " + language + ", like '<strong>ask if...</strong>' at the start of each " +
                "question and often '<strong>if response=???, go to...</strong>' at the end.  They are greyed out to indicate that they do not need to be translated.  These fields will automatically be " +
                "inserted into the final " + language + " draft.", 1));
                body.Append(NumberedListItem("<u>Headings</u>, sometimes pink and sometimes blue, should stay English, so you do not need "+
                "to translate them.  The respondent never sees them; they are for survey development only.", 1));
                body.Append(NumberedListItem("Any [grey]<u>grey cells</u>[/grey] in the " + language + 
                " column do not require translation.  This is text that does not appear to the respondent.", 1));
                body.Append(NumberedListItem("In the English, to <u>emphasize certain words</u>, we sometimes use " +
                "bold font and sometimes upper-case font; this is just because of past technical limitations.  Please use bold in all such cases in " + language + "." ,1));
                body.Append(NumberedListItem("Important: When translating new text, <u>use the same " + language + " wording and terminology</u> as in the rest of the survey, where relevant.", 1));
                body.Append(XMLUtilities.NewParagraph(""));
            }

        }

        private Paragraph NumberedListItem (string text, int numberingID)
        {
            Paragraph p1 = new Paragraph();
            ParagraphProperties p1Pr = new ParagraphProperties();
            p1Pr.Append(new KeepLines());
            SpacingBetweenLines spacing = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" };
            p1Pr.Append(spacing);
            p1Pr.Append(new NumberingProperties(new NumberingLevelReference() { Val = 0 }, new NumberingId() { Val = numberingID }));
            //p1Pr.Append(new Indentation() { Left = "900" });
            p1.Append(p1Pr);

            Run run1 = new Run();
            RunProperties r1Pr = new RunProperties();
            r1Pr.Append(new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" });
            r1Pr.Append(new FontSize() { Val = "24" });
            run1.Append(r1Pr);
            run1.Append(new Text() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            p1.Append(run1);

            return p1;
        }

        private void FormatSubsetTables(Table table)
        {
            // determine heading indices
            int qnumCol = QnumColumn();
            if (qnumCol == -1)
                return;

            int varCol = VarNameColumn();
            if (varCol == -1)
                return;

            int wordCol = FirstSurveyColumn();
            if (wordCol == -1)
                return;

            if (this.SubsetTablesTranslation)
            {

            }
            else
            {

            }

            ReportStatus = "Inserting subset tables...";
            LayoutOptions.FormatSubTables(table, qnumCol, varCol, wordCol);

        }

        /// <summary>
        /// Find any column labelled as a Filters column and change it's font size to 9 (18 in half points).
        /// </summary>
        /// <param name="table"></param>
        private void ChangeFilterColumnFont(Table table)
        {
            var firstRowCells = table.Elements<TableRow>().ElementAt(0).Elements<TableCell>();
            bool firstRow;
            for (int c = 0; c < firstRowCells.Count(); c++)
            {
                if (firstRowCells.ElementAt(c).GetCellText().Contains("Filters"))
                {
                    firstRow = true;
                    foreach (TableRow row in table.Elements<TableRow>())
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                            continue;
                        }
                        foreach (Run r in row.Elements<TableCell>().ElementAt(c).Descendants<Run>())
                        {
                            RunProperties rPr = new RunProperties();
                            rPr.Append(new RunFonts() { Ascii = "Verdana" });
                            rPr.Append(new FontSize() { Val = "18" });
                            r.RemoveAllChildren<RunProperties>();
                            r.PrependChild(rPr);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Format the section headings in this survey. Each section heading is colored Pink (or blue in the case of subheadings), bolded, and centered. The Qnum and VarName for each heading
        /// can be hidden or shown.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="varCol"></param>
        /// <param name="keepVarNames"></param>
        /// <param name="keepQnums"></param>
        /// <param name="subheads"></param>
        private void FormatSectionHeadings(Table table, bool keepVarNames, bool keepQnums, bool subheads)
        {
            var rows = table.Elements<TableRow>();
            int varCol = VarNameColumn();
            int qnumCol = QnumColumn();
            int altqnumCol = GetColumnNumber("AltQnum")-1;

            // cannot process headings if VarName is not present
            if (varCol < 0)
                return;

            for (int i = 0; i <rows.Count(); i++)
            {
                bool firstDone = false;
                TableRow currentRow = rows.ElementAt(i);
                var cells = currentRow.Elements<TableCell>();

                if (cells.Count() == 1)
                    continue;

                string varname = cells.ElementAt(varCol).GetCellText();

                varname = Utilities.RemoveHighlightTags(varname);

                if (!varname.StartsWith("Z"))
                    continue;

                // row properties: row height
                TableRowProperties trPr = currentRow.Elements<TableRowProperties>().FirstOrDefault();

                if (trPr == null)
                {
                    trPr = new TableRowProperties();
                    currentRow.PrependChild<TableRowProperties>(trPr);
                }
                trPr.Append(new TableRowHeight() { Val = 20 });

                for (int c = 0; c < cells.Count(); c++)
                {

                    // cell properties: vertical alignment
                    TableCellProperties tcPr = cells.ElementAt(c).Elements<TableCellProperties>().FirstOrDefault();

                    if (tcPr == null)
                    {
                        tcPr = new TableCellProperties();
                        cells.ElementAt(c).PrependChild(tcPr);
                    }
                    tcPr.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
                  

                    // paragraph properties: horizontal alignment and heading style
                    Paragraph p = cells.ElementAt(c).Elements<Paragraph>().First();
                    if (p.Elements<ParagraphProperties>().Count() == 0)
                        p.PrependChild<ParagraphProperties>(new ParagraphProperties());

                    ParagraphProperties pPr = p.Elements<ParagraphProperties>().First();

                    if (varname.StartsWith("Z") && varname.EndsWith("s") && subheads && c>varCol && !firstDone)
                    {
                        pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "Heading2" };
                        firstDone = true;
                    }
                    else if (varname.StartsWith("Z") && c > varCol && !firstDone)
                    {
                        pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "Heading1" };
                        firstDone = true;
                    }
                   
                    pPr.Append(new Justification() { Val = JustificationValues.Center });
                    pPr.Append(new SpacingBetweenLines() { Before = "120", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto } );

                    // run properties: color, size, bold
                    Run r = p.Elements<Run>().First();
                    RunProperties rPr = r.Elements<RunProperties>().FirstOrDefault();

                    if (rPr == null)
                    {
                        rPr = new RunProperties();
                        r.PrependChild(rPr);
                    }

                    rPr.Append(new Bold());
                    rPr.Append(new FontSize() { Val = "24" });
                    rPr.Append(new Color() { Val = "000000" });
                   

                    if (!keepVarNames)
                        cells.ElementAt(varCol).SetCellText("");

                    if (!keepQnums)
                    {
                        if (qnumCol >= 0) cells.ElementAt(qnumCol).SetCellText("");
                        if (altqnumCol >= 0) cells.ElementAt(altqnumCol).SetCellText("");
                    }

                    if (varname.StartsWith("Z") && varname.EndsWith("s") && subheads)
                    {
                        //   Word.WdColor.wdColorSkyBlue;
                        tcPr.Append(XMLUtilities.SkyBlueShading());
                    }
                    else if (varname.StartsWith("Z"))
                    {
                        //    Word.WdColor.wdColorRose;
                        tcPr.Append(XMLUtilities.RoseShading());
                    }
                }
            }
        }

        /// <summary>
        /// Add columns the table's grid so that there are enough columns to display the response options for table format questions.
        /// </summary>
        /// <param name="table"></param>
        private void AddTableFormatColumns(Table table)
        {
            // get all the table format questions
            List<SurveyQuestion> tfq = Surveys[0].Questions.Where(x => x.TableFormat == true).ToList();

            // determine the maximum number of response options
            int most = 0;
            foreach (SurveyQuestion q in tfq)
            {
                int respCount = q.GetRespNumbers().Count();
                if (respCount > most)
                    most = respCount;
            }

            // add columns to accomodate the question with the most response options
            for (int i = 0; i < most; i++)
            {
                table.Elements<TableGrid>().ElementAt(0).Append(new GridColumn());
            }
        }

        /// <summary>
        /// Format's the column names and properties of the first row in the table.
        /// </summary>
        /// <param name="table"></param>
        private void FormatHeaderRow(Table table)
        {
            // set header row
            TableRow firstRow = table.Elements<TableRow>().ElementAt(0);
            var firstRowCells = table.Elements<TableRow>().ElementAt(0).Elements<TableCell>();
            string header;

            TableRowProperties trPr = new TableRowProperties();

            trPr.Append(new TableHeader());

            firstRow.Append(trPr);

            // for each cell in the row, set
            foreach (TableCell cell in firstRowCells)
            {
                // the border(black)
                TableCellProperties tcPr = new TableCellProperties(XMLUtilities.BlackSingleCellBorder());
                // the background color(rose)
                tcPr.Append(XMLUtilities.RoseShading());

                cell.PrependChild(tcPr);

                // text alignment for each paragraph (center) 
                foreach (Paragraph p in cell.Descendants<Paragraph>())
                {
                    ParagraphProperties prPr = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                    p.PrependChild(prPr);

                    // and bold each run
                    foreach (Run run in p.Descendants<Run>())
                    {
                        RunProperties rPr = new RunProperties();
                        rPr.Append(new Bold());
                        run.PrependChild(rPr);
                    }
                }

                // change the text of the headings
                header = cell.GetCellText();
                switch (header)
                {
                    case "Qnum":
                        cell.SetCellText("Q#");

                        break;
                    case "AltQnum":
                        cell.SetCellText("AltQ#");

                        break;

                    default:
                        // question column with date, format date
                        if (header.Contains(DateTime.Today.ShortDateDash()))
                        {
                            cell.SetCellText(header.Replace(DateTime.Today.ShortDateDash(), ""));
                        }

                        // insert some instructions for translator template
                        if (TranslatorInstructions)
                        {
                            string match = PrimarySurvey().SurveyCode + @"[\s0-9A-Za-z]*" + PrimarySurvey().TransFields[0] + @"[\s0-9A-Za-z]*";
                            if (Regex.IsMatch(header, match))
                                cell.SetCellText(header + "\r\n" + "To Be Updated for " + QnumSurvey().SurveyCode); 
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Resizes the widths of all columns to fit on the page. Columns other than Qnum and VarName are evenly sized.
        /// </summary>
        /// <param name="table"></param>
        private void FormatColumnsXML(Table table)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float varWidth = 0.9f;
            int numCols;
            List<int> setCount = new List<int>(); // index numbers of columns whom's widths were set 
            double dividedWidth; // the remaining width divided by the remaining number of columns

            string header;
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter: widthLeft = 10.25; break;
                case PaperSizes.Legal: widthLeft = 13.25; break;
                case PaperSizes.Eleven17: widthLeft = 16.25; break;
                case PaperSizes.A4: widthLeft = 10.75; break;
                default: widthLeft = 10.25; break;
            }

            TableGrid grid = table.Elements<TableGrid>().ElementAt(0);
            var columns = grid.Elements<GridColumn>();
            var headerRow = table.Elements<TableRow>().ElementAt<TableRow>(0);
            var headerCells = headerRow.Elements<TableCell>();

            numCols = columns.Count();

            for (int i = 0; i < headerCells.Count(); i++)
            {
                header = headerCells.ElementAt(i).GetCellText();

                switch (header)
                {
                    case "Q#":
                        columns.ElementAt(i).Width = Convert.ToString(qnumWidth * 1440);
                        widthLeft -= qnumWidth;
                        setCount.Add(i);
                        break;
                    case "AltQ#":
                        columns.ElementAt(i).Width = Convert.ToString(altqnumWidth * 1440);
                        widthLeft -= altqnumWidth;
                        setCount.Add(i);
                        break;
                    case "VarName":
                        columns.ElementAt(i).Width = Convert.ToString(varWidth * 1440);
                        widthLeft -= varWidth;
                        setCount.Add(i);
                        break;
                    default:

                        // an additional AltQnum column
                        if (header.Contains("AltQnum"))
                        {
                            columns.ElementAt(i).Width = Convert.ToString(altqnumWidth * 1440);
                            widthLeft -= altqnumWidth;
                            setCount.Add(i);
                        }
                        else if (header.Contains("Qnum")) // an additional Qnum column
                        {
                            columns.ElementAt(i).Width = Convert.ToString(qnumWidth * 1440);
                            widthLeft -= qnumWidth;
                            setCount.Add(i);
                        }

                        break;
                }
            }

            // the remaining width evenly divided amongst the remaining columns
            dividedWidth = widthLeft / (numCols - setCount.Count);

            // if the column index is not in the "set" collection of column indices, set it to a share of the remaining width
            for (int i = 0; i < headerCells.Count(); i++)
            {
                header = headerCells.ElementAt(i).GetCellText();
                if (!setCount.Contains(i))
                {
                    columns.ElementAt(i).Width = Convert.ToString(dividedWidth * 1440);
                }
            }

        }


        /// <summary>
        /// Creates a new section at the beginning of the document. Adds a table containing the ITC logo, the survey title and additional information
        /// about the survey.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeTitlePageXML(Body body)
        {
            ReportSurvey s = PrimarySurvey();

            Table table = XMLUtilities.NewTable(1);

            TableProperties tblPr = new TableProperties();

            tblPr.Append(XMLUtilities.NoBorder()); 
            tblPr.Append(new TableLayout() { Type = TableLayoutValues.Fixed });
            tblPr.Append(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" });
            // center table in the page
            tblPr.Append(new TablePositionProperties() { TablePositionYAlignment = VerticalAlignmentValues.Center });
               
            table.Append(tblPr);


            // add info to table
            // add ITC logo
            TableCell pictureCell = new TableCell();
            pictureCell.SetCellText("picture");
            // TODO insert picture into cell (complicated)////t.Rows[1].Cells[1].Range.InlineShapes.AddPicture(Properties.Resources.logoPath, false, true);
            table.Append(new TableRow(pictureCell));

            // add survey title
            TableCell titleCell = new TableCell();
            titleCell.SetCellText(s.Title);
            table.Append(new TableRow(titleCell));
            // cohort
            TableCell cohortCell = new TableCell();
            if (s.Cohort == null)
                cohortCell.SetCellText(" ");
            else 
                cohortCell.SetCellText(s.Cohort.Cohort);
            table.Append(new TableRow(cohortCell));
            // survey code
            TableCell codeCell = new TableCell();
            codeCell.SetCellText("Survey Code: " + s.SurveyCode);
            table.Append(new TableRow(codeCell));
            // languages
            TableCell languageCell = new TableCell();
            languageCell.SetCellText("Languages: " + s.Languages);
            table.Append(new TableRow(languageCell));
            // mode
            TableCell modeCell = new TableCell();
            modeCell.SetCellText("Mode: " + s.Mode.ModeAbbrev);
            table.Append(new TableRow(modeCell));
            // group
            TableCell groupCell = new TableCell();
            groupCell.SetCellText(!s.Group.UserGroup.Equals("") ? "(" + s.Group.UserGroup + ")" : "");
            table.Append(new TableRow(groupCell));

            // center text
            foreach (ParagraphProperties pPr in table.Descendants<ParagraphProperties>())
            {
                pPr.Append(new Justification() { Val = JustificationValues.Center });
            }

            // size text
            foreach (RunProperties rPr in table.Descendants<RunProperties>())
            {
                rPr.RemoveAllChildren();
                rPr.Append(new RunFonts() { Ascii = "Verdana" });
                rPr.Append(new FontSize() { Val = "36" });
            }

            body.Append(table);
            

            Paragraph lastPara = new Paragraph();
            lastPara.Append(new ParagraphProperties(new VerticalTextAlignmentOnPage
            {
                Val = (EnumValue<VerticalJustificationValues>)VerticalJustificationValues.Center
            }));
            
            body.Append(lastPara);
            body.Append(XMLUtilities.PageBreak());

        }

        /// <summary>
        /// Creates a new section at the top of the document. Adds a table of contens in 1 of 2 ways. Either a TableOfContents object is created and 
        /// based on the headings in the document, or the text and Qnums for each heading are listed in a table.
        /// </summary>
        /// <param name="doc">Document object</param>
        private void MakeToCXML(Body body)
        {
            if (LayoutOptions.ToC == TableOfContents.None)
                return;

            // exit if no headings found
            if (QnumSurvey().Questions.Count(x => x.VarName.VarName.StartsWith("Z")) == 0)
                return;

            List<SurveyQuestion> headingQs;
            headingQs = QnumSurvey().Questions.Where(x => x.VarName.RefVarName.StartsWith("Z")).ToList();

            switch (LayoutOptions.ToC)
            {
                case TableOfContents.Qnums: // create a regular, 2-column table and add the headings and their Qnums
                    
                    Table tocTable = XMLUtilities.NewTable(2);

                    tocTable.Append(new TableProperties(XMLUtilities.NoBorder(), new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" }));

                    TableRow header = new TableRow();
                    TableCell headerCell = new TableCell();
                    headerCell.SetCellText("TABLE OF CONTENTS");
                    headerCell.Descendants<RunProperties>().ElementAt(0).Append(new Bold());
                    header.Append(headerCell);
                    tocTable.Append(header);
                    

                    for (int i = 0; i < headingQs.Count; i++)
                    {
                        TableRow row = new TableRow();
                        TableCell sectionHeading = new TableCell();
                        TableCell qnum = new TableCell();
                        sectionHeading.SetCellText(headingQs[i].PreP);
                        qnum.SetCellText(headingQs[i].Qnum.Substring(0,3));
                        qnum.Descendants<ParagraphProperties>().ElementAt(0).Append(new Justification() { Val = JustificationValues.Right });
                     
                        row.Append(sectionHeading);
                        row.Append(qnum);
                        tocTable.Append(row);
                    }

                    foreach (RunProperties rPr in tocTable.Descendants<RunProperties>())
                    {
                        rPr.Append(new RunFonts() { Ascii = "Cambria (Headings)" });
                        rPr.Append(new FontSize() { Val = "24" }); 
                    }

                        body.Append(tocTable);

                    Paragraph lastPara = new Paragraph();
                    lastPara.Append(new ParagraphProperties(XMLUtilities.LandscapeSectionProps()));
                    body.Append(lastPara);

                    break;
                case TableOfContents.PageNums: // create a list of paragraphs containing runs with hyperlink fields that can be updated by word

                    body.Append(XMLUtilities.NewParagraph("TABLE OF CONTENTS"));

                    Paragraph tocStart = XMLUtilities.XMLToC();
                    tocStart.Append(XMLUtilities.XMLToCEntry(headingQs[0].PreP));
                    
                    body.Append(tocStart);

                    for (int i = 1; i < headingQs.Count; i++)
                    {
                        Paragraph entry;
                        if (headingQs[i].VarName.VarName.EndsWith("s"))
                            entry = XMLUtilities.XMLToCParagraph2();
                        else 
                            entry = XMLUtilities.XMLToCParagraph();

                        entry.Append(XMLUtilities.XMLToCEntry(headingQs[i].PreP));
                        body.Append(entry);
                    }

                    Paragraph tocEnd = XMLUtilities.XMLToCEnd(PageOrientationValues.Landscape);
                    body.Append(tocEnd);

                    body.Append(XMLUtilities.PageBreak());
                    break;
            }

        }



        /// <summary>
        /// Creates a table at the end of the report that contains all Survey Notes and Wave notes associated with the surveys appearing in the report.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeSurveyNotesAppendixXML(Body body)
        {
            List<SurveyComment> surveyNotes = new List<SurveyComment>();

            foreach (ReportSurvey s in Surveys)
            {
                surveyNotes.AddRange(s.SurveyNotes);
            }

            if (surveyNotes.Count == 0)
                return;

            body.Append(XMLUtilities.PageBreak());

            body.Append(XMLUtilities.NewParagraph("Appendix", JustificationValues.Center, "40", "Verdana"));
            body.Append(XMLUtilities.NewParagraph("Survey Notes", JustificationValues.Center, "40", "Verdana"));
            body.Append(new Paragraph());


            Table table = XMLUtilities.NewTable(3, TableLayoutValues.Autofit);

            TableProperties tblPr = new TableProperties();

            tblPr.Append(XMLUtilities.GreySingleTableBorder());
            table.Append(tblPr);

            TableRow headerRow = new TableRow();
            TableCell headerCell = new TableCell();

            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Survey");
            headerRow.Append(headerCell);

            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Notes");
            headerRow.Append(headerCell);

            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Author");
            headerRow.Append(headerCell);
            table.Append(headerRow);

            // text alignment for each paragraph (center) 
            foreach (Paragraph p in headerRow.Descendants<Paragraph>())
            {
                ParagraphProperties prPr = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                p.PrependChild(prPr);

                // and bold each run
                foreach (Run run in p.Descendants<Run>())
                {
                    RunProperties rPr = new RunProperties();
                    rPr.Append(new Bold());
                    run.PrependChild(rPr);
                }
            }

            foreach (SurveyComment c in surveyNotes)
            {
                TableRow row = new TableRow();
                TableCell cell = new TableCell();

                cell.SetCellText(c.Survey);
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.Notes.NoteText);
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.Author.Name);
                row.Append(cell);
                table.Append(row);
            }

            body.Append(table);
        }

        /// <summary>
        /// Creates a table at the end of the report that contains all VarName changes related to the surveys appearing the report.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeVarChangesAppendixXML(Body body)
        {
            List<VarNameChange> changes = new List<VarNameChange>();

            foreach (ReportSurvey s in Surveys)
            {
                changes.AddRange(s.VarChanges);
            }

            if (changes.Count == 0)
                return;

            body.Append(XMLUtilities.PageBreak());

            body.Append(XMLUtilities.NewParagraph("Appendix", JustificationValues.Center, "40", "Verdana"));
            body.Append(XMLUtilities.NewParagraph("VarName Changes", JustificationValues.Center, "40", "Verdana"));
            body.Append(new Paragraph());


            Table table = XMLUtilities.NewTable(6, TableLayoutValues.Autofit); 

            TableProperties tblPr = new TableProperties();

            tblPr.Append(XMLUtilities.GreySingleTableBorder());
            table.Append(tblPr);

            TableRow headerRow = new TableRow();
            TableCell headerCell = new TableCell();


            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("New Name");
            headerRow.Append(headerCell);

            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Old Name");
            headerRow.Append(headerCell);

            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Date");
            headerRow.Append(headerCell);
            
            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Survey");
            headerRow.Append(headerCell);
            
            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Changed By");
            headerRow.Append(headerCell);
            
            headerCell = new TableCell();
            headerCell.Append(new TableCellProperties(XMLUtilities.RoseShading(), XMLUtilities.BlackSingleCellBorder()));
            headerCell.SetCellText("Reasoning");
            headerRow.Append(headerCell);

            table.Append(headerRow);

            // text alignment for each paragraph (center) 
            foreach (Paragraph p in headerRow.Descendants<Paragraph>())
            {
                ParagraphProperties prPr = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                p.PrependChild(prPr);

                // and bold each run
                foreach (Run run in p.Descendants<Run>())
                {
                    RunProperties rPr = new RunProperties();
                    rPr.Append(new Bold());
                    run.PrependChild(rPr);
                }
            }

            foreach (VarNameChange c in changes)
            {
                TableRow row = new TableRow();
                TableCell cell = new TableCell();

                cell.SetCellText(c.NewName);
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.OldName);
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.ChangeDate.ToString());
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.GetSurveys());
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.ChangedBy.Name);
                row.Append(cell);

                cell = new TableCell();
                cell.SetCellText(c.Rationale);
                row.Append(cell);

                table.Append(row);
            }

            body.Append(table);

        }

        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReportFileName()
        {
            if (Web)
                return Surveys[0].WebName;

            string finalfilename;

            finalfilename = ListSurveys();

            if (Details != "") { finalfilename += ", " + Details; }
            if (!Batch) { finalfilename += " generated"; }

            return finalfilename;
        }

        public override string ReportTitle()
        {
            if (Web)
                return Surveys[0].WebName;

            if (Surveys.Count == 1)
            {
                return Surveys[0].Title;
            }else
            {
                return ListSurveys();
            }

            

        }

        private string ListSurveys()
        {
            string list;
            ReportSurvey primary = PrimarySurvey();
            string mainSource = primary.SurveyCode;
            string secondSources = "";

            if (primary.Backend != DateTime.Today)
                mainSource += " on " + primary.Backend.ShortDate();


            foreach (ReportSurvey o in NonPrimarySurveys())
            {
                secondSources += o.SurveyCode;
                if (o.Backend != DateTime.Today)
                {
                    secondSources += " on " + o.Backend.ShortDate();
                }

                secondSources += ", ";
            }

            secondSources = Utilities.TrimString(secondSources, ", ");

            list = mainSource;
            if (!string.IsNullOrEmpty(secondSources))
                list += " vs. " + secondSources;

            return list;
        }

        /// <summary>
        /// Returns a string describing the highlighting uses in the document.
        /// </summary>
        /// <returns></returns>
        public string HighlightingKey()
        {
            string currentSurv;
            string others = "";
            string primary = "";
            string otherH = "";
            string primaryH = "";
            string differentH = "";
            string orderChanges = "";
            bool showQnumOrder = false;
            string qnumorder = "";

            string finalKey = "";

            foreach (ReportSurvey s in Surveys)
            {
                currentSurv = s.SurveyCode;
                if (s.Backend != DateTime.Today)
                    currentSurv += " on " + s.Backend.ToString("d");

                if (!s.Primary)
                    others += ", " + currentSurv;
                else
                    primary += currentSurv;

                if (s.Qnum && s.ID != 1)
                {
                    showQnumOrder = true;
                    qnumorder = currentSurv;
                }
            }
            others = Utilities.TrimString(others, ", ");


            if (SurveyCompare.HighlightStyle == HStyle.Classic)
            {
                otherH = "[yellow]In " + others + " only.[/yellow]";
                primaryH = "\t[t]In " + primary + " only.[/t]";
                differentH = "\t[brightgreen] Different in " + primary + " and " + others + "[/brightgreen]";
                if (SurveyCompare.HybridHighlight)
                    differentH += "\r\n" + "<Font Color=Blue>In " + others + " only.</Font>\t<Font Color=Red>In " + primary + " only.</Font>";

            }
            else if (SurveyCompare.HighlightStyle == HStyle.TrackedChanges)
            {
                otherH = "<Font Color=Red>In " + primary + " only.</Font>";
                primaryH = "\t<Font Color=Blue>In " + others + " only.</Font>";
            }

            if (SurveyCompare.ShowOrderChanges)
                orderChanges = "\r\n" + "Pink file: location in " + primary + "\tBlue fill: location in " + others;

            if (showQnumOrder)
                qnumorder = "Question order determined by: " + qnumorder;
            else
                qnumorder = "";

            finalKey = "Highlighting key: " + otherH + differentH;
            if (SurveyCompare.ShowDeletedFields || SurveyCompare.ShowDeletedQuestions)
                finalKey += primaryH;

            finalKey += "\r\n" + qnumorder + orderChanges;



            return finalKey;
        }

        public string FilterLegend()
        {
            string strFilter = "";
            //if (Prefixes.Length >= 0) {
            //    strFilter = "Section filters: " + String.Join(",", Prefixes);
            //}
            //if (QRange != "") {
            //    strFilter = strFilter + "\r\n" + "Questions " + QRange;
            //}

            return strFilter.TrimEnd('\r', '\n');

        }

        

        #region Word Interop Methods (unused)

        /// <summary>
        /// Creates a new section at the beginning of the document. Adds a table containing the ITC logo, the survey title and additional information
        ///about the survey.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeTitlePage(Word.Document doc)
        {
            Word.Table t;
            ReportSurvey s = PrimarySurvey();
            // create new section
            doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            doc.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;

            doc.Sections[1].PageSetup.VerticalAlignment = Word.WdVerticalAlignment.wdAlignVerticalCenter;
            doc.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = "";

            // create a table
            t = doc.Tables.Add(doc.Range(0, 0), 7, 1, Word.WdDefaultTableBehavior.wdWord8TableBehavior, Word.WdAutoFitBehavior.wdAutoFitContent);

            t.Range.Font.Name = "Verdana";
            t.Range.Font.Size = 18;
            t.Rows.VerticalPosition = 1.8f;
            // add info to table
            t.Rows[1].Cells[1].Range.InlineShapes.AddPicture(Properties.Resources.logoPath, false, true);
            t.Rows[2].Cells[1].Range.Text = s.Title;
            if (s.Cohort == null)
                t.Rows[3].Cells[1].Range.Text = "";
            else
                t.Rows[3].Cells[1].Range.Text = s.Cohort.Cohort;
            t.Rows[4].Cells[1].Range.Text = "Survey Code: " + s.SurveyCode;
            t.Rows[5].Cells[1].Range.Text = "Languages: " + s.Languages;
            t.Rows[6].Cells[1].Range.Text = "Mode: " + s.Mode.ModeAbbrev;
            t.Rows[7].Cells[1].Range.Text = s.Group.UserGroup.Equals("") ? "(" + s.Group.UserGroup + ")" : "";
            // format table
            t.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
            t.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
            t.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

        }



        /// <summary>
        /// Creates a table at the end of the report that contains all Survey Notes and Wave notes associated with the surveys appearing in the report.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeSurveyNotesAppendix(Word.Document doc)
        {
            Word.Range r;
            Word.Table t;

            List<SurveyComment> surveyNotes = new List<SurveyComment>();

            foreach (ReportSurvey s in Surveys)
            {
                surveyNotes.AddRange(s.SurveyNotes);
            }

            if (surveyNotes.Count == 0)
                return;

            r = doc.Range(doc.Content.StoryLength - 1, doc.Content.StoryLength - 1);
            r.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            r.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            r.Text = "Appendix\r\nSurveyNotes";
            r.Font.Size = 20;

            r.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            r.InsertParagraph();

            // create table 
            t = doc.Tables.Add(r, surveyNotes.Count + 1, 3);
            // format table
            t.Rows[1].Cells[1].Range.Text = "Survey";
            t.Rows[1].Cells[2].Range.Text = "Notes";
            t.Rows[1].Cells[3].Range.Text = "Author";
            t.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorRose;
            t.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.OutsideColor = Word.WdColor.wdColorGray25;
            t.Borders.InsideColor = Word.WdColor.wdColorGray25;
            t.Rows[1].Borders.OutsideColor = Word.WdColor.wdColorBlack;
            t.Rows[1].Borders.InsideColor = Word.WdColor.wdColorBlack;

            // fill table
            for (int i = 0; i < surveyNotes.Count; i++)
            {
                t.Cell(i + 2, 1).Range.Text = surveyNotes[i].Survey;
                t.Cell(i + 2, 2).Range.Text = surveyNotes[i].Notes.NoteText;
                t.Cell(i + 2, 3).Range.Text = surveyNotes[i].Author.Name;
                t.Rows[i + 2].Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            }

            t.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
        }

        /// <summary>
        /// Creates a table at the end of the report that contains all VarName changes related to the surveys appearing the report.
        /// </summary>
        /// <param name="doc"></param>
        private void MakeVarChangesAppendix(Word.Document doc)
        {
            Word.Range r;
            Word.Table t;

            List<VarNameChange> changes = new List<VarNameChange>();

            foreach (ReportSurvey s in Surveys)
            {
                changes.AddRange(s.VarChanges);
            }

            if (changes.Count == 0)
                return;

            r = doc.Range(doc.Content.StoryLength - 1, doc.Content.StoryLength - 1);
            r.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            r.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            r.Text = "Appendix\r\nVarName Changes";
            r.Font.Size = 20;


            r.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            r.InsertParagraph();

            // create table 
            t = doc.Tables.Add(r, changes.Count + 1, 6);
            // format table
            t.Rows[1].Cells[1].Range.Text = "New Name";
            t.Rows[1].Cells[2].Range.Text = "Old Name";
            t.Rows[1].Cells[3].Range.Text = "Date";
            t.Rows[1].Cells[4].Range.Text = "Survey";
            t.Rows[1].Cells[5].Range.Text = "Changed By";
            t.Rows[1].Cells[6].Range.Text = "Reasoning";
            t.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorRose;
            t.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.OutsideColor = Word.WdColor.wdColorGray25;
            t.Borders.InsideColor = Word.WdColor.wdColorGray25;
            t.Rows[1].Borders.OutsideColor = Word.WdColor.wdColorBlack;
            t.Rows[1].Borders.InsideColor = Word.WdColor.wdColorBlack;

            // fill table

            for (int i = 0; i < changes.Count; i++)
            {
                t.Cell(i + 2, 1).Range.Text = changes[i].NewName;
                t.Cell(i + 2, 2).Range.Text = changes[i].OldName;
                t.Cell(i + 2, 3).Range.Text = changes[i].ChangeDate.ToString();
                t.Cell(i + 2, 4).Range.Text = changes[i].GetSurveys();
                t.Cell(i + 2, 5).Range.Text = changes[i].ChangedBy.Name;
                t.Cell(i + 2, 6).Range.Text = changes[i].Rationale;

                t.Rows[i + 2].Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            }

            t.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
        }

        /// <summary>
        /// Creates a new section at the top of the document. Adds a table of contens in 1 of 2 ways. Either a TableOfContents object is created and 
        /// based on the headings in the document, or the text and Qnums for each heading are listed in a table.
        /// </summary>
        /// <param name="doc">Document object</param>
        private void MakeToC(Word.Document doc)
        {
            // exit if no headings found
            if (QnumSurvey().Questions.Count(x => x.VarName.VarName.StartsWith("Z")) == 0)
                return;

            List<SurveyQuestion> headingQs;

            object missingType = Type.Missing;
            switch (LayoutOptions.ToC)
            {
                case TableOfContents.None:
                    break;
                case TableOfContents.Qnums:
                    Word.Table toc;
                    headingQs = QnumSurvey().Questions.Where(x => x.VarName.RefVarName.StartsWith("Z")).ToList();
                    
                    // create new section in document
                    doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
                    // create table of contents
                    
                    toc = doc.Tables.Add(doc.Range(0, 0), headingQs.Count + 1, 2, Word.WdDefaultTableBehavior.wdWord8TableBehavior, Word.WdAutoFitBehavior.wdAutoFitContent);
                    // format table
                    doc.Sections[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    doc.Sections[1].Range.Font.Name = "Cambria (Headings)";
                    doc.Sections[1].Range.Font.Size = 12;


                    // fill table
                    toc.Cell(1, 1).Range.Text = "TABLE OF CONTENTS";
                    toc.Cell(1, 1).Range.Font.Bold = 1;

                    for (int i = 0; i < headingQs.Count; i++)
                    {
                        toc.Cell(i + 2, 1).Range.Text = headingQs[i].PreP;
                        toc.Cell(i + 2, 2).Range.Text = headingQs[i].Qnum.Substring(0,3);
                        toc.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                    }


                    toc.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                    toc.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;

                    break;
                case TableOfContents.PageNums:
                    // create new section in document
                    doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
                    doc.TablesOfContents.Add(doc.Range(0, 0),true, 1, 3, false, missingType, missingType, missingType, missingType, true);
                    break;
            }

        }

        public override void FormatColumns(Word.Document doc)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float varWidth = 0.9f;
            //float commentWidth = 1f;
            int qCol;
            int otherCols;
            int numCols;
            string header;
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter: widthLeft = 10.5; break;
                case PaperSizes.Legal: widthLeft = 13.5; break;
                case PaperSizes.Eleven17: widthLeft = 16.5; break;
                case PaperSizes.A4: widthLeft = 11; break;
                default: widthLeft = 10.5; break;
            }
            // Qnum and VarName
            otherCols = 2;

            if (Numbering == Enumeration.Both)
            {
                qCol = 4;
                otherCols++; // AltQnum
            }
            else
            {
                qCol = 3;
            }

            doc.Tables[1].AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitFixed);

            numCols = doc.Tables[1].Columns.Count;

            for (int i = 1; i <= numCols; i++)
            {
                // remove underscores
                doc.Tables[1].Rows[1].Cells[i].Range.Text = doc.Tables[1].Rows[1].Cells[i].Range.Text.Replace("_", " ");
                header = doc.Tables[1].Rows[1].Cells[i].Range.Text.TrimEnd('\r', '\a');

                switch (header)
                {
                    case "Qnum":
                        doc.Tables[1].Rows[1].Cells[i].Range.Text = "Q#";
                        doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                        widthLeft -= qnumWidth;
                        break;
                    case "AltQnum":
                        doc.Tables[1].Rows[1].Cells[i].Range.Text = "AltQ#";
                        doc.Tables[1].Columns[i].Width = altqnumWidth * 72;
                        widthLeft -= altqnumWidth;
                        break;
                    case "VarName":
                        doc.Tables[1].Columns[i].Width = varWidth * 72;
                        widthLeft -= varWidth;
                        break;

                    default:
                        // question column with date, format date
                        if (header.Contains(DateTime.Today.ShortDate()))
                        {
                            doc.Tables[1].Rows[1].Cells[i].Range.Text = doc.Tables[1].Rows[1].Cells[i].Range.Text.Replace(DateTime.Today.ShortDate(), "");

                        }

                        // an additional AltQnum column
                        if (header.Contains("AltQnum"))
                        {
                            doc.Tables[1].Columns[i].Width = altqnumWidth * 72;
                            widthLeft -= altqnumWidth;
                        }
                        else if (header.Contains("AltQnum")) // an additional Qnum column
                        {
                            doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                            widthLeft -= qnumWidth;
                        }

                        break;
                }

            }


            for (int i = qCol; i <= numCols; i++)
                doc.Tables[1].Columns[i].Width = (float)(widthLeft / (numCols - qCol + 1)) * 72;

        }
        #endregion

        

        
        #endregion

        /// <summary>
        /// Returns the details of each property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            PropertyInfo[] _PropertyInfos = null;
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();
           
            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }

        public void SetColumnOrder() { }

        public void RemoveDuplicateColums() { }       

        // may be unneeded after a server function returns comments
        public void RemoveRepeatedComments() { }

        #endregion

        #region Unused for now
        // Extract the styles or stylesWithEffects part from a 
        // word processing document as an XDocument instance.
        public static XDocument ExtractStylesPart(string fileName, bool getStylesWithEffectsPart = true)
        {
            // Declare a variable to hold the XDocument.
            XDocument styles = null;

            // Open the document for read access and get a reference.
            using (var document = WordprocessingDocument.Open(fileName, false))
            {
                // Get a reference to the main document part.
                var docPart = document.MainDocumentPart;

                // Assign a reference to the appropriate part to the
                // stylesPart variable.
                StylesPart stylesPart = null;
                if (getStylesWithEffectsPart)
                    stylesPart = docPart.StylesWithEffectsPart;
                else
                    stylesPart = docPart.StyleDefinitionsPart;

                // If the part exists, read it into the XDocument.
                if (stylesPart != null)
                {
                    using (var reader = XmlNodeReader.Create(
                      stylesPart.GetStream(FileMode.Open, FileAccess.Read)))
                    {
                        // Create the XDocument.
                        styles = XDocument.Load(reader);
                    }
                }
            }
            // Return the XDocument instance.
            return styles;
        }
        #endregion  
    }
}
