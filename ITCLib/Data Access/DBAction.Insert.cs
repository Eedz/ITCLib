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

        public static int InsertNote (string noteText)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createNote", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@noteText", noteText);
             

                try
                {
                    sql.InsertCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            return 0;
        }

        public static int InsertQuestion (string surveyCode, SurveyQuestion question)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                // 
            }
            return 1;
        }

        /// <summary>
        /// Saves Qnum field for a specified question. USES TEST BACKEND.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int InsertLabel(string labelType, string newLabel)
        {

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createLabel", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@type", labelType);
                sql.InsertCommand.Parameters.AddWithValue("@label", newLabel);

                try
                {
                    sql.InsertCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            return 0;
        }

        public static int InsertRegion(ITCLib.Region region)
        {
            return 0;
        }

        /// <summary>
        /// Inserts a new study record. USES Test backend
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static int InsertCountry(Study newStudy)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.UpdateCommand = new SqlCommand("proc_createStudy", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.UpdateCommand.Parameters.AddWithValue("@studyName", newStudy.StudyName);
                sql.UpdateCommand.Parameters.AddWithValue("@countryName", newStudy.CountryName);
                sql.UpdateCommand.Parameters.AddWithValue("@ageGroup", newStudy.AgeGroup);
                sql.UpdateCommand.Parameters.AddWithValue("@countryCode", newStudy.CountryCode);
                sql.UpdateCommand.Parameters.AddWithValue("@ISO_Code", newStudy.ISO_Code);

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

        

        public static int InsertWording(Wording wording)
        {
            return 1;
        }

        public static int InsertResponseSet(ResponseSet respSet)
        {
            return 1;
        }

        /// <summary>
        /// Saves Qnum field for a specified question. USES TEST BACKEND.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int BackupComments(int QID)
        {

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_backupComments", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@QID", QID);
                

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
