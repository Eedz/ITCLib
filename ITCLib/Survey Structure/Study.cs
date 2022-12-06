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
        public string StudyName { get; set; }
        public string CountryName { get; set; }
        public string AgeGroup { get; set; }
        public int CountryCode { get; set; }
        public string ISO_Code { get; set; }
        public string Languages { get; set; }
        public int Cohort { get; set; }

        public List<StudyWave> Waves { get; set; }

        public string StudyNameISO { get { return StudyName + " (" + ISO_Code + ")"; } }

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

        public Study(string country)
        {

            StudyName = country;
        }

        public override string ToString()
        {
            return StudyName + "(" + ISO_Code +")";
        }

        public override bool Equals(object obj)
        {
            var study = obj as Study;
            return study != null &&
                   StudyName.Equals(study.StudyName) &&
                   ISO_Code.Equals(study.ISO_Code);
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + StudyName.GetHashCode() + ISO_Code.GetHashCode();
            return hashCode;
        }
    }
}
