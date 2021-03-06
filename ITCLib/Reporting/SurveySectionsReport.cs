﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    // TODO this could be a special case of Survey Report, with the option to match on VarName the default, and an extra column showing first/last varnames

    // TODO include first/last varnames
    // TODO need to accomodate several surveys (special comparison?)
    // either side by side or join on varnames
    public class SurveySectionsReport : SurveyBasedReport
    {

        public int GenerateSectionsReport()
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < Surveys.Count; i++)
            {
                Surveys[i].StdFieldsChosen = new List<string> { "PreP" };
                dt = MakeFinalTable(Surveys[i]);

            }
            
            dt = dt.Select("VarName LIKE 'Z%'").CopyToDataTable();

            ReportTable = new DataView(dt).ToTable(false, new string[] { "Qnum", "VarName", GetQuestionColumnName(Surveys[0]) });

            // sort the report
            DataView dv = ReportTable.DefaultView;
            dv.Sort = "Qnum ASC";
            ReportTable = dv.ToTable();
            // if Qnum not included, remove it
            ReportTable.Columns.Remove("Qnum");

            OutputSectionsReport();

            return 0;
        }

        #region Word Interop Methods

        public void OutputSectionsReport()
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
            // add highlighting key if more than 1 survey (ie a comparison)
            //if (Surveys.Count > 1)
            //{
            //    docReport.Application.Selection.Text = docReport.Application.Selection.Text + "\r\n" + HighlightingKey();
            //}
            docReport.Application.Selection.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // if there are filters, add a description of the filter
            //docReport.Application.Selection.Text = "\r\n" + FilterLegend();
            //docReport.Application.Selection.Font.Size = 12;

            // footer text
            docReport.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter("\t" + ReportTitle() +
                "\t\t" + "Generated on " + DateTime.Today.ToString("d"));

            //
            docReport.Paragraphs.SpaceAfter = 0;

            // format column names and widths
            FormatColumns(docReport);

            // format section headings
            if (ReportType == ReportTypes.Standard)
            {
                // process headings
                //Formatting.FormatHeadings(docReport, (int)Numbering, true, ShowAllQnums, ColorSubs);
            }

            Formatting.FormatTags(appWord, docReport, false);

            FileName += ReportFileName() + ", " + DateTime.Today.ToString("d").Replace("-", "") + " (" + DateTime.Now.ToString("hh.mm.ss") + ")";
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

        #endregion
    }
}
