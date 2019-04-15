using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ITCLib
{
    public enum AccessLevel { SMG = 1, PMG }
    public enum CommentDetails { Existing = 1, LastUsed, New }
    /// <summary>
    /// Static class for interacting with the Database. TODO create stored procedures on server for each of these
    /// </summary>
    public static partial class DBAction
    {

        public static List<string> GetAllRefVars()
        {
            List<string> refVarNames = new List<string>();
          
            string query = "SELECT refVarName FROM qryVariableInfo GROUP BY refVarName ORDER BY refVarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            refVarNames.Add((string)rdr["refVarName"]);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return refVarNames;
        }

        public static List<string> GetAllRefVars(string surveyCode)
        {
            List<string> refVarNames = new List<string>();

            string query = "SELECT refVarName FROM qrySurveyQuestions WHERE Survey =@survey GROUP BY refVarName ORDER BY refVarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyCode);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            refVarNames.Add((string)rdr["refVarName"]);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return refVarNames;
        }

        public static List<string> GetVarNamesByRef(string refVarName)
        {
            List<string> VarNames = new List<string>();

            string query = "SELECT VarName FROM qryVariableInfo WHERE refVarName =@refVarName GROUP BY VarName ORDER BY VarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refVarName", refVarName);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            VarNames.Add((string)rdr["VarName"]);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return VarNames;
        }

        public static bool VarNameExists(string varname)
        {
            bool result = false; ;
            string query = "SELECT VarName FROM qryVariableInfo WHERE VarName =@varName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varName", varname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        if (rdr.Read())
                            result = true;
                        

                        

                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool RefVarNameExists(string refvarname)
        {
            bool result = false; ;
            string query = "SELECT refVarName FROM qryVariableInfo WHERE refVarName =@refvarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refvarname", refvarname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        if (rdr.Read())
                            result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list of RefVariableName objects with the provided refVarName.
        /// </summary>
        /// <param name="refVarName"></param>
        /// <returns></returns>
        public static List<RefVariableName> GetRefVarNames(string refVarName)
        {
            List<RefVariableName> refVarNames = new List<RefVariableName>();
            RefVariableName rv;
            string query = "SELECT refVarName, VarLabel, DomainNum, Domain, TopicNum, Topic, ContentNum, Content, ProductNum, Product FROM qryVariableInfo WHERE refVarName = @refVarName " +
                "GROUP BY refVarName, VarLabel, DomainNum, Domain, TopicNum, Topic, ContentNum, Content, ProductNum, Product ORDER BY refVarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refVarName", refVarName);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            rv = new RefVariableName((string)rdr["refVarName"]);

                            rv.VarLabel = (string)rdr["VarLabel"];
                            rv.Domain = new DomainLabel((int)rdr["DomainNum"], (string)rdr["Domain"]);
                            rv.Topic = new TopicLabel((int)rdr["TopicNum"], (string)rdr["Topic"]);
                            rv.Content = new ContentLabel((int)rdr["ContentNum"], (string)rdr["Content"]);
                            rv.Product = new ProductLabel((int)rdr["ProductNum"], (string)rdr["Product"]);

                            refVarNames.Add(rv);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return refVarNames;
        }

        //
        // Survey Mode
        //
        public static List<SurveyMode> GetModeInfo()
        {
            List<SurveyMode> modes = new List<SurveyMode>();
            SurveyMode m;
            string query = "SELECT * FROM qryMode ORDER BY Mode";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            m = new SurveyMode
                            {
                                ID = (int)rdr["ID"],
                                Mode = (string)rdr["Mode"],
                                ModeAbbrev = (string)rdr["ModeAbbrev"]
                            };

                            modes.Add(m);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return modes;
        }

        //
        // Cohorts
        //
        public static List<SurveyCohort> GetCohortInfo()
        {
            List<SurveyCohort> cohorts = new List<SurveyCohort>();
            SurveyCohort c;
            string query = "SELECT * FROM FN_GetCohortInfo() ORDER BY Cohort";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            c = new SurveyCohort
                            {
                                ID = (int)rdr["ID"],
                                Cohort = (string)rdr["Cohort"],
                                Code = (string)rdr["Code"],
                                WebName = (string)rdr["WebName"],

                            };

                            cohorts.Add(c);

                        }

                    }
                }
                catch (Exception)
                {
                    int i=0;
                }
            }
            return cohorts;
        }

        //
        // Groups
        //
        public static List<SurveyUserGroup> GetGroupInfo()
        {
            List<SurveyUserGroup> groups = new List<SurveyUserGroup>();
            SurveyUserGroup g;
            string query = "SELECT * FROM FN_GetGroupInfo() ORDER BY [Group]";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            g = new SurveyUserGroup
                            {
                                ID = (int)rdr["ID"],
                                UserGroup = (string)rdr["Group"],
                                Code = (string)rdr["Code"],
                                WebName = (string)rdr["WebName"],

                            };

                            groups.Add(g);

                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return groups;
        }

        //
        // Region Info
        //
        public static List<Region> GetRegionInfo(bool getStudies = false)
        {
            List<Region> regions = new List<Region>();
            Region r;
            string query = "SELECT * FROM qryRegion";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            r = new Region
                            {
                                RegionID = (int)rdr["ID"],
                                RegionName = (string)rdr["Region"]
                                
                            };

                            if (getStudies)
                                r.Studies = GetStudies(r.RegionID);

                            regions.Add(r);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return regions;
        }

        //
        // Study Info
        //
        public static List<Study> GetStudyInfo(bool getWaves = false, bool getSurveys = false)
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudyInfo()";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            s =new Study
                            {
                                StudyID = (int)rdr["ID"],
                                CountryName = (string)rdr["Country"],
                                StudyName = (string)rdr["Study"],
                                CountryCode = Int32.Parse((string)rdr["CountryCode"]),
                                ISO_Code = (string)rdr["ISO_Code"],
                                AgeGroup = (string)rdr["AgeGroup"]
                            };

                            if (getWaves)
                                s.Waves = GetWaves(s.StudyID, getSurveys);

                            studies.Add(s);
                        }

                    }
                }
                catch (Exception)
                {
                    
                }
            }

            return studies;
        }

        public static List<Study> GetStudies(int regionID)
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudies(@regionID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@regionID", regionID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            s = new Study
                            {
                                StudyID = (int)rdr["ID"],
                                CountryName = (string)rdr["Country"],
                                StudyName = (string)rdr["Study"],
                                CountryCode = Int32.Parse((string)rdr["CountryCode"]),
                                ISO_Code = (string)rdr["ISO_Code"],
                                AgeGroup = (string)rdr["AgeGroup"]
                            };

                            studies.Add(s);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }

            return studies;
        }

        //
        // Wave Info
        //
        public static List<StudyWave> GetWaveInfo()
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM qryStudyWaves ORDER BY ISO_Code, Wave";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
   

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            w = new StudyWave
                            {
                                WaveID = (int)rdr["WaveID"],
                                ISO_Code = (string)rdr["ISO_Code"],
                                Wave = (double)rdr["Wave"]
                            };

                            

                            waves.Add(w);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return waves;
        }
        public static List<StudyWave> GetWaves(int studyID, bool getSurveys = false)
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM FN_GetWavesByStudy(@studyID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@studyID", studyID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            w = new StudyWave
                            {
                                WaveID = (int)rdr["WaveID"],
                                ISO_Code = (string)rdr["ISO_Code"],
                                Wave = (double)rdr["Wave"]
                            };

                            if (getSurveys)
                                w.Surveys = DBAction.GetSurveys(w.WaveID);

                            waves.Add(w);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return waves;
        }


        

        //
        // Variables
        //

        /// <summary>
        /// Returns the list of all heading variables for a specific survey.
        /// </summary>
        /// <param name="surveyFilter"></param>
        /// <returns></returns>
        public static List<Heading> GetHeadings(string surveyFilter)
        {
            List<Heading> headings = new List<Heading>();
            string query = "SELECT * FROM FN_GetHeadings(@survey)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyFilter);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            headings.Add(new Heading((string)rdr["Qnum"], (string) rdr["PreP"]));
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return headings;
        }

        /// <summary>
        /// Returns the list of all variable prefixes in use by a specific survey. TODO use a stored procedure/function for this (eliminate non-standard vars? or make it an option)
        /// </summary>
        /// <param name="surveyFilter"></param>
        /// <returns></returns>
        public static List<string> GetVariableList(string surveyFilter)
        {
            List<string> varnames = new List<string>();
            string query = "SELECT VarName FROM qrySurveyQuestions WHERE Survey =@survey GROUP BY VarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyFilter);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            varnames.Add((string)rdr["VarName"]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return varnames;
        }

        /// <summary>
        /// Returns the list of all variable prefixes in use by a specific survey. TODO use a stored procedure/function for this (eliminate non-standard vars? or make it an option)
        /// </summary>
        /// <param name="surveyFilter"></param>
        /// <returns></returns>
        public static List<string> GetVariablePrefixes(string surveyFilter)
        {
            List<string> prefixes = new List<string>();
            string query = "SELECT Left(VarName,2) AS Prefix FROM qrySurveyQuestions WHERE Survey =@survey GROUP BY Left(VarName,2)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyFilter);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            prefixes.Add((string)rdr["Prefix"]);
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }

            return prefixes;
        }

        /// <summary>
        /// Returns a VariabelName object with the provided VarName.
        /// </summary>
        /// <param name="varname">A valid VarName.</param>
        /// <returns> Null is returned if the VarName is not found in the database.</returns>
        public static VariableName GetVariable(string varname)
        {
            VariableName v;
            string query = "SELECT * FROM qryVariableInfo WHERE VarName = @varname";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varname", varname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        v = new VariableName ((string)rdr["VarName"])
                        {
                            refVarName = (string)rdr["refVarName"],
                            VarLabel = (string)rdr["VarLabel"],
                            Domain = new DomainLabel ((int)rdr["DomainNum"], ((string)rdr["Domain"])),
                            Topic = new TopicLabel((int)rdr["TopicNum"], ((string)rdr["Topic"])),
                            Content = new ContentLabel((int)rdr["ContentNum"], ((string)rdr["Content"])),
                            Product = new ProductLabel((int)rdr["ProductNum"], ((string)rdr["Product"])),
                        };
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return v;
        }

        //
        // VarNames
        //

        private static string GetPreviousNames(string survey, string varname, bool excludeTempNames)
        {
            string varlist = "";
            string query = "SELECT dbo.FN_VarNamePreviousNames(@varname, @survey, @excludeTemp)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@varname", SqlDbType.VarChar);
                cmd.Parameters["@varname"].Value = varname;
                cmd.Parameters.Add("@survey", SqlDbType.VarChar);
                cmd.Parameters["@survey"].Value = survey;
                cmd.Parameters.Add("@excludeTemp", SqlDbType.Bit);
                cmd.Parameters["@excludeTemp"].Value = excludeTempNames;

                try
                {
                    varlist = (string)cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
#if DEBUG
                    Console.WriteLine(ex.ToString());
#endif
                    return "Error";
                }
            }

            if (!varlist.Equals(varname)) { varlist = "(Prev. " + varlist.Substring(varname.Length + 1) + ")"; } else { varlist = ""; }
            return varlist;
        }

        public static void FillPreviousNames(Survey s, bool excludeTempNames)
        {
            foreach (SurveyQuestion q in s.Questions)
                q.PreviousNames = GetPreviousNames(s.SurveyCode, q.VarName, excludeTempNames);
        }

        //
        // VarName Changes
        // 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static VarNameChange GetVarNameChangeByID(int ID)
        {
            VarNameChange vc = null;
            string query = "SELECT * FROM FN_GetVarNameChangeID (@id)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@id", ID);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            vc = new VarNameChange
                            {
                                ID = (int)rdr["ID"],
                                OldName = new VariableName((string)rdr["OldName"]),
                                NewName = new VariableName ((string)rdr["NewName"]),
                                ChangeDate = (DateTime) rdr["ChangeDate"],
                                ChangedBy = new Person((int)rdr["ChangedBy"]),
                                Authorization = (string) rdr["Authorization"],
                                Rationale = (string)rdr["Reasoning"],
                                HiddenChange = (bool) rdr["TempVar"],
                                



                            };
                            if (!rdr.IsDBNull(rdr.GetOrdinal("ChangeDateApprox"))) vc.ApproxChangeDate = (DateTime)rdr["ChangeDateApprox"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("Source"))) vc.Source = (string)rdr["Source"];
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return vc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<VarNameChange> GetVarNameChangeBySurvey(string surveyCode)
        {
            List<VarNameChange> vcs = new List<VarNameChange>();
            VarNameChange vc = null;

            string query = "SELECT * FROM FN_GetVarNameChangesSurvey (@survey)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyCode);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            vc = new VarNameChange
                            {
                                ID = (int)rdr["ID"],
                                OldName = new VariableName((string)rdr["OldName"]),
                                NewName = new VariableName((string)rdr["NewName"]),
                                ChangeDate = (DateTime)rdr["ChangeDate"],
                                ChangedBy = new Person((string)rdr["ChangedByName"], (int)rdr["ChangedBy"]),
                                HiddenChange = (bool)rdr["TempVar"],
                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("Reasoning"))) vc.Rationale = (string)rdr["Reasoning"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("Authorization"))) vc.Authorization = (string)rdr["Authorization"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("ChangeDateApprox"))) vc.ApproxChangeDate = (DateTime)rdr["ChangeDateApprox"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("Source"))) vc.Source = (string)rdr["Source"];

                            vcs.Add(vc);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }

            return vcs;
        }

        public static List<VarNameChangeNotification> GetVarNameChangeNotifications(int ChangeID)
        {

            return null;
        }
    }
}
