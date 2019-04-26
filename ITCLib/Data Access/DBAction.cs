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
    public enum AccessLevel { SMG = 1, PMG }
    public enum CommentDetails { Existing = 1, LastUsed, New }
    /// <summary>
    /// Static class for interacting with the Database. TODO create stored procedures on server for each of these
    /// </summary>
    public static partial class DBAction
    {

        public static string[][] GetSimilarWords()
        {
            string[][] similarWords = new string[0][];
            string[] words;
            string currentList;
            int i = 1;
            string query = "SELECT * FROM qryAlternateSpelling";

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
                            Array.Resize(ref similarWords, i);
                            currentList = (string)rdr["word"];
                            words = new string[currentList.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Length];
                            words = currentList.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            similarWords[i-1] = words;
                            i++;
                        }
                    }
                }
                catch (Exception)
                {
                    int j = 0;
                }

                
                
            }
            return similarWords;
        }    

        /// <summary>
        /// Returns a list of RefVariableName objects with the provided refVarName.
        /// </summary>
        /// <param name="refVarName"></param>
        /// <returns></returns>
        public static List<RefVariableName> GetRefVarNames(string refVarName)
        {
            List<RefVariableName> refVarNames = new List<RefVariableName>();
            RefVariableName rv;
            string query = "SELECT refVarName, VarLabel, DomainNum, Domain, TopicNum, Topic, ContentNum, Content, ProductNum, Product FROM qryVariableInfo WHERE refVarName = @refVarName " +
                "GROUP BY refVarName, VarLabel, DomainNum, Domain, TopicNum, Topic, ContentNum, Content, ProductNum, Product ORDER BY refVarName";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@refVarName", refVarName);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            rv = new RefVariableName((string)rdr["refVarName"]);

                            rv.VarLabel = (string)rdr["VarLabel"];
                            rv.Domain = new DomainLabel((int)rdr["DomainNum"], (string)rdr["Domain"]);
                            rv.Topic = new TopicLabel((int)rdr["TopicNum"], (string)rdr["Topic"]);
                            rv.Content = new ContentLabel((int)rdr["ContentNum"], (string)rdr["Content"]);
                            rv.Product = new ProductLabel((int)rdr["ProductNum"], (string)rdr["Product"]);

                            refVarNames.Add(rv);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return refVarNames;
        }

        //
        // Survey Mode
        //
        public static List<SurveyMode> GetModeInfo()
        {
            List<SurveyMode> modes = new List<SurveyMode>();
            SurveyMode m;
            string query = "SELECT * FROM qryMode ORDER BY Mode";

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
                            m = new SurveyMode
                            {
                                ID = (int)rdr["ID"],
                                Mode = (string)rdr["Mode"],
                                ModeAbbrev = (string)rdr["ModeAbbrev"]
                            };

                            modes.Add(m);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return modes;
        }

        //
        // Cohorts
        //
        public static List<SurveyCohort> GetCohortInfo()
        {
            List<SurveyCohort> cohorts = new List<SurveyCohort>();
            SurveyCohort c;
            string query = "SELECT * FROM FN_GetCohortInfo() ORDER BY Cohort";

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
                            c = new SurveyCohort
                            {
                                ID = (int)rdr["ID"],
                                Cohort = (string)rdr["Cohort"],
                                Code = (string)rdr["Code"],
                                WebName = (string)rdr["WebName"],

                            };

                            cohorts.Add(c);

                        }

                    }
                }
                catch (Exception)
                {
                    int i=0;
                }
            }
            return cohorts;
        }

        //
        // Groups
        //
        public static List<SurveyUserGroup> GetGroupInfo()
        {
            List<SurveyUserGroup> groups = new List<SurveyUserGroup>();
            SurveyUserGroup g;
            string query = "SELECT * FROM FN_GetGroupInfo() ORDER BY [Group]";

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
                            g = new SurveyUserGroup
                            {
                                ID = (int)rdr["ID"],
                                UserGroup = (string)rdr["Group"],
                                Code = (string)rdr["Code"],
                                WebName = (string)rdr["WebName"],

                            };

                            groups.Add(g);

                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }
            return groups;
        }

        //
        // Region Info
        //
        public static List<Region> GetRegionInfo(bool getStudies = false)
        {
            List<Region> regions = new List<Region>();
            Region r;
            string query = "SELECT * FROM qryRegion";

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
                            r = new Region
                            {
                                RegionID = (int)rdr["ID"],
                                RegionName = (string)rdr["Region"]
                                
                            };

                            if (getStudies)
                                r.Studies = GetStudies(r.RegionID);

                            regions.Add(r);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return regions;
        }

        //
        // Study Info
        //
        public static List<Study> GetStudyInfo(bool getWaves = false, bool getSurveys = false)
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudyInfo()";

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
                            s =new Study
                            {
                                StudyID = (int)rdr["ID"],
                                CountryName = (string)rdr["Country"],
                                StudyName = (string)rdr["Study"],
                                CountryCode = Int32.Parse((string)rdr["CountryCode"]),
                                ISO_Code = (string)rdr["ISO_Code"],
                                AgeGroup = (string)rdr["AgeGroup"]
                            };

                            if (getWaves)
                                s.Waves = GetWaves(s.StudyID, getSurveys);

                            studies.Add(s);
                        }

                    }
                }
                catch (Exception)
                {
                    
                }
            }

            return studies;
        }

        //
        // Study Info
        //
        public static string GetISOCode(Survey survey)
        {
            string iso = "";
            string query = "SELECT C.ISO_Code FROM qryCountryCodes AS C LEFT JOIN qrySurveyInfo AS S ON C.ID = S.CC_ID WHERE S.Survey = @survey";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", survey.SurveyCode);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {

                            iso = (string) rdr["ISO_Code"];
                           
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }

            return iso;
        }

        public static List<Study> GetStudies(int regionID)
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudies(@regionID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@regionID", regionID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            s = new Study
                            {
                                StudyID = (int)rdr["ID"],
                                CountryName = (string)rdr["Country"],
                                StudyName = (string)rdr["Study"],
                                CountryCode = Int32.Parse((string)rdr["CountryCode"]),
                                ISO_Code = (string)rdr["ISO_Code"],
                                AgeGroup = (string)rdr["AgeGroup"]
                            };

                            studies.Add(s);
                        }

                    }
                }
                catch (Exception)
                {
                    int i = 0;
                }
            }

            return studies;
        }

        //
        // Wave Info
        //
        public static List<StudyWave> GetWaveInfo()
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM qryStudyWaves ORDER BY ISO_Code, Wave";

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
                            w = new StudyWave
                            {
                                WaveID = (int)rdr["WaveID"],
                                ISO_Code = (string)rdr["ISO_Code"],
                                Wave = (double)rdr["Wave"]
                            };

                            

                            waves.Add(w);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return waves;
        }
        public static List<StudyWave> GetWaves(int studyID, bool getSurveys = false)
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM FN_GetWavesByStudy(@studyID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@studyID", studyID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            w = new StudyWave
                            {
                                WaveID = (int)rdr["WaveID"],
                                ISO_Code = (string)rdr["ISO_Code"],
                                Wave = (double)rdr["Wave"]
                            };

                            if (getSurveys)
                                w.Surveys = DBAction.GetSurveys(w.WaveID);

                            waves.Add(w);
                        }

                    }
                }
                catch (Exception)
                {

                }
            }

            return waves;
        }


        

        //
        // Variables
        //

        



        /// <summary>
        /// Returns the list of all variable prefixes in use by a specific survey. TODO use a stored procedure/function for this (eliminate non-standard vars? or make it an option)
        /// </summary>
        /// <param name="surveyFilter"></param>
        /// <returns></returns>
        public static List<string> GetVariablePrefixes(string surveyFilter)
        {
            List<string> prefixes = new List<string>();
            string query = "SELECT Left(VarName,2) AS Prefix FROM qrySurveyQuestions WHERE Survey =@survey GROUP BY Left(VarName,2)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@survey", surveyFilter);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            prefixes.Add((string)rdr["Prefix"]);
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }

            return prefixes;
        }

        /// <summary>
        /// Returns a VariabelName object with the provided VarName. TODO server function
        /// </summary>
        /// <param name="varname">A valid VarName.</param>
        /// <returns> Null is returned if the VarName is not found in the database.</returns>
        public static VariableName GetVariable(string varname)
        {
            VariableName v;
            string query = "SELECT * FROM qryVariableInfo WHERE VarName = @varname";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@varname", varname);
                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        rdr.Read();
                        v = new VariableName ((string)rdr["VarName"])
                        {
                            refVarName = (string)rdr["refVarName"],
                            VarLabel = (string)rdr["VarLabel"],
                            Domain = new DomainLabel ((int)rdr["DomainNum"], ((string)rdr["Domain"])),
                            Topic = new TopicLabel((int)rdr["TopicNum"], ((string)rdr["Topic"])),
                            Content = new ContentLabel((int)rdr["ContentNum"], ((string)rdr["Content"])),
                            Product = new ProductLabel((int)rdr["ProductNum"], ((string)rdr["Product"])),
                        };
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return v;
        }



        

        
    }
}
