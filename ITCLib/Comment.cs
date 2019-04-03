using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Note
    {
        public int ID { get; set; }
        public string NoteText { get; set; }
    }

    /// <summary>
    /// Represents a note about a single question.
    /// </summary>
    public class Comment
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }
        public int CID { get; set; }
        public string Notes { get; set; }
        public DateTime NoteDate { get; set; }
        public int NoteInit { get; set; }
        public string Name { get; set; }
        public string SourceName { get; set; }
        public string NoteType { get; set; }
        public string Source { get; set; }
        public int SurvID { get; set; }

        public Comment()
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
