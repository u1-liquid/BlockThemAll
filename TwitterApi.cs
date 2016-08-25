using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockThemAll
{
    internal static class TwitterApi
    {
        public static TwitterOAuth TwitterOAuth { get; internal set; }

        public static TwitterOAuth Login(IniSettings setting)
        {
            string AuthData = "Authenticate";
            string consumerKey = setting.GetValue(AuthData, "ConsumerKey");
            string consumerSecret = setting.GetValue(AuthData, "ConsumerSecret");
            string accessToken = setting.GetValue(AuthData, "AccessToken");
            string accessSecret = setting.GetValue(AuthData, "AccessSecret");
            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                Console.WriteLine("Unable to get consumerKey / Secret. Please check config file.");
                if (string.IsNullOrWhiteSpace(consumerKey)) setting.SetValue(AuthData, "ConsumerKey", "");
                if (string.IsNullOrWhiteSpace(consumerSecret)) setting.SetValue(AuthData, "ConsumerSecret", "");
                if (string.IsNullOrWhiteSpace(accessToken)) setting.SetValue(AuthData, "AccessToken", "");
                if (string.IsNullOrWhiteSpace(accessSecret)) setting.SetValue(AuthData, "AccessSecret", "");
                setting.Save();
                return null;
            }
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessSecret))
            {
                TwitterOAuth = new TwitterOAuth(consumerKey, consumerSecret);
                TwitterOAuth.TokenPair tokens = TwitterOAuth.RequestToken();
                TwitterOAuth.User.Token = tokens.Token;
                TwitterOAuth.User.Secret = tokens.Token;
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

                tokens = TwitterOAuth.AccessToken(verifier);

                if (tokens != null)
                {
                    TwitterOAuth.User.Token = tokens.Token;
                    TwitterOAuth.User.Secret = tokens.Token;
                    accessToken = TwitterOAuth.User.Token;
                    accessSecret = TwitterOAuth.User.Secret;
                    setting.SetValue(AuthData, "AccessToken", tokens.Token);
                    setting.SetValue(AuthData, "AccessSecret", tokens.Secret);
                    setting.Save();
                }

                setting.Save();

                TwitterOAuth = new TwitterOAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            }
            else
            {
                TwitterOAuth = new TwitterOAuth(consumerKey, consumerSecret, accessToken, accessSecret);
            }
            return TwitterOAuth;
        }

        public static string getMyId()
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = TwitterOAuth.MakeRequest("GET", "https://api.twitter.com/1.1/account/verify_credentials.json");

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
                WebRequest req = TwitterOAuth.MakeRequest("GET",
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
                WebRequest req = TwitterOAuth.MakeRequest("GET",
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
                WebRequest req = TwitterOAuth.MakeRequest("GET",
                    "https://api.twitter.com/1.1/blocks/ids.json?stringify_ids=true&cursor=" + cursor);

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88"))
                    {
                        Console.Write("Do you want retry get block list after 15min? (Y/N)");
                        string readLine = Console.ReadLine();
                        if ((readLine != null) && readLine.ToUpper().Trim().Equals("N"))
                            return json.ToString();

                        Console.WriteLine("Wait for 15min... The job will be resumed at : " +
                                          DateTime.Now.AddMinutes(15).ToString("hh:mm:ss"));
                        Thread.Sleep(15 * 60 * 1000);
                        return getMyBlockList(cursor);
                    }
                }
            }

            return json.ToString();
        }

        public static string getMyMuteList(string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = TwitterOAuth.MakeRequest("GET", "https://api.twitter.com/1.1/mutes/users/ids.json?cursor=" + cursor);

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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88"))
                    {
                        Console.Write("Do you want retry get mute list after 15min? (Y/N)");
                        string readLine = Console.ReadLine();
                        if ((readLine != null) && readLine.ToUpper().Trim().Equals("N"))
                            return json.ToString();

                        Console.WriteLine("Wait for 15min... The job will be resumed at : " +
                                          DateTime.Now.AddMinutes(15).ToString("hh:mm:ss"));
                        Thread.Sleep(15 * 60 * 1000);
                        return getMyBlockList(cursor);
                    }
                }
            }

            return json.ToString();
        }

        public static string getFollowers(string username, string cursor)
        {
            StringBuilder json = new StringBuilder();

            try
            {
                WebRequest req = TwitterOAuth.MakeRequest("GET",
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
                WebRequest req = TwitterOAuth.MakeRequest("GET",
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

                WebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/create.json", obj);
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
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream != null)
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

                WebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/blocks/destroy.json", obj);
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
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        Console.WriteLine(reader.ReadToEnd());
            }

            return string.Empty;
        }

        public static string Mute(string id)
        {
            Console.Write("Mute user : " + id);
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                WebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/create.json", obj);
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
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        Console.WriteLine(reader.ReadToEnd());
            }

            return string.Empty;
        }

        public static string UnMute(string id)
        {
            Console.Write("UnMute user : " + id);
            object obj = new {user_id = id};

            try
            {
                byte[] buff = Encoding.UTF8.GetBytes(TwitterOAuth.ToString(obj));

                WebRequest req = TwitterOAuth.MakeRequest("POST", "https://api.twitter.com/1.1/mutes/users/destroy.json", obj);
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
                Stream resStream = ex.Response.GetResponseStream();
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream))
                        Console.WriteLine(reader.ReadToEnd());
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
}