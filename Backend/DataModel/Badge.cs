using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    #region Requests
    
    [DataContract]
    public class BadgeIssue
    {
        public string recipient_email { get; set; }

        public string user_id { get; set; }

        public string badge_template_id { get; set; }

        public string issued_at { get; set; }

        public string issued_to { get; set; }

        public string issued_to_first_name { get; set; }

        public string issued_to_last_name { get; set; }

        public string expires_at { get; set; }

    }

    [DataContract]
    public class BadgeBatch
    {
        public BadgeIssue[] Badges { get; set; }
    }

    [DataContract]
    public class BadgeReplace
    {
        public string badge_template_id { get; set; }

        public string issued_at { get; set; }

        public string issued_to { get; set; }

        public string issued_to_first_name { get; set; }

        public string issued_to_last_name { get; set; }

        public string expires_at { get; set; }

        public BadgeEvidenceParent[] evidence { get; set; }

        public string notification_message { get; set; }

    }

    [DataContract]
    public class BadgeEvidence : BadgeIssue
    {
        public BadgeEvidenceParent[] evidence { get; set; }
    }

    [DataContract]
    public class BadgeEvidenceParent
    {
        public string type { get; set; }

        public string name { get; set; }

        public string key { get; set; }

        public string value { get; set; }

        public BadgeEvidenceParent[] values { get; set; }
    }

    [DataContract]
    public class BadgeRevoke
    {
        public string reason { get; set; }
    }

    #endregion

    #region Responses

    #region Errors

    [DataContract]
    public class BadgeErrorResponse
    {
        public DataError data { get; set; }
    }

    [DataContract]
    public class DataError
    {
        public string message { get; set; }

        public Errors[] errors { get; set; }

        public int error_index { get; set; }
    }

    [DataContract]
    public class Errors
    {
        public string attribute { get; set; }

        public string attribute_label { get; set; }

        public string messages { get; set; }
    }

    #endregion

    [DataContract]
    public class BadgeDataResponse
    {
        public Data Data { get; set; }

        public MetaData MetaData { get; set; }
    }

    [DataContract]
    public class Data
    {

        public User user { get; set; }

        public Issuer issuer { get; set; }

        public BadgeTemplate badge_template { get; set; }

        public string id { get; set; }

        public string issued_to { get; set; }

        public string issued_to_first_name { get; set; }

        public string issued_to_last_name { get; set; }

        public string state { get; set; }

        //public bool public  { get; set; }

        public string recipient_email { get; set; }

        public string replacement_badge_id { get; set; }

        public string revocation_reason { get; set; }

        public string created_at { get; set; }

        public string expires_at { get; set; }

        public string issued_at { get; set; }

        public string state_updated_at { get; set; }

        public string image_url { get; set; }

        public AlignmentsData[] alignments { get; set; }

        public BadgeEvidenceParent[] evidence { get; set; }

        public string accept_badge_url { get; set; }
    }

    [DataContract]
    public class MetaData
    {
        public int count { get; set; }

        public int current_page { get; set; }

        public int total_count { get; set; }

        public int total_pages { get; set; }

        public string previous_page_url { get; set; }

        public string next_page_url { get; set; }
    }

    [DataContract]
    public class User
    {
        public string id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string current_position_name { get; set; }

        public string current_organization_name { get; set; }

        public bool confirmed { get; set; }

        public string photo_url { get; set; }
    }

    [DataContract]
    public class Issuer
    {
        public string type { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public bool verified { get; set; }
    }

    [DataContract]
    public class AlignmentsData
    {
        public string name { get; set; }

        public string description { get; set; }
    }

    [DataContract]
    public class BadgeTemplate
    {
        public string id { get; set; }

        public string description { get; set; }

        public string global_activity_url { get; set; }

        public string name { get; set; }

        public string state { get; set; }

        //public bool public { get; set; }

        public string criteria_url_name { get; set; }

        public string criteria_url { get; set; }

        public int badges_count { get; set; }

        public string archived_at { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }

        public string template_type { get; set; }

        public string image_url { get; set; }

        public Owner owner { get; set; }

        public Alignments[] alignments { get; set; }

        public CriteriaBulletPoints[] criteria_bullet_points { get; set; }

        public Recommendations[] recommendations { get; set; }

        public RequiredBadgeTemplates[] required_badge_templates { get; set; }

        public BadgeTemplateActivities[] badge_template_activities { get; set; }

        public string[] occupations { get; set; }

        public string[] skills { get; set; }
    }

    [DataContract]
    public class Owner
    {
        public string type { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public bool verified { get; set; }
    }

    [DataContract]
    public class Alignments
    {
        public string id { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public string description { get; set; }
    }

    [DataContract]
    public class CriteriaBulletPoints
    {
        public string id { get; set; }

        public string description { get; set; }
    }

    [DataContract]
    public class Recommendations
    {
        public string id { get; set; }

        public string title { get; set; }

        public string type { get; set; }

        public string activity_url { get; set; }
    }

    [DataContract]
    public class RequiredBadgeTemplates
    {

    }

    [DataContract]
    public class BadgeTemplateActivities
    {

    }

    #endregion

}
