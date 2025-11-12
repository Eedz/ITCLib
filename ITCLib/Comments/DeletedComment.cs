using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class DeletedComment : Comment
    {
        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public string Survey { get => _survey; set => SetProperty(ref _survey, value); }
        public string VarName { get => _varname; set => SetProperty(ref _varname, value); }

        public DeletedComment() : base()
        {
            Survey = string.Empty;
            VarName = string.Empty;
        }

        public DeletedComment(Comment basecomment)
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

        private int _survid;
        private string _survey;
        private string _varname;
    }
}
