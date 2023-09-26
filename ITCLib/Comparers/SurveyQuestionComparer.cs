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


    public class RefVarNameBaseComparer : IEqualityComparer<RefVariableName>
    {
        public bool Equals(RefVariableName x, RefVariableName y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            bool p = string.Equals(x.Prefix, y.Prefix, StringComparison.OrdinalIgnoreCase);
            bool n = string.Equals(x.Number, y.Number, StringComparison.OrdinalIgnoreCase);
            return p && n;
        }

        public int GetHashCode(RefVariableName obj)
        {
            int refV = obj.RefVarName.ToLower().GetHashCode();
            return refV;
        }
    }

}
