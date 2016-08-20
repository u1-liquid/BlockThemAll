using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockThemAll
{
    internal class Program
    {
        public static string ini_file = "BlockThemAll.ini";
        public static OAuth oAuth { get; private set; }

        private static void Main()
        {
            Log.Init();
            INIParser setting = new INIParser(ini_file);
            string AuthData = "Authenticate";
            string consumerKey = setting.GetValue(AuthData, "ConsumerKey");
            string consumerSecret = setting.GetValue(AuthData, "ConsumerSecret");
            string accessToken = setting.GetValue(AuthData, "AccessToken");
            string accessSecret = setting.GetValue(AuthData, "AccessSecret");
            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                Log.Error("Program", "Unable to get consumerKey / Secret. Please check config file.");
                if (string.IsNullOrWhiteSpace(consumerKey)) setting.SetValue(AuthData, "ConsumerKey", "");
                if (string.IsNullOrWhiteSpace(consumerSecret)) setting.SetValue(AuthData, "ConsumerSecret", "");
                if (string.IsNullOrWhiteSpace(accessToken)) setting.SetValue(AuthData, "AccessToken", "");
                if (string.IsNullOrWhiteSpace(accessSecret)) setting.SetValue(AuthData, "AccessSecret", "");
                setting.Save();
                return;
            }
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessSecret))
            {
                oAuth = new OAuth(consumerKey, consumerSecret);
                OAuth.TokenPair tokens = oAuth.RequestToken();
                oAuth.User.Token = tokens.Token;
                oAuth.User.Secret = tokens.Token;
                try
                {
                    using (
                        Process.Start(new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://api.twitter.com/oauth/authorize?oauth_token=" + tokens.Token
                        })) {}
                }
                catch
                {
                    // ignored
                }

                string verifier;
                int i;

                do
                {
                    Console.Write("Please input verifier code : ");
                    verifier = Console.ReadLine();
                } while (!int.TryParse(verifier, out i));

                tokens = oAuth.AccessToken(verifier);

                if (tokens != null)
                {
                    oAuth.User.Token = tokens.Token;
                    oAuth.User.Secret = tokens.Token;
                    accessToken = oAuth.User.Token;
                    accessSecret = oAuth.User.Secret;
                    setting.SetValue(AuthData, "AccessToken", tokens.Token);
                    setting.SetValue(AuthData, "AccessSecret", tokens.Secret);
                    setting.Save();
                }

                setting.Save();

                oAuth = new OAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            }
            else
            {
                oAuth = new OAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            }

            if (oAuth.User.Token != null)
            {
                List<string> whitelist = new List<string>();
                List<string> blocklist = new List<string>();

                Console.WriteLine("Loading login info...");
                string myId = getMyId();

                if (!string.IsNullOrEmpty(myId))
                {
                    Console.WriteLine("Get My Friends...");
                    UserIdsObject result = JsonConvert.DeserializeObject<UserIdsObject>(getMyFriends(myId, "-1"));
                    while (result != null)
                    {
                        whitelist.AddRange(result.ids);
                        if (result.next_cursor != 0)
                            result = JsonConvert.DeserializeObject<UserIdsObject>(getMyFriends(myId, result.next_cursor_str));
                        else
                            break;
                    }

                    Console.WriteLine("Get My Followers...");
                    result = JsonConvert.DeserializeObject<UserIdsObject>(getMyFollowers(myId, "-1"));
                    while (result != null)
                    {
                        whitelist.AddRange(result.ids);
                        if (result.next_cursor != 0)
                            result = JsonConvert.DeserializeObject<UserIdsObject>(getMyFollowers(myId, result.next_cursor_str));
                        else
                            break;
                    }

                    whitelist = whitelist.Distinct().ToList();

                    Console.WriteLine("Get My Block List... (Max 250000)");
                    result = JsonConvert.DeserializeObject<UserIdsObject>(getMyBlockList("-1"));
                    while (result != null)
                    {
                        blocklist.AddRange(result.ids);
                        if (result.next_cursor != 0)
                            result = JsonConvert.DeserializeObject<UserIdsObject>(getMyBlockList(result.next_cursor_str));
                        else
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to get your info!");
                }

                Console.WriteLine("Whitelist = {0}, Blocklist = {1}", whitelist.Count, blocklist.Count);

                while (true)
                {
                    Console.Write("Enter @usernames, search phase or filename to Block Them All\n: ");
                    string input = Console.ReadLine();

                    if (input != null)
                    {
                        string[] targets = File.Exists(input.Replace("\"", ""))
                            ? File.ReadAllText(input.Replace("\"", ""))
                                .Split(new[] {",", "\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                            : input.Split(',');

                        Console.WriteLine("Please check your input is correct!");
                        if (DialogResult.No ==
                            MessageBox.Show(
                                "Please check your input is correct. \n \n" + string.Join("\n", targets) + "\n \n Press Yes to go.",
                                "Check your input is correct",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)) continue;

                        foreach (string target in targets)
                        {
                            List<string> targetLists = new List<string>();

                            if (target.StartsWith("@"))
                            {
                                string username = target.Substring(1);
                                Console.WriteLine("Get " + target + "'s Followers...");
                                string json = getFollowers(username, "-1");
                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    UserIdsObject result = JsonConvert.DeserializeObject<UserIdsObject>(json);
                                    while (true)
                                    {
                                        targetLists.AddRange(result.ids);
                                        if (result.next_cursor != 0)
                                            result =
                                                JsonConvert.DeserializeObject<UserIdsObject>(getFollowers(username, result.next_cursor_str));
                                        else
                                            break;
                                    }

                                    Block(username, true);
                                    blocklist.Add(username);
                                }
                                else
                                {
                                    Console.WriteLine("Unable to get target followers.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Search " + target + "...");
                                string json = searchPhase(Uri.EscapeDataString(target), true);
                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    SearchResultObject result = JsonConvert.DeserializeObject<SearchResultObject>(json);
                                    while (result.search_metadata.count > 0)
                                    {
                                        targetLists.AddRange(result.statuses.Select(x => x.user.id_str));
                                        result =
                                            JsonConvert.DeserializeObject<SearchResultObject>(
                                                searchPhase(result.search_metadata.next_results, false));
                                        if (result == null) break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("There is no result.");
                                }
                            }

                            Console.WriteLine("Processing list...");

                            long count = 0;
                            foreach (string s in targetLists)
                            {
                                count++;
                                if (!whitelist.Contains(s) && !blocklist.Contains(s))
                                {
                                    Block(s);
                                    blocklist.Add(s);
                                    Console.WriteLine("Target = {0}, Progress = {1}/{2} ({3}%), Blocklist = {4}",
                                        target.Length < 18 ? target : target.Substring(0, 17) + "...", count,
                                        targetLists.Count, Math.Round(count * 100 / (double) targetLists.Count, 2), blocklist.Count);
                                }
                            }
                        }
                    }

                    Console.Write("Finished ! Do you want continue? (Y/N) : ");
                    string readLine = Console.ReadLine();
                    if ((readLine != null) && readLine.ToUpper().Trim().Equals("N"))
                        break;
                }
            }
        }

        public static string getMyId()
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET", "https://api.twitter.com/1.1/account/verify_credentials.json");

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();


                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        Console.WriteLine(reader.ReadToEnd());
            }

            try
            {
                JObject obj = JObject.Parse(json.ToString());
                return obj["id_str"].ToString();
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        public static string getMyFriends(string id, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/friends/ids.json?stringify_ids=true&cursor=" + cursor + "&user_id=" + id + "&count=5000");

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return json.ToString();
        }

        public static string getMyFollowers(string id, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/followers/ids.json?stringify_ids=true&cursor=" + cursor + "&user_id=" + id + "&count=5000");

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return json.ToString();
        }

        public static string getMyBlockList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET", "https://api.twitter.com/1.1/blocks/ids.json?stringify_ids=true&cursor=" + cursor);

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return json.ToString();
        }

        public static string getFollowers(string username, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/followers/ids.json?stringify_ids=true&cursor=" + cursor + "&screen_name=" + username +
                    "&count=5000");

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return json.ToString();
        }

        public static string searchPhase(string phase, bool newReq)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = oAuth.MakeRequest("GET",
                    newReq
                        ? "https://api.twitter.com/1.1/search/tweets.json?q=" + phase +
                          "&result_type=recent&count=100&include_entities=false"
                        : "https://api.twitter.com/1.1/search/tweets.json" + phase + "&include_entities=false");

                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        json.AppendLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return json.ToString();
        }

        public static void Block(string id, bool isScreenName = false)
        {
            Console.WriteLine("Block user : " + id);
            object obj;
            if (isScreenName)
                obj = new {screen_name = id, include_entities = false, skip_status = true};
            else
                obj = new {user_id = id, include_entities = false, skip_status = true};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(OAuth.ToString(obj));

                WebRequest req = oAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/create.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream == null) return;
                using (StreamReader reader = new StreamReader(resStream))
                    reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        Console.WriteLine(reader.ReadToEnd());
            }
        }

        public class UserIdsObject
        {
            public List<string> ids { get; set; }
            public long next_cursor { get; set; }
            public string next_cursor_str { get; set; }
            public long previous_cursor { get; set; }
            public string previous_cursor_str { get; set; }
        }

        public class SearchResultObject
        {
            public List<TwitStatusObject> statuses { get; set; }
            public SearchMetadataObject search_metadata { get; set; }
        }

        public class TwitStatusObject
        {
            public long id { get; set; }
            public string id_str { get; set; }
            public UserInfoObject user { get; set; }
        }

        public class SearchMetadataObject
        {
            public long max_id { get; set; }
            public string max_id_str { get; set; }
            public string next_results { get; set; }
            public int count { get; set; }
            public long since_id { get; set; }
            public string since_id_str { get; set; }
        }

        public class UserInfoObject
        {
            public long id { get; set; }
            public string id_str { get; set; }
            public string screen_name { get; set; }
        }

        public class OAuth
        {
            private static readonly string[] oauth_array =
            {
                "oauth_consumer_key", "oauth_version", "oauth_nonce", "oauth_signature",
                "oauth_signature_method", "oauth_timestamp", "oauth_token", "oauth_callback"
            };

            private static readonly DateTime GenerateTimeStampDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            static OAuth()
            {
                ServicePointManager.Expect100Continue = false;
            }

            public OAuth(string appToken, string appSecret)
                : this(appToken, appSecret, null, null) {}

            public OAuth(string appToken, string appSecret, string userToken, string userSecret)
            {
                App = new TokenPair(appToken, appSecret);
                User = new TokenPair(userToken, userSecret);
            }

            public TokenPair App { get; }
            public TokenPair User { get; }

            public WebRequest MakeRequest(string method, string url, object data = null)
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

                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
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

    public class Log
    {
        public static bool IsInited { get; private set; }

        public static void Init()
        {
            try
            {
                IsInited = true;
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
            }
            catch
            {
                IsInited = false;
            }
        }

        public static void Print(string tag, string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Output(tag, message);
        }

        public static void Http(string tag, string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Output(tag, message);
        }

        public static void Debug(string tag, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Output(tag, message);
        }

        public static void Warning(string tag, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output(tag, message);
        }

        public static void Error(string tag, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Output(tag, message);
        }

        private static void Output(string tag, string message)
        {
            string log = $"{tag} : {message}";
            if (IsInited)
                Trace.WriteLine(log);
            else
                Console.WriteLine(log);
        }

        public static void StackTrace()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (IsInited)
                Trace.Write(new StackTrace(true));
            else
                Console.WriteLine("Inflate stacktrace() : \n{0}", new StackTrace(true));
        }

        public static void Indent()
        {
            if (!IsInited) return;
            Trace.Indent();
        }

        public static void Unindent()
        {
            if (!IsInited) return;
            Trace.Unindent();
        }
    }

    public class INIParser
    {
        private readonly IniData data;
        private readonly string iniPath;

        public INIParser(string path)
        {
            iniPath = path;
            if (File.Exists(iniPath))
            {
                data = new FileIniDataParser().ReadFile(iniPath);
                string @out = $"Read INI - [{iniPath}]";
                foreach (SectionData section in data.Sections)
                {
                    @out += $"\n    [{section.SectionName}]";
                    foreach (KeyData key in section.Keys)
                        @out += $"\n        {key.KeyName} = {data[section.SectionName][key.KeyName]}";
                }
                Log.Debug("INIParser", @out);
            }
            else
            {
                Log.Error("INIParser", $"[{iniPath}] is not correct directory. new inidata generated.");
                data = new IniData();
            }
        }

        public string GetValue(string Section, string Key)
        {
            try
            {
                return data[Section][Key];
            }
            catch
            {
                return null;
            }
        }

        public void SetValue(string Section, string Key, object Value)
        {
            if (!data.Sections.ContainsSection(Section)) data.Sections.AddSection(Section);
            if (!data[Section].ContainsKey(Key)) data[Section].AddKey(Key);

            data[Section][Key] = Value.ToString();
        }

        internal void Save()
        {
            new FileIniDataParser().WriteFile(iniPath, data, Encoding.UTF8);
        }

        #region WINApi

        //[DllImport( "kernel32.dll" )]
        //private static extern int GetPrivateProfileString(
        //	String section,
        //	String key,
        //	String def,
        //	StringBuilder retVal,
        //	int size,
        //	String filePath );

        //[DllImport( "kernel32.dll" )]
        //private static extern long WritePrivateProfileString(
        //	String section,
        //	String key,
        //	String val,
        //	String filePath );

        //public String GetValue( String Section, String Key )
        //{
        //	StringBuilder temp = new StringBuilder(255);
        //	int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
        //	return temp.ToString( );
        //}

        //public void SetValue( String Section, String Key, String Value )
        //{
        //	WritePrivateProfileString( Section, Key, Value, iniPath );
        //}

        #endregion

        #region FileIniDataParser

        #endregion
    }
}