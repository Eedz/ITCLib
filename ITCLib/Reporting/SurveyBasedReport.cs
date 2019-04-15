using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;

namespace ITCLib
{
    public class SurveyBasedReport : ITCReport
    {
      
        public BindingList<ReportSurvey> Surveys { get; set; } 

        public bool VarChangesCol { get; set; }
        public bool SubsetTables { get; set; }
        public bool SubsetTablesTranslation { get; set; }
        public bool ShowAllQnums { get; set; }
        public bool ShowAllVarNames { get; set; }
        public bool ShowQuestion { get; set; }
        public bool ShowSectionBounds { get; set; } // true if, for each heading question, we should include the first and last question in that section

        /// <summary>
        /// Initializes a new instance of the SurveyBasedReport class.
        /// </summary>
        public SurveyBasedReport() : base()
        {

            Surveys = new BindingList<ReportSurvey>();

            ShowQuestion = true;
        }

        /// <summary>
        /// Sets the column order based on properties of this object.
        /// Enumeration
        /// Surveys and their "extra fields"
        /// 
        /// </summary>
        public override void UpdateColumnOrder()
        {
            // enumeration
            switch (Numbering)
            {
                case Enumeration.Qnum:

                    break;
                case Enumeration.AltQnum:
                    break;
                case Enumeration.Both:
                    break;
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string ReportFileName()
        {
            string finalfilename = "";
            string surveyCodes = "";
            

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ToString("d"); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            finalfilename = surveyCodes;
            if (Details != "") { finalfilename += ", " + Details; }
            if (!Batch) { finalfilename += " generated"; }

            return finalfilename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addDate"></param>
        /// <returns>String</returns>
        public string ReportTitle(bool addDate = false)
        {
            string title = "";
            string surveyCodes = "";

            if (Surveys.Count == 1)
            {
                title = Surveys[0].Title;
                if (Surveys[0].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[0].Backend.ToString(); }
                return title;
            }

            for (int i = 0; i < Surveys.Count; i++)
            {
                surveyCodes += Surveys[i].SurveyCode;
                if (Surveys[i].Backend != DateTime.Today) { surveyCodes += " on " + Surveys[i].Backend.ToString(); }
                surveyCodes += " vs. ";
            }
            // trim off " vs. "
            if (surveyCodes.EndsWith(" vs. ")) { surveyCodes = surveyCodes.Substring(0, surveyCodes.Length - 5); }
            title += surveyCodes;
            if (addDate) { title += ", Generated on " + DateTime.Today.ToString("d").Replace("-", ""); }

            return title;
        }

        /// <summary>
        /// Builds a table using the lists of questions, comments, translations etc.
        /// </summary>
        public DataTable MakeFinalTable(ReportSurvey s)
        {

            DataTable finalTable;
            List<string> columnNames = new List<string>();
            List<string> columnTypes = new List<string>();
            string questionColumnName = GetQuestionColumnName(s);


            // construct finalTable
            // finalTable will have fields for ID, Qnum, VarName, Question Text, and Labels
            // then comments, translations, filters will be added after the main table is finished

            columnNames.Add("ID");
            columnTypes.Add("int");

            columnNames.Add("SortBy");
            columnTypes.Add("string");

            columnNames.Add("Qnum");
            columnTypes.Add("string");

            columnNames.Add("AltQnum");
            columnTypes.Add("string");

            columnNames.Add("VarName");
            columnTypes.Add("string");

            columnNames.Add("refVarName");
            columnTypes.Add("string");

            columnNames.Add(questionColumnName);
            columnTypes.Add("string");

            columnNames.Add("VarLabel");
            columnTypes.Add("string");

            columnNames.Add("Domain");
            columnTypes.Add("string");

            columnNames.Add("Topic");
            columnTypes.Add("string");

            columnNames.Add("Content");
            columnTypes.Add("string");

            columnNames.Add("Product");
            columnTypes.Add("string");

            if (s.CommentFields != null && s.CommentFields.Count != 0)
            {
                columnNames.Add("Comments");
                columnTypes.Add("string");
            }

            foreach (string lang in s.TransFields)
            {
                columnNames.Add(questionColumnName + " " + lang);
                columnTypes.Add("string");
            }

            if (s.FilterCol)
            {
                columnNames.Add("Filters");
                columnTypes.Add("string");
            }

            columnNames.Add("CorrectedFlag");
            columnTypes.Add("bool");

            columnNames.Add("TableFormat");
            columnTypes.Add("bool");

            if (ShowSectionBounds)
            {
                columnNames.Add(questionColumnName + " FirstVarName");
                columnTypes.Add("string");

                columnNames.Add(questionColumnName + " LastVarName");
                columnTypes.Add("string");
            }

            // create the final table 
            finalTable = Utilities.CreateDataTable(s.SurveyCode + s.ID + "_Final", columnNames.ToArray(), columnTypes.ToArray());

            DataRow newrow;

            // for each question, edit the fields according to the chosen options,
            // then add the fields to a new row in the final table.
            foreach (SurveyQuestion q in s.Questions)
            {

                // insert Qnums before variable names
                if (QNInsertion)
                {
                    s.InsertQnums(q, Numbering);
                    s.InsertOddQnums(q, Numbering); // TODO implement
                }

                // insert Country codes into variable names
                if (CCInsertion) s.InsertCountryCodes(q);

                // remove long lists in response option column
                if (!ShowLongLists && !String.IsNullOrEmpty(q.RespOptions))
                {
                    if (Utilities.CountLines(q.RespOptions) >= 25)
                    {
                        q.RespOptions = "[center](Response options omitted)[/center]";
                    }
                }

                // NRFormat
                if (NrFormat != ReadOutOptions.Neither && !string.IsNullOrEmpty(q.NRCodes))
                {
                    q.NRCodes = s.FormatNR(q.NRCodes, NrFormat);
                }

                // semi tel

                // in-line routing
                if (InlineRouting && !String.IsNullOrEmpty(q.PstP))
                {
                    s.FormatRouting(q);
                }

                // subset tables
                if (SubsetTables)
                {
                    if (SubsetTablesTranslation)
                    {
                        // TODO translation subset tables
                    }
                    else
                    {
                        if (q.TableFormat && q.Qnum.EndsWith("a"))
                        {
                            q.RespOptions = "[TBLROS]" + q.RespOptions;
                            q.NRCodes = q.NRCodes + "[TBLROE]";
                            q.LitQ = "[LitQ]" + q.LitQ + "[/LitQ]";
                        }
                    }
                }

                // varname changes
                if (VarChangesCol && !String.IsNullOrEmpty(q.VarName) && !q.VarName.StartsWith("Z"))
                {
                    q.VarName = q.VarName + " " + q.PreviousNames;
                }

                // corrected 
                if (q.CorrectedFlag)
                {
                    if (s.Corrected) { q.VarName = q.VarName + "\r\n" + "[C]"; }
                    else { q.VarName = q.VarName + "\r\n" + "[A]"; }
                }

                // now we can add the fields to a DataRow to be inserted into the final table
                newrow = finalTable.NewRow();

                newrow["ID"] = q.ID;
                newrow["SortBy"] = q.Qnum;
                newrow["Qnum"] = q.GetQnum();
                newrow["VarName"] = q.VarName;
                newrow["refVarName"] = q.refVarName;

                // concatenate the question fields, and if this is varname BI104, attach the essential questions list
                newrow[questionColumnName] = q.GetQuestionText(s.StdFieldsChosen);
                if (q.refVarName.Equals("BI104"))
                {
                    newrow[questionColumnName] += "\r\n<strong>" + s.EssentialList + "</strong>";
                }

                // labels (only show labels for non-headings)
                if (!q.VarName.StartsWith("Z") || !ShowQuestion)
                {
                    newrow["VarLabel"] = q.VarLabel;
                    newrow["Topic"] = q.Topic.LabelText;
                    newrow["Content"] = q.Content.LabelText;
                    newrow["Domain"] = q.Domain.LabelText;
                    newrow["Product"] = q.Product.LabelText;
                }

                // comments
                try
                {
                    foreach (QuestionComment c in q.Comments)
                        newrow["Comments"] += c.GetComments() + "\r\n\r\n";
                }
                catch
                {

                }

                // translations
                foreach (string lang in s.TransFields)
                    newrow[questionColumnName + " " + lang] = q.GetTranslationText(lang).Replace("<br>", "\r\n");

                // filters
                if (s.FilterCol)
                {
                    newrow["Filters"] = q.Filters;
                }

                newrow["CorrectedFlag"] = q.CorrectedFlag;
                newrow["TableFormat"] = q.TableFormat;

                // section bounds
                if (ShowSectionBounds) {
                    newrow[questionColumnName + " FirstVarName"] =s.GetSectionLowerBound(q);
                    newrow[questionColumnName + " LastVarName"] = s.GetSectionUpperBound(q);
                }

                // now add a new row to the finalTable DataTable
                // the new row will be a susbet of columns in the rawTable, after the above modifications have been applied
                finalTable.Rows.Add(newrow);
            }


            string questionFilter = s.GetQuestionFilter();
            if (!questionFilter.Equals(""))
            {
                try
                {
                    finalTable = finalTable.Select(questionFilter).CopyToDataTable().Copy();
                }
                catch (InvalidOperationException)
                {
                    return null;// filters resulted in 0 records
                }

            }

            // change the primary key to be the refVarName column
            // so that surveys from differing countries can still be matched up
            finalTable.PrimaryKey = new DataColumn[] { finalTable.Columns["refVarName"] };

            // remove unneeded fields

            if (!ShowQuestion)
                finalTable.Columns.Remove(questionColumnName);

            // check enumeration and delete AltQnum
            if (Numbering == Enumeration.Qnum)
                finalTable.Columns.Remove("AltQnum");

            if (Numbering == Enumeration.AltQnum)
                finalTable.Columns.Remove("Qnum");

            if (!s.DomainLabelCol)
                finalTable.Columns.Remove("Domain");

            if (!s.TopicLabelCol)
                finalTable.Columns.Remove("Topic");

            if (!s.ContentLabelCol)
                finalTable.Columns.Remove("Content");

            if (!s.VarLabelCol)
                finalTable.Columns.Remove("VarLabel");

            if (!s.ProductLabelCol)
                finalTable.Columns.Remove("Product");


            // these are no longer needed
            finalTable.Columns.Remove("CorrectedFlag");
            finalTable.Columns.Remove("TableFormat");
            finalTable.Columns.Remove("ID");

            return finalTable;
        }

        /// <summary>
        /// Returns the name of the column, in the final survey table, containing the question text.
        /// </summary>
        /// <returns>Returns: string.</returns>
        protected string GetQuestionColumnName(ReportSurvey s)
        {
            string column = "";
            column = s.SurveyCode.Replace(".", "");
            if (!s.Backend.Equals(DateTime.Today)) { column += "_" + s.Backend.ToString("d"); }
            if (s.Corrected) { column += "_Corrected"; }
            if (s.Marked) { column += "_Marked"; }
            return column;
        }

        // Returns the survey object that has been designated primary
        public ReportSurvey GetPrimarySurvey()
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].Primary) { s = Surveys[i]; break; }
            }
            return s;
        }
        // Returns the survey object that defines the Qnum order
        public ReportSurvey QnumSurvey()
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].Qnum) { s = Surveys[i]; break; }
            }
            return s;
        }

        // Add a Survey object to the list of surveys and set it's ID to the next available number starting with 1
        public void AddSurvey(ReportSurvey s)
        {
            int newID = 1;
            Surveys.Add(s);

            while (GetSurvey(newID) != null)
            {
                newID++;
            }
            if (newID == 1)
                s.Qnum = true;
            else
                s.Qnum = false;

            s.ID = newID;
            ColumnOrder.Add(new ReportColumn(s.SurveyCode + " " + s.Backend.ToString("d"), ColumnOrder.Count + 1));
        }

        // TODO implement
        public void RemoveSurvey(ReportSurvey s)
        {
           
        }

        // Returns the first survey object matching the specified code.
        public ReportSurvey GetSurvey(string code)
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].SurveyCode == code) { s = Surveys[i]; break; }
            }
            return s;
        }

        // Returns the first survey object matching the specified id.
        public ReportSurvey GetSurvey(int id)
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].ID == id) { s = Surveys[i]; break; }
            }
            return s;

        }

        public string[] SurveyCodes() { return null; }

        
    }
}
