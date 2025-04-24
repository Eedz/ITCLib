using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITCLib;
using System.Text.RegularExpressions;

namespace ITCLib
{
    public class LinkedQuestion : SurveyQuestion
    {
        public Dictionary<int, LinkedQuestion> PossibleNext;

        public List<LinkedQuestion> FilteredOn;
        public List<List<FilterInstruction>> FilterList; 

        public double seconds { get; set; }
        public Weight Weight { get; set; }
        public string GetWeight { get { return VarName.RefVarName + ": " + Weight.Value; } }

        public LinkedQuestion()
        {
            FilteredOn = new List<LinkedQuestion>();
            PossibleNext = new Dictionary<int, LinkedQuestion>();
            FilterList = new List<List<FilterInstruction>>();
            Weight = new Weight();
        }

        public LinkedQuestion(SurveyQuestion q)
        {
            if (q != null)
            {
                this.ID = q.ID;
                this.SurveyCode = q.SurveyCode;
                this.VarName = q.VarName;
                this.Qnum = q.Qnum;
                this.PrePW = q.PrePW;
                this.PreIW = q.PreIW;
                this.PreAW = q.PreAW;
                this.LitQW = q.LitQW;
                this.PstIW = q.PstIW;
                this.PstPW = q.PstPW;
                this.RespOptionsS = q.RespOptionsS;
                this.NRCodesS = q.NRCodesS;
            }

            if (q.IsDerived()|| q.IsProgramming() || q.VarName.VarName.StartsWith("RS"))
                Internal = true;

            

            FilteredOn = new List<LinkedQuestion>();
            FilterList = new List<List<FilterInstruction>>();

            PossibleNext = new Dictionary<int, LinkedQuestion>();

            Weight = new Weight();
        }

        /// <summary>
        /// Returns the time in seconds, to read this question at a particular WPM.
        /// </summary>
        /// <param name="wpm"></param>
        /// <param name="includeNotes"></param>
        /// <returns></returns>
        public double GetTiming(int wpm, bool smart = true, bool includeNotes = false)
        {
         
            double time = WordCount(smart, includeNotes) * 60;

            return (double) time / wpm;

        }

        
        /// <summary>
        /// Returns the number of words in this question. A word is any sequence of characters surrounded by spaces. Only the PreA, LitQ and Responses are counted.
        /// </summary>
        /// <param name="includeNotes">True if interviewer notes are to be included in the count.</param>
        /// <returns></returns>
        public int WordCount(bool smart = true, bool includeNotes = false)
        {
            int result = 0;
            string text = "";
            // if Qnum ends in a, or doesn't end in a letter get whole question text
            if (Qnum.EndsWith("a") || Qnum.Length == 3)
            {
                if (includeNotes) text = PreIW.WordingText + " ";
                // try cuttin litq and prea in half if there are conditional wordings


                //if (PreA.Contains("<strong>"))
                //    text += PreA.Substring(0, PreA.Length / 2) + " ";
                //else
                //    text += PreA + " ";

                //if (LitQ.Contains("<strong>"))
                //    text += LitQ.Substring(0, LitQ.Length / 2) + " ";
                //else
                //    text += LitQ + " ";
                if (smart)
                {
                    if (HasConditionalWording(LitQW.WordingText))
                        text += LitQW.WordingText.Substring(0, LitQW.WordingText.Length / 2) + " ";
                    else
                        text += LitQW.WordingText + " ";

                    if (HasConditionalWording(PreAW.WordingText))
                        text += PreAW.WordingText.Substring(0, PreAW.WordingText.Length / 2) + " ";
                    else
                        text += PreAW.WordingText + " ";
                    text += RespOptionsS.RespList;
                }
                else
                {
                    text += PreAW.WordingText + " " + LitQW.WordingText + " " + RespOptionsS.RespList;
                }



                    
                if (includeNotes) text += " " + PstIW.WordingText;
            }
            else if (!Qnum.EndsWith("a") && Qnum.Length > 3)
            {
                // if Qnum ends in a letter just get the LitQ
                // if the PreI contains "checklist" and the response options are just Yes/No, use only half the LitQ
                if (PreIW.WordingText.Contains("checklist") && (RespOptionsS.RespSetName.Equals("select") || RespOptionsS.RespSetName.Equals("yesno")) || HasConditionalWording(LitQW.WordingText)) // and has yesno
                    text = LitQW.WordingText.Substring(0, LitQW.WordingText.Length / 2);
                else
                    text = LitQW.WordingText;
            }

            string[] words = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            result = words.Length;
            return result;
            
        }

        private void WordCount(out int qLength, out int rLength)
        {

            string qText = "";
            string rText = "";
            // if Qnum ends in a, or doesn't end in a letter get whole question text
            if (Qnum.EndsWith("a") || Qnum.Length == 3)
            {
                //text = lq.PreI + " " + lq.PreA + " " + lq.LitQ + " " + lq.RespOptions + " " + lq.PstI;
                qText = PreIW.WordingText + " " + PreAW.WordingText + " " + LitQW.WordingText + " " + PstIW.WordingText;
                rText = RespOptionsS.RespList;
            }
            else if (!Qnum.EndsWith("a") && Qnum.Length > 3)
            {
                qText = LitQW.WordingText;
                rText = "";
            }

            char[] delimiters = new char[] { ' ', '\r', '\n' };
            qLength = qText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            rLength = rText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;


        }


        private bool HasConditionalWording(string text)
        {
            if (Regex.IsMatch(text, "<strong>[A-Z][A-Z][0-9][0-9][0-9]") || Regex.IsMatch(text, "All:"))
                return true;
            else
                return false;
        }

        private int WordCountPreA()
        {

            return 0;
        }

        /// <summary>
        /// Returns a string of all filter instructions
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public string PrintDirectFilterInstructions()
        {
            string result = "";
            if (FilterList.Count == 0)
                return result + "(no filter)\r\n";
            else
            {
                foreach (List<FilterInstruction> fl in FilterList)
                {
                    foreach (FilterInstruction fi in fl)
                        result += fi + "\r\n";

                    result += "OR\r\n";
                }

                result = result.Trim("OR\r\n".ToCharArray());

            }

            return result;
        }

        /// <summary>
        /// Return a list of filter instructions, 1 per response option.
        /// </summary>
        /// <returns></returns>
        public List<FilterInstruction> GetFiltersByResponse()
        {
            List<FilterInstruction> responses = new List<FilterInstruction>();

            List<string> responseCodes = GetRespNumbers(); ;

            if (RespOptionsS.RespSetName.Equals("0") && !NRCodesS.RespSetName.Equals("0"))
            {
                List<string> responseCodesNR = GetNonRespNumbers();
                int firstNR = 0;
                if (responseCodesNR.Count != 0)
                    firstNR = Int32.Parse(responseCodesNR[0]);

                for (int i = 0; i < Math.Min(firstNR, 100); i++)
                {
                    responseCodes.Add(i.ToString());
                }
            }

            // for each response number, create a filter condition in the form [VarName]=response
            foreach (string response in responseCodes)
            {
                responses.Add(new FilterInstruction()
                {
                    VarName = VarName.RefVarName,
                    Oper = Operation.Equals,
                    ValuesStr = new List<string>() { response },
                    FilterExpression = VarName.RefVarName + "=" + response

                });

            }
            return responses;
        }

        public bool ValidResponse(Respondent r)
        {
            // if there is a response to this question, check that the response is not 7, 77 etc.
            bool respOK = false;
            var resps = r.Responses.Where(x => x.VarName.Equals(this.VarName.RefVarName));
            var valid = this.GetRespNumbers();

            if (valid.Count() == 0)
                return true;

            foreach (Answer a in resps)
            {
                if (valid.Contains(a.ResponseCode))
                {
                    respOK = true;
                    break;
                }
            }



            return respOK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private List<string> ListFilterVars()
        {
            List<string> result = new List<string>();


            foreach (LinkedQuestion lq in this.FilteredOn)
                if (!result.Contains(lq.VarName.RefVarName))
                    result.Add(lq.VarName.RefVarName);

            foreach (LinkedQuestion q in this.FilteredOn)
            {
                List<string> indirect = q.ListFilterVars();

                foreach (string i in indirect)
                    if (!result.Contains(i))
                        result.Add(i);
            }

            return result;
        }

        /// <summary>
        /// Returns the list of questions that filter the given question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private List<LinkedQuestion> ListFilters()
        {
            List<LinkedQuestion> result = new List<LinkedQuestion>();


            foreach (LinkedQuestion lq in this.FilteredOn)
                if (!result.Contains(lq))
                    result.Add(lq);

            foreach (LinkedQuestion q in this.FilteredOn)
            {
                List<LinkedQuestion> indirect = q.ListFilters();

                foreach (LinkedQuestion i in indirect)
                    if (!result.Contains(i))
                        result.Add(i);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of LinkedQuestions that indirectly filter the give LQ
        /// </summary>
        /// <param name="lq"></param>
        /// <returns></returns>
        private List<LinkedQuestion> GetIndirectFilterVars(LinkedQuestion lq)
        {
            //List<LinkedQuestion> result = new List<LinkedQuestion>();

            //foreach (LinkedQuestion q in lq.FilteredOn)
            //{
            //    if (q.Equals(lq))
            //    {
            //        result.Add(q);

            //    }
            //    result.AddRange(GetIndirectFilterVars(q));

            //}

            //return result;
            return null;
        }

        public bool QuestionHasFilter(FilterInstruction fi)
        {
            bool hasFilter = false;

            if (fi.ValuesStr.Count() == 0)
                return false;


            string fiValue = fi.ValuesStr[0];

            foreach (List<FilterInstruction> list in FilterList)
            {

                if (fiValue.Length == 1 && char.IsLetter(Char.Parse(fiValue)))
                {
                    // check for letter-type response (C or P usually)
                    if (list.Any(x => x.VarName.Equals(fi.VarName) && x.ValuesStr.Contains(fiValue)))
                    {
                        hasFilter = true;
                        break;
                    }
                }

                else
                {
                    if (list.Any(x => x.Oper == Operation.Equals && x.VarName.Equals(fi.VarName) && x.ValuesStr.Contains(Int32.Parse(fiValue).ToString())))
                    {
                        hasFilter = true;
                        break;
                    }
                    else if (list.Any(x => x.Oper == Operation.NotEquals && x.VarName.Equals(fi.VarName) && !x.ValuesStr.Contains(Int32.Parse(fiValue).ToString())))
                    {
                        hasFilter = true;
                        break;
                    }
                    else if (list.Any(x => x.Oper == Operation.GreaterThan && x.VarName.Equals(fi.VarName) && x.ValuesStr.Any(y => Int32.Parse(y) < Int32.Parse(fiValue))))
                    {
                        // check if fiValue is less than all list values
                        hasFilter = true;
                        break;
                    }
                    else if (list.Any(x => x.Oper == Operation.LessThan && x.VarName.Equals(fi.VarName) && x.ValuesStr.Any(y => Int32.Parse(y) > Int32.Parse(fiValue))))
                    {
                        // check if fiValue is greater than all list values
                        hasFilter = true;
                        break;
                    }


                    // 
                    // TODO <>
                    //if (list.Any(x => x.VarName.Equals(fi.VarName)))

                    //{
                    //    if (list.Any(x=>x.ValuesStr.Contains(Int32.Parse(fi.ValuesStr[0]).ToString())))
                    //    hasFilter = true;
                    //    break;
                    //}
                }
            }

            return hasFilter;
        }

        // return a filter instruction that contains the provided filter instructions VarName and value
        public string GetFilterExpression(FilterInstruction fi)
        {
            string result = "";
            bool foundIt = false;
            // get the filter instruction that contains the fi
            foreach (List<FilterInstruction> filterList in FilterList)
            {
                foreach (FilterInstruction f in filterList)
                {
                    if (f.VarName.Equals(fi.VarName) && f.ContainsResponse(fi.ValuesStr[0]))
                    {
                        result += f.FilterExpression;
                        foundIt = true;
                        break;
                    }
                }
                if (foundIt)
                    break;
            }

            return result;
        }


        // TODO IN PROGRESS
        public string ExpandPreP()
        {
            if (FilteredOn.Count() == 0 || FilterList.Count() == 0)
                return PrePW.WordingText;

            string expanded = PrePW.WordingText;

            //foreach (FilterInstruction fi in )

                return expanded;
        }

        public bool FilteredOnRtypeOnly()
        {
            bool rtypeOnly = false;

            if (FilteredOn.Count() > 1)
                return false;

            if (FilteredOn.Count() == 1 && FilteredOn[0].VarName.RefVarName.ToLower().Equals("rtype"))
                return true;

            return rtypeOnly;
        }


        public override string ToString()
        {
            return VarName.RefVarName;

            //string output = VarName.VarName + " -> ";
            //foreach (KeyValuePair<int, LinkedQuestion> p in PossibleNext)
            //{
            //    if (p.Value == null)
            //    {
            //        output += "blank,";
            //    }
            //    else
            //    {
            //        output += "(" + p.Key + ") " + p.Value.VarName.VarName + ",";
            //    }
            //}
            //return output;
        }

        

    }

    public class LinkedQuestionComparer : IEqualityComparer<LinkedQuestion>
    {
        public bool Equals(LinkedQuestion x, LinkedQuestion y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            bool s = string.Equals(x.VarName.RefVarName, y.VarName.RefVarName, StringComparison.OrdinalIgnoreCase);
            return string.Equals(x.VarName.RefVarName, y.VarName.RefVarName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(LinkedQuestion obj)
        {
            int refV = obj.VarName.RefVarName.ToLower().GetHashCode();
            return refV;
        }
    }

    public class Weight
    {
        public double Value { get; set; }
        public string Source { get; set; }

        public Weight()
        {
            Value = -1;
            Source = "unknown";
        }

        public Weight(double v, string s)
        {
            Value = v;
            Source = s;
        }
    }

}
