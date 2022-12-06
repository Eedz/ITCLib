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

        public override string ToString()
        {
            return NoteText;
        }
    }

    /// <summary>
    /// Represents a note about a single question.
    /// </summary>
    public class Comment
    {
        public int ID { get; set; }
        public Note Notes { get; set; }
        public DateTime? NoteDate { get; set; }
        public string NoteDateOnly { get
            {
                return NoteDate.Value.ShortDate();
            }
        }
        public Person Author { get; set; }
        public Person Authority { get; set; }
        public string SourceName { get; set; } // TODO get rid of this property and use Authority
        public CommentType NoteType { get; set; }
        public string Source { get; set; }

        public Comment()
        {
            Notes = new Note();
            Author = new Person();
            Authority = new Person();
            NoteType = new CommentType();
            SourceName = "";
            Source = "";
        }
        

        /// <summary>
        /// 
        /// </summary>
        public string GetComments()
        {
            return "(" + NoteType.ShortForm + ") " + NoteDate.Value.ShortDate() + ".    " + Notes.NoteText;
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
            Survey = string.Empty;
            VarName = string.Empty;
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

    public class DeletedComment : Comment
    {
        public int SurvID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }

        public DeletedComment() : base()
        {
            Survey = string.Empty;
            VarName = string.Empty;
        }
    }

    public class RefVarComment: Comment
    {
        public string RefVarName { get; set; }

        public RefVarComment() :base()
        {
            RefVarName = string.Empty;
        }
    }

    public class CommentType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public string ShortForm { get; set; }

        public CommentType()
        {
            ID = 0;
            TypeName = string.Empty;
            ShortForm = string.Empty;
        }

        public override string ToString()
        {
            return TypeName;
        }

        public override bool Equals(object obj)
        {
            var type = obj as CommentType;
            return type != null &&
                   ID == type.ID &&
                   TypeName == type.TypeName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            return hashCode;
        }
    }


}
