﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    /// <summary>
    /// Base class for reports that display one or more surveys. 
    /// </summary>
    public class SurveyBasedReport : IReport, INotifyPropertyChanged
    {
      
        public BindingList<ReportSurvey> Surveys { get; set; } 

        public bool VarChangesCol { get; set; }
        public bool SurvNotes { get; set; }
        public bool VarChangesApp { get; set; }
        public bool ExcludeTempChanges { get; set; }

        // comparison?
        public bool CompareWordings { get; set; }

        // formatting options
        public bool SemiTel { get; set; }
        public bool SubsetTables { get; set; }
        public bool SubsetTablesTranslation { get; set; }
        public bool ShowAllQnums { get; set; }
        public bool ShowAllVarNames { get; set; }
        public bool ShowQuestion { get; set; }
        public bool ShowSectionBounds { get; set; } // true if, for each heading question, we should include the first and last question in that section

        public DataTable ReportTable { get; set; } // the final report table, which will be output to Word

        public ReportTypes ReportType { get; set; }
        public bool Batch { get; set; }

        public string FileName { get; set; } // this value will initially contain the path up to the file name, which will be added in the Output step

        // formatting and layout options
        public ReportFormatting Formatting { get; set; }
        public ReportLayout LayoutOptions { get; set; }

        public bool RepeatedHeadings { get; set; }
        public bool ColorSubs { get; set; }

        public bool InlineRouting { get; set; }
        public bool ShowLongLists { get; set; }
        public bool QNInsertion { get; set; }
        public bool AQNInsertion { get; set; }
        public bool CCInsertion { get; set; }

        public List<ReportColumn> ColumnOrder { get; set; }

        public ReadOutOptions NrFormat { get; set; }
        public Enumeration Numbering { get; set; }


        public string Details { get; set; }
        public string ReportStatus
        {
            get
            {
                return this._reportStatus;
            }
            set
            {
                if (value != this._reportStatus)
                {
                    this._reportStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // other details        
        public bool Web
        {
            get { return _web; }
            set
            {
                _web = value;
                LayoutOptions.CoverPage = value;
                if (_web)
                    LayoutOptions.FileFormat = FileFormats.PDF;
                else
                    LayoutOptions.FileFormat = FileFormats.DOC;
            }
        }
        

        /// <summary>
        /// Initializes a new instance of the SurveyBasedReport class.
        /// </summary>
        public SurveyBasedReport()
        {

            Surveys = new BindingList<ReportSurvey>();
            CompareWordings = true;

            Formatting = new ReportFormatting();
            LayoutOptions = new ReportLayout();

            RepeatedHeadings = true;
            ColorSubs = true;

            // intialize the column order collection with the default columns
            ColumnOrder = new List<ReportColumn>
            {
                new ReportColumn("Qnum", 1),
                new ReportColumn("VarName", 2)
            };

            Numbering = Enumeration.Qnum;
            NrFormat = ReadOutOptions.Neither;

            ReportType = ReportTypes.Standard;

            VarChangesCol = false;
            ExcludeTempChanges = true;

            FileName = "";
            Details = "";
            ReportStatus = "Generating report...";
            ShowQuestion = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //public IReadOnlyCollection<ReportSurvey> Surveys
        //{
        //    get { return surveys.AsReadOnly(); }
        //}

        /// <summary>
        /// Format the header row so with the appropriate widths and titles
        /// </summary>
        /// <param name="doc"></param>
        public virtual void FormatColumns(Word.Document doc)
        {
            double widthLeft;
            float qnumWidth = 0.51f;
            float altqnumWidth = 0.86f;
            float varWidth = 0.9f;
            float tcWidth = 1.2f;
            float respWidth = 0.86f;
            float commentWidth = 1f;
            int qCol;
            int otherCols;
            int numCols;
            string header;
            switch (LayoutOptions.PaperSize)
            {
                case PaperSizes.Letter: widthLeft = 10.5; break;
                case PaperSizes.Legal: widthLeft = 13.5; break;
                case PaperSizes.Eleven17: widthLeft = 16.5; break;
                case PaperSizes.A4: widthLeft = 11; break;
                default: widthLeft = 10.5; break;
            }
            // Qnum and VarName
            otherCols = 2;

            if (Numbering == Enumeration.Both)
            {
                qCol = 4;
                otherCols++; // AltQnum
            }
            else
            {
                qCol = 3;
            }

            doc.Tables[1].AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitFixed);

            numCols = doc.Tables[1].Columns.Count;

            for (int i = 1; i <= numCols; i++)
            {
                // remove underscores
                doc.Tables[1].Rows[1].Cells[i].Range.Text = doc.Tables[1].Rows[1].Cells[i].Range.Text.Replace("_", " ");
                header = doc.Tables[1].Rows[1].Cells[i].Range.Text.TrimEnd('\r', '\a');

                switch (header)
                {
                    case "Qnum":
                        doc.Tables[1].Rows[1].Cells[i].Range.Text = "Q#";
                        doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                        widthLeft -= qnumWidth;
                        break;
                    case "AltQnum":
                        doc.Tables[1].Rows[1].Cells[i].Range.Text = "AltQ#";
                        doc.Tables[1].Columns[i].Width = altqnumWidth * 72;
                        widthLeft -= altqnumWidth;
                        break;
                    case "VarName":
                        doc.Tables[1].Columns[i].Width = varWidth * 72;
                        widthLeft -= varWidth;
                        break;
                    case "SortBy":
                        doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                        widthLeft -= qnumWidth;
                        break;
                    case "Comments":
                        doc.Tables[1].Columns[i].Width = commentWidth * 72;
                        widthLeft -= commentWidth;
                        break;
                    default:
                        // question column with date, format date
                        if (header.Contains(DateTime.Today.ToString("d").Replace("-", "")))
                        {
                            doc.Tables[1].Rows[1].Cells[i].Range.Text = doc.Tables[1].Rows[1].Cells[i].Range.Text.Replace(DateTime.Today.ToString("d"), "");
                        }

                        // an additional AltQnum column
                        if (header.Contains("AltQnum"))
                        {
                            doc.Tables[1].Columns[i].Width = altqnumWidth * 72;
                            widthLeft -= altqnumWidth;
                        }
                        else if (header.Contains("AltQnum")) // an additional Qnum column
                        {
                            doc.Tables[1].Columns[i].Width = qnumWidth * 72;
                            widthLeft -= qnumWidth;
                        }

                        // filter column
                        if (header.Contains("Filters"))
                        {
                            // TODO set to Verdana 9 font
                        }

                        // test these
                        //if (ReportType == ReportTypes.Order)
                        //{
                        //    if (header.Contains("VarName"))
                        //    {
                        //        doc.Tables[1].Columns[i].Width = varWidth * 72;
                        //        widthLeft -= varWidth;
                        //    }
                        //    else if (header.Contains("Qnum"))
                        //    {
                        //        doc.Tables[1].Columns[i].Width = (qnumWidth * 2) * 72;
                        //        widthLeft -= qnumWidth;
                        //    }
                        //    else if (header.Contains("Question"))
                        //    {
                        //        doc.Tables[1].Columns[i].Width = (float)3.5 * 72;
                        //        widthLeft -= 3.5;
                        //    }
                        //}

                        break;
                }

            }
            
            for (int i = qCol; i <= numCols; i++)
                doc.Tables[1].Columns[i].Width = (float)(widthLeft / (numCols - qCol + 1)) * 72;
        }

        /// <summary>
        ///  Automatically sets the primary survey to be the 2nd survey if there are 2 surveys, otherwise, the 1st survey.
        /// </summary>
        private void AutoSetPrimary()
        {
            if (Surveys.Count == 0) return;
            for (int i = 0; i < Surveys.Count; i++) { Surveys[i].Primary = false; }
            if (Surveys.Count == 2)
            {
                Surveys[1].Primary = true;
            }
            else
            {
                Surveys[0].Primary = true;
            }
        }

        /// <summary>
        /// Sets the column order based on properties of this object.
        /// Enumeration
        /// Surveys and their "extra fields"
        /// 
        /// </summary>
        public void UpdateColumnOrder()
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

        public void RemoveColumn(string name)
        {
            for (int i = 0; i < ColumnOrder.Count; i++)
                if (ColumnOrder[i].ColumnName == name)
                {
                    ColumnOrder.RemoveAt(i);
                    break;
                }
        }

        public void AddColumn(string name)
        {
            int count = ColumnOrder.Count;

            ColumnOrder.Add(new ReportColumn(name, count + 1));
        }

        /// <summary>
        /// Adds a new item to the collection of report columns. The ordinal is always 1 more than the number of columns, making the new column the right most column.
        /// </summary>
        /// <param name="name"></param>
        public void AddColumn(string name, int ordinal)
        {
            int count = ColumnOrder.Count;

            for (int i = ordinal ; i<ColumnOrder.Count; i++)
            {
                ColumnOrder[i].Ordinal = i + 1;
            }

            ColumnOrder.Add(new ReportColumn(name, ordinal));
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
        /// Builds a table using the provided ReportSurvey data.
        /// </summary>
        /// <param name="s">Report survey</param>
        public DataTable MakeFinalTable(ReportSurvey s)
        {
            DataTable finalTable;
            DataRow newrow;
            string questionColumnName = GetQuestionColumnName(s);
            string varname; // (potentially) edited VarName field
            string questionFilter;

            // construct finalTable
            // finalTable will have fields for ID, Qnum, VarName, Question Text, and Labels by default
            // comments, translations, filters will be added if needed
            finalTable = new DataTable();
            finalTable.Columns.Add("ID", Type.GetType("System.Int32"));
            finalTable.Columns.Add("SortBy", Type.GetType("System.String"));
            finalTable.Columns.Add("Qnum", Type.GetType("System.String"));
            finalTable.Columns.Add("AltQnum", Type.GetType("System.String"));
            finalTable.Columns.Add("VarName", Type.GetType("System.String"));
            finalTable.Columns.Add("refVarName", Type.GetType("System.String"));
            finalTable.Columns.Add(questionColumnName, Type.GetType("System.String"));
            finalTable.Columns.Add("VarLabel", Type.GetType("System.String"));
            finalTable.Columns.Add("Domain", Type.GetType("System.String"));
            finalTable.Columns.Add("Topic", Type.GetType("System.String"));
            finalTable.Columns.Add("Content", Type.GetType("System.String"));
            finalTable.Columns.Add("Product", Type.GetType("System.String"));
            finalTable.Columns.Add("CorrectedFlag", Type.GetType("System.Boolean"));
            finalTable.Columns.Add("TableFormat", Type.GetType("System.Boolean"));

            // comment column
            if (s.CommentFields != null && s.CommentFields.Count != 0)
                finalTable.Columns.Add("Comments", Type.GetType("System.String"));
            // translation column
            foreach (string lang in s.TransFields)
                finalTable.Columns.Add(questionColumnName + " " + lang, Type.GetType("System.String"));
            // filter columns
            if (s.FilterCol)          
                finalTable.Columns.Add("Filters", Type.GetType("System.String"));
            // section bounds
            if (ShowSectionBounds)
            {
                finalTable.Columns.Add(questionColumnName + " FirstVarName", Type.GetType("System.String"));
                finalTable.Columns.Add(questionColumnName + " LastVarName", Type.GetType("System.String"));
            }

            // for each question, edit the fields according to the chosen options,
            // then add the fields to a new row in the final table.
            foreach (SurveyQuestion q in s.Questions)
            {
                // create a deep copy of just the wordings so that we can format them without affecting the original wordings
                SurveyQuestion wordings = q.DeepCopyWordings(); 

                // insert Qnums before variable names
                if (QNInsertion)
                {
                    s.InsertQnums(wordings, Numbering);
                    s.InsertOddQnums(wordings, Numbering); // TODO implement
                }

                // insert Country codes into variable names
                if (CCInsertion) s.InsertCountryCodes(wordings);

                // remove long lists in response option column
                if (!ShowLongLists && Utilities.CountLines(q.RespOptions) >= 25)
                    wordings.RespOptions = "[center](Response options omitted)[/center]";
                
                // NRFormat
                if (NrFormat != ReadOutOptions.Neither && !string.IsNullOrEmpty(q.NRCodes))
                    wordings.NRCodes = s.FormatNR(q.NRCodes, NrFormat);

                // TODO semitel

                // in-line routing
                if (InlineRouting && !String.IsNullOrEmpty(q.PstP))
                    s.FormatRouting(wordings);

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
                            wordings.RespOptions = "[TBLROS]" + wordings.RespOptions;
                            wordings.NRCodes += "[TBLROE]";
                            wordings.LitQ = "[LitQ]" + wordings.LitQ + "[/LitQ]";
                        }
                    }
                }

                // edit VarName, but don't edit the SurveyQuestion's VarName field, since this would update the refVarName field as well
                varname = q.VarName;

                // varname changes
                if (VarChangesCol && !string.IsNullOrEmpty(q.VarName) && !q.VarName.StartsWith("Z") && !string.IsNullOrEmpty(q.PreviousNames))
                    varname += " " + q.PreviousNames;
 
                // corrected 
                if (q.CorrectedFlag)
                {
                    if (s.Corrected)
                        varname += "\r\n" + "[C]";
                    else
                        varname += "\r\n" + "[A]";
                }

                // now we can add the fields to a DataRow to be inserted into the final table
                newrow = finalTable.NewRow();

                newrow["ID"] = q.ID;
                newrow["SortBy"] = q.Qnum;
                newrow["Qnum"] = q.GetQnum();
                newrow["VarName"] = varname;
                newrow["refVarName"] = q.RefVarName;

                // concatenate the question fields, and if this is varname BI104, attach the essential questions list
                newrow[questionColumnName] = wordings.GetQuestionText(s.StdFieldsChosen);
                if (q.RefVarName.Equals("BI104"))
                    newrow[questionColumnName] += "\r\n<strong>" + s.EssentialList + "</strong>";

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
                    newrow[questionColumnName + " " + lang] = wordings.GetTranslationText(lang).Replace("<br>", "\r\n");

                // filters
                if (s.FilterCol)
                    newrow["Filters"] = q.Filters;

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

            // apply the question filters

            questionFilter = s.GetQuestionFilter();
            if (!questionFilter.Equals(""))
            {
                try {
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
            if (!s.Backend.Equals(DateTime.Today)) column += "_" + s.Backend.ToString("d"); 
            if (s.Corrected) column += "_Corrected"; 
            if (s.Marked) column += "_Marked"; 
            return column;
        }

        // Returns the survey object that has been designated primary
        public ReportSurvey PrimarySurvey()
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].Primary)
                {
                    s = Surveys[i];
                    break;
                }
            }
            return s;
        }
        // Returns the survey object that defines the Qnum order
        public ReportSurvey QnumSurvey()
        {
            ReportSurvey s = null;
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (Surveys[i].Qnum)
                {
                    s = Surveys[i];
                    break;
                }
            }
            return s;
        }

        public List<ReportSurvey> NonPrimarySurveys()
        {
            List<ReportSurvey> s = new List<ReportSurvey>();
            for (int i = 0; i < Surveys.Count; i++)
            {
                if (!Surveys[i].Primary)
                    s.Add(Surveys[i]);
            }
            return s;
        }

        // Add a Survey object to the list of surveys and set it's ID to the next available number starting with 1
        public void AddSurvey(ReportSurvey s)
        {
            int newID = 1;
            
            Surveys.Add(s);

            SetQnumSurvey();

            while (GetSurvey(newID) != null)
            {
                newID++;
            }

            s.ID = newID;

            AutoSetPrimary();
            //TODO add to columnorder collection ColumnOrder.Add(new ReportColumn(s.SurveyCode + " " + s.Backend.ToString("d"), ColumnOrder.Count + 1));
        }

        /// <summary>
        /// Remove a survey from the list
        /// </summary>
        /// <param name="s"></param>
        /// <remarks>Update the primary survey, then renumber the remaining surveys.</remarks>
        public void RemoveSurvey(ReportSurvey s)
        {
            Surveys.Remove(s);

            AutoSetPrimary();

            SetQnumSurvey();

            // renumber surveys
            for (int i = 1; i <= Surveys.Count; i ++)
            {
                Surveys[i-1].ID = i;
            }
            // TODO remove columns from columnorder collection
        }

        private void SetQnumSurvey()
        {
            if (Surveys.Count == 0) return;

            if (QnumSurvey() != null)
                return;

            Surveys[0].Qnum = true;
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

        private bool _web;
        private string _reportStatus;
    }
}
