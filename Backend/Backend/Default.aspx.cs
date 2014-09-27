using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataModel;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Web.Script.Serialization;
using MySql.Data.MySqlClient;

namespace Backend
{
    public partial class _Default : Page
    {

        private static string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static string _apiStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["AcclaimApiConnection"].ConnectionString;
        Connector connector = new Connector();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Gets the feed of the latest nominations.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetFeed(string datetime)
        {
            try
            {
                List<Nomination> nominations = new List<Nomination>();
                int user_id = -1;

                using (MySqlConnection sqlConn = new MySqlConnection(_conStr))
                {
                    sqlConn.Open();

                    MySqlCommand cmdNom = new MySqlCommand();
                    cmdNom.Connection = sqlConn;
                    cmdNom.CommandType = System.Data.CommandType.Text;
                    cmdNom.CommandText = String.Format("SELECT * FROM nominations WHERE timestamp >= {0};", datetime);

                    // get nomination
                    using (MySqlDataReader rdrNom = cmdNom.ExecuteReader())
                    {
                        while (rdrNom.Read())
                        {
                            // get nomination
                            Nomination nom = new Nomination();
                            nom.nomination_id = rdrNom.GetInt32("nomination_id");
                            nom.organization_id = rdrNom.GetString("organization_id");
                            nom.nomination_description = rdrNom.GetString("nomination_description");
                            nom.vote_count = rdrNom.GetInt32("vote_count");
                            nom.req_vote_count = rdrNom.GetInt32("req_vote_count");
                            nom.timestamp = rdrNom.GetString("timestamp");

                            // get Badge
                            Badge badge = new Badge();
                            badge.badge_id = rdrNom.GetString("badge_id");
                            badge.badge_name = rdrNom.GetString("badge_name");
                            badge.badge_description = rdrNom.GetString("badge_description");
                            badge.badge_url = rdrNom.GetString("badge_url");

                            user_id = rdrNom.GetInt32("user_id");

                            nom.badge = badge;
                            nom.ratings = new Rating[0];

                            // Get nominated user
                            if (user_id >= 0)
                            {
                                using (MySqlConnection sqlConn2 = new MySqlConnection(_conStr))
                                {
                                    sqlConn2.Open();

                                    MySqlCommand cmdUsr = new MySqlCommand();
                                    cmdUsr.Connection = sqlConn2;
                                    cmdUsr.CommandType = System.Data.CommandType.Text;
                                    cmdUsr.CommandText = String.Format("SELECT * FROM users WHERE user_id = {0};", user_id);

                                    using (MySqlDataReader rdrUsr = cmdUsr.ExecuteReader())
                                    {
                                        while (rdrUsr.Read())
                                        {
                                            KudosUser user = new KudosUser();
                                            user.user_id = rdrUsr.GetInt32("user_id");
                                            user.user_first_name = rdrUsr.GetString("user_first_name");
                                            user.user_last_name = rdrUsr.GetString("user_last_name");
                                            user.user_name = rdrUsr.GetString("user_name");
                                            user.email = rdrUsr.GetString("email");
                                            user.user_pic_url = rdrUsr.GetString("user_pic_url");

                                            nom.user = user;
                                            break;
                                        }
                                    }
                                }
                            }
                            nominations.Add(nom);
                        }
                    }
                }

                return new JavaScriptSerializer().Serialize(nominations.ToArray());
            }
            catch (Exception ex)
            {
                return "false: " + ex.Message;
            }
        }
    
        /// <summary>
        /// Gets the back a recognition nomination page.
        /// </summary>
        /// <param name="nomination_id"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetBacking(string nomination_id)
        {
            try
            {
                Nomination nom = new Nomination();
                int user_id = -1;

                using (MySqlConnection sqlConn = new MySqlConnection(_conStr))
                {
                    sqlConn.Open();

                    MySqlCommand cmdNom = new MySqlCommand();
                    cmdNom.Connection = sqlConn;
                    cmdNom.CommandType = System.Data.CommandType.Text;
                    cmdNom.CommandText = String.Format("SELECT * FROM nominations WHERE nomination_id = {0};", nomination_id);

                    // get nomination
                    using (MySqlDataReader rdrNom = cmdNom.ExecuteReader())
                    {
                        rdrNom.Read();
                        // get nomination
                        nom.nomination_id = rdrNom.GetInt32("nomination_id");
                        nom.organization_id = rdrNom.GetString("organization_id");
                        nom.nomination_description = rdrNom.GetString("nomination_description");
                        nom.vote_count = rdrNom.GetInt32("vote_count");
                        nom.req_vote_count = rdrNom.GetInt32("req_vote_count");
                        nom.timestamp = rdrNom.GetString("timestamp");

                        // get Badge
                        Badge badge = new Badge();
                        badge.badge_id = rdrNom.GetString("badge_id");
                        badge.badge_name = rdrNom.GetString("badge_name");
                        badge.badge_description = rdrNom.GetString("badge_description");
                        badge.badge_url = rdrNom.GetString("badge_url");

                        user_id = rdrNom.GetInt32("user_id");

                        nom.badge = badge;
                        nom.ratings = new Rating[0];
                    }

                    // Get nominated user
                    if (user_id >= 0)
                    {
                        MySqlCommand cmdUsr = new MySqlCommand();
                        cmdUsr.Connection = sqlConn;
                        cmdUsr.CommandType = System.Data.CommandType.Text;
                        cmdUsr.CommandText = String.Format("SELECT * FROM users WHERE user_id = {0};", user_id);

                        using (MySqlDataReader rdrUsr = cmdUsr.ExecuteReader())
                        {
                            rdrUsr.Read();
                            KudosUser user = new KudosUser();
                            user.user_id = rdrUsr.GetInt32("user_id");
                            user.user_first_name = rdrUsr.GetString("user_first_name");
                            user.user_last_name = rdrUsr.GetString("user_last_name");
                            user.user_name = rdrUsr.GetString("user_name");
                            user.email = rdrUsr.GetString("email");
                            user.user_pic_url = rdrUsr.GetString("user_pic_url");

                            nom.user = user;
                        }
                    }

                    // Get Ratings
                    MySqlCommand cmdRat = new MySqlCommand();
                    cmdRat.Connection = sqlConn;
                    cmdRat.CommandType = System.Data.CommandType.Text;
                    cmdRat.CommandText = String.Format("SELECT * FROM ratings WHERE nomination_id = {0} ORDER BY timestamp DESC;", nomination_id);

                    List<Rating> ratingsList = new List<Rating>();

                    using (MySqlDataReader rdrRat = cmdRat.ExecuteReader())
                    {
                        while (rdrRat.Read())
                        {
                            Rating rating = new Rating();
                            rating.rating_id = rdrRat.GetInt32("rating_id");
                            rating.nomination_id = rdrRat.GetString("nomination_id");
                            rating.review = rdrRat.GetString("review");
                            rating.timestamp = rdrRat.GetString("timestamp");

                            user_id = rdrRat.GetInt32("user_id");

                            // Get rating user
                            if (user_id >= 0)
                            {
                                using (MySqlConnection sqlConn2 = new MySqlConnection(_conStr))
                                {
                                    sqlConn2.Open();

                                    MySqlCommand cmdUsr = new MySqlCommand();
                                    cmdUsr.Connection = sqlConn2;
                                    cmdUsr.CommandType = System.Data.CommandType.Text;
                                    cmdUsr.CommandText = String.Format("SELECT * FROM users WHERE user_id = {0};", user_id);

                                    using (MySqlDataReader rdrUsr = cmdUsr.ExecuteReader())
                                    {
                                        rdrUsr.Read();
                                        KudosUser user = new KudosUser();
                                        user.user_id = rdrUsr.GetInt32("user_id");
                                        user.user_first_name = rdrUsr.GetString("user_first_name");
                                        user.user_last_name = rdrUsr.GetString("user_last_name");
                                        user.user_name = rdrUsr.GetString("user_name");
                                        user.email = rdrUsr.GetString("email");
                                        user.user_pic_url = rdrUsr.GetString("user_pic_url");

                                        rating.user = user;
                                    }
                                }
                            }

                            ratingsList.Add(rating);
                        } // while
                    } // using

                    nom.ratings = ratingsList.ToArray();

                } // using
                
                return new JavaScriptSerializer().Serialize(nom);
            }
            catch (Exception ex)
            {
                return "false: " + ex.Message;
            }
        }

        /// <summary>
        /// Creates a new nomination and subsequently a new user.
        /// </summary>
        /// <param name="user_first_name"></param>
        /// <param name="user_last_name"></param>
        /// <param name="email"></param>
        /// <param name="badge_id"></param>
        /// <param name="badge_name"></param>
        /// <param name="badge_description"></param>
        /// <param name="badge_url"></param>
        /// <param name="nomination_description"></param>
        /// <param name="req_vote_count"></param>
        /// <param name="organization_id"></param>
        /// <returns></returns>
        [WebMethod]
        public static string CreateNomination(string user_first_name, string user_last_name, string email,
            string badge_id, string badge_name, string badge_description, string badge_url,
            string nomination_description, int req_vote_count, string organization_id)
        {
            Dictionary<string, object> paramUsr = new Dictionary<string, object>();
            Dictionary<string, object> paramNom = new Dictionary<string, object>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_conStr))
                {
                    conn.Open();
                    int user_id = Connector.GetMaxId("users", "user_id") + 1;
                    paramUsr.Add("user_id", user_id);
                    paramUsr.Add("user_first_name", user_first_name);
                    paramUsr.Add("user_last_name", user_last_name);
                    paramUsr.Add("user_name", user_first_name + " " + user_last_name);
                    paramUsr.Add("email", email);
                    paramUsr.Add("user_pic_url", "http://www.finmap-fp7.eu/no_pic.png");

                    if (paramUsr.Count > 0)
                    {
                        MySqlCommand cmd = Connector.CreateInsertCmd("users", paramUsr);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }

                    paramNom.Add("user_id", user_id);
                    paramNom.Add("nomination_id", Connector.GetMaxId("nominations", "nomination_id") + 1);
                    paramNom.Add("badge_id", badge_id);
                    paramNom.Add("badge_name", badge_name);
                    paramNom.Add("badge_description", badge_description);
                    paramNom.Add("badge_url", badge_url);
                    paramNom.Add("nomination_description", nomination_description);
                    paramNom.Add("vote_count", 0);
                    paramNom.Add("req_vote_count", req_vote_count);
                    paramNom.Add("organization_id", organization_id);
                    paramNom.Add("timestamp", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now));

                    if (paramNom.Count > 0)
                    {
                        MySqlCommand cmd = Connector.CreateInsertCmd("Nominations", paramNom);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                }

                return "true";
            }
            catch (Exception ex)
            {
                return "false: " + ex.Message;
            }
        }

        /// <summary>
        /// Creates a new nomination rating/review entry.
        /// </summary>
        /// <param name="nomination_id"></param>
        /// <param name="user_id"></param>
        /// <param name="review"></param>
        /// <returns></returns>
        [WebMethod]
        public static string CreateRating(int nomination_id, int user_id, string review)
        {
            Dictionary<string, object> paramRat = new Dictionary<string, object>();
            Dictionary<string, object> paramNom = new Dictionary<string, object>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_conStr))
                {
                    conn.Open();

                    if (!String.IsNullOrEmpty(review))
                    {
                        paramRat.Add("rating_id", Connector.GetMaxId("ratings", "rating_id") + 1);
                        paramRat.Add("review", review);
                        paramRat.Add("nomination_id", nomination_id);
                        paramRat.Add("user_id", user_id);
                        paramRat.Add("timestamp", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now));

                        if (paramRat.Count > 0)
                        {
                            MySqlCommand cmd = Connector.CreateInsertCmd("ratings", paramRat);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MySqlCommand cmdNom = new MySqlCommand();
                    cmdNom.Connection = conn;
                    cmdNom.CommandType = System.Data.CommandType.Text;
                    cmdNom.CommandText = String.Format("SELECT vote_count FROM nominations WHERE nomination_id = {0};", nomination_id);

                    int vote_count = -1;
                    using (MySqlDataReader rdrNom = cmdNom.ExecuteReader())
                    {
                        rdrNom.Read();
                        vote_count = rdrNom.GetInt32("vote_count");
                    }

                    if(vote_count >= 0)
                    {
                        paramNom.Add("vote_count", vote_count++);
                        MySqlCommand cmd = Connector.CreateUpdateCmd("nominations", paramNom, new Tuple<string, object>("nomination_id", nomination_id));
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                }

                return "true";
            }
            catch (Exception ex)
            {
                return "false: " + ex.Message;
            }
        }

        /// <summary>
        /// Gets the badges issued by the organization.
        /// </summary>
        /// <param name="organization_id"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetBadges(string organization_id)
        {
            try
            {
                return getApiLogin().BadgeGetIssueRequestJson();
            }
            catch (Exception ex)
            {
                return "false: " + ex.Message;
            }
        }

        /// <summary>
        /// Gets the username, password, and organization for the API login.
        /// </summary>
        /// <returns></returns>
        private static AcclaimApi getApiLogin()
        {
            string[] parts = _apiStr.Split(new char[]{';'});
            if (parts.Length != 3)
                throw new Exception("Invalid login string.");
            string username = parts[0];
            string password = parts[1];
            string organization = parts[2];

            return new AcclaimApi(username, password, organization);
        }

    }
}