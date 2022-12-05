using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    public class StudyWave : INotifyPropertyChanged
    {
        public string ISO_Code
        {
            get
            {
                return _isocode;
            }
            set
            {
                if (_isocode != value)
                {
                    _isocode = value;
                    if (_wave == 0)
                        WaveCode = _isocode + "p";
                    else
                        WaveCode = _isocode + Convert.ToString(_wave);

                    NotifyPropertyChanged();
                }
            }
        }
        public double Wave
        {
            get
            {
                return _wave;
            }
            set
            {
                if (_wave != value)
                {
                    _wave = value;
                    if (_wave == 0)
                        WaveCode = _isocode + "p";
                    else
                        WaveCode = _isocode + Convert.ToString(_wave);

                    NotifyPropertyChanged();
                }
            }
        }
        public string WaveCode { get; private set; }
        public bool EnglishRouting
        {
            get { return _englishrouting; }
            set { if (_englishrouting != value) _englishrouting = value; NotifyPropertyChanged(); }
        }
        public string Countries
        {
            get { return _countries; }
            set { if (_countries != value) _countries = value; NotifyPropertyChanged(); }
        }
    
        public List<Survey> Surveys { get; set; }

        public List<Fieldwork> FieldworkDates { get; set; }
        
        public string SampleInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // IDEA: grants 
        // IDEA: list of ITCCountry objects OR just use fieldwork to represent where the survey took place

        public StudyWave()
        {
            ISO_Code = string.Empty;
            WaveCode = string.Empty;
            Countries = string.Empty;

            FieldworkDates = new List<Fieldwork>();
            SampleInfo = string.Empty;

            Surveys = new List<Survey>();
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

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _isocode;
        private double _wave;
        private bool _englishrouting;
        private string _countries;
    }

    public class Fieldwork
    {
        public ITCCountry Country { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class ITCCountry
    {
        public string CountryName { get; set; }
    }
}
