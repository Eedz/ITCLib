using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class ParallelQuestion
    {
        public int ID { get; set; }
        public int MatchID { get; set; }
        public SurveyQuestion Question { get; set; }

        public ParallelQuestion()
        {
            Question = new SurveyQuestion();
        }
    }
}
