using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class StudyWave
    {
        public int WaveID { get; set; }
        private string _isocode;
        public string ISO_Code
        {
            get
            {
                return _isocode;
            }
            set
            {
                _isocode = value;
                WaveCode = ISO_Code + Convert.ToString(_wave);
            }
        }
        public double _wave;
        public double Wave
        {
            get
            {
                return _wave;
            }
            set
            {
                _wave = value;
                WaveCode = ISO_Code + Convert.ToString(_wave);
            }
        }
        public string WaveCode { get; private set; }
        public bool EnglishRouting { get; set; }
        public string Countries { get; set; }
        public List<Survey> Surveys { get; set; }

        // TODO add info from ITC Database like field work dates, survey samples, funding sources
        

        public StudyWave()
        {

        }

        public override string ToString()
        {
            return WaveCode;
        }
    }
}
