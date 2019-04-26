using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.ComponentModel;


namespace ITCLib
{
    /// <summary>
    /// This class represents a report outputs one or more surveys to Word.
    /// </summary>
    /// <remarks>If 2+ surveys, they are matched up by refVarName, with one survey's Qnums defining the order.</remarks>
    public class SurveyReport : SurveyBasedReport
    {
        #region Survey Report Properties

        public List<DataTable> FinalSurveyTables { get; set; }
        public DataTable qnumSurveyTable;

        // comparison class
        public Comparison SurveyCompare { get; set; }
        
        // other options
        bool CheckOrder { get; set; }
        bool CheckTables { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SurveyReport object.
        /// </summary>
        public SurveyReport() :base() {


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

            // intialize the column order collection with the default columns
            ColumnOrder = new List<ReportColumn>
            {
                new ReportColumn("Qnum", 1),
                new ReportColumn("VarName", 2)
            };

            // comparison options
            SurveyCompare = new Comparison();

            this.Surveys = sbr.Surveys;

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

            this.RepeatedHeadings = sbr.RepeatedHeadings;
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

        //public void AddCommentField(ReportSurvey s, string commentType)
        //{
        //    bool addColumn = s.CommentFields.Count == 0;

        //    foreach (ReportSurvey rs in Surveys)
        //    {
        //        if (rs == s && !s.CommentFields.Contains(commentType))
        //            s.CommentFields.Add(commentType);
        //    }

        //    if (addColumn)
        //        AddColumn(s.SurveyCode + " " + s.Backend.ToString("d") + " Comments");


        //}

        //public void RemoveCommentField(ReportSurvey s)
        //{
        //    foreach (ReportSurvey rs in Surveys)
        //    {
        //        if (rs == s)
        //        {
                    
        //        }
                    
        //    }
        //}


        #region ISR
     
        /// <summary>
        /// Creates a survey report in standard form. Standard form displays one or more surveys in Qnum order. Additional surveys are matched up
        /// based on refVarName and have their wordings compared to each other.
        /// </summary>
        /// <returns>0 for success, 1 for failure.</returns>
        public int GenerateReport()
        {


            // perform comparisons
            if (Surveys.Count > 1 && CompareWordings)
            {
                DoComparisons();
            }

            // create final tables
            CreateFinalSurveyTables();


            // compile final tables into report table
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

            // remove primary if chosen TODO Test with back dates
            if (SurveyCompare.HidePrimary)
            {
                foreach (ReportSurvey s in Surveys)
                {
                    if (s.Primary)
                        ReportTable.Columns.Remove(s.SurveyCode);
                }   
            }

            ReportTable.PrimaryKey = new DataColumn[] { ReportTable.Columns["VarName"] };
            ReportTable.Columns.Remove("refVarName");

            // ensure that the first 2-3 columns are in the right order
            if (Numbering == Enumeration.AltQnum)
            {
                ReportTable.Columns["AltQnum"].SetOrdinal(0);
                ReportTable.Columns["VarName"].SetOrdinal(1);

            }
            else if (Numbering == Enumeration.Qnum)
            {
                ReportTable.Columns["Qnum"].SetOrdinal(0);
                ReportTable.Columns["VarName"].SetOrdinal(1);
            }
            else
            {
                ReportTable.Columns["Qnum"].SetOrdinal(0);
                ReportTable.Columns["AltQnum"].SetOrdinal(1);
                ReportTable.Columns["VarName"].SetOrdinal(2);
            }

            if (LayoutOptions.BlankColumn)
                ReportTable.Columns.Add(new DataColumn("Comments", Type.GetType("System.String")));


            // TODO set the column order as defined by the ColumnOrder property


            // at this point the reportTable should be exactly how we want it to appear, minus interpreting tags

            // sort the report
            DataView dv = ReportTable.DefaultView;
            dv.Sort = "SortBy ASC";
            ReportTable = dv.ToTable();
            ReportTable.Columns.Remove("SortBy");

            return 0;
        }

        
        ///<summary>
        ///Exports the final report DataTable to Word. The table is formatted in Word, including headings, colors, formatting tags like bold, italics, etc.
        ///
        ///</summary>
        public void OutputReportTable()
        {

            Word.Application appWord;   // instance of MSWord
            Word.Document docReport;    // the report document
            Word.Table surveyTable;     // the table in the document containing the survey(s)

            int rowCount = ReportTable.Rows.Count;          // number of rows in the survey table
            int columnCount = ReportTable.Columns.Count;    // number of columns in the survey table
            int clearCols; // the number of columns that should have their contents cleared, for headings

            // create the instance of Word
            appWord = new Word.Application
            {
                Visible = false
            };
            // disable spelling and grammar checks (useful for foreign languages)
            appWord.Options.CheckSpellingAsYouType = false;
            appWord.Options.CheckGrammarAsYouType = false;

            // create the document
            //  TODO store template path somewhere
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter:
                    docReport = appWord.Documents.Add("\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\SMGLandLet.dotx");
                    break;
                case PaperSizes.Legal:
                    docReport = appWord.Documents.Add("\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\SMGLandLeg.dotx");
                    break;
                case PaperSizes.Eleven17:
                    docReport = appWord.Documents.Add("\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\SMGLand11.dotx");
                    break;
                case PaperSizes.A4:
                    docReport = appWord.Documents.Add("\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\SMGLandA4.dotx");
                    break;
                default:
                    docReport = appWord.Documents.Add("\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\SMGLandLet.dotx");
                    break;
            }
            // add a table
            surveyTable = docReport.Tables.Add(docReport.Range(0, 0), rowCount + 1, columnCount);

            // fill header row
            for (int c = 1; c <= columnCount; c++)
            {
                surveyTable.Cell(1, c).Range.Text = ReportTable.Columns[c - 1].Caption;
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
            docReport.Application.Selection.Text = ReportTitle();
            // add highlighting key if more than 1 survey (ie a comparison)
            if (Surveys.Count > 1)
            {
                docReport.Application.Selection.Text = docReport.Application.Selection.Text + "\r\n" + HighlightingKey();
            }
            docReport.Application.Selection.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // if there are filters, add a description of the filter
            docReport.Application.Selection.Text = "\r\n" + FilterLegend();
            docReport.Application.Selection.Font.Size = 12;

            // footer text
            docReport.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                "\t\t" + "Generated on " + DateTime.Today.ToString("d"));

            //
            docReport.Paragraphs.SpaceAfter = 0;

            // format column names and widths
            FormatColumns(docReport);

            // TODO format subset tables (TO TEST)
            if (SubsetTables && Numbering == Enumeration.Qnum && ReportType == ReportTypes.Standard)
            {
                //appWord.Visible = true;
                LayoutOptions.FormatTables(docReport, SubsetTablesTranslation);
            }

            // create TOC
            if (LayoutOptions.ToC != TableOfContents.None) { MakeToC(docReport); }

            // create title page
            if (LayoutOptions.CoverPage) { MakeTitlePage(docReport); }

            // format section headings
            if (ReportType == ReportTypes.Standard)
            {
                // process headings
                Formatting.FormatHeadings(docReport, (int)Numbering, ShowAllVarNames, ShowAllQnums, ColorSubs);
            }

            // update TOC due to formatting changes (see if the section headings can be done first, then the TOC could update itself)
            if (LayoutOptions.ToC == TableOfContents.PageNums && docReport.TablesOfContents.Count > 0) { docReport.TablesOfContents[1].Update(); }

            // add survey notes appendix
            if (SurvNotes) { MakeSurveyNotesAppendix(docReport); }

            // add varname changes appendix
            if (VarChangesApp) { MakeVarChangesAppendix(docReport); }

            // interpret formatting tags
            Formatting.FormatTags(appWord, docReport, SurveyCompare.Highlight);

            // TODO convert TC tags into real tracked changes
            if (SurveyCompare.ConvertTrackedChanges) { Formatting.ConvertTC(docReport); }

            // TODO format shading for order comparisons
            if (ReportType == ReportTypes.Order) { Formatting.FormatShading(docReport); }

            FileName += ReportFileName() + ", " + DateTime.Today.ToString("d").Replace("-", "") + " (" + DateTime.Now.ToString("hh.mm.ss") + ")";
            FileName += ".doc";

            //save the file
            docReport.SaveAs2(FileName);




            // close the document and word if this is an automatic survey
            if (Batch)
            {
                if (LayoutOptions.FileFormat == FileFormats.PDF)
                {
                    docReport.ExportAsFixedFormat(FileName.Replace(".doc", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, true,
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
                        docReport.ExportAsFixedFormat(FileName.Replace(".doc", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, true,
                            Word.WdExportOptimizeFor.wdExportOptimizeForPrint, Word.WdExportRange.wdExportAllDocument, 1, 1,
                            Word.WdExportItem.wdExportDocumentContent, true, true, Word.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false);
                    }
                    catch (Exception)
                    {
                        // TODO handle the error (PDF converter not installed, or file in use
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

        }

        public override void FormatColumns(Word.Document doc)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float varWidth = 0.9f;
            float tcWidth = 1.2f;
            float respWidth = 0.86f;
            float commentWidth = 1f;
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
                        if (header.Contains(DateTime.Today.ToString("d").Replace("-", "")))
                        {
                            doc.Tables[1].Rows[1].Cells[i].Range.Text = doc.Tables[1].Rows[1].Cells[i].Range.Text.Replace(DateTime.Today.ToString("d"), "");
                            
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

                        // filter column
                        if (header.Contains("Filters"))
                        {
                            // TODO set to Verdana 9 font
                        }

                        break;
                }

            }

            for (int i = qCol; i <= numCols; i ++)
                doc.Tables[1].Columns[i].Width = (float)(widthLeft / (numCols - qCol + 1)) * 72;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReportFileName()
        {
            string finalfilename = "";
            string surveyCodes = "";
            if (Web)
            {
                return Surveys[0].WebName;
            }

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ToString("d"); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            finalfilename = surveyCodes;
            if (Details != "") { finalfilename += ", " + Details; }
            if (!Batch) { finalfilename += " generated"; }

            return finalfilename;
        }

        /// <summary>
        /// Creates a new section at the beginning of the document. Adds a table containing the ITC logo, the survey title and additional information
        ///about the survey.
        /// </summary>
        /// <param name="doc"></param>
        public void MakeTitlePage(Word.Document doc)
        {
            Word.Table t;
            ReportSurvey s = PrimarySurvey();
            // create new section
            doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            doc.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;

            doc.Sections[1].PageSetup.VerticalAlignment = Word.WdVerticalAlignment.wdAlignVerticalCenter;
            doc.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = "";

            // create a table
            t = doc.Tables.Add(doc.Range(0, 0), 6, 1, Word.WdDefaultTableBehavior.wdWord8TableBehavior, Word.WdAutoFitBehavior.wdAutoFitContent);

            t.Range.Font.Name = "Verdana";
            t.Range.Font.Size = 18;
            t.Rows.VerticalPosition = 1.8f;
            // add info to table
            // TODO see if the resource file can be used here
            t.Rows[1].Cells[1].Range.InlineShapes.AddPicture(@"\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\logo.JPG", false, true);
            t.Rows[2].Cells[1].Range.Text = s.Title;
            t.Rows[3].Cells[1].Range.Text = "Survey Code: " + s.SurveyCode;
            t.Rows[4].Cells[1].Range.Text = "Languages: " + s.Languages;
            t.Rows[5].Cells[1].Range.Text = "Mode: " + s.Mode.ModeAbbrev;
            t.Rows[6].Cells[1].Range.Text = s.Group.UserGroup.Equals("") ? "(" + s.Group.UserGroup + ")" : "";
            // format table
            t.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
            t.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
            t.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

        }

        /// <summary>
        /// Creates a table at the end of the report that contains all Survey Notes and Wave notes associated with the surveys appearing in the report.
        /// </summary>
        /// <param name="doc"></param>
        public void MakeSurveyNotesAppendix(Word.Document doc)
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
                t.Cell(i + 2, 2).Range.Text = surveyNotes[i].Notes;
                t.Cell(i + 2, 3).Range.Text = surveyNotes[i].Name;
                t.Rows[i + 2].Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            }
            
            t.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
        }

        /// <summary>
        /// Creates a table at the end of the report that contains all VarName changes related to the surveys appearing the report.
        /// </summary>
        /// <param name="doc"></param>
        public void MakeVarChangesAppendix(Word.Document doc)
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

            for (int i = 0;i < changes.Count; i ++)
            {
                t.Cell(i + 2, 1).Range.Text = changes[i].NewName.VarName;
                t.Cell(i + 2, 2).Range.Text = changes[i].OldName.VarName;
                t.Cell(i + 2, 3).Range.Text = changes[i].ChangeDate.ToString(); 
                t.Cell(i + 2, 4).Range.Text = changes[i].GetSurveys(); 
                t.Cell(i + 2, 5).Range.Text = changes[i].ChangedBy.Name;
                t.Cell(i + 2, 6).Range.Text = changes[i].Rationale;

                t.Rows[i + 2].Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            }

            t.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
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

            if (ReportType == ReportTypes.Standard)
            {
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

            }
            
            return finalKey;
        }

        /// <summary>
        /// Creates a new section at the top of the document. Adds a table of contens in 1 of 2 ways. Either a TableOfContents object is created and 
        /// based on the headings in the document, or the text and Qnums for each heading are listed in a table.
        /// </summary>
        /// <param name="doc">Document object</param>
        public void MakeToC(Word.Document doc)
        {
            // exit if no headings found
            if (QnumSurvey().Questions.Count(x => x.VarName.StartsWith("Z")) == 0)
                return;
            
            DataRow[] headingRows;
            string[,] headings;

            object missingType = Type.Missing;
            switch (LayoutOptions.ToC)
            {
                case TableOfContents.None:
                    break;
                case TableOfContents.Qnums:

                    headingRows = qnumSurveyTable.Select("VarName Like 'Z%'");
                    headings = new string[headingRows.Length, 2];

                    for (int i = 0; i < headingRows.Length; i++)
                    {
                        headings[i, 0] = (string)headingRows[i]["PreP"];
                        headings[i, 1] = (string)headingRows[i]["SortBy"];
                        headings[i, 1] = headings[i, 1].Substring(0, 3);
                    }
                    // create new section in document
                    doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
                    // create table of contents
                    doc.Tables.Add(doc.Range(0, 0), headings.GetUpperBound(0) + 1, 2, Word.WdDefaultTableBehavior.wdWord8TableBehavior, Word.WdAutoFitBehavior.wdAutoFitContent);
                    // format table
                    doc.Sections[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    doc.Sections[1].Range.Font.Name = "Cambria (Headings)";
                    doc.Sections[1].Range.Font.Size = 12;

                    // fill table
                    doc.Tables[1].Cell(1, 1).Range.Text = "TABLE OF CONTENTS";
                    doc.Tables[1].Cell(1, 1).Range.Font.Bold = -1;
                    for (int i = 0; i < headings.GetUpperBound(0); i++)
                    {
                        doc.Tables[1].Cell(i + 2, 1).Range.Text = headings[i, 0];
                        doc.Tables[1].Cell(i + 2, 2).Range.Text = headings[i, 1];
                        doc.Tables[1].Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                    }
                    doc.Tables[1].Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                    doc.Tables[1].Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;

                    break;
                case TableOfContents.PageNums:
                    // create new section in document
                    doc.Range(0, 0).InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
                    doc.TablesOfContents.Add(doc.Range(0, 0), true, 1, 3, false, missingType, missingType, missingType, missingType, true);
                    break;
            }

        }

       
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

        // TODO 
        public void IncludeOrderChanges() { }

       

        // TODO 
        public void MarkOrderChanges() { }
  
        public string GetReInsertedComments() { return ""; }

        public void SetColumnOrder() { }

        public void RemoveDuplicateColums() { }

        public void StatusUpdate() { }

        public bool IsCompleteTF() { return true; }

        public bool IsCompleteSurvey() { return true; }

        

        // may be unneeded after a server function returns comments
        public void RemoveRepeatedComments() { }

        #endregion

    }
}
