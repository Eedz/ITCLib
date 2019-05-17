using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    

    public class SurveyQuestionComparer : IEqualityComparer<SurveyQuestion>
    {
        public bool Equals(SurveyQuestion x, SurveyQuestion y)
        {
            return x.RefVarName == y.RefVarName;
        }

        public int GetHashCode(SurveyQuestion obj)
        {
            return obj.RefVarName.GetHashCode();
        }
    }

    
}
