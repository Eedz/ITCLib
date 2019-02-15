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

                sql.UpdateCommand = new SqlCommand("proc_createLabel", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.UpdateCommand.Parameters.AddWithValue("@type", labelType);
                sql.UpdateCommand.Parameters.AddWithValue("@label", newLabel);

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

                sql.UpdateCommand = new SqlCommand("proc_insertStudy", conn)
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
    }
}
