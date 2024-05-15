using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    /// <summary>
    /// Represents the Note part of a comment.
    /// </summary>
    public class Note : ObservableObject
    {
        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string NoteText { get => _notetext; set => SetProperty(ref _notetext, value); }

        public Note()
        {
            NoteText = string.Empty;
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

        private int _id;
        private string _notetext;
    }
}
