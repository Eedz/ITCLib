using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    /// <summary>
    /// Represents an ITC Study. An ITC Study is defined as a cohort study that attempts to targets the same group of people within the same country or group of countries.
    /// </summary>
    public class Study
    {
        public int StudyID { get; set; }
        public string StudyName { get; set; }
        public string CountryName { get; set; }
        public string AgeGroup { get; set; }
        public int CountryCode { get; set; }
        public string ISO_Code { get; set; }
        public string Languages { get; set; }
        public int Cohort { get; set; }

        public List<StudyWave> Waves { get; set; }

        public Study()
        {
            Waves = new List<StudyWave>();
        }
    }
}
