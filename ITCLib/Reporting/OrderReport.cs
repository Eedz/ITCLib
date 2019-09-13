using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    class OrderReport: SurveyBasedReport
    {

        // order comparison
        public bool IncludeWordings { get; set; }          // true if order comparison is to include wordings
        public bool BySection { get; set; }                // true if order comparison should break the report into sections

        private int GenerateOrderReport()
        {
           // foreach (Survey s in Surveys)
           // {
                //s.GenerateSourceTable();
              

               // if (s.questions.Count == 0)
              //  {
              //      return 1;
              //  }
               


          //  }



            return 0;
        }

        #region Order Comparison
        private void CompareSurveyOrder()
        {

        }

        /// <summary>
        /// Returns a string describing the highlighting uses in the document.
        /// </summary>
        /// <returns></returns>
        public string HighlightingKey()
        {
            string currentSurv;
            string others = "";
            string primary = "";
           
            string orderChanges = "";
            bool showQnumOrder = false;
            string qnumorder = "";

            string finalKey = "";

            foreach (ReportSurvey s in Surveys)
            {
                currentSurv = s.SurveyCode;
                if (s.Backend != DateTime.Today)
                    currentSurv += " on " + s.Backend.ToString("d");

                if (!s.Primary)
                    others += ", " + currentSurv;
                else
                    primary += currentSurv;

                if (s.Qnum && s.ID != 1)
                {
                    showQnumOrder = true;
                    qnumorder = currentSurv;
                }


            }
            others = Utilities.TrimString(others, ", ");
           
            finalKey = "Highlighting key:  [yellow] In " + primary + " only [/yellow]";
           
            finalKey += "   [t]   In " + others + " only [/t] ";
            

            return finalKey;
        }
        #endregion
    }
}
