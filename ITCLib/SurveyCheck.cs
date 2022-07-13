using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyCheck
    {
        public List<SurveyQuestion> QuestionList { get; set; }
        public List<CanonicalRefVarName> CanonVars { get; set; }
        public List<List<string>> ReferenceQuestionLists { get; set; }
        
        public SurveyCheck (List<SurveyQuestion> questionList)
        {
            QuestionList = questionList;
            ReferenceQuestionLists = new List<List<string>>();
        }

        #region Routing Check
        public List<RoutingCheckQuestion> GetRoutingCheckResults()
        {
            List<RoutingCheckQuestion> results = new List<RoutingCheckQuestion>();


            foreach (SurveyQuestion sq in QuestionList)
            {

                RoutingCheckQuestion addToResult = new RoutingCheckQuestion(sq);
                bool add = false;

                // prep
                List<string> filterVars = sq.GetFilterVars();

                foreach (string var in filterVars)
                {
                    if (var.Equals(sq.VarName.RefVarName)) // skip if question refers to itself
                        continue;

                    SurveyQuestion found = QuestionList.FirstOrDefault(x => x.VarName.RefVarName.Equals(var));

                    // add to the result list if:
                    // not found, and not in screener or is suffixed 
                    // OR 
                    // it is found, but isn't allowed in the PreP
                    if (found == null)
                    {
                        if (!IsSuffixed(var) && !IsInReferenceList(var))
                        {
                            addToResult.MissingPrePVars.Add(var);
                            add = true;
                        }
                    }
                    else if (!IsAllowedInPreP(found, sq))
                    {
                        addToResult.LatePrePVars.Add(var);
                        add = true;
                    }
                }

                // pstp
                List<string> routingVars = sq.GetRoutingVars();

                foreach (string var in routingVars)
                {
                    if (var.Equals(sq.VarName.RefVarName)) // skip if question refers to itself
                        continue;

                    SurveyQuestion found = QuestionList.FirstOrDefault(x => x.VarName.RefVarName.Equals(var));
                    // add to the result list if:
                    // not found, and not in screener or is suffixed 
                    // OR 
                    // it is found, but isn't allowed in the PreP
                    if (found == null )
                    {
                        if (!IsSuffixed(var) && !IsInReferenceList(var))
                        {
                            addToResult.MissingPstPVars.Add(var);
                            add = true;
                        }
                    }
                    else if (!IsAllowedInPstP(found, sq))
                    {
                        addToResult.EarlyPstPVars.Add(var);
                        add = true;
                    }

                }

                if (add)
                    results.Add(addToResult);
            }

            return results;
        }

        /// <summary>
        /// Returns true if there is a VarName in the question list that begins with 'var' but is longer than 'var'. I.E. a suffixed version of 'var'.
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        private bool IsSuffixed (string var)
        {
            return QuestionList.Any(x => x.VarName.RefVarName.StartsWith(var) && x.VarName.RefVarName.Length > var.Length);
        }

        /// <summary>
        /// Returns true if 'var' exists in at least one of the ReferenceQuestionLists. I.E. it exists in a screener or recruitment survey.
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        private bool IsInReferenceList (string var)
        {
            foreach (List<string> refList in ReferenceQuestionLists)
            {
                if (refList.Contains(var))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if q1's refVarName is allowed to be in q2's PstP.This is allowed if q1's Qnum is greater than q2's, q2 refers to q1 at LSD or if there is a
        /// "go to q1" instruction in q2's PreP.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        private bool IsAllowedInPreP(SurveyQuestion q1, SurveyQuestion q2)
        {
            string refVar = q1.VarName.RefVarName;
            bool qnumGreater = q1.GetQnumValue() <= q2.GetQnumValue();
            bool refersToLSD = q2.PreP.Contains(refVar + "@LSD") || q2.PreP.Contains(refVar + " at LSD");
            bool hasGoTo = q2.PreP.Contains("go to " + refVar);

            return qnumGreater || refersToLSD || hasGoTo;
        }

        /// <summary>
        /// Returns true if q1's refVarName is allowed to be in q2's PreP. 
        /// This is allowed if q1's Qnum is less than or equal to q2's, 
        /// or if there is a "go back to q1" instruction in q2's PstP, 
        /// or if q2 is BI473 (essential questions list).
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        private bool IsAllowedInPstP(SurveyQuestion q1, SurveyQuestion q2)
        {
            string refVar = q1.VarName.RefVarName;
            bool qnumGreater = q1.GetQnumValue() >= q2.GetQnumValue();
            bool hasGoTo = q2.PstP.Contains("go back to " + refVar);
            bool isEssentialList = q1.VarName.RefVarName.Equals("BI473");

            return qnumGreater || hasGoTo || isEssentialList;
        }

        #endregion

        #region Series Check

        public List<SurveyQuestion> SeriesHomogeneityCheck()
        {
            List<SurveyQuestion> results = new List<SurveyQuestion>();
          
            SurveyQuestion refsq = QuestionList[0];

            foreach (SurveyQuestion sq in QuestionList)
            {
                if (!char.IsLetter(sq.Qnum[sq.Qnum.Length-1]))
                    continue;

                if (sq.VarName.RefVarName.EndsWith("o"))
                    continue;

                // hit first series, set the reference question and move to the next
                if (refsq == null || sq.Qnum.EndsWith("a"))
                {
                    refsq = sq;
                    continue;
                }

                // if the value of the qnums matches but not all the repeated fields match, add the reference and the current question to the list
                if (sq.GetQnumValue() == refsq.GetQnumValue() && !RepeatedWordingsMatch(sq, refsq))
                {
                    results.Add(sq);
                }
                else
                {
                    refsq = sq;
                }
            }

            return results;
        }

        private bool RepeatedWordingsMatch (SurveyQuestion sq1, SurveyQuestion sq2)
        {
            return sq1.PreP.Equals(sq2.PreP) &&
                sq1.PreI.Equals(sq2.PreI) &&
                sq1.PreA.Equals(sq2.PreA) &&
                sq1.PstI.Equals(sq2.PstI) &&
                sq1.PstP.Equals(sq2.PstP) &&
                sq1.RespOptions.Equals(sq2.RespOptions) &&
                sq1.NRCodes.Equals(sq2.NRCodes);
        }

        public List<string> SeriesCompare (List<SurveyQuestion> refList)
        {
            List<string> results = new List<string>();
            foreach (SurveyQuestion sq in QuestionList.Where(x=>x.IsSeries()))
            {
                SurveyQuestion refQ = refList.Where(x => x.VarName.RefVarName.Equals(sq.VarName.RefVarName)).FirstOrDefault();
                if (refQ!=null && !refQ.IsSeries())
                {
                    results.Add(sq.VarName + ";" + sq.Qnum + ";" + refQ.Qnum);
                }
            }

            return results;
        }

        #endregion


        public List<SurveyQuestion> GetBlankLabels()
        {
            return QuestionList.Where(x => x.VarName.Domain.ID == 0 || x.VarName.Topic.ID == 0 || x.VarName.Content.ID == 0 || x.VarName.Product.ID == 0).ToList();
        }

        public List<SurveyQuestion> GetYQnums()
        {
            return QuestionList.Where(x => x.Qnum.EndsWith("y")).ToList();
        }

        public List<CanonicalRefVarName> GetMissingCanonicalVars()
        {
            // check if all canonical vars are in the survey
            List<CanonicalRefVarName> results = new List<CanonicalRefVarName>();

            foreach (CanonicalRefVarName c in CanonVars)
                if (!QuestionList.Any(x => x.VarName.RefVarName.Equals(c.RefVarName)))
                    results.Add(c);

            return results;

        }

        public List<SurveyQuestion> TranslationVarsCheck()
        {
            return null;
            // check if all PreP vars are in the translation
            // check if all PstP vars are in the translation
        }
        


    }

    public class RoutingCheckQuestion : SurveyQuestion 
    {
        public List<string> LatePrePVars { get; set; }
        public List<string> MissingPrePVars { get; set; }
        public List<string> EarlyPstPVars { get; set; }
        public List<string> MissingPstPVars { get; set; }


        public RoutingCheckQuestion (SurveyQuestion sq)
        {
            LatePrePVars = new List<string>();
            MissingPrePVars = new List<string>();
            EarlyPstPVars = new List<string>();
            MissingPstPVars = new List<string>();

            VarName = new VariableName(sq.VarName.VarName);
            Qnum = sq.Qnum;
            PreP = sq.PreP;
            PreI = sq.PreI;
            PreA = sq.PreA;
            LitQ = sq.LitQ;
            PstI = sq.PstI;
            PstP = sq.PstP;
            RespOptions = sq.RespOptions;
            NRCodes = sq.NRCodes;
        }
    }
}
