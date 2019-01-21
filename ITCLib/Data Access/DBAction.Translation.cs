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
        // Translations
        //
        // TODO Test
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SurvID"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static List<Translation> GetTranslationBySurvey(int SurvID, string language)
        {
            List<Translation> ts = new List<Translation>();
            Translation t;
            string query = "SELECT * FROM qrySurveyQuestionsTranslation WHERE SurvID = @sid AND Lang = @language";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", SurvID);
                sql.SelectCommand.Parameters.AddWithValue("@lang", language);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            t = new Translation
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Language = (string)rdr["Lang"],
                                TranslationText = (string)rdr["Translation"],
                                Bilingual = (bool)rdr["Bilingual"]
                            };
                            ts.Add(t);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return ts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="language"></param>
        public static void FillTranslationsBySurvey(Survey s, string language)
        {
            Translation t;
            string query = "SELECT * FROM qrySurveyQuestionsTranslations WHERE SurvID = @sid AND Lang = @lang";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();
                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", s.SID);
                sql.SelectCommand.Parameters.AddWithValue("@lang", language);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            t = new Translation
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Language = (string)rdr["Lang"],
                                TranslationText = (string)rdr["Translation"],
                                Bilingual = (bool)rdr["Bilingual"]
                            };
                            s.QuestionByID(t.QID).Translations.Add(t);

                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

        }

        public static List<string> GetLanguages (string survey)
        {
            List<string> langs = new List<string>();
            string query = "SELECT Lang FROM qrySurveyQuestionsTranslations WHERE SurvID = @survey GROUP BY Lang";

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
                            langs.Add((string)rdr["Lang"]);

                        }
                    }
                }
                catch (Exception)
                {
                    //return null;
                }
            }
            return langs;
        }

        /// <summary>
        /// Creates a DataTable containing the translation data for a given survey and language(s). A question filter can be supplied to get a specific set of records.
        /// <returns>The merger of the individual translation data tables.</returns>
        /// </summary>
        public static DataTable GetTranslationsBySurvey(string surveyCode, List<string> langs, string qFilter = null)
        {
            string query = "";
            string where = "";
            string whereLang = "";

            // instantiate the return table
            DataTable translationTable = new DataTable();

            // instantiate the data tables list
            List<DataTable> translationTables = new List<DataTable>();

            // create the filter for the query
            where = "WHERE Survey = '" + surveyCode + "'";

            if (qFilter != "") { where += " AND " + qFilter; }

            // create a data table for each language, set its primary key, add it to the list of translation tables
            for (int i = 0; i < langs.Count; i++)
            {
                DataTable t;
                t = new DataTable();
                whereLang = " AND Lang ='" + langs[i] + "'";

                query = "SELECT QID AS ID, VarName, refVarName, " +
                    "Replace(Replace(Replace(Replace(Translation, '&gt;', '>'), '&lt;', '<'), '&nbsp;', ' '), '<br>','\r\n') AS [" + langs[i] + "] " +
                    "FROM qrySurveyQuestionsTranslations " + where + whereLang;


                // run the query and fill the data table
                using (SqlDataAdapter sql = new SqlDataAdapter())
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
                {
                    conn.Open();
                    sql.SelectCommand = new SqlCommand(query, conn);
                    sql.Fill(t);
                }

                t.PrimaryKey = new DataColumn[] { t.Columns["ID"] };

                // TODO get corrected wordings (see GetCorrectedWordings)
                // TODO get headings

                translationTables.Add(t);
            }

            // merge all translations into one table
            translationTable = new DataTable();

            foreach (DataTable t in translationTables)
                translationTable.Merge(t);

            return translationTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <param name="langs"></param>
        /// <param name="qFilter"></param>
        /// <returns></returns>
        public static DataTable GetTranslationBySurveyFromBackup(string surveyCode, List<string> langs, string qFilter = null)
        {
            string query = "";
            string where = "";
            string whereLang = "";

            // instantiate the return table
            DataTable translationTable = new DataTable();

            // instantiate the data tables list
            List<DataTable> translationTables = new List<DataTable>();

            // create the filter for the query
            where = "WHERE Survey = '" + surveyCode + "'";

            if (qFilter != "") { where += " AND " + qFilter; }

            // create a data table for each language, set its primary key, add it to the list of translation tables
            for (int i = 0; i < langs.Count; i++)
            {
                DataTable t;
                t = new DataTable();
                whereLang = " AND Lang ='" + langs[i] + "'";

                query = "SELECT QID AS ID, VarName, refVarName, " +
                    "Replace(Replace(Replace(Replace(Translation, '&gt;', '>'), '&lt;', '<'), '&nbsp;', ' '), '<br>','\r\n') AS [" + langs[i] + "] " +
                    "FROM qrySurveyQuestionsTranslations " + where + whereLang;


                // run the query and fill the data table
                using (SqlDataAdapter sql = new SqlDataAdapter())
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
                {
                    conn.Open();
                    sql.SelectCommand = new SqlCommand(query, conn);
                    sql.Fill(t);
                }

                t.PrimaryKey = new DataColumn[] { t.Columns["ID"] };

                // TODO get corrected wordings (see GetCorrectedWordings)
                // TODO get headings

                translationTables.Add(t);
            }

            // merge all translations into one table
            translationTable = new DataTable();

            foreach (DataTable t in translationTables)
                translationTable.Merge(t);

            return translationTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QID"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static List<Translation> GetTranslationByQuestion(int QID, string language = null)
        {
            Translation t;
            List<Translation> list = new List<Translation>();
            string query = "SELECT * FROM qryTranslation WHERE QID = @qid";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@qid", QID);

                // if language is not null, add another parameter for language
                if (language != null)
                {
                    query += " AND Lang = @lang";
                    sql.SelectCommand.Parameters.AddWithValue("@lang", language);
                }

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            t = new Translation
                            {
                                ID = (int)rdr["ID"],
                                QID = (int)rdr["QID"],
                                Language = (string)rdr["Lang"],
                                TranslationText = (string)rdr["Translation"],
                                Bilingual = (bool)rdr["Bilingual"]
                            };

                            list.Add(t);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return list;
        }
    }
}
