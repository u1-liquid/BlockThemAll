using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlockThemAll
{
    public class TwitterOAuth
    {
        private static readonly string[] oauth_array =
        {
            "oauth_consumer_key", "oauth_version", "oauth_nonce", "oauth_signature",
            "oauth_signature_method", "oauth_timestamp", "oauth_token", "oauth_callback"
        };

        private static readonly DateTime GenerateTimeStampDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        static TwitterOAuth()
        {
            ServicePointManager.Expect100Continue = false;
        }

        public TwitterOAuth(string appToken, string appSecret)
            : this(appToken, appSecret, null, null) {}

        public TwitterOAuth(string appToken, string appSecret, string userToken, string userSecret)
        {
            App = new TokenPair(appToken, appSecret);
            User = new TokenPair(userToken, userSecret);
        }

        public TokenPair App { get; }
        public TokenPair User { get; }

        public HttpWebRequest MakeRequest(string method, string url, object data = null)
        {
            method = method.ToUpper();
            Uri uri = new Uri(url);
            SortedDictionary<string, object> dic = new SortedDictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(uri.Query))
                AddDictionary(dic, uri.Query);

            if (data != null)
                AddDictionary(dic, data);

            if (!string.IsNullOrWhiteSpace(User.Token))
                dic.Add("oauth_token", UrlEncode(User.Token));

            dic.Add("oauth_consumer_key", UrlEncode(App.Token));
            dic.Add("oauth_nonce", GetNonce());
            dic.Add("oauth_timestamp", GetTimeStamp());
            dic.Add("oauth_signature_method", "HMAC-SHA1");
            dic.Add("oauth_version", "1.0");

            string hashKey = $"{UrlEncode(App.Secret)}&{(User.Secret == null ? null : UrlEncode(User.Secret))}";
            string hashData =
                $"{method.ToUpper()}&{UrlEncode($"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Host}{uri.AbsolutePath}")}&{UrlEncode(ToString(dic))}";

            using (HMACSHA1 hash = new HMACSHA1(Encoding.UTF8.GetBytes(hashKey)))
                dic.Add("oauth_signature", UrlEncode(Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(hashData)))));

            StringBuilder sbData = new StringBuilder();
            sbData.Append("OAuth ");
            foreach (KeyValuePair<string, object> st in dic)
                if (Array.IndexOf(oauth_array, st.Key) >= 0)
                    sbData.AppendFormat("{0}=\"{1}\",", st.Key, Convert.ToString(st.Value));
            sbData.Remove(sbData.Length - 1, 1);

            HttpWebRequest req = WebRequest.CreateHttp(uri);
            req.Method = method;
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            req.UserAgent = "Twitter Client";
            req.Headers.Add("Authorization", sbData.ToString());

            if (method == "POST")
                req.ContentType = "application/x-www-form-urlencoded";

            return req;
        }

        private static string GetNonce()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string GetTimeStamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - GenerateTimeStampDateTime).TotalSeconds).ToString();
        }

        private static string UrlEncode(string str)
        {
            string uriData = Uri.EscapeDataString(str);
            StringBuilder sb = new StringBuilder(uriData.Length);

            foreach (char t in uriData)
                switch (t)
                {
                    case '!':
                        sb.Append("%21");
                        break;
                    case '*':
                        sb.Append("%2A");
                        break;
                    case '\'':
                        sb.Append("%5C");
                        break;
                    case '(':
                        sb.Append("%28");
                        break;
                    case ')':
                        sb.Append("%29");
                        break;
                    default:
                        sb.Append(t);
                        break;
                }

            return sb.ToString();
        }

        private static string ToString(IDictionary<string, object> dic)
        {
            if (dic == null) return null;

            StringBuilder sb = new StringBuilder();

            if (dic.Count > 0)
            {
                foreach (KeyValuePair<string, object> st in dic)
                    if (st.Value is bool)
                        sb.AppendFormat("{0}={1}&", st.Key, (bool) st.Value ? "true" : "false");
                    else
                        sb.AppendFormat("{0}={1}&", st.Key, Convert.ToString(st.Value));

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public static string ToString(object values)
        {
            if (values == null) return null;

            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo p in values.GetType().GetProperties())
            {
                if (!p.CanRead) continue;

                string name = p.Name;
                object value = p.GetValue(values, null);

                if (value is bool)
                    sb.AppendFormat("{0}={1}&", name, (bool) value ? "true" : "false");
                else
                    sb.AppendFormat("{0}={1}&", name, UrlEncode(Convert.ToString(value)));
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        private static void AddDictionary(IDictionary<string, object> dic, string query)
        {
            if ((query != null) && (!string.IsNullOrWhiteSpace(query) || (query.Length > 1)))
            {
                int read = 0;

                if (query[0] == '?')
                    read = 1;

                while (read < query.Length)
                {
                    int find = query.IndexOf('=', read);
                    string key = query.Substring(read, find - read);
                    read = find + 1;

                    find = query.IndexOf('&', read);
                    string val;
                    if (find > 0)
                    {
                        val = find - read == 1 ? null : query.Substring(read, find - read);

                        read = find + 1;
                    }
                    else
                    {
                        val = query.Substring(read);

                        read = query.Length;
                    }

                    dic[key] = val;
                }
            }
        }

        private static void AddDictionary(IDictionary<string, object> dic, object values)
        {
            foreach (PropertyInfo p in values.GetType().GetProperties())
            {
                if (!p.CanRead) continue;
                object value = p.GetValue(values, null);

                if (value is bool)
                    dic[p.Name] = (bool) value ? "true" : "false";
                else
                    dic[p.Name] = UrlEncode(Convert.ToString(value));
            }
        }

        public TokenPair RequestToken()
        {
            try
            {
                WebRequest req = MakeRequest("POST", "https://api.twitter.com/oauth/request_token");
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string str = reader.ReadToEnd();

                        TokenPair token = new TokenPair
                        {
                            Token = Regex.Match(str, @"oauth_token=([^&]+)").Groups[1].Value,
                            Secret = Regex.Match(str, @"oauth_token_secret=([^&]+)").Groups[1].Value
                        };

                        return token;
                    }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public TokenPair AccessToken(string verifier)
        {
            try
            {
                var obj = new {oauth_verifier = verifier};
                byte[] buff = Encoding.UTF8.GetBytes(ToString(obj));

                WebRequest req = MakeRequest("POST", "https://api.twitter.com/oauth/access_token", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string str = reader.ReadToEnd();

                        TokenPair token = new TokenPair
                        {
                            Token = Regex.Match(str, @"oauth_token=([^&]+)").Groups[1].Value,
                            Secret = Regex.Match(str, @"oauth_token_secret=([^&]+)").Groups[1].Value
                        };

                        return token;
                    }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public class TokenPair
        {
            public TokenPair() {}

            public TokenPair(string token, string secret)
            {
                Token = token;
                Secret = secret;
            }

            public string Token { get; set; }
            public string Secret { get; set; }
        }
    }
}