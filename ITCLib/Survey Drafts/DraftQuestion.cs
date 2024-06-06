using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
namespace ITCLib
{
    public class DraftQuestion : ObservableObject
    {
        public int ID { get => _id; set => SetProperty (ref _id, value); }
        public int DraftID { get => _draftID; set =>SetProperty (ref _draftID, value); }
        public string Qnum { get => _qnum; set => SetProperty (ref _qnum, value); }
        public float SortBy { get => _sortby; set => SetProperty (ref _sortby, value); }
        public string AltQnum { get => _altqnum; set => SetProperty (ref _altqnum, value); }
        public string VarName { get => _varname; set => SetProperty (ref _varname, value); }
        public string RefVarName { get => _refvarname; set => SetProperty (ref _refvarname, value); }
        public string QuestionText { get => _questionText; set => SetProperty(ref _questionText, value); }
        public string Comments { get => _comments; set => SetProperty (ref _comments, value); }
        public string Extra1 { get => _extra1; set => SetProperty (ref _extra1, value); }
        public string Extra2 { get => _extra2; set => SetProperty (ref _extra2, value); }
        public string Extra3 { get => _extra3; set => SetProperty(ref _extra3, value); }
        public string Extra4 { get => _extra4; set => SetProperty(ref _extra4, value); }      
        public string Extra5 { get => _extra5; set => SetProperty(ref _extra5, value); }
        public bool Deleted { get => _deleted; set => SetProperty(ref _deleted, value); }
        public bool Inserted { get => _inserted; set => SetProperty(ref _inserted, value); }

        public DraftQuestion()
        {
            Qnum = string.Empty;
            AltQnum = string.Empty;
            VarName = string.Empty;
            RefVarName = string.Empty;
            QuestionText = string.Empty;
            Comments = string.Empty;
            Extra1 = string.Empty;
            Extra2 = string.Empty;
            Extra3 = string.Empty;
            Extra4 = string.Empty;
            Extra5 = string.Empty;
        }

        public string GetExtraFieldData(int fieldNumber)
        {
            switch (fieldNumber)
            {
                case 1:
                    return Extra1;
                case 2:
                    return Extra2;
                case 3:
                    return Extra3;
                case 4:
                    return Extra4;
                case 5:
                    return Extra5;
                default:
                    return string.Empty;

            }
        }

        private int _id;
        private int _draftID;
        private string _qnum;
        private float _sortby;
        private string _altqnum;
        private string _varname;
        private string _refvarname;
        private string _questionText;
        private string _comments;
        private string _extra1;
        private string _extra2;
        private string _extra3;
        private string _extra4;
        private string _extra5;
        private bool _deleted;
        private bool _inserted;
    }
}
