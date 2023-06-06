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
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXMLHelper;
using Microsoft.Office.Interop.Word;

namespace ITCLib
{
    public class TopicContentReport : SurveyBasedReport
    {
        
        public bool ProductCrosstab { get; set; }
        public bool PlainFilters { get; set; }

        public TopicContentReport() : base()
        {
            ReportType = ReportTypes.Label;
            
        }

        public TopicContentReport(SurveyBasedReport sbr)
        {
            this.Surveys = sbr.Surveys;

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

        

        public int GenerateLabelReport()
        {

            // TODO use question filters (if specified) in each survey to get the desired subset of questions 
            foreach (ReportSurvey s in Surveys)
            {
                s.RemoveRepeatsTC();
            }

            // now we have each survey fully formed including topic and content labels, combine them into the final report
            if (ProductCrosstab)
                ReportTable = CreateTCPReport(new List<ReportSurvey>(Surveys));
            else 
                ReportTable = CreateTCReport(new List<ReportSurvey>(Surveys));

            DataView dv = ReportTable.DefaultView;
            dv.Sort = "SortBy ASC";
            ReportTable = dv.ToTable();
            ReportTable.Columns.Remove("SortBy");
            
            return 0;
        }

        
       #region Topic/Label Comparison
      
       /// <summary>
       /// Returns a DataTable containing all combinations of Topic and Content labels found in the survey list. Each question that appears under these 
       /// combinations is displayed under it's own survey heading. The table is sorted by the Qnum from the first survey and any labels not found in that 
       /// survey are collected at the bottom of the table.
       /// </summary>
       public System.Data.DataTable CreateTCReport(List<ReportSurvey> surveyList) {
            System.Data.DataTable report = new System.Data.DataTable();
            DataRow newrow;            
            string currentT;
            string currentC;
            string firstQnum = "";
            string otherFirstQnum = "";
            StringBuilder sb = new StringBuilder();
           
            List<SurveyQuestion> foundQs;
            ReportSurvey qnumSurvey = null;

            // start with a table containing all Topic/Content combinations present in the surveys
            report = CreateTCBaseTable(surveyList);

            foreach (ReportSurvey s in surveyList)
            {
                if (s.Qnum) qnumSurvey = s;
            }

            // for each T/C combination, add each survey's questions that match 
            // there should be one row for each T/C combo, so we need to concatenate all questions with that combo
            foreach (DataRow tc in report.Rows)
            {
                currentC = (string)tc["Info"];
                currentC = currentC.Substring(currentC.IndexOf("<em>") + 4, currentC.IndexOf("</em>") - currentC.IndexOf("<em>")-4);

                currentT = (string)tc["Info"];
                currentT = currentT.Substring(8, currentT.IndexOf("</strong>") - 8);               

                // now for each survey, add the questions that match the topic content pair
                foreach (ReportSurvey s in surveyList)
                {
                  
                    foundQs = s.Questions.Where(x => x.VarName.Topic.LabelText.Equals(currentT) && x.VarName.Content.LabelText.Equals(currentC)).ToList();
                  
                    foreach (SurveyQuestion sq in foundQs)
                    {
                        if (firstQnum.Equals(""))
                            firstQnum = sq.Qnum;

                        sb.Append("<strong>" + sq.Qnum + "</strong> (" + sq.VarName + ")\r\n");
                        sb.Append("[green]" + sq.GetFullVarLabel() + "[/green]" + "\r\n");
                        if (PlainFilters && !string.IsNullOrEmpty(sq.FilterDescription))
                            sb.Append(sq.FilterDescription + "\r\n");
                        sb.Append(sq.GetQuestionText(s.StdFieldsChosen, true) + "\r\n\r\n");
                    }

                    if (sb.Length>0) sb.Remove(sb.Length - 4, 4);

                    tc[s.SurveyCode] = sb.ToString(); 
                    if (s.Qnum)
                    {
                        tc["SortBy"] = firstQnum;
                        tc["Q#"] = firstQnum;
                    }
                    else
                    {

                        if (tc["SortBy"] == DBNull.Value || tc["SortBy"].Equals(""))
                        {
                             otherFirstQnum = GetFirstQnum(currentT, currentC, qnumSurvey);

                             if (otherFirstQnum.Equals("z"))
                                firstQnum = otherFirstQnum + firstQnum;
                            else
                                firstQnum = otherFirstQnum;

                            tc["SortBy"] = firstQnum;
                        }
                    }
                    sb.Clear();
                    firstQnum = "";
                }
                tc.AcceptChanges();
            }
            // add a row to start the section for unmatched labels (labels that do not exist in the Qnum survey)
            newrow = report.NewRow();
            newrow["Info"] = "<strong>Unmatches Labels</strong>";
            newrow["SortBy"] = "z000";
            report.Rows.Add(newrow);
            report.AcceptChanges();

            return report;
        }


        public System.Data.DataTable CreateTCPReport(List<ReportSurvey> surveyList)
        {
            System.Data.DataTable report = new System.Data.DataTable();
            DataRow newrow;
            string currentT;
            string currentC;
            string firstQnum = "";
            string otherFirstQnum = "";

            List<SurveyQuestion> foundQs;
            ReportSurvey qnumSurvey = null;

            // start with a table containing all Topic/Content combinations present in the surveys
            report = CreateTCPBaseTable(surveyList);

            foreach (ReportSurvey s in surveyList)
            {
                if (s.Qnum) qnumSurvey = s;
            }

            // for each T/C combination, add each survey's questions that match 
            // there should be one row for each T/C combo, so we need to concatenate all questions with that combo
            foreach (DataRow tc in report.Rows)
            {
                currentC = (string)tc["Info"];
                currentC = currentC.Substring(currentC.IndexOf("<em>") + 4, currentC.IndexOf("</em>") - currentC.IndexOf("<em>") - 4);

                currentT = (string)tc["Info"];
                currentT = currentT.Substring(8, currentT.IndexOf("</strong>") - 8);

                // now for each survey, add the questions that match the topic content pair
                foreach (ReportSurvey s in surveyList)
                {

                    foundQs = s.Questions.Where(x => x.VarName.Topic.LabelText.Equals(currentT) && x.VarName.Content.LabelText.Equals(currentC)).ToList();

                    foreach (SurveyQuestion sq in foundQs)
                    {
                        if (firstQnum.Equals(""))
                            firstQnum = sq.Qnum;

                        StringBuilder sb = new StringBuilder();

                        sb.Append("<strong>" + sq.Qnum + "</strong> (" + sq.VarName + ")\r\n");
                        sb.Append("[green]" + sq.GetFullVarLabel() + "[/green]" + "\r\n");
                        if (PlainFilters && !string.IsNullOrEmpty(sq.FilterDescription))
                            sb.Append(sq.FilterDescription + "\r\n");
                        sb.Append(sq.GetQuestionText(s.StdFieldsChosen, true) + "\r\n\r\n");
                        
                        tc[sq.VarName.Product.LabelText] = sb.ToString();
                    }

                    for (int i = 0;i < report.Columns.Count; i++)
                    {
                        tc[i] = Utilities.TrimString(tc[i].ToString(), "\r\n\r\n");
                    }          

                    if (s.Qnum)
                    {
                        tc["SortBy"] = firstQnum;
                        tc["Q#"] = firstQnum;
                    }
                    else
                    {
                        if (tc["SortBy"] == DBNull.Value || tc["SortBy"].Equals(""))
                        {
                            otherFirstQnum = GetFirstQnum(currentT, currentC, qnumSurvey);

                            if (otherFirstQnum.Equals("z"))
                                firstQnum = otherFirstQnum + firstQnum;
                            else
                                firstQnum = otherFirstQnum;

                            tc["SortBy"] = firstQnum;
                        }
                    }
                    firstQnum = "";
                }
                tc.AcceptChanges();
            }
            // add a row to start the section for unmatched labels (labels that do not exist in the Qnum survey)
            newrow = report.NewRow();
            newrow["Info"] = "<strong>Unmatches Labels</strong>";
            newrow["SortBy"] = "z000";
            report.Rows.Add(newrow);
            report.AcceptChanges();

            // remove N/A column
            report.Columns.Remove("N/A");
            for (int r = report.Rows.Count-1; r>=0; r--)
            {
  
                bool data = false;
                for (int i=3;i<report.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(report.Rows[r][i].ToString()))
                        data = true;
                }
                if (!data)
                    report.Rows.RemoveAt(r);
            }


            return report;
        }

        /// <summary>
        /// Get sortby by looking for the topic label and content label in the qnum survey and using that qnum
        /// if the content label isnt there, use the qnum for the last instance of the topic label, adding !00 to it
        /// if neither are there, use z
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="content"></param>
        /// <param name="qnumSurvey"></param>
        /// <returns>string. The first Qnum that contains the provided Topic and Content labels</returns>
        private string GetFirstQnum(string topic, string content, ReportSurvey qnumSurvey)
        {
            string firstQnum;
           
            List<SurveyQuestion> foundQs;
            
            foundQs = qnumSurvey.Questions.Where(x => x.VarName.Topic.LabelText.Equals(topic) && x.VarName.Content.LabelText.Equals(content)).ToList();
           
            if (foundQs.Count!=0)
            {
                firstQnum = foundQs[0].Qnum; 
            }
            else
            {
               
                foundQs = qnumSurvey.Questions.Where(x => x.VarName.Topic.LabelText.Equals(topic)).ToList();
               
                if (foundQs.Count!=0)
                {
                    firstQnum = foundQs[0].Qnum + "!00"; 
                }
                else
                {
                    firstQnum = "z";
                }
            }

            return firstQnum;
        }

        /// <summary>
        /// Create a DataTable that contains the all the Topic/Content combinations found in the list of surveys. A column for each survey is also created.
        /// </summary>
        private System.Data.DataTable CreateTCBaseTable(List<ReportSurvey> surveyList)
        {
            System.Data.DataTable report = new System.Data.DataTable();
           List<string> topicContent = new List<string>(); // list of all topic/content combinations
           string currentTC;
           DataRow newrow;
           report.Columns.Add(new DataColumn("Info", System.Type.GetType("System.String")));
           report.Columns.Add(new DataColumn("Q#", System.Type.GetType("System.String")));
           report.Columns.Add(new DataColumn("SortBy", System.Type.GetType("System.String")));

            

           foreach (ReportSurvey s in surveyList)
           {
                // add a column for the survey questions
               report.Columns.Add(new DataColumn(s.SurveyCode, System.Type.GetType("System.String")));
               // add a column for comments if specified
               if (s.CommentFields.Count > 0)
                    report.Columns.Add(new DataColumn(s.SurveyCode + " Comments", System.Type.GetType("System.String")));


                foreach (SurveyQuestion sq in s.Questions)              
                {
                    currentTC = "<strong>" + sq.VarName.Topic.LabelText + "</strong>\r\n<em>" + sq.VarName.Content.LabelText + "</em>";
                    if (!topicContent.Contains(currentTC))
                        topicContent.Add(currentTC);

                }
           }

           // now add each topic content pair to the table
           for (int i = 0; i < topicContent.Count; i++)
           {
               newrow = report.NewRow();
               newrow["Info"] = topicContent[i];
               report.Rows.Add(newrow);
               report.AcceptChanges();
           }

           return report;
        }

        /// <summary>
        /// Create a DataTable that contains the all the Topic/Content combinations found in the list of surveys. A column for each survey is also created.
        /// </summary>
        private System.Data.DataTable CreateTCPBaseTable(List<ReportSurvey> surveyList)
        {
            System.Data.DataTable report = new System.Data.DataTable();
            List<string> topicContent = new List<string>(); // list of all topic/content combinations
            string currentTC;
            DataRow newrow;
            report.Columns.Add(new DataColumn("Info", System.Type.GetType("System.String")));
            report.Columns.Add(new DataColumn("Q#", System.Type.GetType("System.String")));
            report.Columns.Add(new DataColumn("SortBy", System.Type.GetType("System.String")));

            

            foreach (ReportSurvey s in surveyList)
            {
                foreach (SurveyQuestion sq in s.Questions)
                {
                    currentTC = "<strong>" + sq.VarName.Topic.LabelText + "</strong>\r\n<em>" + sq.VarName.Content.LabelText + "</em>";
                    if (!topicContent.Contains(currentTC))
                        topicContent.Add(currentTC);

                }
            }

            // get all the products that appear in the surveys and add a heading for each one
            List<string> products = new List<string>();
            foreach (ReportSurvey rs in surveyList)
            {
                var labels = rs.Questions.Select(x => x.VarName.Product.LabelText).ToList<string>();
                labels.Sort();
                foreach (string s in labels)
                {
                    if (!products.Contains(s))
                    { 
                        products.Add(s);
                        report.Columns.Add(new DataColumn(s, System.Type.GetType("System.String")));
                    }
                
                }
            }

            // add blank comment column if needed
            if (LayoutOptions.BlankColumn)
                report.Columns.Add(new DataColumn("Comments", System.Type.GetType("System.String")));

            // now add each topic content pair to the table
            for (int i = 0; i < topicContent.Count; i++)
            {
                newrow = report.NewRow();
                newrow["Info"] = topicContent[i];
                report.Rows.Add(newrow);
                report.AcceptChanges();
            }

            return report;
        }
        #endregion

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

            if (ProductCrosstab)
                appWord.ActiveWindow.View.Type = WdViewType.wdWebView;

            docReport.Range(0, 0).Delete(); // delete the extra paragraph that word will insert upon opening the file (bookmark related?)

            ReportStatus = "Formatting report...";

            // disable spelling and grammar checks (useful for foreign languages)
            appWord.Options.CheckSpellingAsYouType = false;
            appWord.Options.CheckGrammarAsYouType = false;

            // footer text            
            docReport.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                "\t\t" + "Generated on " + DateTime.Today.ShortDateDash());
            
            ReportStatus = "Interpreting formatting tags...";
            // interpret formatting tags
            Formatting.FormatTags(appWord, docReport, false);

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

            DocumentFormat.OpenXml.Wordprocessing.Document document = (DocumentFormat.OpenXml.Wordprocessing.Document)wd.body.Parent;

            var settingsPart = document.MainDocumentPart.DocumentSettingsPart;//  AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings { BordersDoNotSurroundFooter = new BordersDoNotSurroundFooter() { Val = true } };

            settingsPart.Settings.Append(new UpdateFieldsOnOpen() { Val = true });

            // create title of report
            string titleText = ReportTitle();

            // include filter info if specified
            titleText += "\r\n" + FilterLegend();

            titleText = Utilities.TrimString(titleText, "\r\n");

            wd.AddTitleParagraph(titleText);

            // add survey content
            MakeSurveyContentTable(wd);

            // add bulleted list numbering
            XMLUtilities.AddBulletNumbering(wd.doc.MainDocumentPart);
            // format any bulleted lists in the doc
            XMLUtilities.FormatBullets(wd.body, 1);

            wd.Close();

        }

        private void MakeSurveyContentTable(WordDocumentMaker wd)
        {
            // create the table with the main content of the report
            DocumentFormat.OpenXml.Wordprocessing.Table table = wd.AddTable(ReportTable);

           
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

            // format the column widths
            ReportStatus = "Formatting column widths...";
            if (ProductCrosstab)
                FormatColumnsXMLWebView(table);
            else 
                FormatColumnsXML(table);                         

            // remove space after paragraphs
            foreach (ParagraphProperties pPr in table.Descendants<ParagraphProperties>())
            {
                pPr.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            }

            DocumentFormat.OpenXml.Wordprocessing.Paragraph lastPara = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            lastPara.Append(new ParagraphProperties(XMLUtilities.LandscapeSectionProps()));

            wd.body.Append(lastPara);
        }

        /// <summary>
        /// Format's the column names and properties of the first row in the table.
        /// </summary>
        /// <param name="table"></param>
        private void FormatHeaderRow(DocumentFormat.OpenXml.Wordprocessing.Table table)
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
                foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph p in cell.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
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
               
                // question column with date, format date
                if (header.Contains(DateTime.Today.ShortDate()))
                {
                    cell.SetCellText(header.Replace(DateTime.Today.ShortDate(), ""));
                }
                       
                
            }
        }

        /// <summary>
        /// Resizes the widths of all columns to fit on the page. Columns other than Qnum and VarName are evenly sized.
        /// </summary>
        /// <param name="table"></param>
        private void FormatColumnsXML(DocumentFormat.OpenXml.Wordprocessing.Table table)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float infoWidth = 1.2f;
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
                    case "Qnum":
                        columns.ElementAt(i).Width = Convert.ToString(qnumWidth * 1440);
                        widthLeft -= qnumWidth;
                        setCount.Add(i);
                        break;
                    case "Info":
                        columns.ElementAt(i).Width = Convert.ToString(infoWidth * 1440);
                        widthLeft -= infoWidth;
                        setCount.Add(i);
                        break;
                }
            }

            // the remaining width evenly divided amongst the remaining columns
            dividedWidth = widthLeft / (numCols - setCount.Count);

            // if the column index is not in the "set" collection of column indices, set it to a share of the remaining width
            for (int i = 0; i < headerCells.Count(); i++)
            {
                if (!setCount.Contains(i))
                {
                    columns.ElementAt(i).Width = Convert.ToString(dividedWidth * 1440);
                }
            }

        }

        /// <summary>
        /// Resizes the widths of all columns to fit on the page. Columns other than Qnum and VarName are evenly sized.
        /// </summary>
        /// <param name="table"></param>
        private void FormatColumnsXMLWebView(DocumentFormat.OpenXml.Wordprocessing.Table table)
        {

            float qnumWidth = 0.51f;
            float infoWidth = 1.2f;
            float questionWidth = 3.5f;
  
            string header;

            TableGrid grid = table.Elements<TableGrid>().ElementAt(0);
            var columns = grid.Elements<GridColumn>();
            var headerRow = table.Elements<TableRow>().ElementAt<TableRow>(0);
            var headerCells = headerRow.Elements<TableCell>();

            for (int i = 0; i < headerCells.Count(); i++)
            {
                header = headerCells.ElementAt(i).GetCellText();

                switch (header)
                {
                    case "Q#":
                    case "Qnum":
                        columns.ElementAt(i).Width = Convert.ToString(qnumWidth * 1440);
          
     
                        break;
                    case "Info":
                        columns.ElementAt(i).Width = Convert.ToString(infoWidth * 1440);
                 
                       
                        break;
                    default:
                        columns.ElementAt(i).Width = Convert.ToString(questionWidth * 1440);
                        break;
                }
            }

            

        }

        /// <summary>
        /// Format the header row so with the appropriate widths and titles
        /// </summary>
        /// <param name="doc"></param>
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
                    case "Response":
                        doc.Tables[1].Columns[i].Width = respWidth * 72;
                        widthLeft -= respWidth;
                        break;
                    case "Info":
                        doc.Tables[1].Columns[i].Width = tcWidth * 72;
                        widthLeft -= tcWidth;
                        break;
                    case "SortBy":
                        doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                        widthLeft -= qnumWidth;
                        break;
                    case "Comments":
                        doc.Tables[1].Columns[i].Width = commentWidth * 72;
                        widthLeft -= commentWidth;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReportFileName()
        {
            string finalfilename;
            string surveyCodes = "";
           

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ShortDate(); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            finalfilename = "Topic Content Report - " + surveyCodes;
            if (Details != "") { finalfilename += ", " + Details; }
            if (!Batch) { finalfilename += " generated"; }

            return finalfilename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addDate"></param>
        /// <returns>String</returns>
        public string ReportTitle(bool addDate = false)
        {
            string title = "Topic/Content Report - ";
            string surveyCodes = "";

            if (Surveys.Count == 1)
            {
                title = "Topic/Content Report - " + Surveys[0].SurveyCode;
                if (Surveys[0].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[0].Backend.ShortDate(); }
                return title;
            }

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ShortDate(); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            title += surveyCodes;
            if (addDate) { title += ", Generated on " + DateTime.Today.ShortDate(); }

            return title;
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
    }
}
