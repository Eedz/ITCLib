using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public enum PriorityLevel { Critical = 1, High, Moderate, Low, VeryLow }
    public class BugReport
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Application { get; set; }
        public string Form { get; set; }
        public PriorityLevel Priority { get; set; }
        public string Survey { get; set; }
        public DateTime? BugDate { get; set; }
        public Person Submitter { get; set; }
        public DateTime? FixDate { get; set; }
        public Person Fixer { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public Person Resolver { get; set; }

        public List<BugResponse> Responses { get; set; }

        public BugReport()
        {
            Title = "";
            Description = "";
            Application = "";
            Form = "";
            Priority = PriorityLevel.Low;
            Survey = "";
            BugDate = null;
            Submitter = null;
            FixDate = null;
            Fixer = null;
            ResolutionDate = null;
            Resolver = null;
            Responses = new List<BugResponse>();
        }
    }

    public class BugResponse
    {
        public int ID { get; set; }
        public int BugID { get; set; }
        public string Response { get; set; }
        public DateTime ResponseDate { get; set;}
        public Person Responder { get; set; }

    }
}
