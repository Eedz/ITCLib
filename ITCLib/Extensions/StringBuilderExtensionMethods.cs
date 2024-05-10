using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ITCLib
{
    public static class StringBuilderExtensionMethods
    {
        public static StringBuilder RemoveLastLine(this StringBuilder sb)
        {
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (Environment.NewLine.Contains(sb[i]))
                {
                    sb.Remove(i, sb.Length - i);
                    return sb;
                }
            }
            return sb;
        }
    }
}
