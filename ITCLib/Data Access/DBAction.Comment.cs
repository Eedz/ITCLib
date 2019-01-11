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
        // Comments
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SurvID"></param>
        /// <returns></returns>
        public static List<Comment> GetCommentsBySurvey(int SurvID)
        {
            List<Comment> cs = new List<Comment>();
            Comment c;
            string query = "SELECT * FROM qryCommentsQues WHERE SurvID = @sid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", SurvID);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            c = new Comment
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Survey = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                CID = (int)rdr["CID"],
                                Notes = (string)rdr["Notes"],
                                NoteDate = (DateTime)rdr["NoteDate"],
                                NoteInit = (int)rdr["NoteInit"],
                                Name = (string)rdr["Name"],
                                SourceName = (string)rdr["SourceName"],
                                NoteType = (string)rdr["NoteType"],
                                Source = (string)rdr["Source"],
                                SurvID = (int)rdr["SurvID"]
                            };

                            cs.Add(c);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return cs;
        }

        // TODO TEST with all arguments
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SurvID"></param>
        /// <param name="commentTypes"></param>
        /// <param name="commentDate"></param>
        /// <param name="commentAuthors"></param>
        /// <param name="commentSources"></param>
        /// <returns></returns>
        public static List<Comment> GetCommentsBySurvey(int SurvID, List<string> commentTypes = null, DateTime? commentDate = null, List<int> commentAuthors = null, List<string> commentSources = null)
        {
            List<Comment> cs = new List<Comment>();
            Comment c;
            string query = "SELECT * FROM qryCommentsQues WHERE SurvID = @sid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", SurvID);

                if (commentTypes != null && commentTypes.Count != 0)
                {
                    sql.SelectCommand.Parameters.AddWithValue("@commentTypes", String.Join(",", commentTypes));
                    query += " AND NoteType IN (@commentTypes)";
                }

                if (commentDate != null)
                {
                    sql.SelectCommand.Parameters.AddWithValue("@commentDate", commentDate);
                    query += " AND NoteDate >= (@commentDate)";
                }

                if (commentAuthors != null && commentAuthors.Count != 0)
                {
                    sql.SelectCommand.Parameters.AddWithValue("@commentAuthors", String.Join(",", commentAuthors));
                    query += " AND NoteInit IN (@commentAuthors)";
                }

                if (commentSources != null && commentSources.Count != 0)
                {
                    sql.SelectCommand.Parameters.AddWithValue("@commentSources", String.Join(",", commentSources));
                    query += " AND Source IN (@commentSources)";
                }

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            c = new Comment
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Survey = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                CID = (int)rdr["CID"],
                                Notes = (string)rdr["Notes"],
                                NoteDate = (DateTime)rdr["NoteDate"],
                                NoteInit = (int)rdr["NoteInit"],
                                Name = (string)rdr["Name"],
                                SourceName = (string)rdr["SourceName"],
                                NoteType = (string)rdr["NoteType"],
                                Source = (string)rdr["Source"],
                                SurvID = (int)rdr["SurvID"]
                            };
                            cs.Add(c);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return cs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QID"></param>
        /// <returns></returns>
        public static List<Comment> GetCommentsByQuestion(int QID) { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="commentTypes"></param>
        /// <param name="commentDate"></param>
        /// <param name="commentAuthors"></param>
        /// <param name="commentSources"></param>
        public static void FillCommentsBySurvey(Survey s, List<string> commentTypes = null, DateTime? commentDate = null, List<int> commentAuthors = null, List<string> commentSources = null)
        {
            Comment c;
            string query = "SELECT * FROM qryCommentsQues WHERE SurvID = @sid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand();
                sql.SelectCommand.Parameters.AddWithValue("@sid", s.SID);

                if (commentTypes != null && commentTypes.Count != 0)
                {
                    query += " AND (";
                    for (int i = 0; i < commentTypes.Count; i++)
                    {
                        sql.SelectCommand.Parameters.AddWithValue("@commentTypes" + i, commentTypes[i]);
                        query += " NoteType = @commentTypes" + i + " OR ";
                    }
                    query = Utilities.TrimString(query, " OR ");
                    query += ")";
                }

                if (commentDate != null)
                {
                    sql.SelectCommand.Parameters.AddWithValue("@commentDate", commentDate.Value);
                    query += " AND NoteDate >= @commentDate";
                }

                if (commentAuthors != null && commentAuthors.Count != 0)
                {
                    query += " AND (";
                    for (int i = 0; i < commentAuthors.Count; i++)
                    {
                        sql.SelectCommand.Parameters.AddWithValue("@commentAuthors" + i, commentAuthors[i]);
                        query += " NoteInit = @commentAuthors" + i + " OR ";
                    }
                    query = Utilities.TrimString(query, " OR ");
                    query += ")";
                }

                if (commentSources != null && commentSources.Count != 0)
                {
                    query += " AND (";
                    for (int i = 0; i < commentAuthors.Count; i++)
                    {
                        sql.SelectCommand.Parameters.AddWithValue("@commentSources" + i, commentSources[i]);
                        query += " SourceName = @commentSources" + i + " OR ";
                    }
                    query = Utilities.TrimString(query, " OR ");
                    query += ")";
                }

                query += " ORDER BY NoteDate DESC";

                sql.SelectCommand.CommandText = query;
                sql.SelectCommand.Connection = conn;

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            c = new Comment
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Survey = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                CID = (int)rdr["CID"],
                                Notes = (string)rdr["Notes"],
                                NoteDate = (DateTime)rdr["NoteDate"],
                                NoteInit = (int)rdr["NoteInit"],
                                Name = (string)rdr["Name"],
                                NoteType = (string)rdr["NoteType"],
                                SurvID = (int)rdr["SurvID"]
                            };
                            if (!rdr.IsDBNull(rdr.GetOrdinal("SourceName"))) c.SourceName = (string)rdr["SourceName"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("Source"))) c.Source = (string)rdr["Source"];

                            s.QuestionByID((int)rdr["QID"]).Comments.Add(c);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return;
                }
            }
        }

    }
}
