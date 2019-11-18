using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    /// <summary>
    /// Represents the Note part of a comment.
    /// </summary>
    public class Note
    {
        public int ID { get; set; }
        public string NoteText { get; set; }

        public Note()
        {
            NoteText = "";
        }

        public Note (int id, string text)
        {
            ID = id;
            NoteText = text;
        }
    }

    /// <summary>
    /// Represents a note about a single question.
    /// </summary>
    public class Comment
    {
        public Note Notes { get; set; }
        public int CID { get; set; }
        public DateTime NoteDate { get; set; }
        public int NoteInit { get; set; }
        public string Name { get; set; }
        public string SourceName { get; set; }
        public string NoteType { get; set; }
        public string Source { get; set; }
        public string ShortNoteType { get; set; }

        public Comment()
        {
            Notes = new Note();
            Name = "";
            SourceName = "";
            NoteType = "";
            Source = "";
            ShortNoteType = "";
        }
        

        /// <summary>
        /// 
        /// </summary>
        public string GetComments()
        {
            return "(" + ShortNoteType + ") " + NoteDate.ToString("dd-MMM-yyyy") + ".    " + Notes.NoteText;
        }
    }

    public class QuestionComment : Comment
    {
        public int QID { get; set; }
        public int SurvID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }

        public QuestionComment() : base()
        {
            Survey = "";
            VarName = "";
        }

    }

    public class SurveyComment : Comment
    {
        
        public int SurvID { get; set; }
        public string Survey { get; set; }
        
        public SurveyComment() : base()
        {
            Survey = "";
        }
    }

    public class WaveComment : Comment
    {
        public int WaveID { get; set; }
        public string StudyWave { get; set; }

        public WaveComment ():base()
        {
            StudyWave = "";
        }
    }


}
