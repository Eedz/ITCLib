using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class LockedSurvey : Survey 
    {
        public Person UnlockedBy { get => _unlockedby; set => SetProperty(ref _unlockedby, value); }
        public int UnlockedFor { get => _unlockedfor; set => SetProperty(ref _unlockedfor, value); }
        public DateTime UnlockedAt { get => _unlockedat; set => SetProperty(ref _unlockedat, value); }
        public double UnlockedForMin
        {
            get
            {
                TimeSpan ts = UnlockedAt.AddMinutes(UnlockedFor) - DateTime.Now;
                return Math.Round(ts.TotalMinutes,2);
            }
        }

        public LockedSurvey() : base() 
        { 
            UnlockedBy = new Person();
        }

        private Person _unlockedby;
        private int _unlockedfor;
        private DateTime _unlockedat;        
    }
}
