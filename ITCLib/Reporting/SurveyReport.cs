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
using OpenXMLExtensions;

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

        private int GetColumnNumber(string columnName)
        {
            foreach (ReportColumn rc in ColumnOrder)
            {
                if (rc.ColumnName == columnName)
                    return rc.Ordinal;
            }
            return -1;
        }

        private int QnumColumn()
        {
            return GetColumnNumber("Qnum") -1;
        }
        private int VarNameColumn()
        {
            return GetColumnNumber("VarName") - 1;
        }
        private int FirstSurveyColumn()
        {
            foreach (ReportColumn rc in ColumnOrder)
            {
                if (rc.ColumnName.StartsWith(Surveys[0].SurveyCode))
                    return rc.Ordinal-1;
            }
            return -1;
        }


        #region ISR

        /// <summary>
        /// Creates a survey report in standard form. Standard form displays one or more surveys in Qnum order. Additional surveys are matched up
        /// based on refVarName and have their wordings compared to each other.
        /// </summary>
        /// <returns>0 for success, 1 for failure.</returns>
        public int GenerateReport()
        {
            FinalSurveyTables.Clear();


            // perform comparisons
            if (Surveys.Count > 1 && CompareWordings)
            {
                ReportStatus = "Running comparisons...";
                DoComparisons();
            }

            ReportStatus = "Creating survey table(s)...";
            // create final tables
            CreateFinalSurveyTables();

            ReportStatus = "Creating final report table...";
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



        #region XML Table methods
        /// <summary>
        /// Create an OpenXML document and add content and formatting.
        /// </summary>
        /// <param name="filePath"></param>
        private void CreateXMLDoc(string filePath)
        {
            WordDocumentMaker wd = new WordDocumentMaker(filePath);


            string titleText = ReportTitle();
            if (Surveys.Count > 1)
                titleText += "\r\n" + HighlightingKey();

            titleText += "\r\n" + FilterLegend();

            titleText = Utilities.TrimString(titleText, "\r\n");
            titleText += "\r\n";

            wd.AddTitleParagraph(titleText);
            Table table = wd.AddTable(ReportTable);


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
                rPr.PrependChild(new RunFonts() { Ascii = "Verdana" });
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

            // format section headings
            if (ReportType == ReportTypes.Standard)
                FormatSectionHeadings(table, ShowAllVarNames, ShowAllQnums, ColorSubs);

            // remove space after paragraphs
            foreach (ParagraphProperties pPr in table.Descendants<ParagraphProperties>())
            {
                pPr.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            }

            wd.Close();

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
            

            ReportStatus = "Inserting subset tables...";
            LayoutOptions.FormatSubTables(table, qnumCol, varCol, wordCol);

        }

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

                    pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "Heading1" };
                   
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
                        tcPr.Append(SkyBlueShading());
                    }
                    else if (varname.StartsWith("Z"))
                    {
                        //    Word.WdColor.wdColorRose;
                        tcPr.Append(RoseShading());
                    }
                }
            }
        }

        private void AddTableFormatColumns(Table table)
        {
            // we need to add grid columns to the table based on the number of response options in the subset questions
            List<SurveyQuestion> tfq = Surveys[0].Questions.Where(x => x.TableFormat == true).ToList();
            int most = 0;
            foreach (SurveyQuestion q in tfq)
            {
                int respCount = q.GetRespNumbers().Count();
                if (respCount > most)
                    most = respCount;
            }

            for (int i = 0; i < most; i++)
            {
                table.Elements<TableGrid>().ElementAt(0).Append(new GridColumn());
            }
        }

        private TableCellBorders BlackSingleBorder()
        {
            return new TableCellBorders(
                 new TopBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new BottomBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new LeftBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new RightBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new InsideHorizontalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new InsideVerticalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 });
        }

        private Shading RoseShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "ffa8d4" };
        }

        private Shading SkyBlueShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "6de2fc" };
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

            if (RepeatedHeadings)
                trPr.Append(new TableHeader());

            firstRow.Append(trPr);

            foreach (TableCell cell in firstRowCells)
            {

                TableCellProperties tcPr = new TableCellProperties(BlackSingleBorder());

                tcPr.Append(RoseShading());

                cell.PrependChild(tcPr);

                foreach (Paragraph p in cell.Descendants<Paragraph>())
                {
                    ParagraphProperties prPr = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                    p.PrependChild(prPr);


                    foreach (Run run in p.Descendants<Run>())
                    {
                        RunProperties rPr = new RunProperties();
                        rPr.Append(new Bold());
                        run.PrependChild(rPr);


                    }
                }

                // fix text
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
                        if (header.Contains(DateTime.Today.ToString("d").Replace("-", "")))
                        {
                            cell.SetCellText(header.Replace(DateTime.Today.ToString("d"), ""));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Resizes the widths of all columns to fit on the page. Columns other than Qnum and VarName are evenly sized.
        /// </summary>
        /// <param name="table"></param>
        public void FormatColumnsXML(Table table)
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
        #endregion

        public void OutputReportTableXML()
        {
            Word.Application appWord;   // instance of MSWord
            Word.Document docReport;
            Word.Table surveyTable;     // the table in the document containing the survey(s)

            string templatePath;

            ReportStatus = "Outputting report...";

            if (Batch)
                if (Surveys[0].FilterCol)
                    FileName += ReportFileName() + ", with filters, " + DateTime.Today.ToString("d").Replace("-", "");
                else
                    FileName += ReportFileName() + ", " + DateTime.Today.ToString("d").Replace("-", "");
            else
                FileName += ReportFileName() + ", " + DateTime.Today.ToString("d").Replace("-", "") + " (" + DateTime.Now.ToString("hh.mm.ss") + ")";

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

            // now open the file and add content to it using OpenXML, save it and close it
            CreateXMLDoc(FileName);

            // open it again in word to set all the formatting options
            docReport = appWord.Documents.Open(FileName);
            
            ReportStatus = "Formatting report...";

            // disable spelling and grammar checks (useful for foreign languages)
            appWord.Options.CheckSpellingAsYouType = false;
            appWord.Options.CheckGrammarAsYouType = false;
            
            surveyTable = docReport.Tables[1];

            // table style
            surveyTable.Rows.Alignment = Word.WdRowAlignment.wdAlignRowLeft;

            // footer text
            docReport.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                "\t\t" + "Generated on " + DateTime.Today.ToString("d"));
            
            // create TOC
            if (LayoutOptions.ToC != TableOfContents.None)
            {
                ReportStatus = "Creating table of contents...";
                MakeToC(docReport);
            }

            // create title page
            if (LayoutOptions.CoverPage)
            {
                ReportStatus = "Creating title page...";
                MakeTitlePage(docReport);
            }
           
            // update TOC due to formatting changes (see if the section headings can be done first, then the TOC could update itself)
            if (LayoutOptions.ToC == TableOfContents.PageNums && docReport.TablesOfContents.Count > 0) docReport.TablesOfContents[1].Update();

            // add survey notes appendix
            if (SurvNotes)
            {
                ReportStatus = "Creating survey notes appendix...";
                MakeSurveyNotesAppendix(docReport);
            }

            // add varname changes appendix
            if (VarChangesApp)
            {
                ReportStatus = "Creating VarName changes appendix...";
                MakeVarChangesAppendix(docReport);
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

        
    

        public override void FormatColumns(Word.Document doc)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float varWidth = 0.9f;
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
            t.Rows[1].Cells[1].Range.InlineShapes.AddPicture(Properties.Resources.logoPath, false, true);
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
