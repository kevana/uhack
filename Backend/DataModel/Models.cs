using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [DataContract]
    public class Nomination
    {
        public int nomination_id { get; set; }

        // user_id

        public KudosUser user { get; set; } // Not in table

        public Badge badge { get; set; }
        
        public string organization_id { get; set; }

        public string nomination_description { get; set; }

        public Rating[] ratings { get; set; } // Not in table

        public int vote_count { get; set; }

        public int req_vote_count { get; set; }

        public string timestamp { get; set; }
    }

    [DataContract]
    public class Badge // Part of Nominations table
    {
        public string badge_id { get; set; }

        public string badge_name { get; set; }

        public string badge_description { get; set; }

        public string badge_url { get; set; }
    }

    [DataContract]
    public class Rating
    {
        public int rating_id { get; set; }

        public string review { get; set; }

        // user_id

        public KudosUser user { get; set; } // Not in table

        public string nomination_id { get; set; }

        public string timestamp { get; set; }
    }

    [DataContract]
    public class KudosUser
    {
        public int user_id { get; set; }

        public string user_first_name { get; set; }

        public string user_last_name { get; set; }

        public string user_name { get; set; }

        public string email { get; set; }

        public string user_pic_url { get; set; }
    }
}
