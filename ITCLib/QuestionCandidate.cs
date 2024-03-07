using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public enum WordingType { PreP, PreI, PreA, LitQ, PstI, PstP, RespOptions, NRCodes }
    public enum ResponseType { RespOptions, NRCodes }

    public class QuestionCandidatePreview : ObservableObject 
    {
        public int QID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }
        public string Qnum { get; set; }

        private QuestionCandidate _original;
        public QuestionCandidate Original { get => _original; set { SetProperty(ref _original, value); } }
        private QuestionCandidate _revised;
        public QuestionCandidate Revised { get => _revised; set { SetProperty(ref _revised, value); } }

        public bool IsNewQuestion { get => QID == 0; }

        public bool IsDeletion { get => Revised.IsBlank(); }

        public string Status
        {
            get
            {
                if (IsDeletion)
                    return "(Deletion)";
                if (IsNewQuestion)
                    return "(New)";
                else if (!NoChanges())
                    return "(Edited)";
                else
                    return "(Unchanged)";
            }
        }

        public string Descriptor { get => string.Join(" - ", new string[] { Survey, VarName, Qnum, Status }); }

        public string SummaryOfChanges { get
            {
                StringBuilder summary = new StringBuilder();
                if (Revised.PreP.NewWording)
                    summary.AppendLine("PreP will be created.");
                else if (Original.PreP.WordID != Revised.PreP.WordID)
                    summary.AppendLine("PreP will be changed to " + Revised.PreP.WordID);

                if (Revised.PreI.NewWording)
                    summary.AppendLine("PreI will be created.");
                else if (Original.PreI.WordID != Revised.PreI.WordID)
                    summary.AppendLine("PreI will be changed to " + Revised.PreI.WordID);

                if (Revised.PreA.NewWording)
                    summary.AppendLine("PreA will be created.");
                else if (Original.PreA.WordID != Revised.PreA.WordID)
                    summary.AppendLine("PreA will be changed to " + Revised.PreA.WordID);

                if (Revised.LitQ.NewWording)
                    summary.AppendLine("LitQ will be created.");
                else if (Original.LitQ.WordID != Revised.LitQ.WordID)
                    summary.AppendLine("LitQ will be changed to " + Revised.LitQ.WordID);

                if (Revised.PstI.NewWording)
                    summary.AppendLine("PstI will be created.");
                else if (Original.PstI.WordID != Revised.PstI.WordID)
                    summary.AppendLine("PstI will be changed to " + Revised.PstI.WordID);

                if (Revised.PstP.NewWording)
                    summary.AppendLine("PstP will be created.");
                else if (Original.PstP.WordID != Revised.PstP.WordID)
                    summary.AppendLine("PstP will be changed to " + Revised.PstP.WordID);

                if (Revised.RespOptions.NewWording)
                    summary.AppendLine("RespOptions will be created.");
                else if (Original.RespOptions.SetName != Revised.RespOptions.SetName)
                    summary.AppendLine("RespOptions will be changed to " + Revised.RespOptions.SetName);

                if (Revised.NRCodes.NewWording)
                    summary.AppendLine("NRCodes will be created.");
                else if (Original.NRCodes.SetName != Revised.NRCodes.SetName)
                    summary.AppendLine("NRCodes will be changed to " + Revised.NRCodes.SetName);

                if (Comments.Count() > 0)
                    summary.AppendLine(Comments.Count() + " comments will be created.");
                

                return summary.ToString();
            } }

        public string QuestionPreview
        {
            get => new SurveyQuestion()
            {
                PreP = Revised.PreP.Text,
                PreI = Revised.PreI.Text,
                PreA = Revised.PreA.Text,
                LitQ = Revised.LitQ.Text,
                PstI = Revised.PstI.Text,
                PstP = Revised.PstP.Text,
                RespOptions = Revised.RespOptions.Text,
                NRCodes = Revised.NRCodes.Text
            }.GetQuestionTextHTML();
        }

        public ObservableCollection<QuestionComment> Comments { get; set; }

        public ObservableCollection<string> DeletedItems { get; set; }
        public ObservableCollection<QuestionComment> DeletedComments { get; set; }

        public event EventHandler WordingCollectionChanged;
        public event EventHandler CommentCollectionChanged;

        public QuestionCandidatePreview()
        {
            Original = new QuestionCandidate();
            Revised = new QuestionCandidate();
            Comments = new ObservableCollection<QuestionComment>();
            DeletedItems = new ObservableCollection<string>();
            DeletedComments = new ObservableCollection<QuestionComment>();

            Revised.PreP.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.PreI.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.PreA.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.LitQ.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.PstI.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.PstP.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.RespOptions.Lines.CollectionChanged += Wording_CollectionChanged;
            Revised.NRCodes.Lines.CollectionChanged += Wording_CollectionChanged;

            Comments.CollectionChanged += Comments_CollectionChanged;
        }

        private void Comments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CommentCollectionChanged?.Invoke(this, e);
        }

        private void Wording_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            WordingCollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Returns true if the original and revised questions are identical.
        /// </summary>
        /// <returns></returns>
        public bool NoChanges()
        {
            return Revised.PreP.Equals(Original.PreP) &&
                Revised.PreI.Equals(Original.PreI) &&
                Revised.PreA.Equals(Original.PreA) &&
                Revised.LitQ.Equals(Original.LitQ) &&
                Revised.PstI.Equals(Original.PstI) &&
                Revised.PstP.Equals(Original.PstP) &&
                Revised.RespOptions.Equals(Original.RespOptions) &&
                Revised.NRCodes.Equals(Original.NRCodes);
        }

    }

    public class QuestionCandidate : ObservableObject
    {
        public string Survey { get; set; }
        public string VarName { get; set; }
        public WordingCandidate PreP { get; set; }
        public WordingCandidate PreI { get; set; }
        public WordingCandidate PreA { get; set; }
        public WordingCandidate LitQ { get; set; }
        public WordingCandidate PstI { get; set; }
        public WordingCandidate PstP { get; set; }
        public ResponseSetCandidate RespOptions { get; set; }
        public ResponseSetCandidate NRCodes { get; set; }

        public QuestionCandidate()
        {
            Survey = string.Empty;
            VarName = string.Empty;
            PreP = new WordingCandidate(WordingType.PreP);
            PreI = new WordingCandidate(WordingType.PreI);
            PreA = new WordingCandidate(WordingType.PreA);
            LitQ = new WordingCandidate(WordingType.LitQ); 
            PstI = new WordingCandidate(WordingType.PstI);
            PstP = new WordingCandidate(WordingType.PstP);
            RespOptions = new ResponseSetCandidate(ResponseType.RespOptions);
            NRCodes = new ResponseSetCandidate(ResponseType.NRCodes);
        }

        public bool IsBlank()
        {
            return string.IsNullOrEmpty(PreP.Text) &&
                string.IsNullOrEmpty(PreI.Text) &&
                string.IsNullOrEmpty(PreA.Text) &&
                string.IsNullOrEmpty(LitQ.Text) &&
                string.IsNullOrEmpty(PstI.Text) &&
                string.IsNullOrEmpty(PstP.Text) &&
                string.IsNullOrEmpty(RespOptions.Text) &&
                string.IsNullOrEmpty(NRCodes.Text);
        }

        

    }

    public class WordingCandidate : ObservableObject
    {
        public WordingType FieldName { get; set; }
        private bool _newwording;
        public bool NewWording
        {
            get => _newwording;
            set
            {
                SetProperty(ref _newwording, value);
                OnPropertyChanged(nameof(Status));
            }
        }
        private int _wordid;
        public int WordID { 
            get => _wordid; 
            set
            {
                SetProperty(ref _wordid, value);
                OnPropertyChanged(nameof(WordID));
                OnPropertyChanged(nameof(Status));
            }
        }
        private ObservableCollection<string> _lines;
        public ObservableCollection<string> Lines { get => _lines; }
        public string Text { get => string.Join("<br>", _lines); }
        public string Status { get
            {
                if (NewWording)
                    return "(New)";
                else
                    return WordID.ToString();
            } }
        

        public WordingCandidate (WordingType type)
        {
            FieldName = type;
            _lines = new ObservableCollection<string>();
        }

        public WordingCandidate(WordingType type, ObservableCollection<string> lines)
        {
            FieldName = type;
            _lines = lines;
        }

        public void AddLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            switch (FieldName)
            {
                case WordingType.PreP:
                case WordingType.PstP:
                    _lines.Add(line.Replace("<strong>", "").Replace("</strong>", ""));
                    break;
                case WordingType.PreI:
                case WordingType.PstI:
                    _lines.Add(line.Replace("<em>", "").Replace("</em>", ""));
                    break;
                default:
                    _lines.Add(line);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            WordingCandidate other = obj as WordingCandidate;
            if (other.Text.Equals(this.Text))
                return true;
            else 
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ResponseSetCandidate : ObservableObject
    {
        public ResponseType FieldName { get; set; }
        private bool _newwording;
        public bool NewWording { get => _newwording; 
            set 
            { 
                SetProperty(ref _newwording, value); 
                OnPropertyChanged(nameof(Status)); 
            } 
        }
        private string _setname;
        public string SetName { 
            get => _setname; 
            set {
                SetProperty(ref _setname, value); 
                OnPropertyChanged(nameof(SetName));
                OnPropertyChanged(nameof(Status));
            } 
        }
        private ObservableCollection<string> _lines;
        public ObservableCollection<string> Lines { get => _lines; }
        public string Text { get => string.Join("<br>", _lines); }
        public string Status
        {
            get
            {
                if (NewWording)
                    return SetName + " (New)";
                else
                    return SetName;
            }
        }

        public ResponseSetCandidate(ResponseType type)
        {
            FieldName = type;
            _lines = new ObservableCollection<string>();
        }

        public ResponseSetCandidate(ResponseType type, ObservableCollection<string> lines)
        {
            FieldName = type;
            _lines = lines;
        }

        public void AddLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            switch (FieldName)
            {
                case ResponseType.RespOptions:
                case ResponseType.NRCodes:
                    _lines.Add(line);

                    break;
            }
        }

        public override bool Equals(object obj)
        {
            ResponseSetCandidate other = obj as ResponseSetCandidate;
            if (other.Text.Equals(this.Text))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
