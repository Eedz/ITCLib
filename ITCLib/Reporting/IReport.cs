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
    
    
    /// <summary>
    /// Defines the basics of an ITC Report.
    /// </summary>
    public interface IReport
    {
   
        #region Properties
        
        DataTable ReportTable { get; set; }
        
        ReportTypes ReportType { get; set; }
        bool Batch { get; set; }

        string FileName { get; set ; } // this value will initially contain the path up to the file name, which will be added in the Output step

        // formatting and layout options
        ReportFormatting Formatting { get; set; }
        ReportLayout LayoutOptions { get; set; }

        bool ColorSubs { get; set; }
        bool InlineRouting { get; set; }
        bool ShowLongLists { get; set; }
        bool QNInsertion { get; set; }
        bool AQNInsertion { get; set; }
        bool CCInsertion { get; set; }
       
        List<ReportColumn> ColumnOrder { get; set; }

        ReadOutOptions NrFormat { get; set; }
        Enumeration Numbering { get; set; }

        string Details { get; set; }

        string ReportStatus { get; set; }

        /// <summary>
        /// True if the report should be shown when completed
        /// </summary>
        bool OpenFinalReport { get; set; }
        #endregion

        /// <summary>
        /// Adds a new item to the collection of report columns. The ordinal is always 1 more than the number of columns, making the new column the right most column.
        /// </summary>
        /// <param name="name"></param>
        void AddColumn(string name);

        /// <summary>
        /// Adds a new item to the collection of report columns. The ordinal is always 1 more than the number of columns, making the new column the right most column.
        /// </summary>
        /// <param name="name"></param>
        void AddColumn(string name, int ordinal);

        void RemoveColumn(string name);

        void UpdateColumnOrder();

        /// <summary>
        /// Format the header row so with the appropriate widths and titles
        /// </summary>
        /// <param name="doc"></param>
        void FormatColumns(Word.Document doc);
      

        

        

        
    }
}
