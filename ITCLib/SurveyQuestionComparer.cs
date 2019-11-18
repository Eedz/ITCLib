using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ITCLib
{
    

    public class SurveyQuestionComparer : IEqualityComparer<SurveyQuestion>
    {
        public bool Equals(SurveyQuestion x, SurveyQuestion y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            bool s = string.Equals(x.RefVarName, y.RefVarName, StringComparison.OrdinalIgnoreCase);
            return string.Equals(x.RefVarName, y.RefVarName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(SurveyQuestion obj)
        {
            int refV = obj.RefVarName.ToLower().GetHashCode();
            return refV;
        }
    }

}
