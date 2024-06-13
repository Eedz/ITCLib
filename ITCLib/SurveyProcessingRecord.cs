using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
   
namespace ITCLib
{
    public class SurveyProcessingRecord : ObservableObject
    {
        private int _id;
        private Survey _surveyid;
        private SurveyProcessingStage _stage;

        public int ID { get; set; }
        public Survey SurveyID { get; set; }
        public SurveyProcessingStage Stage { get; set; }
        public bool NotApplicable { get; set; }
        public bool Done { get; set; }
        public List<SurveyProcessingDate> StageDates { get; set; }
    }


    public class SurveyProcessingDate : ObservableObject
    {
        public int ID { get; set; }
        public int StageID { get; set; }
        public DateTime? StageDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public Person EnteredBy { get; set; }
        public Person Contact { get; set; }
        public List<SurveyProcessingNote> Notes { get; set; }

        public SurveyProcessingDate()
        {
            StageDate = null;
            EntryDate = null;
            EnteredBy = new Person();
            Contact = new Person();
            Notes = new List<SurveyProcessingNote>();
        }
    }

    public class SurveyProcessingNote : ObservableObject
    {
        public int ID { get; set; }
        public int DateID { get; set; }
        public string Note { get; set; }
        public DateTime? NoteDate { get; set; }
        public Person Author { get; set; }

        public SurveyProcessingNote()
        {
            NoteDate = null;
            Author = new Person();
        }
    }

    public class SurveyProcessingStage : ObservableObject
    {
        public int ID { get; set; }
        public string StageName { get; set; }       
    }
}
