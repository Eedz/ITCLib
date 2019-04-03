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
        //
        // Domain Label
        //
        /// <summary>
        /// TODO add withCount arg to count uses at the same time
        /// </summary>
        /// <returns></returns>
        public static List<DomainLabel> GetDomainLabels()
        {
            List<DomainLabel> domains = new List<DomainLabel>();
            DomainLabel d;
            string query = "SELECT * FROM qryDomain ORDER BY Domain";

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
                            d = new DomainLabel ((int)rdr["ID"], (string)rdr["Domain"]);

                            domains.Add(d);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return domains;
        }

        public static int GetDomainLabelsUses(int DomainID)
        {

            int count=0;
            string query = "SELECT COUNT(*) AS DomainCount FROM qryVariableInfo WHERE DomainNum = @domainID";

            using (SqlDataAdapter sql = new SqlDataAdapter())
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ISISConnectionStringTest"].ConnectionString))
            {
                conn.Open();

                sql.SelectCommand = new SqlCommand(query, conn);
                sql.SelectCommand.Parameters.AddWithValue("@domainID", DomainID);

                try
                {
                    using (SqlDataReader rdr = sql.SelectCommand.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            count = (int)rdr["DomainCount"];

                            
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return count;
        }

        //
        // Topic Label
        //
        public static List<TopicLabel> GetTopicLabels()
        {
            List<TopicLabel> topics = new List<TopicLabel>();
            TopicLabel t;
            string query = "SELECT * FROM qryTopic ORDER BY Topic";

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
                            t = new TopicLabel ( (int)rdr["ID"], (string)rdr["Topic"]);

                            topics.Add(t);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return topics;
        }

        //
        // Content Label
        //
        public static List<ContentLabel> GetContentLabels()
        {
            List<ContentLabel> contents = new List<ContentLabel>();
            ContentLabel c;
            string query = "SELECT * FROM qryContent ORDER BY Content";

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
                            c = new ContentLabel((int)rdr["ID"], (string)rdr["Content"]);
                            

                            contents.Add(c);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return contents;
        }

        //
        // Product Label
        //
        public static List<ProductLabel> GetProductLabels()
        {
            List<ProductLabel> products = new List<ProductLabel>();
            ProductLabel t;
            string query = "SELECT * FROM qryProduct ORDER BY Product";

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
                            t = new ProductLabel ((int)rdr["ID"],(string)rdr["Product"]);

                            products.Add(t);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return products;
        }
    }
}
