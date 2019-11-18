using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace ITCLib
{
    //TODO create null-safe column read
    
    /// <summary>
    /// Static class for interacting with the Database. TODO create stored procedures on server for each of these
    /// </summary>
    public static partial class DBAction
    {

        

        /// <summary>
        /// Returns the list of regions.
        /// </summary>
        /// <returns></returns>
        public static List<Region> GetRegionInfo()
        {
            List<Region> regions = new List<Region>();
            Region r;
            string query = "SELECT * FROM FN_GetAllRegions()";

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
                            r = new Region
                            {
                                RegionID = (int)rdr["ID"],
                                RegionName = (string)rdr["Region"]
                                
                            };

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

        /// <summary>
        /// Returns the list of studies.
        /// </summary>
        /// <returns></returns>
        public static List<Study> GetStudyInfo()
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudyInfo()";

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
                            s =new Study
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
                    
                }
            }

            return studies;
        }

        /// <summary>
        /// Returns the studies for a particular region.
        /// </summary>
        /// <param name="regionID"></param>
        /// <returns></returns>
        public static List<Study> GetStudies(int regionID)
        {
            List<Study> studies = new List<Study>();
            Study s;
            string query = "SELECT * FROM FN_GetStudies(@regionID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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

                }
            }

            return studies;
        }

        /// <summary>
        /// Returns a list of survey waves.
        /// </summary>
        /// <returns></returns>
        public static List<StudyWave> GetWaveInfo()
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM FN_GetAllWaves() ORDER BY ISO_Code, Wave";

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

        /// <summary>
        /// Returns the list of waves for a study.
        /// </summary>
        /// <param name="studyID"></param>
        /// <param name="getSurveys"></param>
        /// <returns></returns>
        public static List<StudyWave> GetWaves(int studyID)
        {
            List<StudyWave> waves = new List<StudyWave>();
            StudyWave w;
            string query = "SELECT * FROM FN_GetWavesByStudy(@studyID)";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionString"].ConnectionString))
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

        

    }
}
