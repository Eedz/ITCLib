using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    /// <summary>
    /// Base class for other comment types.
    /// </summary>
    public class Comment : ObservableObject
    {
        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public Note Notes { get => _notes; set => SetProperty(ref _notes, value); }
        public DateTime? NoteDate { get => _notedate; set => SetProperty(ref _notedate, value); }
        public Person Author { get => _author; set => SetProperty(ref _author, value); }
        public Person Authority { get => _authority; set => SetProperty(ref _authority, value); }
        public CommentType NoteType { get => _notetype; set => SetProperty(ref _notetype, value); }
        public string Source { get => _source; set => SetProperty(ref _source, value); }

        public string NoteDateOnly { get => NoteDate.Value.ShortDate(); }

        public Comment()
        {
            Notes = new Note();
            Author = new Person();
            Authority = new Person();
            NoteType = new CommentType();
            Source = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetComments()
        {
            return "(" + NoteType.ShortForm + ") " + NoteDate.Value.ShortDate() + ".    " + Notes.NoteText;
        }

        private int _id;
        private Note _notes;
        private DateTime? _notedate;
        private Person _author;
        private Person _authority;
        private string _source;
        private CommentType _notetype;
    }
}
