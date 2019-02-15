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


        public static List<Wording> GetWordings(string fieldname)
        {
            List<Wording> wordList = new List<Wording>();
            Wording w;
            string query = "SELECT * FROM FN_GetWordings(@field) ORDER BY ID";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
                                ID = (int)rdr["ID"],
                                WordID = (int)rdr["ID"],
                                FieldName = fieldname,
                                WordingText = (string)rdr["WordingText"]

                            };



                            wordList.Add(w);
                        }
                    }
                }
                catch
                {
                    int i = 0;
                }
            }

            return wordList;
        }

        public static string GetWordingText(string field, int wordID)
        {
            string text = "";

            string query = "SELECT * FROM Wording_AllFields WHERE FieldName = @field AND [W#] = @wordID";


            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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
    }
}
