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
        /// <summary>
        /// Updates Study info for specified study object.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int UpdateStudy(Study study)
        {

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.UpdateCommand = new SqlCommand("proc_updateQnum", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                //sql.UpdateCommand.Parameters.AddWithValue("@newqnum", sq.Qnum);
               // sql.UpdateCommand.Parameters.AddWithValue("@qid", sq.ID);

                try
                {
                    sql.UpdateCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Saves Qnum field for a specified question. USES TEST BACKEND.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int UpdateQnum(SurveyQuestion sq)
        {
           
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.UpdateCommand = new SqlCommand("proc_updateQnum", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.UpdateCommand.Parameters.AddWithValue("@newqnum", sq.Qnum);
                sql.UpdateCommand.Parameters.AddWithValue("@qid", sq.ID);

                try
                {
                    sql.UpdateCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Saves User Preferences for specified user. USES TEST BACKEND. 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static int UpdateUser(UserPrefs u)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.UpdateCommand = new SqlCommand("proc_updateUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.UpdateCommand.Parameters.AddWithValue("@userid", u.userid);
                sql.UpdateCommand.Parameters.AddWithValue("@accessLevel", u.accessLevel);
                sql.UpdateCommand.Parameters.AddWithValue("@reportPath", u.ReportPath);
                sql.UpdateCommand.Parameters.AddWithValue("@reportPrompt", u.reportPrompt);
                sql.UpdateCommand.Parameters.AddWithValue("@wordingNumbers", u.wordingNumbers);

                try
                {
                    sql.UpdateCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            return 0;
        }


    }
}
