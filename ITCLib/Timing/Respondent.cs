using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ITCLib
{
    public class Respondent
    {
        public string Survey { get; set; }
        public string Description { get; set; }
        public List<Answer> Responses { get; set; }
        public double TotalTime { get; set; }
        public double TotalMaxTime { get; set; }
        public double TotalMinTime { get; set; }
        public double WeightedTime { get; set; }
        public double Weight { get; set; }
        public double MedianTime { get; set; }

        public Respondent()
        {
            Survey = "No survey specified";
            Description = "Non-descript";
            Responses = new List<Answer>();
            Weight = 0;
        }

        public Respondent (Respondent r)
        {
            Survey = r.Survey;
            Description = r.Description;
            Responses = new List<Answer>();
            foreach (Answer a in r.Responses)
            {
                if (a.Skipped)
                    this.AddSkip(a.VarName);
                else 
                    this.AddResponse(a.VarName, a.ResponseCode);
            }
            Weight = r.Weight;
        }

        public Respondent(Respondent r, bool overwrite)
        {
            Survey = r.Survey;
            Description = r.Description;
            Responses = new List<Answer>();
            foreach (Answer a in r.Responses)
            {
                if (a.Skipped)
                    this.AddSkip(a.VarName);
                else
                    this.AddResponse(a.VarName, a.ResponseCode, overwrite);
            }
            Weight = r.Weight;
        }

        public Respondent(string xmlDoc)
        {
            Responses = new List<Answer>();

            XmlDocument userData = new XmlDocument();

            userData.LoadXml(xmlDoc);

            Survey = userData.SelectSingleNode("/UserDefinition").Attributes["Survey"].InnerText;
            Description = userData.SelectSingleNode("/UserDefinition").Attributes["Description"].InnerText;
            TotalMaxTime = Double.Parse(userData.SelectSingleNode("/UserDefinition").Attributes["MaxTime"].InnerText) ;
            TotalMaxTime = Double.Parse(userData.SelectSingleNode("/UserDefinition").Attributes["MinTime"].InnerText) ;
            Weight = Double.Parse(userData.SelectSingleNode("/UserDefinition").Attributes["Weight"].InnerText) ;

            foreach (XmlNode r in userData.SelectNodes("/UserDefinition/Answers/Answer"))
            { 
                string var = r.Attributes["VarName"].InnerText;
                string code = r.Attributes["ResponseCode"].InnerText;
                bool skipped = Boolean.Parse(r.Attributes["Skipped"].InnerText);
                Answer a = new Answer(var, code);
                a.Skipped = skipped;
                Responses.Add(a);
            }
        }

        public string SaveToXML()
        {
            XmlDocument userData = new XmlDocument();

            XmlNode respondentNode = userData.CreateElement("UserDefinition");

            XmlAttribute survey = userData.CreateAttribute("Survey");
            XmlAttribute description = userData.CreateAttribute("Description");
            XmlAttribute weight = userData.CreateAttribute("Weight");
            XmlAttribute maxTime = userData.CreateAttribute("MaxTime");
            XmlAttribute minTime = userData.CreateAttribute("MinTime");

            survey.Value = Survey;
            description.Value = Description;
            weight.Value = Weight.ToString();
            maxTime.Value = TotalMaxTime.ToString();
            minTime.Value = TotalMinTime.ToString();

            respondentNode.Attributes.Append(survey);
            respondentNode.Attributes.Append(description);
            respondentNode.Attributes.Append(weight);
            respondentNode.Attributes.Append(maxTime);
            respondentNode.Attributes.Append(minTime);

            XmlNode answerNode = userData.CreateElement("Answers");
            foreach (Answer a in Responses)
            {
                XmlNode answer = userData.CreateElement("Answer");

                XmlAttribute varname = userData.CreateAttribute("VarName");
                XmlAttribute responseCode = userData.CreateAttribute("ResponseCode");
                XmlAttribute skipped = userData.CreateAttribute("Skipped");

                varname.Value = a.VarName;
                responseCode.Value = a.ResponseCode;
                skipped.Value = a.Skipped.ToString();

                answer.Attributes.Append(varname);
                answer.Attributes.Append(responseCode);
                answer.Attributes.Append(skipped);

                answerNode.AppendChild(answer);
            }

            respondentNode.AppendChild(answerNode);
            userData.AppendChild(respondentNode);
            return userData.InnerXml;
        }

        /// <summary>
        /// Adds a response item. Removes any previous response for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void AddResponse(string varname, int value)
        {
            RemoveAnswer(varname);
            Responses.Add(new Answer(varname, value));
        }

        /// <summary>
        /// Adds a response item. Removes any previous response for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void AddResponse(string varname, string value)
        {
            RemoveAnswer(varname);
            Responses.Add(new Answer(varname, value));
        }

        /// <summary>
        /// Adds a response item. Removes any previous response for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        //public void AddResponse(string varname, string value, double weight)
        //{
        //    RemoveAnswer(varname);
        //    Responses.Add(new Answer(varname, value, weight));
        //}

        /// <summary>
        /// Adds a response item. Removes any previous response for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void AddResponse(string varname, string value, bool overwrite)
        {
            if (overwrite)
                RemoveAnswer(varname);

            Responses.Add(new Answer(varname, value));
        }

        /// <summary>
        /// Adds a response in the form of a skip. Removes any response for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void AddSkip(string varname)
        {
            RemoveAnswer(varname);
            Responses.Add(new Answer(varname, true));
        }

        /// <summary>
        /// Removes all answers for the given varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        private void RemoveAnswer(string varname)
        {
            Responses.RemoveAll(x => x.VarName.Equals(varname));
        }

        /// <summary>
        /// Returns true if this respondent satisfies all the filters in the provided list.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool RespondentSatisfiesFilter(List<FilterInstruction> filters)
        {
            bool satisfied = false;
            int count = 0;

            foreach (FilterInstruction fi in filters)
            {
                satisfied = false;

                foreach (Answer a in Responses)
                {
                    if (a.SatisfiesFilter(fi))
                    {
                        count++;
                        satisfied = true;
                    }
                    if (satisfied) break;
                }

            }

            if (count == filters.Count)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return Description;
        }
    }


    public class Answer
    {
        public string VarName { get; set; }
        public string ResponseCode { get; set; }
        public int Value { get; set; }
        public bool Skipped { get; set; }

        public Answer (string varname, int val)
        {
            VarName = varname;
            Value = val;
            Skipped = false;
        }

        public Answer(string varname, string val)
        {
            VarName = varname;
            ResponseCode = val;
            Skipped = false;
        }

        public Answer(string varname, bool skip)
        {
            VarName = varname;
            ResponseCode = "";
            Skipped = skip;
        }

        /// <summary>
        /// Returns true if this answer satisfies the provided FilterInstruction.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public bool SatisfiesFilter(FilterInstruction fi)
        {
            bool result = false;

            // answer is skipped
            if (Skipped)
                return false;

            // wrong varname
            if (!VarName.Equals(fi.VarName))
                return false;

            // compare this answer with those in the filter instruction
            switch (fi.Oper)
            {
                case Operation.Equals:
                    // look for exact match in filter's values
                    // if this answer is a letter then see if the filter contains this letter
                    if (fi.ValuesStr.Any(x => char.IsLetter(x[0])) && fi.ValuesStr.Contains(ResponseCode))
                    {
                        return true;
                    }
                    // if this answer is not a letter, then see if any of the filter's values contain this answer as an integer
                    else if (!fi.ValuesStr.Any(x => char.IsLetter(x[0])) && fi.ValuesStr.Any(x => Int32.Parse(ResponseCode) == Int32.Parse(x)))
                    {
                        return true;
                    }
                    break;
                case Operation.GreaterThan:
                    // check if this answer is greater than any of the filter's values
                    if (fi.ValuesStr.Any(x => Int32.Parse(ResponseCode) > Int32.Parse(x)))
                    {
                        return true;
                    }
                    break;
                case Operation.LessThan:
                    // check if this answer is less than any of the filter's values
                    if (fi.ValuesStr.Any(x => Int32.Parse(ResponseCode) < Int32.Parse(x)))
                    {
                        return true;
                    }
                    break;
                case Operation.NotEquals:
                    // check if this answer does not appear in the filter's values
                    if (!fi.ValuesStr.Any(x => Int32.Parse(ResponseCode) == Int32.Parse(x)))
                    {
                        return true;
                    }
                    break;
            }

            return result;
        }

        public override string ToString()
        {
            if (Skipped)
                return VarName +"=NA";
            else if (string.IsNullOrEmpty(VarName) && string.IsNullOrEmpty(ResponseCode))
                return "";
            else 
                return VarName + "=" + ResponseCode;
        }
    }

    public class SelectableRespondent : Respondent 
    {
        public bool Selected { get; set; }
        public SelectableRespondent () : base ()
        {
            Selected = false;
        }

        public SelectableRespondent(Respondent r)
        {
            this.Survey = r.Survey;
            this.Responses = r.Responses;
            this.Description = r.Description;
            this.Weight = r.Weight;
        }
    }
}
