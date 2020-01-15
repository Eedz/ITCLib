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
            bool s = string.Equals(x.VarName.RefVarName, y.VarName.RefVarName, StringComparison.OrdinalIgnoreCase);
            return string.Equals(x.VarName.RefVarName, y.VarName.RefVarName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(SurveyQuestion obj)
        {
            int refV = obj.VarName.RefVarName.ToLower().GetHashCode();
            return refV;
        }
    }

}
