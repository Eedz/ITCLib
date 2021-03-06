﻿using System;
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
    public class TopicContentReport : SurveyBasedReport
    {
        
         
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
            ReportTable = CreateTCReport(Surveys);

            DataView dv = ReportTable.DefaultView;
            dv.Sort = "SortBy ASC";
            ReportTable = dv.ToTable();
            ReportTable.Columns.Remove("SortBy");
            
            return 0;
        }

        
       #region Topic/Label Comparison
      
       // TODO create method for getting SortBy (even for Qnum survey)


       /// <summary>
       /// Returns a DataTable containing all combinations of Topic and Content labels found in the survey list. Each question that appears under these 
       /// combinations is displayed under it's own survey heading. The table is sorted by the Qnum from the first survey and any labels not found in that 
       /// survey are collected at the bottom of the table.
       /// </summary>
       public DataTable CreateTCReport(BindingList<ReportSurvey> surveyList) {
            DataTable report = new DataTable();
            DataRow newrow;            
            string currentT;
            string currentC;
            string qs = "";
            string firstQnum = "";
            string otherFirstQnum = "";
           
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
                           

                        qs += "<strong>" + sq.Qnum + "</strong> (" + sq.VarName + ")" + "\r\n" + sq.GetQuestionText(s.StdFieldsChosen, true) + "\r\n\r\n"; 
                    }

                    qs = Utilities.TrimString(qs, "\r\n\r\n");
                    tc[s.SurveyCode] = qs;
                    if (s.Qnum)
                    {
                        tc["SortBy"] = firstQnum;
                        tc["Qnum"] = firstQnum;
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
                    qs = "";
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
        private DataTable CreateTCBaseTable(BindingList<ReportSurvey> surveyList)
        {
           DataTable report = new DataTable();
           List<string> topicContent = new List<string>(); // list of all topic/content combinations
           string currentTC;
           DataRow newrow;
           report.Columns.Add(new DataColumn("Info", System.Type.GetType("System.String")));
           report.Columns.Add(new DataColumn("Qnum", System.Type.GetType("System.String")));
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
        #endregion

        public void OutputReportTable()
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
            surveyTable.Rows[1].HeadingFormat = -1;
            

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

            // interpret formatting tags
            Formatting.FormatTags(appWord, docReport, false);

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

            for (int i = qCol; i <= numCols; i++)
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
        /// 
        /// </summary>
        /// <param name="addDate"></param>
        /// <returns>String</returns>
        public new string ReportTitle(bool addDate = false)
        {
            string title = "";
            string surveyCodes = "";

            if (Surveys.Count == 1)
            {
                title = Surveys[0].Title;
                if (Surveys[0].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[0].Backend.ToString(); }
                return title;
            }

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ToString(); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            title += surveyCodes;
            if (addDate) { title += ", Generated on " + DateTime.Today.ToString("d").Replace("-", ""); }

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
