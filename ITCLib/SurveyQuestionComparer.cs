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
            return x.refVarName == y.refVarName;
        }

        public int GetHashCode(SurveyQuestion obj)
        {
            return obj.refVarName.GetHashCode();
        }
    }

    
}
