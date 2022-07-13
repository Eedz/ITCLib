using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    public class SurveyDraft : INotifyPropertyChanged
    {
        private int _survid;
        public int SurvID  // should this be a Survey object?
        {
            get { return _survid; }
            set
            {
                if (_survid != value)
                {
                    _survid = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _drafttitle;
        public string DraftTitle
        {
            get { return _drafttitle; }
            set
            {
                if (_drafttitle != value)
                {
                    _drafttitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime? _draftdate;
        public DateTime? DraftDate
        {
            get { return _draftdate; }
            set
            {
                if (_draftdate != value)
                {
                    _draftdate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _draftcomments;
        public string DraftComments {
            get { return _draftcomments; }
            set
            {
                if (_draftcomments != value)
                {
                    _draftcomments = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _investigator;
        public int Investigator
        {
            get { return _investigator; }
            set
            {
                if (_investigator != value)
                {
                    _investigator = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DateAndTitle {
            get
            {
                if (DraftDate != null && !string.IsNullOrWhiteSpace(DraftTitle))
                    return DraftDate.Value.ToString("d") + " - " + DraftTitle;
                else if (DraftDate != null && string.IsNullOrWhiteSpace(DraftTitle))
                    return DraftDate.Value.ToString("d");
                else if (DraftDate == null && !string.IsNullOrWhiteSpace(DraftTitle))
                    return DraftTitle;
                else
                    return string.Empty;

            }
        }

        public string Extra1Label { get; set; }
        public string Extra2Label { get; set; }
        public string Extra3Label { get; set; }
        public string Extra4Label { get; set; }
        public string Extra5Label { get; set; }

        public List<DraftQuestion> Questions { get; set; }
        public List<SurveyDraftExtraField> ExtraFields { get; set; }    

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public SurveyDraft()
        {
            DraftTitle = "";
            DraftComments = "";
            DraftDate = DateTime.Today;
            Questions = new List<DraftQuestion>();
            ExtraFields = new List<SurveyDraftExtraField>();
        }
        

        public SurveyDraft(string title)
        {
            DraftTitle = title;
            DraftComments = "";
            DraftDate = DateTime.Today;
            Questions = new List<DraftQuestion>();
            ExtraFields = new List<SurveyDraftExtraField>();
        }

        
    }

    public class SurveyDraftExtraField : INotifyPropertyChanged
    {
        private int _fieldnumber;
        public int FieldNumber {
            get { return _fieldnumber; }
            set
            {
                if (_fieldnumber != value)
                {
                    _fieldnumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _label { get; set; }
        public string Label {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
