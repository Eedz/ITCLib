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
   partial class DBAction
    {
        //
        // VarNames
        // 

        /// <summary>
        /// Returns a list containing every unique refVarName.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllRefVars()
        {
            List<string> refVarNames = new List<string>();

            string query = "SELECT * FROM FN_GetAllRefVars()";

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

        /// <summary>
        /// Returns a list of containing all refVarNames for a particular survey.
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns></returns>
        public static List<string> GetAllRefVars(string surveyCode)
        {
            List<string> refVarNames = new List<string>();

            string query = "SELECT  * FROM FN_GetSurveyRefVars(@survey)";

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

        /// <summary>
        /// Returns a list containing all VarNames with a particular refVarName.
        /// </summary>
        /// <param name="refVarName"></param>
        /// <returns></returns>
        public static List<string> GetVarNamesByRef(string refVarName)
        {
            List<string> VarNames = new List<string>();

            string query = "SELECT * FROM FN_GetVarNamesByRef(@refVarName)";

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

        /// <summary>
        /// Returns true if the provided VarName exists in the database.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public static bool VarNameExists(string varname)
        {
            bool result = false; ;
            string query = "SELECT FN_VarNameExists(@varname)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varname", varname);

                try
                {
                    result = (bool)sql.SelectCommand.ExecuteScalar();
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
            string query = "SELECT FN_RefVarNameExists(@refvarname)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refvarname", refvarname);

                try
                {
                    result = (bool)sql.SelectCommand.ExecuteScalar();
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }

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
                            headings.Add(new Heading((string)rdr["Qnum"], (string)rdr["PreP"]));
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
        /// Returns the list of all variables in use by a specific survey.
        /// </summary>
        /// <param name="surveyFilter"></param>
        /// <returns></returns>
        public static List<string> GetVariableList(string surveyFilter)
        {
            List<string> varnames = new List<string>();
            string query = "SELECT * FROM FN_GetSurveyVarNames(@survey)";

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
        /// Gets the list of previous VarNames for a provided survey and varname.
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="varname"></param>
        /// <param name="excludeTempNames"></param>
        /// <returns></returns>
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

        //
        // Fill methods
        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="excludeTempNames"></param>
        public static void FillPreviousNames(Survey s, bool excludeTempNames)
        {
            foreach (SurveyQuestion q in s.Questions)
                q.PreviousNames = GetPreviousNames(s.SurveyCode, q.VarName, excludeTempNames);
        }
    }
}
