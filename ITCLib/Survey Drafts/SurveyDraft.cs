using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class SurveyDraft : ObservableObject
    {
        public int ID { get => _id; set => SetProperty (ref _id, value); }
        
        public int SurvID { get => _survid; set => SetProperty (ref _survid, value); }

        public string DraftTitle { get => _drafttitle; set => SetProperty(ref _drafttitle, value); }
        
        public DateTime? DraftDate { get => _draftdate; set => SetProperty(ref _draftdate, value); }
        
        public string DraftComments { get => _draftcomments; set => SetProperty(ref _draftcomments, value); }
        
        public int Investigator { get => _investigator; set => SetProperty(ref _investigator, value); }

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

        public List<DraftQuestion> Questions { get; set; }
        public List<SurveyDraftExtraField> ExtraFields { get; set; }    

        public SurveyDraft()
        {
            DraftTitle = string.Empty;
            DraftComments = string.Empty;
            DraftDate = DateTime.Today;
            Questions = new List<DraftQuestion>();
            ExtraFields = new List<SurveyDraftExtraField>();
        }
        
        public SurveyDraft(string title)
        {
            DraftTitle = title;
            DraftComments = string.Empty;
            DraftDate = DateTime.Today;
            Questions = new List<DraftQuestion>();
            ExtraFields = new List<SurveyDraftExtraField>();
        }

        private int _id;
        private int _survid;
        private string _drafttitle;
        private DateTime? _draftdate;
        private string _draftcomments;
        private int _investigator;
    }

    public class SurveyDraftExtraField : ObservableObject
    {
        private int _id;
        private int _draftid;
        private int _fieldnumber;
        private string _label;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public int DraftID { get => _draftid; set => SetProperty(ref _draftid, value); }
        public int FieldNumber { get => _fieldnumber; set => SetProperty(ref _fieldnumber, value); }
        public string Label { get => _label; set => SetProperty(ref _label, value); }

        public SurveyDraftExtraField()
        {

        }
    }
}
