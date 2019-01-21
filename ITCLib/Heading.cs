using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class Heading
    {
        public string Qnum { get; set; }
        public string Varname { get; set; }
        public string PreP { get; set; }

       
        
        public Heading (string qnum, string prep)
        {
            Qnum = qnum;
            PreP = prep;
        }
    }
}
