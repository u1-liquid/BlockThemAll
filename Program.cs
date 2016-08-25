using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockThemAll
{
    internal class Program
    {
        public static string ini_file = "BlockThemAll.ini";
        public static OAuth oAuth { get; private set; }
        [STAThread]
        private static void Main()
        {
            IniSettings setting = new IniSettings(new FileInfo(ini_file));
            
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

                string readLine;
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
                    
                    Console.Write("Do you have backup of blocklist? (Y/N)");
                    readLine = Console.ReadLine();
                    if ((readLine != null) && readLine.ToUpper().Trim().Equals("Y"))
                    {
                        Console.Write("Enter path of your blocklist\n: ");
                        string input = Console.ReadLine();
                        if (input != null && File.Exists(input.Replace("\"", "")))
                            blocklist.AddRange(File.ReadAllText(input.Replace("\"", "")).Split(','));
                    }
                    else
                    {
                        Console.WriteLine("Get My Block List... (Max 250000 per 15min)");
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
                    readLine = Console.ReadLine();
                    if ((readLine != null) && readLine.ToUpper().Trim().Equals("N"))
                        break;
                }

                Console.Write("Do you want export your block list? (Y/N) : ");
                readLine = Console.ReadLine();
                if ((readLine != null) && readLine.ToUpper().Trim().Equals("Y"))
                    File.WriteAllText("blocklist_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm") + ".csv", string.Join(",", blocklist));
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
                {
                    string response = reader.ReadToEnd();
                    Console.WriteLine(response);

                    if (Regex.IsMatch(response, @"(?i)""code""\s*:\s*88"))
                    {
                        Console.Write("Do you want retry get block list after 15min? (Y/N)");
                        string readLine = Console.ReadLine();
                        if ((readLine != null) && readLine.ToUpper().Trim().Equals("N"))
                            return json.ToString();

                        Console.WriteLine("Wait for 15min... The job will be resumed at : " + DateTime.Now.AddMinutes(15).ToString("hh:mm:ss"));
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
    }
}