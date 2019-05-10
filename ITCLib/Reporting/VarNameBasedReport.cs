using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    // a report that is based on a selection of VarNames
    // harmony (has wordings, labels, survey list)
    // var list (has headings that are surveys)
    public class VarNameBasedReport : IReport
    {
        #region Properties

        public BindingList<VariableName> VarNames { get; set; }
        public List<SurveyQuestion> questions;

        public DataTable ReportTable { get; set; }

        public ReportTypes ReportType { get; set; }
        public bool Batch { get; set; }

        public string FileName { get; set; } // this value will initially contain the path up to the file name, which will be added in the Output step

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

        public VarNameBasedReport()
        {

            VarNames = new BindingList<VariableName>();
            questions = new List<SurveyQuestion>();
            

        }

        /// <summary>
        /// Adds a new item to the collection of report columns. The ordinal is always 1 more than the number of columns, making the new column the right most column.
        /// </summary>
        /// <param name="name"></param>
        public void AddColumn(string name) { }

        /// <summary>
        /// Adds a new item to the collection of report columns. The ordinal is always 1 more than the number of columns, making the new column the right most column.
        /// </summary>
        /// <param name="name"></param>
        public void AddColumn(string name, int ordinal) { }

        public void RemoveColumn(string name) { }

        public void UpdateColumnOrder() { }

        /// <summary>
        /// Format the header row so with the appropriate widths and titles
        /// </summary>
        /// <param name="doc"></param>
        public void FormatColumns(Word.Document doc) { }


    }
}
