using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ITCLib
{
    
    public enum SyntaxFormat { EpiData, SAS, SPSS }
    public class SyntaxReport :SurveyBasedReport
    {
        public string OutputPath { get; set; }

        public void CreateSyntax(ReportSurvey s, SyntaxFormat format)
        {
            OutputPath += "\\" + s.SurveyCode + " " + DateTime.Today.ToString("d"); // add the extension to this
            switch (format)
            {
                case SyntaxFormat.EpiData:
                    CreateEpiQES(s);
                    CreateEpiCHK(s);
                    break;

            }


        }

        private void CreateEpiCHK(ReportSurvey s)
        {
            using (StreamWriter tw = new StreamWriter(OutputPath + ".chk"))
            {
                // for each question, if it has response options, create a block containing:
                // VarName
                //   LEGAL...END (if ro/nr not null)
                //   AFTER ENTRY...END (if pstp not null)
                // END
                foreach (SurveyQuestion sq in s.Questions)
                {
                    if (sq.ScriptOnly) continue;

                    // varname
                    tw.WriteLine(sq.VarName);

                    // RANGE 
                    if (!string.IsNullOrEmpty(sq.RespOptions) || !string.IsNullOrEmpty(sq.NRCodes)){
                        if (!string.IsNullOrEmpty(sq.RespOptions) && !sq.VarType.Equals("numeric")){
                            tw.WriteLine("  Range 0 " + new string('7', sq.NumCol));
                        }
                    }

                    // LEGAL
                    if (!string.IsNullOrEmpty(sq.RespOptions) || !string.IsNullOrEmpty(sq.NRCodes) && sq.VarType.Equals("numeric")){
                        tw.WriteLine("  LEGAL");

                        tw.WriteLine("    " + string.Join("\r\n    ", sq.GetRespNumbers()));

                        tw.WriteLine("  END");
                    }
                    
                    // AFTER ENTRY
                    if (!string.IsNullOrEmpty(sq.PstP))
                    {
                        tw.WriteLine("  AFTER ENTRY");

                        tw.WriteLine(GetAfterEntry(sq.VarName.FullVarName,new QuestionRouting (sq.PstP, sq.RespOptions),s.Questions.ToList()));
                        tw.WriteLine("  END");
                    }

                    // END
                    tw.WriteLine("END");
                    tw.WriteLine("");
                }
            }
        }

       
        private string GetAfterEntry(string var, QuestionRouting qr, List<SurveyQuestion> questionList)
        {
            bool startListing = false;
            string afterEntry = "";
            List<string> conds = new List<string>();
            List<string> actions = new List<string>();
            
            
            foreach (RoutingVar v in qr.RoutingVars)
            {
                
                for (int i = 0; i < v.ResponseCodes.Count; i++) {
                    conds.Add("(" + var + " = " + Convert.ToString(v.ResponseCodes[i]) + ")"); 
                }

                actions.Add("      GOTO " + v.Varname.Replace("go to ", ""));
                foreach (SurveyQuestion sq in questionList)
                {
                    if (sq.ScriptOnly) continue;
                    if (sq.VarName.Equals(var)) 
                    {
                        startListing = true;
                        continue;
                    }

                    if (v.Varname.Contains(sq.VarName.RefVarName))
                    {
                        startListing = false;
                        break;
                    }

                    if (startListing)
                    {
                        if (sq.VarType == "string")
                        {
                            actions.Add("      " + sq.VarName + "=\"NA\"");
                        }
                        else
                        {
                            actions.Add("      " + sq.VarName + "=" + new string('7', sq.NumCol));
                        }
                    }
                }

                afterEntry += "    IF" + String.Join(" OR ", conds) + " THEN";
                afterEntry += "\r\n";
                afterEntry += String.Join("\r\n", actions);
                afterEntry += "\r\n    ENDIF";

                conds.Clear();
                actions.Clear();
            }

         

            return afterEntry;
        }

        private void CreateEpiQES(ReportSurvey s)
        {

            using (StreamWriter tw = new StreamWriter(OutputPath + ".qes"))
            {
                string qnumPre;
                string line;
                int longestVarLabel = 0;
                int longestLine = 0;

                // create a header section
                tw.WriteLine(s.Title);
                tw.WriteLine("");

                // ID Code section
                tw.WriteLine("- ID Code -");
                tw.WriteLine("");

                // survey/screener section
                if (s.SurveyCode.EndsWith("sc"))
                {
                    qnumPre = "S";
                    tw.WriteLine("- SCREENER SECTION -");
                }
                else
                {
                    qnumPre = "Q";
                    tw.WriteLine("- SURVEY SECTION -");
                }

                // determine the longest varlabel
                foreach (SurveyQuestion sq in s.Questions)
                {
                    if (sq.VarName.VarLabel.Length > longestVarLabel) longestVarLabel = sq.VarName.VarLabel.Length;
                }

                // longest possible line is:
                // VarName with CC and suffix  (10)
                // 2 spaces (2)
                // Qnum with suffix and 'Q' (5)
                // space dash dash space (4)
                // longest varlabel in list of questions
                longestLine = 10 + 2 + 5 + 4 + longestVarLabel;

                foreach (SurveyQuestion sq in s.Questions)
                {
                    if (sq.ScriptOnly) continue;

                    line = "{" + sq.VarName + "}  " + qnumPre + sq.Qnum + " -- " + sq.VarName.VarLabel.Replace("#", "num");

                    while (line.Length < longestLine)
                    {
                        line += " ";
                    }

                    switch (sq.VarType)
                    {
                        case "numeric":
                            for (int i = 0; i < sq.NumCol; i++)
                            {
                                line += "#";
                            }
                            break;
                        case "string":
                            for (int i = 0; i < sq.NumCol; i++)
                            {
                                line += "_";
                            }
                            break;

                    }

                    tw.WriteLine(line);
                }
            }
            
        }
    }
}
