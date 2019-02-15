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
                            wordingNumbers = (bool)rdr["WordingNumbers"],
                            commentDetails = (int)rdr["CommentDetails"]
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

        public static void FillUserSurveyFilters(UserPrefs user)
        {
            UserPrefs u;
            string query = "SELECT * FROM qrySessionManager WHERE PersonnelID = @id";
            string se1 = "4C1";
            string se2 = "4C1";
            string se3 = "4C1";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@id", user.userid);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if ((string)rdr["FormName"] == "frmSurveyEntry")
                                se1 = (string)rdr["Filter"];
                            else if ((string)rdr["FormName"] == "frmSurveyEntry2")
                                se2 = (string)rdr["Filter"];
                            else if ((string)rdr["FormName"] == "frmSurveyEntry3")
                                se3 = (string)rdr["Filter"];
                        }

                    }
                }
                catch (Exception)
                {
                    //return;
                }
            }

            user.SurveyEntryCodes.Add(se1);
            user.SurveyEntryCodes.Add(se2);
            user.SurveyEntryCodes.Add(se3);

            return;
        }


    }
}
