using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ITCLib
{
    
    public enum SyntaxFormat { EpiData, SAS, SPSS }
    public class SyntaxReport : SurveyBasedReport
    {
        public string OutputPath { get; set; }

        public bool VarNameInLabel { get; set; }
        public bool QnumInLabel { get; set; }
        public bool UseQnum { get; set; }

        public void CreateSyntax(ReportSurvey s, SyntaxFormat format)
        {
            OutputPath += "\\" + s.SurveyCode + "_syntax (" + DateTime.Now.ToString("d") +")"; // add the extension to this
            switch (format)
            {
                case SyntaxFormat.EpiData:
                    CreateEpiQES(s);
                    CreateEpiCHK(s);
                    break;
                case SyntaxFormat.SAS:
                    CreateSAS(s, VarNameInLabel, UseQnum , QnumInLabel);
                    break;
                case SyntaxFormat.SPSS:
                    CreateSPSS(s);
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
                    if (sq.GetVarType().Equals("")) continue;

                    // varname
                    tw.WriteLine(sq.VarName);

                    // RANGE 
                    if (!string.IsNullOrEmpty(sq.RespOptions) || !string.IsNullOrEmpty(sq.NRCodes)){
                        if (!string.IsNullOrEmpty(sq.RespOptions) && !sq.GetVarType().Equals("numeric")){
                            tw.WriteLine("  Range 0 " + new string('7', sq.GetNumCols()));
                        }
                    }

                    // LEGAL
                    if (!string.IsNullOrEmpty(sq.RespOptions) || !string.IsNullOrEmpty(sq.NRCodes) && sq.GetVarType().Equals("numeric")){
                        tw.WriteLine("  LEGAL");

                        tw.WriteLine("    " + string.Join("\r\n    ", sq.GetRespNumbers()));

                        tw.WriteLine("  END");
                    }
                    
                    // AFTER ENTRY
                    if (!string.IsNullOrEmpty(sq.PstP))
                    {
                        tw.WriteLine("  AFTER ENTRY");

                        tw.WriteLine(GetAfterEntry(sq.VarName.VarName,new QuestionRouting (sq.PstP, sq.RespOptions),s.Questions.ToList()));
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
                    if (sq.GetVarType().Equals("")) continue;
                    if (sq.VarName.VarName.Equals(var)) 
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
                        if (sq.GetVarType() == "string")
                        {
                            actions.Add("      " + sq.VarName.VarName + "=\"NA\"");
                        }
                        else
                        {
                            actions.Add("      " + sq.VarName.VarName + "=" + new string('7', sq.GetNumCols()));
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

                    line = "{" + sq.VarName.VarName + "}  " + qnumPre + sq.Qnum + " -- " + sq.VarName.VarLabel.Replace("#", "num");

                    while (line.Length < longestLine)
                    {
                        line += " ";
                    }

                    switch (sq.GetVarType())
                    {
                        case "numeric":
                            for (int i = 0; i < sq.GetNumCols(); i++)
                            {
                                line += "#";
                            }
                            break;
                        case "string":
                            for (int i = 0; i < 50; i++)
                            {
                                line += "_";
                            }
                            break;

                    }

                    tw.WriteLine(line);
                }
            }
            
        }

        private void CreateSAS(ReportSurvey s, bool varname, bool qnum, bool questnum)
        {
            using (StreamWriter tw = new StreamWriter(OutputPath + ".sas"))
            {
                tw.WriteLine("label");

                foreach (SurveyQuestion sq in s.Questions)
                {
                    if (qnum)
                    {
                        if (varname)
                        {
                            if (questnum)
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarName + "... Q." + sq.Qnum + " -- " + sq.VarName.VarLabel + "\"");
                            else
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarName + "... " + sq.VarName.VarLabel + "\"");
                        }
                        else
                        {
                            if (questnum)
                                tw.WriteLine(sq.VarName.VarName + " = \"Q." + sq.Qnum + " -- " + sq.VarName.VarLabel + "\"");
                            else
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarLabel + "\"");
                        }
                    }
                    else
                    {
                        if (varname)
                        {
                            if (questnum)
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarName + "... Q." + sq.AltQnum + " -- " + sq.VarName.VarLabel + "\"");
                            else
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarName + "... " + sq.VarName.VarLabel + "\"");
                        }
                        else
                        {
                            if (questnum)
                                tw.WriteLine(sq.VarName.VarName + " = \"Q." + sq.AltQnum + " -- " + sq.VarName.VarLabel + "\"");
                            else
                                tw.WriteLine(sq.VarName.VarName + " = \"" + sq.VarName.VarLabel + "\"");
                        }
                    }
                }

                tw.WriteLine(";");
                tw.WriteLine("");
                tw.WriteLine("/* Saved Labelled Data */");
                tw.WriteLine("");
                tw.WriteLine("");
                // sort by respname

                tw.WriteLine("proc format");

                var allResps = from r in s.Questions
                               group r by r.RespName + "_" + r.NRName into rsort
                               orderby rsort.Key
                               select new { rsort.Key, rsort };

                foreach (var r in allResps)
                {
                    tw.WriteLine("value " + r.rsort.Key + "fmt");
                    
                    SurveyQuestion q = r.rsort.ToList()[0];
                    List<string> nums = q.GetRespNumbers(true);
                    List<string> labels = q.GetRespLabels(true);
                    
                    for (int i = 0; i < nums.Count(); i++)
                    {
                        tw.WriteLine(nums[i] + " = \"" + labels[i] + "\"");
                    }

                }

                tw.WriteLine("run;");
                tw.WriteLine("");
                tw.WriteLine("format");
                tw.WriteLine("");

                foreach (var r in allResps)
                {
                    List<SurveyQuestion> qs = r.rsort.ToList();

                    foreach(SurveyQuestion q in qs)
                    {
                        tw.WriteLine(q.VarName.VarName);
                    }

                    tw.WriteLine("     " + r.rsort.Key + "fmt.");
                }
                tw.WriteLine("");
                tw.WriteLine("/* Saved formatted data from the labeled data */");

                tw.WriteLine("");
                foreach (SurveyQuestion sq in s.Questions)
                {
                    string var = sq.VarName.RefVarName;
                    //If AB932 in (77, .) and AB932r<>. then AB932= AB932r; 
                    if (var.EndsWith("r") )
                    {
                        var match = s.Questions.FirstOrDefault(x => x.VarName.RefVarName.Equals(sq.VarName.RefVarName.Substring(0, sq.VarName.RefVarName.Length - 1)));
                        if (match != null)
                            tw.WriteLine("If " + match.VarName.RefVarName + " in (77, .) and " + var + "<>. then " + match.VarName.RefVarName + "=" + var);
                    }
                }
            }
        }

        private void CreateSPSS(ReportSurvey s)
        {
            using (StreamWriter tw = new StreamWriter(OutputPath + ".sps"))
            {
                tw.WriteLine("*  VARIABLE LIST FOR SPSS -- for " + s.SurveyCode + ".");
                tw.WriteLine("DATA LIST    FREE/");

                foreach(SurveyQuestion sq in s.Questions)
                {
                    tw.WriteLine(sq.VarName.VarName + " " + sq.NumFmt);
                }
                tw.WriteLine(".");
                tw.WriteLine("BEGIN DATA .");
                tw.WriteLine("END DATA .");
                tw.WriteLine("EXECUTE .");
                tw.WriteLine("");
                tw.WriteLine("*  VARIABLE LABELS IN SPSS -- for " + s.SurveyCode + " .");

                foreach (SurveyQuestion sq in s.Questions)
                {
                    tw.WriteLine("Variable Label " + sq.VarName.VarName + " =  \"" + sq.VarName.VarLabel + "\"");
                }
                tw.WriteLine(".");
                tw.WriteLine(" execute .");

                tw.WriteLine("* VALUE LABELS IN SPSS -- definitio and assignment -- for " + s.SurveyCode + " .");

                var allResps = from r in s.Questions
                               group r by r.RespName + "-" + r.NRName into rsort
                               orderby rsort.Key
                               select new { rsort.Key, rsort };

                foreach (var r in allResps)
                {
                    tw.WriteLine("Value " + r.rsort.Key + "fmt");

                    List<SurveyQuestion> qs = r.rsort.ToList();

                    foreach (SurveyQuestion sq in qs)
                    {
                        tw.WriteLine(sq.VarName.VarName);
                    }

                    SurveyQuestion q = r.rsort.ToList()[0];
                    List<string> nums = q.GetRespNumbers(true);
                    List<string> labels = q.GetRespLabels(true);

                    for (int i = 0; i < nums.Count(); i++)
                    {
                        tw.WriteLine(nums[i] + " = \"" + labels[i] + "\"");
                    }

                    tw.WriteLine(";");
                }

                tw.WriteLine("SAVE OUTFILE='P:\\ITC\\Access\\Data Templates\\SPSS\\" + s.SurveyCode + ".sav' /COMPRESSED.");
                tw.WriteLine(".");
            }
        }
    }
}
