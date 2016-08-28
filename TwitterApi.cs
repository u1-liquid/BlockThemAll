using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BlockThemAll
{
    internal static class TwitterApi
    {
        public static TwitterOAuth TwitterOAuth { get; internal set; }
        public static UserInfoObject MyUserInfo { get; internal set; }

        public static void Login(IniSettings setting) => Login(setting, "Authenticate");

        public static void Login(IniSettings setting, string section)
        {
            string consumerKey = Convert.ToString(setting.GetValue(section, "ConsumerKey", string.Empty));
            string consumerSecret = Convert.ToString(setting.GetValue(section, "ConsumerSecret", string.Empty));
            string accessToken = Convert.ToString(setting.GetValue(section, "AccessToken", string.Empty));
            string accessSecret = Convert.ToString(setting.GetValue(section, "AccessSecret", string.Empty));
            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                setting.Save();
                Console.WriteLine("Unable to get consumerKey / Secret. Please check config file.");
                Console.ReadKey(true);
                return;
            }

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessSecret))
            {
                TwitterOAuth = new TwitterOAuth(consumerKey, consumerSecret);
                TwitterOAuth.TokenPair tokens = TwitterOAuth.RequestToken();
                string authorizationUrlString = "https://api.twitter.com/oauth/authorize?oauth_token=" + tokens.Token;
                try
                {
                    using (Process.Start("explorer", authorizationUrlString)) {}
                }
                catch
                {
                    Console.WriteLine($"Failed to open web browser.\nYou have to access manually this url:\n{authorizationUrlString}");
                }

                string verifier;

                do
                {
                    Console.Write("Please input verifier code : ");
                    verifier = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(verifier));

                tokens = TwitterOAuth.AccessToken(verifier);

                if (tokens != null)
                {
                    setting.SetValue(section, "AccessToken", accessToken = TwitterOAuth.User.Token = tokens.Token);
                    setting.SetValue(section, "AccessSecret", accessSecret = TwitterOAuth.User.Secret = tokens.Token);
                    setting.Save();
                }
                else
                {
                    Console.WriteLine("Unable to login to your account.");
                    TwitterOAuth = null;
                    Console.ReadKey(true);
                    return;
                }
            }

            TwitterOAuth = new TwitterOAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            MyUserInfo = getMyUserInfo();
        }

        private static UserInfoObject getMyUserInfo()
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET", "https://api.twitter.com/1.1/account/verify_credentials.json");

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

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
        }

        public static string getMyFriends(string id, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET",
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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {target = id, cursor = cursor};
                }
            }

            return json.ToString();
        }

        public static string getMyFollowers(string id, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/followers/ids.json?stringify_ids=true&cursor=" + cursor + "&user_id=" + id + "&count=5000");

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {target = id, cursor = cursor};
                }
            }

            return json.ToString();
        }

        public static string getMyBlockList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/blocks/ids.json?stringify_ids=true&cursor=" + cursor);

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {cursor = cursor};
                }
            }

            return json.ToString();
        }

        public static string getMyMuteList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET", "https://api.twitter.com/1.1/mutes/users/ids.json?cursor=" + cursor);

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {cursor = cursor};
                }
            }

            return json.ToString();
        }

        public static string getFollowers(string username, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/followers/ids.json?stringify_ids=true&cursor=" + cursor + "&screen_name=" + username +
                    "&count=5000");

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88"))
                        throw new RateLimitException {target = username, cursor = cursor};
                }
            }

            return json.ToString();
        }

        public static string searchPhase(string phase, bool newReq)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = TwitterOAuth.MakeRequest("GET",
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
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return json.ToString();
                using (StreamReader reader = new StreamReader(resStream))
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {target = phase};
                }
            }

            return json.ToString();
        }

        public static string Block(string id, bool isScreenName = false)
        {
            Console.WriteLine("Block user : " + id);
            object obj;
            if (isScreenName)
                obj = new {screen_name = id, include_entities = false, skip_status = true};
            else
                obj = new {user_id = id, include_entities = false, skip_status = true};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/create.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    UserInfoObject result = JsonConvert.DeserializeObject<UserInfoObject>(reader.ReadToEnd());
                    return result.id_str;
                }
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return string.Empty;
        }

        public static string UnBlock(string id, bool isScreenName = false)
        {
            Console.WriteLine("UnBlock user : " + id);
            object obj;
            if (isScreenName)
                obj = new {screen_name = id, include_entities = false, skip_status = true};
            else
                obj = new {user_id = id, include_entities = false, skip_status = true};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/destroy.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    UserInfoObject result = JsonConvert.DeserializeObject<UserInfoObject>(reader.ReadToEnd());
                    return result.id_str;
                }
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                    Console.WriteLine(reader.ReadToEnd());
            }

            return string.Empty;
        }

        public static string Mute(string id)
        {
            Console.WriteLine("Mute user : " + id);
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/create.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    UserInfoObject result = JsonConvert.DeserializeObject<UserInfoObject>(reader.ReadToEnd());
                    return result.id_str;
                }
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {target = id};
                }
            }

            return string.Empty;
        }

        public static string UnMute(string id)
        {
            Console.WriteLine("UnMute user : " + id);
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/destroy.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
                Stream resStream = req.GetResponse().GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    UserInfoObject result = JsonConvert.DeserializeObject<UserInfoObject>(reader.ReadToEnd());
                    return result.id_str;
                }
            }
            catch (WebException ex)
            {
                Stream resStream = ex.Response?.GetResponseStream();
                if (resStream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(resStream))
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException {target = id};
                }
            }

            return string.Empty;
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

    [Serializable]
    public class RateLimitException : Exception
    {
        public RateLimitException()
        {
            thrownAt = DateTime.Now;
        }

        public DateTime thrownAt { get; }
        public string target { get; set; }
        public string cursor { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("thrownAt", thrownAt);
            info.AddValue("target", target);
            info.AddValue("cursor", cursor);

            base.GetObjectData(info, context);
        }
    }
}