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
        // Surveys
        //

        public static List<Survey> GetAllSurveys()
        {
            List<Survey> surveys = new List<Survey>();
            string query = "SELECT Survey FROM qrySurveyInfo ORDER BY ISO_Code, Wave, Survey";

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
                            surveys.Add(GetSurveyInfo((string)rdr["Survey"]));
                        }
                    }
                }
                catch (Exception)
                {

                }

            }

            return surveys;
        }

        public static List<Survey> GetSurveys(int waveID)
        {
            List<Survey> surveys = new List<Survey>();
            string query = "SELECT Survey FROM qrySurveyInfo WHERE WaveID = @waveID ORDER BY ISO_Code, Wave, Survey";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@waveID", waveID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            surveys.Add(GetSurveyInfo((string)rdr["Survey"]));
                        }
                    }
                }
                catch (Exception)
                {

                }

            }

            return surveys;
        }

        /// <summary>
        /// Returns a Survey object with the provided survey code.
        /// </summary>
        /// <param name="code">A valid survey code. Null is returned if the survey code is not found in the database.</param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns></returns>
        public static Survey GetSurveyInfo(string code)
        {
            Survey s;
            string query = "SELECT * FROM FN_GetSurveyInfo (@survey)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
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
                            CountryCode = (int)rdr["CC_ID"],
                            WaveID = (int)rdr["WaveID"],
                            Locked = (bool)rdr["Locked"],
                            EnglishRouting = (bool)rdr["EnglishRouting"],
                            HideSurvey = (bool)rdr["HideSurvey"],
                            ReRun = (bool)rdr["ReRun"],
                            NCT = (bool)rdr["NCT"]
                        };
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Languages"))) s.Languages = (string)rdr["Languages"];

                        if (!rdr.IsDBNull(rdr.GetOrdinal("GroupName")))
                        {
                            s.Group = new SurveyUserGroup
                            {
                                ID = (int)rdr["GroupID"],
                                UserGroup = (string)rdr["GroupName"]
                            };
                        }
                        else
                        {
                            s.Group = new SurveyUserGroup
                            {
                                ID = 0,
                                UserGroup = ""
                            };
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Mode")))
                        {
                            s.Mode = new SurveyMode
                            {
                                ID = (int)rdr["Mode"],
                                Mode = (string)rdr["ModeLong"],
                                ModeAbbrev = (string)rdr["ModeAbbrev"]
                            };
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Cohort")))
                        {
                            s.Cohort = new SurveyCohort
                            {
                                ID = (int)rdr["CohortID"],
                                Cohort = (string)rdr["Cohort"],
                                
                            } ;
                            if (!rdr.IsDBNull(rdr.GetOrdinal("CohortCode")))
                                s.Cohort.Code = (string)rdr["CohortCode"];
                            else
                                s.Cohort.Code = "";
                        }

                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSurveyList()
        {
            List<string> surveyCodes = new List<string>();
            string query = "SELECT Survey FROM qrySurveyInfo WHERE HideSurvey = 0 ORDER BY ISO_Code, Wave, Survey";

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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
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
                            CountryCode = (int)rdr["CC_ID"],
                            WaveID = (int)rdr["WaveID"],
                            Locked = (bool)rdr["Locked"]
                        };

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Languages"))) s.Languages = (string)rdr["Languages"];
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Group")))
                            s.Group = new SurveyUserGroup
                            {
                                UserGroup = (string)rdr["Group"]
                            };

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Mode")))
                        {
                            s.Mode = new SurveyMode
                            {
                                ID = (int)rdr["Mode"],
                                Mode = (string)rdr["ModeLong"],
                                ModeAbbrev = (string)rdr["ModeAbbrev"]
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            s.AddQuestions(GetQuestionsBySurvey(s.SID));

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
            string query = "SELECT * FROM FN_GetSurveyInfo (@survey)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
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
                            CountryCode = (int)rdr["CC_ID"],
                            WaveID = (int)rdr["WaveID"],
                            Locked = (bool)rdr["Locked"]
                        };
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Languages"))) s.Languages = (string)rdr["Languages"];
                        if (!rdr.IsDBNull(rdr.GetOrdinal("GroupName")))
                            s.Group = new SurveyUserGroup
                            {
                                UserGroup = (string)rdr["GroupName"]
                            };

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Mode")))
                        {
                            s.Mode = new SurveyMode
                            {
                                ID = (int)rdr["Mode"],
                                Mode = (string)rdr["ModeLong"],
                                ModeAbbrev = (string)rdr["ModeAbbrev"]
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            s.AddQuestions (GetQuestionsBySurvey(s.SID, withComments, withTranslation));


            return s;
        }
    }
}
