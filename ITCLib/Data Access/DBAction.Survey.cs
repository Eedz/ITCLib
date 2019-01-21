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
        //
        // Surveys
        //

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSurveyList()
        {
            List<string> surveyCodes = new List<string>();
            string query = "SELECT Survey FROM qrySurveyInfo WHERE HideSurvey = 0 ORDER BY ISO_Code, Wave, Survey";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            surveyCodes.Add((string)rdr["Survey"]);
                        }
                    }
                }
                catch (Exception)
                {
                    
                }

            }

            return surveyCodes;
        }

        /// <summary>
        /// Returns the survey code for a particular Question ID.
        /// </summary>
        /// <param name="qid">Valid Question ID.</param>
        /// <returns>Survey Code as string, empty string if Question ID is invalid.</returns>
        public static string GetSurveyCodeByQID(int qid)
        {
            string surveyCode = "";
            string query = "SELECT Survey FROM qrySurveyQuestions WHERE ID = @qid ORDER BY Qnum";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@qid", qid);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            surveyCode = (string)rdr["Survey"];
                        }
                    }
                }
                catch (Exception)
                {
                    return "";
                }

            }

            return surveyCode;
        }

        /// <summary>
        /// Creates a Survey object with the provided ID.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Survey GetSurvey(int ID)
        {
            Survey s;
            string query = "SELECT * FROM qrySurveyInfo WHERE ID = @sid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", ID);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        s = new Survey
                        {
                            SID = (int)rdr["ID"],
                            SurveyCode = (string)rdr["Survey"],
                            Title = (string)rdr["SurveyTitle"],
                            Mode = (string)rdr["ModeLong"],
                            CountryCode = Int32.Parse((string)rdr["CountryCode"])
                        };

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Languages"))) s.Languages = (string)rdr["Languages"];
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Group"))) s.Group = (string)rdr["Group"];
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            s.questions = GetQuestionsBySurvey(s.SID);
            s.GetEssentialQuestions();

            return s;
        }

        /// <summary>
        /// Returns a Survey object with the provided survey code.
        /// </summary>
        /// <param name="code">A valid survey code. Null is returned if the survey code is not found in the database.</param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns></returns>
        public static Survey GetSurvey(string code, bool withComments = false, bool withTranslation = false)
        {
            Survey s;
            string query = "SELECT * FROM qrySurveyInfo WHERE Survey = @survey";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", code);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        s = new Survey
                        {
                            SID = (int)rdr["ID"],
                            SurveyCode = (string)rdr["Survey"],
                            Title = (string)rdr["SurveyTitle"],
                            Mode = (string)rdr["ModeLong"],
                            CountryCode = Int32.Parse((string)rdr["CountryCode"])
                        };
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Languages"))) s.Languages = (string)rdr["Languages"];
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Group"))) s.Group = (string)rdr["Group"];
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            s.questions = GetQuestionsBySurvey(s.SID, withComments, withTranslation);
            s.GetEssentialQuestions();

            return s;
        }
    }
}
