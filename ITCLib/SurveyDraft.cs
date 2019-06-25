using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyDraft
    {

        public int DraftID { get; set; } // database ID
        public int SurvID { get; set; }
        public string DraftTitle { get; set; }
        public DateTime? DraftDate { get; set; }
        public string DraftComments { get; set; }
        public string Extra1Label { get; set; }
        public string Extra2Label { get; set; }
        public string Extra3Label { get; set; }
        public string Extra4Label { get; set; }
        public string Extra5Label { get; set; }
        public List<DraftQuestion> Questions { get; set; }


        public SurveyDraft()
        {
            DraftTitle = "";
            DraftComments = "";
            DraftDate = DateTime.Today;
            Questions = new List<DraftQuestion>();
        }
        public SurveyDraft(int ID)
        {
            DraftID = ID;
            DraftTitle = "";
            DraftComments = "";
            Questions = new List<DraftQuestion>();
        }

        public SurveyDraft(int ID, string title)
        {
            DraftID = ID;
            DraftTitle = title;
            DraftComments = "";
            Questions = new List<DraftQuestion>();
        }
    }
}
