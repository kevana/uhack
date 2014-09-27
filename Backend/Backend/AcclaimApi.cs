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
        private string _userName;
        private System.Security.SecureString _password;
        private string _orgId;
        private static string _baseUrl = "https://sandbox.youracclaim.com/api/v1/users/self";

        /// <summary>
        /// 
        /// </summary>
        public AcclaimApi()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="orgId"></param>
        public AcclaimApi(string userName, System.Security.SecureString password, string orgId)
        {
            _userName = userName;
            _password = password;
            _orgId = orgId;
        }

        /// <summary>
        /// Issue Badge to a user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<object> BadgeIssueRequest(BadgeIssue data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return await RestProxy.ProxyPost<BadgeDataResponse, BadgeIssue, BadgeErrorResponse>(_userName, _password, "", data);
        }

        /// <summary>
        /// Issue Badge with evidence to user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<object> BadgeIssueEvidenceRequest(BadgeEvidence data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges", _orgId);
            return await RestProxy.ProxyPost<BadgeDataResponse, BadgeEvidence, BadgeErrorResponse>(_userName, _password, "", data);
        }

        /// <summary>
        /// Issue a batch of Badges
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<object> BadgeIssueBatchRequest(BadgeBatch data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/batch", _orgId);
            return await RestProxy.ProxyPost<BadgeDataResponse, BadgeBatch, BadgeErrorResponse>(_userName, _password, url, data);
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
        public async Task<string> BadgeGetIssueRequest(Dictionary<FilterEnum, string> filters = null, Dictionary<SortEnum, string> sorts = null, int pages = -1)
        {
            bool filterPresent = false;
            string filter = "";
            if (filters != null && filters.Count > 0)
            {
                filterPresent = true;
                filter = "filter=";
                foreach(KeyValuePair<FilterEnum, string> pair in filters)
                {
                    if(pair.Key != FilterEnum.badge_templates)
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
                if(filterPresent)
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
                page += pages.ToString();
            }

            string url = _baseUrl + String.Format("/organizations/{0}/badges?{1}{2}{3}", _orgId, filter, sort, page);
            return await RestProxy.ProxyGet(_userName, _password, url);
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
        public async Task<object> BadgeReplaceRequest(string badgeId, BadgeReplace data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/{1}", _orgId, badgeId);
            return await RestProxy.ProxyPost<BadgeDataResponse, BadgeReplace, BadgeErrorResponse>(_userName, _password, "", data);
        }

        public async Task<object> BadgeRevokeRequest(string badgeId, BadgeRevoke data)
        {
            string url = _baseUrl + String.Format("/organizations/{0}/badges/{1}", _orgId, badgeId);
            return await RestProxy.ProxyPut<BadgeDataResponse, BadgeRevoke, BadgeErrorResponse>(_userName, _password, "", data);
        }
    
    }

    public class RestProxy
    {
        public static async Task<T1> ProxyPost<T1, T2>(string userName, System.Security.SecureString password, string url, T2 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T2));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PostAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();
                        MemoryStream stream = new MemoryStream(data);

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                        T1 result = (T1)serializer.ReadObject(stream);
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<object> ProxyPost<T1, T2, T3>(string userName, System.Security.SecureString password, string url, T2 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T2));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PostAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();
                        MemoryStream stream = new MemoryStream(data);

                        if (!response.IsSuccessStatusCode) // error
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T3));
                            T3 result = (T3)serializer.ReadObject(stream);
                            return result;
                        }
                        else
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                            T1 result = (T1)serializer.ReadObject(stream);
                            return result;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<string> ProxyPost<T1>(string userName, System.Security.SecureString password, string url, T1 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T1));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PostAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();

                        return System.Text.Encoding.UTF8.GetString(data);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<T1> ProxyGet<T1>(string userName, System.Security.SecureString password, string url)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        response = await client.GetAsync(url);
                        data = await response.Content.ReadAsByteArrayAsync();
                        MemoryStream stream = new MemoryStream(data);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                        T1 result = (T1)serializer.ReadObject(stream);
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<object> ProxyGet<T1, T2>(string userName, System.Security.SecureString password, string url)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        response = await client.GetAsync(url);
                        data = await response.Content.ReadAsByteArrayAsync();
                        if (!response.IsSuccessStatusCode) // error
                        {
                            MemoryStream stream = new MemoryStream(data);
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T2));
                            T2 result = (T2)serializer.ReadObject(stream);
                            return result;
                        }
                        else
                        {
                            MemoryStream stream = new MemoryStream(data);
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                            T1 result = (T1)serializer.ReadObject(stream);
                            return result;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<string> ProxyGet(string userName, System.Security.SecureString password, string url)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        response = await client.GetAsync(url);
                        data = await response.Content.ReadAsByteArrayAsync();
                        return System.Text.Encoding.UTF8.GetString(data);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<T1> ProxyPut<T1, T2>(string userName, System.Security.SecureString password, string url, T2 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T2));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PutAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();
                        MemoryStream stream = new MemoryStream(data);

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                        T1 result = (T1)serializer.ReadObject(stream);
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<string> ProxyPut<T1>(string userName, System.Security.SecureString password, string url, T1 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T1));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PutAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();

                        return System.Text.Encoding.UTF8.GetString(data);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<object> ProxyPut<T1, T2, T3>(string userName, System.Security.SecureString password, string url, T2 postData)
        {
            HttpResponseMessage response = null;
            byte[] data;
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (Stream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer requestSerializer = new DataContractJsonSerializer(typeof(T2));
                            requestSerializer.WriteObject(ms, postData);
                            ms.Position = 0;
                            HttpContent content = new StreamContent(ms);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PutAsync(url, content);
                        }
                        data = await response.Content.ReadAsByteArrayAsync();
                        MemoryStream stream = new MemoryStream(data);
                        if (!response.IsSuccessStatusCode) // error
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T3));
                            T3 result = (T3)serializer.ReadObject(stream);
                            return result;
                        }
                        else
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T1));
                            T1 result = (T1)serializer.ReadObject(stream);
                            return result;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return default(T1);
            }
        }

        public static async Task<byte[]> ProxyStream(string userName, System.Security.SecureString password, string Url)
        {
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.Credentials = new System.Net.NetworkCredential(userName, password);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        byte[] data = await client.GetByteArrayAsync(Url);
                        return data;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}