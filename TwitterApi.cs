using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BlockThemAll
{
    public class TwitterApi
    {
        public UserStatus Status { get; internal set; } = UserStatus.NO_CREDITIONAL;
        public TwitterOAuth OAuth { get; internal set; }
        public UserInfoObject MyUserInfo { get; internal set; }
        private IniSettings setting;
        private string section;

        public static TwitterApi Login(IniSettings setting) => Login(setting, "Authenticate");

        public static TwitterApi Login(IniSettings setting, string section)
        {
            TwitterApi api = new TwitterApi
            {
                setting = setting,
                section = section
            };

            string consumerKey = Convert.ToString(setting.GetValue(section, "ConsumerKey", string.Empty));
            string consumerSecret = Convert.ToString(setting.GetValue(section, "ConsumerSecret", string.Empty));
            string accessToken = Convert.ToString(setting.GetValue(section, "AccessToken", string.Empty));
            string accessSecret = Convert.ToString(setting.GetValue(section, "AccessSecret", string.Empty));
            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                setting.Save();
                api.Status = UserStatus.NO_APIKEY;
                return api;
            }

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessSecret))
            {
                api.OAuth = new TwitterOAuth(consumerKey, consumerSecret);
                TwitterOAuth.TokenPair tokens = api.OAuth.RequestToken();
                if (tokens == null)
                {
                    api.Status = UserStatus.NO_APIKEY;
                    return api;
                }

                api.OAuth.User.Token = tokens.Token;
                api.OAuth.User.Secret = tokens.Secret;
                api.Status = UserStatus.LOGIN_REQUESTED;
                return api;
            }

            api.OAuth = new TwitterOAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            api.MyUserInfo = api.getMyUserInfo();
            api.Status = api.MyUserInfo == null ? UserStatus.INVALID_CREDITIONAL : UserStatus.LOGIN_SUCCESS;

            return api;
        }

        public void Authenticate(string verifier)
        {
            TwitterOAuth.TokenPair tokens = OAuth.AccessToken(verifier);

            if (tokens == null)
            {
                OAuth = null;
                Status = UserStatus.NO_CREDITIONAL;
                return;
            }

            setting.SetValue(section, "AccessToken", OAuth.User.Token = tokens.Token);
            setting.SetValue(section, "AccessSecret", OAuth.User.Secret = tokens.Secret);
            setting.Save();

            MyUserInfo = getMyUserInfo();
            Status = MyUserInfo == null ? UserStatus.INVALID_CREDITIONAL : UserStatus.LOGIN_SUCCESS;
        }     

        private UserInfoObject getMyUserInfo()
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET", "https://api.twitter.com/1.1/account/verify_credentials.json");

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
                        MainForm.Instance.Log(reader.ReadToEnd());
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
        }

        public UserIdsObject getMyFriends(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/friends/ids.json?stringify_ids=true&cursor=" + cursor + "&user_id=" + MyUserInfo.id_str + "&count=5000");

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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = MyUserInfo.id_str, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public UserIdsObject getMyFollowers(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/followers/ids.json?stringify_ids=true&cursor=" + cursor + "&user_id=" + MyUserInfo.id_str + "&count=5000");

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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = MyUserInfo.id_str, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public UserIdsObject getMyBlockList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/blocks/ids.json?stringify_ids=true&cursor=" + cursor);

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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = MyUserInfo.id_str, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public UserIdsObject getMyMuteList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET", "https://api.twitter.com/1.1/mutes/users/ids.json?cursor=" + cursor);

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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = MyUserInfo.id_str, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public UserIdsObject getFollowers(string username, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
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
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = username, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public UserIdsObject getRetweeters(string tweetid, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/statuses/retweeters/ids.json?stringify_ids=true&id=" + tweetid + "&cursor=" + cursor + "&count=100");

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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = tweetid, cursor = cursor };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserIdsObject>(json.ToString()) : null;
        }

        public List<UserInfoObject> lookupUsers(string[] targets, bool isScreenName = false)
        {
            List<UserInfoObject> result = new List<UserInfoObject>();

            foreach (List<string> ts in targets.Select((x, i) => new {Index = i, Value = x}).GroupBy(x => x.Index / 100).Select(x => x.Select(v => v.Value).ToList()))
            {
                StringBuilder json = new StringBuilder();

                object obj;
                if (isScreenName)
                    obj = new { screen_name = string.Join(",", ts), include_entities = false };
                else
                    obj = new { user_id = string.Join(",", ts), include_entities = false };

                try
                {
                    byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                    HttpWebRequest req = OAuth.MakeRequest("POST", "https://api.twitter.com/1.1/users/lookup.json", obj);
                    req.GetRequestStream().Write(buff, 0, buff.Length);
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
                        {
                            string response = reader.ReadToEnd();
                            MainForm.Instance.Log(response);
                        }
                }

                if (json.Length > 0)
                    result.AddRange(JsonConvert.DeserializeObject<List<UserInfoObject>>(json.ToString()));
            }

            return result;
        }

        public SearchResultObject searchPhase(string phase, bool newReq)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                HttpWebRequest req = OAuth.MakeRequest("GET",
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
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);

                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = phase };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<SearchResultObject>(json.ToString()) : null;
        }

        public UserInfoObject Block(string id, bool isScreenName = false)
        {
            StringBuilder json = new StringBuilder();

            object obj;
            if (isScreenName)
                obj = new {screen_name = id, include_entities = false, skip_status = true};
            else
                obj = new {user_id = id, include_entities = false, skip_status = true};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = OAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/create.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
        }

        public UserInfoObject UnBlock(string id, bool isScreenName = false)
        {
            StringBuilder json = new StringBuilder();

            object obj;
            if (isScreenName)
                obj = new {screen_name = id, include_entities = false, skip_status = true};
            else
                obj = new {user_id = id, include_entities = false, skip_status = true};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = OAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/destroy.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
        }

        public UserInfoObject Mute(string id)
        {
            StringBuilder json = new StringBuilder();
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = OAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/create.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);
                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = id };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
        }

        public UserInfoObject UnMute(string id)
        {
            StringBuilder json = new StringBuilder();
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                HttpWebRequest req = OAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/destroy.json", obj);
                req.GetRequestStream().Write(buff, 0, buff.Length);
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
                    {
                        string response = reader.ReadToEnd();
                        MainForm.Instance.Log(response);
                        if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88")) throw new RateLimitException { target = id };
                    }
            }

            return json.Length > 0 ? JsonConvert.DeserializeObject<UserInfoObject>(json.ToString()) : null;
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
        public long in_reply_to_status_id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
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