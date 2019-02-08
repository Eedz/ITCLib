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

        public static List<string> GetCommentTypes(string survey)
        {
            List<string> types = new List<string>();
            string query = "SELECT NoteType FROM qryCommentsQues WHERE Survey = @survey GROUP BY NoteType";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", survey);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            types.Add ((string)rdr["NoteType"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            return types;
        }

        public static List<Person> GetCommentAuthors(int SurvID)
        {
            List<Person> ps = new List<Person>();
            Person p;
            string query = "SELECT NoteInit, Name FROM qryCommentsQues WHERE SurvID = @sid";

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
                            p = new Person((string)rdr["Name"], (int)rdr["NoteInit"]);

                            ps.Add(p);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
            }
            return ps;
        }

        public static List<Person> GetCommentAuthors(string SurveyCode)
        {
            List<Person> ps = new List<Person>();
            Person p;
            string query = "SELECT NoteInit, Name FROM qryCommentsQues WHERE Survey = @survey GROUP BY NoteInit, Name";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", SurveyCode);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            p = new Person((string)rdr["Name"], (int)rdr["NoteInit"]);

                            ps.Add(p);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    //return null;
                }
            }
            return ps;
        }

        public static List<string> GetCommentSourceNames(string SurveyCode)
        {
            List<string> sourceList = new List<string>();
            string query = "SELECT SourceName FROM qryCommentsQues WHERE Survey = @survey GROUP BY SourceName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", SurveyCode);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            sourceList.Add((string)rdr["SourceName"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    //return null;
                }
            }
            return sourceList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SurvID"></param>
        /// <returns></returns>
        public static List<SurveyComment> GetSurveyCommentsBySurvey(int SurvID)
        {
            List<SurveyComment> cs = new List<SurveyComment>();
            SurveyComment c;
            string query = "SELECT * FROM qryCommentsSurv WHERE SID = @sid";

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
                            c = new SurveyComment
                            {
                                ID = (int)rdr["ID"],
                                SID = (int)rdr["SID"],
                                Survey = (string)rdr["Survey"],
                                CID = (int)rdr["CID"],
                                Notes = (string)rdr["Notes"]
                            };
                            if (!rdr.IsDBNull(rdr.GetOrdinal("NoteDate"))) c.NoteDate = (DateTime)rdr["NoteDate"];
                            c.NoteInit = (int)rdr["NoteInit"];
                            c.Name = (string)rdr["Name"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("SourceName"))) c.SourceName = (string)rdr["SourceName"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("NoteType"))) c.NoteType = (string)rdr["NoteType"];
                            if (!rdr.IsDBNull(rdr.GetOrdinal("Source"))) c.Source = (string)rdr["Source"];
                                
                            

                            cs.Add(c);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return cs;
        }
    }
}
