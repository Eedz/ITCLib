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
    /// <summary>
    /// Static class for interacting with the Database. TODO create stored procedures on server for each of these
    /// </summary>
    public static partial class DBAction
    { 
      // test  
        //
        // Variables
        //
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

        public static List<VarNameChangeNotification> GetVarNameChangeNotifications(int ChangeID)
        {

            return null;
        }
    }
}
