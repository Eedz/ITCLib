using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class VarNameChange
    {
        public int ID { get; set; }
        public VariableName OldName { get; set; }
        public VariableName NewName { get; set; }

        public DateTime ChangeDate { get; set; }
        public DateTime ApproxChangeDate { get; set; }
        public Person ChangedBy { get; set; }
        public string Authorization { get; set; }
        public string Rationale { get; set; }
        public string Source { get; set; }
        public bool HiddenChange { get; set; }

        public List<Person> Notifications {get;set;}
        public List<Survey> SurveysAffected { get; set; }

        public VarNameChange()
        {

        }
    }
}
