using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;

namespace ITCLib
{
    public static partial class DBAction
    {

        public static List<Wording> GetWordings()
        {
            List<Wording> wordings = new List<Wording>();
            Wording w;
            string query = "SELECT * FROM Wording_AllFields ORDER BY FieldName, W#";

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
                            w = new Wording
                            {
                                ID = (int)rdr["ID"],
                                WordID = (int)rdr["WordID"],
                                FieldName = (string)rdr["FieldName"],
                                WordingText = (string)rdr["WordingText"]

                            };

                            wordings.Add(w);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return wordings;
        }

        public static List<Wording> GetWordings(string fieldname)
        {
            List<Wording> wordings = new List<Wording>();
            Wording w;
            string query = "SELECT * FROM FN_GetWordings(@field) ORDER BY WordID";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@field", fieldname);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            w = new Wording
                            {
                                ID = (int) rdr["ID"],
                                WordID = (int)rdr["WordID"],
                                FieldName = fieldname,
                                WordingText = (string) rdr["WordingText"]

                            };

                            wordings.Add(w);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return wordings;
        }

    

        public static List<ResponseSet> GetResponseSets(string fieldname)
        {
            List<ResponseSet> setList = new List<ResponseSet>();
            ResponseSet rs;
            string query = "SELECT * FROM FN_GetResponseSets(@field) ORDER BY RespName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@field", fieldname);
                
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            rs = new ResponseSet
                            {
                                RespSetName = (string)rdr["RespName"],
                                FieldName = fieldname,
                                RespList = (string)rdr["ResponseList"]

                            };

                            setList.Add(rs);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return setList;
        }

        public static string GetWordingText(string field, int wordID)
        {
            string text = "";

            string query = "SELECT * FROM Wording_AllFields WHERE FieldName = @field AND [W#] = @wordID";


            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@field", field);
                sql.SelectCommand.Parameters.AddWithValue("@wordID", wordID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Wording"))) text = (string)rdr["Wording"];
                    }
                }
                catch
                {
                    return "";
                }
            }

            return text;
        }

        public static string GetResponseText(string field, string respName)
        {
            string text = "";
            string query;
            if (field == "RespName")
            {
                query = "SELECT * FROM qryRespOptions WHERE RespName = @respName";
            }
            else if (field == "NRName")
            {
                query = "SELECT * FROM qryNonRespOptions WHERE NRName = @respName";
            }
            else
            {
                return "";
            }


            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                //sql.SelectCommand.Parameters.AddWithValue("@field", field);
                sql.SelectCommand.Parameters.AddWithValue("@respName", respName);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        if (field == "RespName")
                        {
                            if (!rdr.IsDBNull(rdr.GetOrdinal("RespOptions"))) text = (string)rdr["RespOptions"];
                        }
                        else if (field == "NRName")
                        {
                            if (!rdr.IsDBNull(rdr.GetOrdinal("NRCodes"))) text = (string)rdr["NRCodes"];
                        }
                        
                    }
                }
                catch
                {
                    return "";
                }
            }

            return text;
        }

        /// <summary>
        /// Returns a list of WordingUsage objects which represent the questions that use the provided field/wordID combination.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="wordID"></param>
        /// <returns></returns>
        public static List<WordingUsage> GetWordingUsage(string fieldname, int wordID)
        {
            List<WordingUsage> qList = new List<WordingUsage>();
            WordingUsage sq;
            string query = "SELECT * FROM FN_GetWordingUsage (@field, @wordID)";
           
            if (query == "")
                return null;

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@field", fieldname);
                sql.SelectCommand.Parameters.AddWithValue("@wordID", wordID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            sq = new WordingUsage
                            {
                                VarName = (string)rdr["VarName"],
                                VarLabel = (string)rdr["VarLabel"],
                                SurveyCode = (string)rdr["Survey"],
                                WordID = wordID,
                                Qnum = (string)rdr["Qnum"],
                                Locked = (bool)rdr["Locked"]
                                
                            };

                            qList.Add(sq);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return qList;
        }

        /// <summary>
        /// Returns a list of WordingUsage objects which represent the questions that use the provided field/wordID combination.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="wordID"></param>
        /// <returns></returns>
        public static List<ResponseUsage> GetResponseUsage(string fieldname, string respName)
        {
            List<ResponseUsage> qList = new List<ResponseUsage>();
            ResponseUsage sq;
            string query = "SELECT * FROM FN_GetResponseUsage (@field, @wordID)";

            if (query == "")
                return null;

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@field", fieldname);
                sql.SelectCommand.Parameters.AddWithValue("@wordID", respName);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            sq = new ResponseUsage
                            {
                                VarName = (string)rdr["VarName"],
                                VarLabel = (string)rdr["VarLabel"],
                                SurveyCode = (string)rdr["Survey"],
                                RespName = respName,
                                Qnum = (string)rdr["Qnum"],
                                Locked = (bool)rdr["Locked"]

                            };

                            qList.Add(sq);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return qList;
        }
    }
}
