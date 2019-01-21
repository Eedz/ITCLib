using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    /// <summary>
    /// Represents a note about a single survey.
    /// </summary>
    public class SurveyComment
    {
        public int ID { get; set; }
        public int SID { get; set; }
        public string Survey { get; set; }
        public int CID { get; set; }
        public string Notes { get; set; }
        public DateTime NoteDate { get; set; }
        public int NoteInit { get; set; }
        public string Name { get; set; }
        public string SourceName { get; set; }
        public string NoteType { get; set; }
        public string Source { get; set; }

        public SurveyComment()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        public string GetComments()
        {
            return NoteDate.ToString("dd-MMM-yyyy") + ".    " + Notes;
        }
    }
}
