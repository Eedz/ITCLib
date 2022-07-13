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
        public int ID { get; set; }
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
            StudyName = string.Empty;
            CountryName = string.Empty;
            AgeGroup = string.Empty;

            ISO_Code = string.Empty;
            Languages = string.Empty;

            Cohort = 1;

            Waves = new List<StudyWave>();
        }

        public Study (int id, string country)
        {
            ID = id;
            StudyName = country;
        }

        public override string ToString()
        {
            return StudyName;
        }

        public override bool Equals(object obj)
        {
            var study = obj as Study;
            return study != null &&
                   ID == study.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }
    }
}
