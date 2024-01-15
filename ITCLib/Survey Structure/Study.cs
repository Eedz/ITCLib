using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    /// <summary>
    /// Represents an ITC Study. An ITC Study is defined as a cohort study that attempts to targets the same group of people within the same country or group of countries.
    /// </summary>
    public class Study : ObservableObject
    {
        public int ID 
        { 
            get => _id; 
            set => SetProperty(ref _id, value);
        }
        public int RegionID 
        { 
            get => _regionID; 
            set => SetProperty(ref _regionID, value); 
        }
        public string StudyName
        {
            get => _studyname;
            set => SetProperty(ref _studyname, value);
        }
        public string CountryName {
            get => _countryname;
            set => SetProperty(ref _countryname, value);
        }
        public string AgeGroup {
            get => _agegroup;
            set => SetProperty(ref _agegroup, value);
        }
        public int CountryCode {
            get => _countrycode;
            set => SetProperty(ref _countrycode, value);
        }
        public string ISO_Code {
            get => _isocode;
            set => SetProperty(ref _isocode, value);
        }
        public string Languages {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }
        public int Cohort {
            get => _cohort;
            set => SetProperty(ref _cohort, value);
        }

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

        private int _id;
        private int _regionID;
        private string _studyname;
        private string _countryname;
        private string _agegroup;
        private int _countrycode;
        private string _isocode;
        private string _languages;
        private int _cohort;

    }
}
