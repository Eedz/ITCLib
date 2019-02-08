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
        public string ISO_Code { get; set; }
        public double Wave { get; set; }
        public List<Survey> Surveys { get; set; }

        // TODO add info from ITC Database like field work dates, survey samples, funding sources
        

        public StudyWave()
        {

        }
    }
}
