using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public static class SASUtilities
    {

        private static string GenerateSASCodeOverallFreq(string project, string projectWave)
        {
            StringBuilder s = new StringBuilder();

            //s.AppendLine("libname itc" + project + " \"D:\\users\\m6yan\\My Data\\ITC\\6Europe\"");
            s.AppendLine("libname itc" + project + " \"[dataset file path]\"");
            s.AppendLine();
            s.AppendLine("Proc format cntlin = itc" + project + ".itc" + project + "_formats; run;");
            s.AppendLine();
            s.AppendLine("Data Core;");
            s.AppendLine("    set itc" + project + ".itc" + project + "_core;");
            s.AppendLine("    if country = 6;");
            s.AppendLine("run; Proc sort; by uniqid; run;");
            s.AppendLine();
            s.AppendLine("Data SP2;");
            s.AppendLine("    set itc" + project + ".itc" + project + "_Wave2;");
            s.AppendLine("    keep _all_");

            return s.ToString();
        }
    }
}
