using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    public enum ReportTemplate { Standard, StandardTranslation, Website, WebsiteTranslation, Automatic }
    public enum Enumeration { Qnum = 1, AltQnum, Both }
    public enum ReadOutOptions { Neither, DontRead, DontReadOut }
    public enum RoutingType { Other, IfResponse, Otherwise, If }
    public enum FileFormats { DOC = 1, PDF }
    public enum TableOfContents { None, Qnums, PageNums }
    public enum PaperSizes { Letter = 1, Legal, Eleven17, A4 }
    public enum ReportTypes { Standard = 1, Label, Order }
    public enum ReportPreset { SurveyList = 1, TopicContent, OrderCompare, Overview, Sections, Syntax, Harmony, VarList, ProductCrosstab }

    public class ITCReport
    {

        #region Properties
        
        public DataTable reportTable;
        
        public ReportTypes ReportType { get; set; }
        public bool Batch { get; set; }

        public string FileName { get; set ; } // this value will initially contain the path up to the file name, which will be added in the Output step

        // formatting and layout options
        public ReportFormatting Formatting { get; set; }
        public ReportLayout LayoutOptions { get; set; }

        public bool RepeatedHeadings { get; set; }
        public bool ColorSubs { get; set; }
        
        public bool InlineRouting { get; set; }
        public bool ShowLongLists { get; set; }
        public bool QNInsertion { get; set; }
        public bool AQNInsertion { get; set; }
        public bool CCInsertion { get; set; }
       
        public List<ReportColumn> ColumnOrder { get; set; }

        public ReadOutOptions NrFormat { get; set; }
        public Enumeration Numbering { get; set; }

        public string Details { get; set; }
        #endregion


        public ITCReport()
        {
            Formatting = new ReportFormatting();
            LayoutOptions = new ReportLayout();

            RepeatedHeadings = true;
            ColorSubs = true;

            Numbering = Enumeration.Qnum;
            NrFormat = ReadOutOptions.Neither;
            
            FileName = "";
            Details = "";
        }

        /// <summary>
        /// Format the header row so with the appropriate widths and titles
        /// </summary>
        /// <param name="doc"></param>
        public void FormatColumns(Word.Document doc)
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
            {// && reportType !=ReportTypes.Label) {
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

                        // filter column
                        if (header.Contains("Filters"))
                        {
                            // TODO set to Verdana 9 font
                        }

                        //TODO test these
                        if (ReportType == ReportTypes.Order)
                        {
                            if (header.Contains("VarName"))
                            {
                                doc.Tables[1].Columns[i].Width = varWidth * 72;
                                widthLeft -= varWidth;
                            }
                            else if (header.Contains("Qnum"))
                            {
                                doc.Tables[1].Columns[i].Width = (qnumWidth * 2) * 72;
                                widthLeft -= qnumWidth;
                            }
                            else if (header.Contains("Question"))
                            {
                                doc.Tables[1].Columns[i].Width = (float)3.5 * 72;
                                widthLeft -= 3.5;
                            }
                        }

                        break;
                }

            }
            // TODO distribute the rest of the columns

        }

        

        

        
    }
}
