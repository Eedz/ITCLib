using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class WaveComment : Comment
    {
        public int WaveID { get => _waveid; set => SetProperty(ref _waveid, value); }
        public string StudyWave { get => _studywave; set => SetProperty(ref _studywave, value); }

        public WaveComment() : base()
        {
            StudyWave = string.Empty;
        }

        private int _waveid;
        private string _studywave;
    }
}
