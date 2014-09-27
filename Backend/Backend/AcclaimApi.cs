using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Backend
{
    public class AcclaimApi
    {
        private string _username;
        private string _password;
        private string _orgId;
        private static string _baseUrl = "https://sandbox.youracclaim.com/api/v1";

        /// <summary>
        /// 
        /// </summary>
        public AcclaimApi()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="orgId"></param>
        public AcclaimApi(string username, string password, string orgId)
        {
            _username = username;
            _password = password;
            _orgId = orgId;
        }

        /// <summary>
        /// Issue Badge to a user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BadgeDataResponse BadgeIssueRequest(BadgeIssue data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return RestProxy.ProxyPost<BadgeDataResponse, BadgeIssue>(_username, _password, "", data);
        }

        /// <summary>
        /// Issue Badge to a user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string BadgeIssueRequestJson(BadgeIssue data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return RestProxy.ProxyPost<BadgeIssue>(_username, _password, "", data);
        }

        /// <summary>
        /// Issue Badge with evidence to user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BadgeDataResponse BadgeIssueEvidenceRequest(BadgeEvidence data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return RestProxy.ProxyPost<BadgeDataResponse, BadgeEvidence>(_username, _password, "", data);
        }

        /// <summary>
        /// Issue Badge with evidence to user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string BadgeIssueEvidenceRequestJson(BadgeEvidence data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return RestProxy.ProxyPost<BadgeEvidence>(_username, _password, "", data);
        }

        /// <summary>
        /// Issue a batch of Badges
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BadgeDataResponse BadgeIssueBatchRequest(BadgeBatch data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/batch", _orgId);
            return RestProxy.ProxyPost<BadgeDataResponse, BadgeBatch>(_username, _password, url, data);
        }

        /// <summary>
        /// Issue a batch of Badges
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string BadgeIssueBatchRequestJson(BadgeBatch data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/batch", _orgId);
            return RestProxy.ProxyPost<BadgeBatch>(_username, _password, url, data);
        }

        public enum FilterEnum { query, state, user_id, issuer_id, badge_templates };

        public enum SortEnum { issued_at, state_updated_at, badge_templates, users, n_issued_at, n_state_updated_at, n_badge_templates, n_users };

        /// <summary>
        /// Get filtered and sorted issued badges results
        /// </summary>
        /// <param name="filters">Dictionary of filter types and values. Set null for no filters.</param>
        /// <param name="sorts">Dictionary of sort types and values. Set null for no sorts.</param>
        /// <param name="pages">The page of results to return.</param>
        /// <returns></returns>
        public BadgeDataResponse BadgeGetIssueRequest(Dictionary<FilterEnum, string> filters = null, Dictionary<SortEnum, string> sorts = null, int pages = -1)
        {
            string url = parseFilterSort(filters, sorts, pages);
            return RestProxy.ProxyGet<BadgeDataResponse>(_username, _password, url);
        }

        /// <summary>
        /// Get filtered and sorted issued badges results
        /// </summary>
        /// <param name="filters">Dictionary of filter types and values. Set null for no filters.</param>
        /// <param name="sorts">Dictionary of sort types and values. Set null for no sorts.</param>
        /// <param name="pages">The page of results to return.</param>
        /// <returns></returns>
        public string BadgeGetIssueRequestJson(Dictionary<FilterEnum, string> filters = null, Dictionary<SortEnum, string> sorts = null, int pages = -1)
        {
            string url = parseFilterSort(filters, sorts, pages);
            return RestProxy.ProxyGet(_username, _password, url);
        }

        private string parseFilterSort(Dictionary<FilterEnum, string> filters = null, Dictionary<SortEnum, string> sorts = null, int pages = -1)
        {
            bool filterPresent = false;
            string filter = "";
            if (filters != null && filters.Count > 0)
            {
                filterPresent = true;
                filter = "filter=";
                foreach (KeyValuePair<FilterEnum, string> pair in filters)
                {
                    if (pair.Key != FilterEnum.badge_templates)
                        filter += String.Format("{0}::{1}|", FilterEnumToString(pair.Key), pair.Value);
                    else
                        filter += String.Format("{0}[{1}]|", FilterEnumToString(pair.Key), pair.Value);
                }
                filter.Remove(filter.Length - 1);
            }

            string sort = "";
            if (sorts != null && sorts.Count > 0)
            {
                sort = "sort=";
                if (filterPresent)
                    sort = "&" + sort;

                filterPresent = true;
                foreach (KeyValuePair<SortEnum, string> pair in sorts)
                {
                    if (pair.Key == SortEnum.issued_at || pair.Key == SortEnum.n_issued_at ||
                        pair.Key == SortEnum.n_state_updated_at || pair.Key == SortEnum.state_updated_at)
                        sort += String.Format("{0}|", SortEnumToString(pair.Key));
                    else
                        sort += String.Format("{0}[{1}]|", SortEnumToString(pair.Key), pair.Value);
                }
                sort.Remove(sort.Length - 1);
            }

            string page = "";
            if (pages >= 0)
            {
                page = "page=";
                if (filterPresent)
                    page = "&" + page;
                filterPresent = true;
                page += pages.ToString();
            }
            string urlAdd = "/organizations/{0}/badges{1}{2}{3}";
            if (filterPresent)
                urlAdd = "/organizations/{0}/badges?{1}{2}{3}";
            return _baseUrl + String.Format(urlAdd, _orgId, filter, sort, page);
        }

        public static string FilterEnumToString(FilterEnum value)
        {
            switch(value)
            {
                case FilterEnum.badge_templates:
                    return "badge_templates";
                case FilterEnum.issuer_id:
                    return "issuer_id";
                case FilterEnum.query:
                    return "query";
                case FilterEnum.state:
                    return "state";
                case FilterEnum.user_id:
                    return "user_id";
                default:
                    return "";
            }
        }

        public static string SortEnumToString(SortEnum value)
        {
            switch(value)
            {
                case SortEnum.badge_templates:
                    return "badge_templates";
                case SortEnum.issued_at:
                    return "issued_at";
                case SortEnum.state_updated_at:
                    return "state_updated_at";
                case SortEnum.users:
                    return "users";
                case SortEnum.n_badge_templates:
                    return "-badge_templates";
                case SortEnum.n_issued_at:
                    return "-issued_at";
                case SortEnum.n_state_updated_at:
                    return "-state_updated_at";
                case SortEnum.n_users:
                    return "-users";
                default:
                    return "";

            }
        }

        /// <summary>
        /// Replace a Badge
        /// </summary>
        /// <param name="badgeId">The Badge to be replaced</param>
        /// <param name="data">The replacing Badge</param>
        /// <returns></returns>
        public BadgeDataResponse BadgeReplaceRequest(string badgeId, BadgeReplace data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/{1}", _orgId, badgeId);
            return RestProxy.ProxyPost<BadgeDataResponse, BadgeReplace>(_username, _password, "", data);
        }

        /// <summary>
        /// Revokes the URL of the Badge
        /// </summary>
        /// <param name="badgeId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public BadgeDataResponse BadgeRevokeRequest(string badgeId, BadgeRevoke data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/{1}", _orgId, badgeId);
            return RestProxy.ProxyPut<BadgeDataResponse, BadgeRevoke>(_username, _password, "", data);
        }
    
    }

    public class RestProxy
    {
        public static string ProxyPost<T1>(string username, string password, string url, T1 postData)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                if (postData == null)
                    throw new ArgumentNullException();

                Stream dataStream = request.GetRequestStream();
                Serialize(dataStream, postData);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                byte[] buff = new byte[512];
                StringBuilder str = new StringBuilder();
                while (stream.Read(buff, 0, 512) > 0)
                {
                    str.Append(System.Text.Encoding.UTF8.GetString(buff));
                }

                return str.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T1 ProxyPost<T1, T2>(string username, string password, string url, T2 postData)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                if (postData == null)
                    throw new ArgumentNullException();

                Stream dataStream = request.GetRequestStream();
                Serialize(dataStream, postData);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                T1 result = (T1)serializer.ReadObject(stream);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ProxyGet(string username, string password, string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                byte[] buff = new byte[512];
                StringBuilder str = new StringBuilder();
                while (stream.Read(buff, 0, 512) > 0)
                {
                    str.Append(System.Text.Encoding.UTF8.GetString(buff));
                }

                return str.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T1 ProxyGet<T1>(string username, string password, string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                T1 result = (T1)serializer.ReadObject(stream);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ProxyPut<T1>(string username, string password, string url, T1 postData)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                if (postData == null)
                    throw new ArgumentNullException();

                Stream dataStream = request.GetRequestStream();
                Serialize(dataStream, postData);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                byte[] buff = new byte[512];
                StringBuilder str = new StringBuilder();
                while (stream.Read(buff, 0, 512) > 0)
                {
                    str.Append(System.Text.Encoding.UTF8.GetString(buff));
                }

                return str.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T1 ProxyPut<T1, T2>(string username, string password, string url, T2 postData)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;

                if (postData == null)
                    throw new ArgumentNullException();

                Stream dataStream = request.GetRequestStream();
                Serialize(dataStream, postData);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                T1 result = (T1)serializer.ReadObject(stream);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void Serialize(Stream output, object input)
        {
            var ser = new DataContractSerializer(input.GetType());
            ser.WriteObject(output, input);
        }
    }
}