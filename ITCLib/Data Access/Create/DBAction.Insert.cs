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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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

        public static int InsertSurveyDraft(SurveyDraft draft)
        {
            int newID;
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createSurveyDraft", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@SurvID", draft.SurvID);
                sql.InsertCommand.Parameters.AddWithValue("@DraftTitle", draft.DraftTitle);
                sql.InsertCommand.Parameters.AddWithValue("@DraftDate", draft.DraftDate);
                sql.InsertCommand.Parameters.AddWithValue("@DraftComments", draft.DraftComments);
                sql.InsertCommand.Parameters.Add(new SqlParameter("@newID", SqlDbType.Int)).Direction = ParameterDirection.Output;

                try
                {
                    sql.InsertCommand.ExecuteNonQuery();
                    newID = Convert.ToInt32(sql.InsertCommand.Parameters["@newID"].Value);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            return newID;
        }

        public static int InsertSurveyDraftExtraInfo(int draftID, int extraFieldNum, string extraFieldLabel)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createSurveyDraftExtraInfo", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@DraftID", draftID);
                sql.InsertCommand.Parameters.AddWithValue("@ExtraFieldNum", extraFieldNum);
                sql.InsertCommand.Parameters.AddWithValue("@ExtraFieldLabel", extraFieldLabel);

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

        public static int InsertDraftQuestion(DraftQuestion dq)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createSurveyDraftQuestion", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@DraftID", dq.DraftID);
                sql.InsertCommand.Parameters.AddWithValue("@SortBy", dq.SortBy);
                sql.InsertCommand.Parameters.AddWithValue("@Qnum", dq.qnum);
                sql.InsertCommand.Parameters.AddWithValue("@AltQnum", dq.altqnum);
                sql.InsertCommand.Parameters.AddWithValue("@VarName", dq.varname);
                sql.InsertCommand.Parameters.AddWithValue("@QuestionText", dq.questionText);
                sql.InsertCommand.Parameters.AddWithValue("@Comment", dq.comment);
                sql.InsertCommand.Parameters.AddWithValue("@Extra1", dq.extra1);
                sql.InsertCommand.Parameters.AddWithValue("@Extra2", dq.extra2);
                sql.InsertCommand.Parameters.AddWithValue("@Extra3", dq.extra3);
                sql.InsertCommand.Parameters.AddWithValue("@Extra4", dq.extra4);
                sql.InsertCommand.Parameters.AddWithValue("@Extra5", dq.extra5);
                sql.InsertCommand.Parameters.AddWithValue("@Inserted", dq.inserted);
                sql.InsertCommand.Parameters.AddWithValue("@Deleted", dq.deleted);

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

        public static int InsertTranslation(Translation tq)
        {
            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.InsertCommand = new SqlCommand("proc_createTranslation", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sql.InsertCommand.Parameters.AddWithValue("@survey", tq.Survey);
                sql.InsertCommand.Parameters.AddWithValue("@varname", tq.VarName);
                sql.InsertCommand.Parameters.AddWithValue("@text", tq.TranslationText);
                sql.InsertCommand.Parameters.AddWithValue("@lang", tq.Language);
                sql.InsertCommand.Parameters.AddWithValue("@bilingual", tq.Bilingual);
              

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
    }
}
