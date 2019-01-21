using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ITCSurveyReportLib
{
    public enum AccessLevel { SMG = 1, PMG }
    /// <summary>
    /// Static class for interacting with the Database. TODO create stored procedures on server for each of these
    /// </summary>
    public static partial class DBAction
    { 
        //
        // Users
        //
        public static UserPrefs GetUser(string username)
        {
            UserPrefs u;
            string query = "SELECT * FROM FN_GetUserPrefs (@username)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@username", username);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        u = new UserPrefs
                        {
                            userid = (int)rdr["PersonnelID"],
                            Username = (string)rdr["username"],
                            accessLevel = (AccessLevel)rdr["AccessLevel"],
                            ReportPath = (string)rdr["ReportFolder"],
                            reportPrompt = (bool)rdr["ReportPrompt"],
                            wordingNumbers = (bool)rdr["WordingNumbers"]
                        };
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return u;
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varname", varname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        v = new VariableName
                        {
                            VarName = (string)rdr["VarName"],
                            refVarName = (string)rdr["refVarName"],
                            VarLabel = (string)rdr["VarLabel"],
                            DomainLabel = (string)rdr["Domain"],
                            TopicLabel = (string)rdr["Topic"],
                            ContentLabel = (string)rdr["Content"],
                            ProductLabel = (string)rdr["Product"]
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
