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
    partial class DBAction
    {
        public static void UpdateQnumA (SurveyQuestion sq)
        {
            string select = "SELECT Qnum FROM tblSurveyNumbers WHERE ID = @qid";
            string update = "UPDATE tblSurveyNumbers SET Qnum = @newqnum WHERE ID = @qid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(select, conn);
                
                sql.SelectCommand.Parameters.AddWithValue("@qid", sq.ID);

                sql.UpdateCommand = new SqlCommand(update, conn);
                sql.UpdateCommand.Parameters.AddWithValue("@newqnum", sq.Qnum);
                sql.UpdateCommand.Parameters.AddWithValue("@qid", sq.ID);
            }
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
                catch (Exception e)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
