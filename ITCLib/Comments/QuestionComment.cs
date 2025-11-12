using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class QuestionComment : Comment
    {
        public int QID { get => _qid; set => SetProperty(ref _qid, value); }
        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public string Survey { get => _survey; set => SetProperty(ref _survey, value); }
        public string VarName { get => _varname; set => SetProperty(ref _varname, value); }

        public QuestionComment() : base()
        {
            Survey = string.Empty;
            VarName = string.Empty;
        }

        public QuestionComment(Comment basecomment)
        {
            Survey = string.Empty;
            VarName = string.Empty;
            Notes = basecomment.Notes;
            NoteDate = basecomment.NoteDate;
            Author = basecomment.Author;
            Authority = basecomment.Authority;
            NoteType = basecomment.NoteType;
            Source = basecomment.Source;
        }

        public QuestionComment (DeletedComment comment)
        {
            Survey = comment.Survey;
            VarName = comment.VarName;
            NoteDate= comment.NoteDate;
            Author = comment.Author;
            Authority = comment.Authority;
            NoteType= comment.NoteType;
            Source = comment.Source;
            Notes = comment.Notes;
        }

        private int _qid;
        private int _survid;
        private string _survey;
        private string _varname;
    }
}
