using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    // IDEA: grants 
    // IDEA: list of ITCCountry objects OR just use fieldwork to represent where the survey took place
    public class StudyWave : ObservableObject
    {
        public int ID 
        {
            get => _id; 
            set => SetProperty(ref _id, value); 
        }
        public int StudyID
        {
            get => _studyid;
            set => SetProperty(ref _studyid, value);
        }

        public string ISO_Code
        {
            get => _isocode;
            set
            {
                SetProperty(ref _isocode, value);
                WaveCode = _isocode + (_wave == 0 ? "p" : Convert.ToString(_wave));
            }
        }
        public double Wave
        {
            get => _wave;
            set
            {
                SetProperty(ref _wave, value);
                WaveCode = _isocode + (_wave == 0 ? "p" : Convert.ToString(_wave));
            }
        }
        public string WaveCode { get; private set; }

        public bool EnglishRouting
        {
            get => _englishrouting;
            set => SetProperty(ref _englishrouting, value);
        }
        
        public string Countries
        {
            get => _countries; 
            set => SetProperty(ref _countries, value);
        }
    
        public List<Survey> Surveys { get; set; }

        public List<Fieldwork> FieldworkDates { get; set; }
        
        public string SampleInfo 
        { 
            get => _sampleinfo; 
            set => SetProperty(ref _sampleinfo, value); 
        }

        public StudyWave()
        {
            ISO_Code = string.Empty;
            WaveCode = string.Empty;
            Countries = string.Empty;

            FieldworkDates = new List<Fieldwork>();
            SampleInfo = string.Empty;

            Surveys = new List<Survey>();
        }

        public StudyWave(string iso, double wave) : this()
        {
            ISO_Code = iso;
            Wave = wave;
        }

        public int GetFieldworkStart()
        {
            if (FieldworkDates == null || FieldworkDates.Count == 0)
                return 0;

            var min = FieldworkDates.Min(x => x.Start);

            if (min != null)
                return min.Value.Year;
            else
                return 0;
        }

        public string GetFieldworkYear()
        {

            if (FieldworkDates == null || FieldworkDates.Count == 0)
                return String.Empty;

            var earliest = FieldworkDates.Min(x => x.Start);
            var latest = FieldworkDates.Max(x => x.End);

            if (earliest.Value.Year == latest.Value.Year)
                return earliest.Value.Year.ToString();
            else if (earliest.Value.Year < latest.Value.Year)
                return earliest.Value.Year + "-" + latest.Value.Year.ToString().Substring(2,2);

            return string.Empty;
        }

        public override string ToString()
        {
            return WaveCode;
        }

        private int _id;
        private int _studyid;
        private string _isocode;
        private double _wave;
        private bool _englishrouting;
        private string _countries;
        private string _sampleinfo;
    }

    public class Fieldwork
    {
        public ITCCountry Country { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string CountryName { get { return Country.CountryName; } }
    }

    public class ITCCountry
    {
        public string CountryName { get; set; }

        public override string ToString()
        {
            return CountryName;
        }
    }
}
