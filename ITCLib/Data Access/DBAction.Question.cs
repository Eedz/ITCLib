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
        // SurveyQuestions
        //
        /// <summary>
        /// Returns a SurveyQuestion with the provided ID. 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns>SurveyQuestion if ID is valid, null otherwise.</returns>
        public static SurveyQuestion GetSurveyQuestion(int ID, bool withComments = false, bool withTranslation = false)
        {
            SurveyQuestion q = null;
            string query = "SELECT * FROM qrySurveyQuestions WHERE ID = @id";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@id", ID);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                refVarName = (string)rdr["refVarName"],
                                Qnum = (string)rdr["Qnum"],
                                //AltQnum = (string)rdr["AltQnum"],
                                PrePNum = (int)rdr["PreP#"],
                                PreP = (string)rdr["PreP"],
                                PreINum = (int)rdr["PreI#"],
                                PreI = (string)rdr["PreI"],
                                PreANum = (int)rdr["PreA#"],
                                PreA = (string)rdr["PreA"],
                                LitQNum = (int)rdr["LitQ#"],
                                LitQ = (string)rdr["LitQ"],
                                PstINum = (int)rdr["PstI#"],
                                PstI = (string)rdr["PstI"],
                                PstPNum = (int)rdr["PstP#"],
                                PstP = (string)rdr["PstP"],
                                RespName = (string)rdr["RespName"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRName = (string)rdr["NRName"],
                                NRCodes = (string)rdr["NRCodes"],
                                VarLabel = (string)rdr["VarLabel"],
                                TopicLabel = (string)rdr["Topic"],
                                ContentLabel = (string)rdr["Content"],
                                ProductLabel = (string)rdr["Product"],
                                DomainLabel = (string)rdr["Domain"],
                                TableFormat = (bool)rdr["TableFormat"],
                                CorrectedFlag = (bool)rdr["CorrectedFlag"],
                                NumCol = (int)rdr["NumCol"],
                                NumDec = (int)rdr["NumDec"],
                                VarType = (string)rdr["VarType"],
                                ScriptOnly = (bool)rdr["ScriptOnly"]
                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("NumFmt"))) q.NumFmt = (string)rdr["NumFmt"];

                            if (withComments)
                                q.Comments = GetCommentsByQuestion(q.ID);

                            if (withTranslation)
                                q.Translations = GetTranslationByQuestion(q.ID);

                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return q;
        }

        /// <summary>
        /// Retrieves a set of records for a particular survey ID and returns a list of SurveyQuestion objects. 
        /// </summary>
        /// <param name="SurvID">Survey ID.</param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns>List of SurveyQuestions</returns>
        public static List<SurveyQuestion> GetQuestionsBySurvey(int SurvID, bool withComments = false, bool withTranslation = false)
        {
            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            SurveyQuestion q;
            string query = "SELECT * FROM qrySurveyQuestions WHERE SurvID = @sid ORDER BY Qnum";

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
                            q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                refVarName = (string)rdr["refVarName"],
                                Qnum = (string)rdr["Qnum"],
                                //AltQnum = (string)rdr["AltQnum"],
                                PrePNum = (int)rdr["PreP#"],
                                PreP = (string)rdr["PreP"],
                                PreINum = (int)rdr["PreI#"],
                                PreI = (string)rdr["PreI"],
                                PreANum = (int)rdr["PreA#"],
                                PreA = (string)rdr["PreA"],
                                LitQNum = (int)rdr["LitQ#"],
                                LitQ = (string)rdr["LitQ"],
                                PstINum = (int)rdr["PstI#"],
                                PstI = (string)rdr["PstI"],
                                PstPNum = (int)rdr["PstP#"],
                                PstP = (string)rdr["PstP"],
                                RespName = (string)rdr["RespName"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRName = (string)rdr["NRName"],
                                NRCodes = (string)rdr["NRCodes"],
                                VarLabel = (string)rdr["VarLabel"],
                                TopicLabel = (string)rdr["Topic"],
                                ContentLabel = (string)rdr["Content"],
                                ProductLabel = (string)rdr["Product"],
                                DomainLabel = (string)rdr["Domain"],
                                TableFormat = (bool)rdr["TableFormat"],
                                CorrectedFlag = (bool)rdr["CorrectedFlag"],
                                NumCol = (int)rdr["NumCol"],
                                NumDec = (int)rdr["NumDec"],
                                VarType = (string)rdr["VarType"],
                                ScriptOnly = (bool)rdr["ScriptOnly"]
                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("NumFmt"))) q.NumFmt = (string)rdr["NumFmt"];

                            if (withComments)
                                q.Comments = GetCommentsByQuestion(q.ID);

                            if (withTranslation)
                                q.Translations = GetTranslationByQuestion(q.ID);

                            qs.Add(q);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return qs;
        }

        /// <summary>
        /// Retrieves a set of records for a particular VarName and returns a list of SurveyQuestion objects. 
        /// </summary>
        /// <param name="varname">A valid VarName.</param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns>List of SurveyQuestions</returns>
        public static List<SurveyQuestion> GetQuestionsByVarName(string varname, bool withComments = false, bool withTranslation = false)
        {
            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            SurveyQuestion q;
            string query = "SELECT * FROM qrySurveyQuestions WHERE VarName = @varname ORDER BY Qnum";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varname", varname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                refVarName = (string)rdr["refVarName"],
                                Qnum = (string)rdr["Qnum"],
                                //AltQnum = (string)rdr["AltQnum"],
                                PrePNum = (int)rdr["PreP#"],
                                PreP = (string)rdr["PreP"],
                                PreINum = (int)rdr["PreI#"],
                                PreI = (string)rdr["PreI"],
                                PreANum = (int)rdr["PreA#"],
                                PreA = (string)rdr["PreA"],
                                LitQNum = (int)rdr["LitQ#"],
                                LitQ = (string)rdr["LitQ"],
                                PstINum = (int)rdr["PstI#"],
                                PstI = (string)rdr["PstI"],
                                PstPNum = (int)rdr["PstP#"],
                                PstP = (string)rdr["PstP"],
                                RespName = (string)rdr["RespName"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRName = (string)rdr["NRName"],
                                NRCodes = (string)rdr["NRCodes"],
                                VarLabel = (string)rdr["VarLabel"],
                                TopicLabel = (string)rdr["Topic"],
                                ContentLabel = (string)rdr["Content"],
                                ProductLabel = (string)rdr["Product"],
                                DomainLabel = (string)rdr["Domain"],
                                TableFormat = (bool)rdr["TableFormat"],
                                CorrectedFlag = (bool)rdr["CorrectedFlag"],
                                NumCol = (int)rdr["NumCol"],
                                NumDec = (int)rdr["NumDec"],
                                VarType = (string)rdr["VarType"],
                                ScriptOnly = (bool)rdr["ScriptOnly"]

                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("NumFmt"))) q.NumFmt = (string)rdr["NumFmt"];

                            if (withComments)
                                q.Comments = GetCommentsByQuestion(q.ID);

                            if (withTranslation)
                                q.Translations = GetTranslationByQuestion(q.ID);

                            qs.Add(q);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return qs;
        }

        /// <summary>
        /// Retrieves a set of records for a particular VarName and returns a list of SurveyQuestion objects. 
        /// </summary>
        /// <param name="refvarname">A valid VarName.</param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        /// <returns>List of SurveyQuestions</returns>
        public static List<SurveyQuestion> GetQuestionsByRefVarName(string refvarname, bool withComments = false, bool withTranslation = false)
        {
            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            SurveyQuestion q;
            string query = "SELECT * FROM qrySurveyQuestions WHERE refVarName = @refvarname ORDER BY Qnum";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refvarname", refvarname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                refVarName = (string)rdr["refVarName"],
                                Qnum = (string)rdr["Qnum"],
                                //AltQnum = (string)rdr["AltQnum"],
                                PrePNum = (int)rdr["PreP#"],
                                PreP = (string)rdr["PreP"],
                                PreINum = (int)rdr["PreI#"],
                                PreI = (string)rdr["PreI"],
                                PreANum = (int)rdr["PreA#"],
                                PreA = (string)rdr["PreA"],
                                LitQNum = (int)rdr["LitQ#"],
                                LitQ = (string)rdr["LitQ"],
                                PstINum = (int)rdr["PstI#"],
                                PstI = (string)rdr["PstI"],
                                PstPNum = (int)rdr["PstP#"],
                                PstP = (string)rdr["PstP"],
                                RespName = (string)rdr["RespName"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRName = (string)rdr["NRName"],
                                NRCodes = (string)rdr["NRCodes"],
                                VarLabel = (string)rdr["VarLabel"],
                                TopicLabel = (string)rdr["Topic"],
                                ContentLabel = (string)rdr["Content"],
                                ProductLabel = (string)rdr["Product"],
                                DomainLabel = (string)rdr["Domain"],
                                TableFormat = (bool)rdr["TableFormat"],
                                CorrectedFlag = (bool)rdr["CorrectedFlag"],
                                NumCol = (int)rdr["NumCol"],
                                NumDec = (int)rdr["NumDec"],
                                VarType = (string)rdr["VarType"],
                                ScriptOnly = (bool)rdr["ScriptOnly"]
                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("NumFmt"))) q.NumFmt = (string)rdr["NumFmt"];

                            if (withComments)
                                q.Comments = GetCommentsByQuestion(q.ID);

                            if (withTranslation)
                                q.Translations = GetTranslationByQuestion(q.ID);

                            qs.Add(q);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            //if (withComments || withTranslation)
            //{
            //    foreach (SurveyQuestion sq in qs)
            //    {
            //        if (withComments)
            //            sq.Comments = GetCommentsByQuestion(sq.ID);

            //        if (withTranslation)
            //            sq.Translations = GetTranslationByQuestion(sq.ID);
            //    }
            //}

            return qs;
        }

        /// <summary>
        /// Fills the raw survey table with wordings, labels, corrected and table flags from a backup database.
        /// </summary>
        /// <remarks>
        /// This could be achieved by changing the FROM clause in GetSurveyTable but often there are columns that don't exist in the backups, due to 
        /// their age and all the changes that have happened to the database over the years. 
        /// </remarks>
        public static List<SurveyQuestion> GetQuestionsBySurveyFromBackup(string surveyCode, DateTime backup)
        {

            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            SurveyQuestion q;
            DataTable rawTable;
            string filePath = backup.ToString("yyyy-MM-dd") + ".7z";
            BackupConnection bkp = new BackupConnection(filePath);
            string select = "SELECT tblSurveyNumbers.[ID], [Qnum] AS SortBy, [Survey], tblSurveyNumbers.[VarName], refVarName, Qnum, AltQnum, CorrectedFlag, TableFormat, tblDomain.[Domain], " +
                "[Topic], [Content], VarLabel, [Product], PreP, [PreP#], PreI, [PreI#], PreA, [PreA#], LitQ, [LitQ#], PstI, [PstI#], PstP, [PstP#], RespOptions, tblSurveyNumbers.RespName, NRCodes, tblSurveyNumbers.NRName " ;
            string where = "Survey = '" + surveyCode + "'";


            if (bkp.Connected)
            {
                Console.Write("unzipped");
                rawTable = bkp.GetSurveyTable(select, where);
            }
            else
            {
                // could not unzip backup/7zip not installed etc. 
                return null;
            }

            foreach (DataRow r in rawTable.Rows)
            {
                q = new SurveyQuestion();

                q.ID = (int)r["ID"];
                q.SurveyCode = (string)r["Survey"];
                q.VarName = (string)r["VarName"];
                q.refVarName = (string)r["refVarName"];
                q.Qnum = (string)r["Qnum"];
                if (!DBNull.Value.Equals(r["AltQnum"])) q.AltQnum = (string)r["AltQnum"];
                q.PrePNum = Convert.ToInt32(r["PreP#"]);
                q.PreP = (string)r["PreP"];
                q.PreINum = Convert.ToInt32(r["PreI#"]);
                q.PreI = (string)r["PreI"];
                q.PreANum = Convert.ToInt32(r["PreA#"]);
                q.PreA = (string)r["PreA"];
                q.LitQNum = Convert.ToInt32(r["LitQ#"]);
                q.LitQ = (string)r["LitQ"];
                q.PstINum = Convert.ToInt32(r["PstI#"]);
                q.PstI = (string)r["PstI"];
                q.PstPNum = Convert.ToInt32(r["PstP#"]);
                q.PstP = (string)r["PstP"];
                q.RespName = (string)r["RespName"];
                q.RespOptions = (string)r["RespOptions"];
                q.NRName = (string)r["NRName"];
                q.NRCodes = (string)r["NRCodes"];
                q.VarLabel = (string)r["VarLabel"];
                q.TopicLabel = (string)r["Topic"];
                q.ContentLabel = (string)r["Content"];
                q.ProductLabel = (string)r["Product"];
                q.DomainLabel = (string)r["Domain"];
                q.TableFormat = (bool)r["TableFormat"];
                q.CorrectedFlag = (bool)r["CorrectedFlag"];
                    //NumCol = (int)r["NumCol"],
                    //NumDec = (int)r["NumDec"],
                    //VarType = (string)r["VarType"],
                    //ScriptOnly = (bool)r["ScriptOnly"]
                

                //if (!string.IsNullOrEmpty((string)r["NumFmt"])) q.NumFmt = (string)r["NumFmt"];

                qs.Add(q);
            }

            return qs;
        }

        /// <summary>
        /// Returns the list of corrected questions for a specified survey.
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns></returns>
        public static List<SurveyQuestion> GetCorrectedWordings(string surveyCode)
        {
            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            string query = "SELECT C.QID AS ID, SN.VarName, C.PreP, C.PreI, C.PreA, C.LitQ, C.PstI, C.PstP, C.RespOptions," +
                "C.NRCodes FROM qrySurveyQuestionsCorrected AS C INNER JOIN qrySurveyQuestions AS SN ON C.QID = SN.ID " +
                "WHERE SN.Survey =@survey";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyCode);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            SurveyQuestion q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                PreP = (string)rdr["PreP"],
                                PreI = (string)rdr["PreI"],
                                PreA = (string)rdr["PreA"],
                                LitQ = (string)rdr["LitQ"],
                                PstI = (string)rdr["PstI"],
                                PstP = (string)rdr["PstP"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRCodes = (string)rdr["NRCodes"],
                            };

                            qs.Add(q);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return qs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="withComments"></param>
        /// <param name="withTranslation"></param>
        public static void FillQuestionsBySurvey(Survey s, bool withComments = false, bool withTranslation = false)
        {
            List<SurveyQuestion> qs = new List<SurveyQuestion>();
            SurveyQuestion q;
            string query = "SELECT * FROM qrySurveyQuestions WHERE SurvID = @sid ORDER BY Qnum";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@sid", s.SID);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            q = new SurveyQuestion
                            {
                                ID = (int)rdr["ID"],
                                SurveyCode = (string)rdr["Survey"],
                                VarName = (string)rdr["VarName"],
                                refVarName = (string)rdr["refVarName"],
                                Qnum = (string)rdr["Qnum"],
                                //AltQnum = (string)rdr["AltQnum"],
                                PrePNum = (int)rdr["PreP#"],
                                PreP = (string)rdr["PreP"],
                                PreINum = (int)rdr["PreI#"],
                                PreI = (string)rdr["PreI"],
                                PreANum = (int)rdr["PreA#"],
                                PreA = (string)rdr["PreA"],
                                LitQNum = (int)rdr["LitQ#"],
                                LitQ = (string)rdr["LitQ"],
                                PstINum = (int)rdr["PstI#"],
                                PstI = (string)rdr["PstI"],
                                PstPNum = (int)rdr["PstP#"],
                                PstP = (string)rdr["PstP"],
                                RespName = (string)rdr["RespName"],
                                RespOptions = (string)rdr["RespOptions"],
                                NRName = (string)rdr["NRName"],
                                NRCodes = (string)rdr["NRCodes"],
                                VarLabel = (string)rdr["VarLabel"],
                                TopicLabel = (string)rdr["Topic"],
                                ContentLabel = (string)rdr["Content"],
                                ProductLabel = (string)rdr["Product"],
                                DomainLabel = (string)rdr["Domain"],
                                TableFormat = (bool)rdr["TableFormat"],
                                CorrectedFlag = (bool)rdr["CorrectedFlag"],
                                NumCol = (int)rdr["NumCol"],
                                NumDec = (int)rdr["NumDec"],
                                VarType = (string)rdr["VarType"],
                                ScriptOnly = (bool)rdr["ScriptOnly"]
                            };

                            if (!rdr.IsDBNull(rdr.GetOrdinal("NumFmt"))) q.NumFmt = (string)rdr["NumFmt"];

                            if (withComments)
                                q.Comments = GetCommentsByQuestion(q.ID);

                            if (withTranslation)
                                q.Translations = GetTranslationByQuestion(q.ID);

                            s.questions.Add(q);
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
