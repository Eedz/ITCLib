using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class DraftQuestion
    {
        public int ID { get; set; } // database ID
        public int DraftID { get; set; }
        public string qnum { get; set; }
        public float SortBy { get; set; }
        public string altqnum { get; set; }
        public string varname { get; set; }
        public string questionText { get; set; }
        public string comment { get; set; }
        public string extra1 { get; set; }
        public string extra2 { get; set; }
        public string extra3 { get; set; }
        public string extra4 { get; set; }
        public string extra5 { get; set; }
        public bool deleted { get; set; }
        public bool inserted { get; set; }

        public DraftQuestion()
        {
            qnum = "";
            
            altqnum = "";
            varname = "";
            questionText = "";
            comment = "";
            extra1 = "";
            extra2 = "";
            extra3 = "";
            extra4 = "";
            extra5 = "";

        }
    }
}
