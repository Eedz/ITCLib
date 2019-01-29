using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class Country
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string StudyName { get; set; }
        public string AgeGroup { get; set; }
        public int CountryCode { get; set; }
        public string ISO_Code { get; set; }

        public Country()
        {

        }
    }
}
