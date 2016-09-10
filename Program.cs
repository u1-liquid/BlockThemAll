using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BlockThemAll
{
    internal class Program
    {
        public static string ini_file = "BlockThemAll.ini";
        public static IniSettings settings;

        [STAThread]
        private static void Main()
        {
            settings = new IniSettings(new FileInfo(ini_file));

            Console.WriteLine("Loading login info...");
            TwitterApi.Login(settings);
            if ((TwitterApi.TwitterOAuth == null) || (TwitterApi.TwitterOAuth.User.Token == null)) return;

            HashSet<string> whitelist = new HashSet<string>();
            HashSet<string> blocklist = new HashSet<string>();

            string readLine;
            if ((TwitterApi.MyUserInfo != null) && !string.IsNullOrEmpty(TwitterApi.MyUserInfo.id_str))
            {
                Console.WriteLine("Get My Friends...");
                UserIdsObject result =
                    JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFriends(TwitterApi.MyUserInfo.id_str, "-1"));
                while (result != null)
                {
                    whitelist.UnionWith(result.ids);
                    if (result.next_cursor == 0)
                        break;
                    result =
                        JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFriends(TwitterApi.MyUserInfo.id_str,
                            result.next_cursor_str));
                }

                Console.WriteLine("Get My Followers...");
                result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFollowers(TwitterApi.MyUserInfo.id_str, "-1"));
                while (result != null)
                {
                    whitelist.UnionWith(result.ids);
                    if (result.next_cursor == 0)
                        break;
                    result =
                        JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFollowers(TwitterApi.MyUserInfo.id_str,
                            result.next_cursor_str));
                }

                Console.Write("Do you have backup of blocklist? (Y/N)");
                readLine = Console.ReadLine();
                if ((readLine != null) && readLine.ToUpper().Trim().Equals("Y"))
                {
                    Console.Write("Enter path of your blocklist\n: ");
                    string input = Console.ReadLine();
                    if ((input != null) && File.Exists(input.Replace("\"", "")))
                        blocklist.UnionWith(File.ReadAllText(input.Replace("\"", "")).Split(','));
                }
                else
                {
                    Console.WriteLine("Get My Block List... (Max 250000 per 15min)");
                    string cursor = "-1";

                    while (true)
                        try
                        {
                            result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyBlockList(cursor));
                            while (result != null)
                            {
                                blocklist.UnionWith(result.ids);
                                if (result.next_cursor == 0)
                                    break;
                                result =
                                    JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyBlockList(cursor = result.next_cursor_str));
                            }

                            break;
                        }
                        catch (RateLimitException)
                        {
                            if (Convert.ToBoolean(settings.GetValue("Configuration", "AutoRetry_GetBlockList", false)) == false)
                            {
                                Console.Write("Do you want retry get block list after 15min? (Yes/No/Auto)");
                                readLine = Console.ReadLine();
                                if (readLine != null)
                                {
                                    if (readLine.ToUpper().Trim().StartsWith("N"))
                                        break;
                                    if (readLine.ToUpper().Trim().StartsWith("A"))
                                        settings.SetValue("Configuration", "AutoRetry_GetBlockList", true);
                                }

                                settings.Save();
                            }

                            Console.WriteLine("Wait for 15min... The job will be resumed at : " +
                                              DateTime.Now.AddMinutes(15).ToString("hh:mm:ss"));
                            Thread.Sleep(TimeSpan.FromMinutes(15));
                        }
                }
            }
            else
            {
                Console.WriteLine("Failed to get your info!");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Whitelist = {whitelist.Count}, Blocklist = {blocklist.Count}");

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
                            string.Join("\n \n", "Please check your input is correct. ", string.Join("\n", targets), " Press Yes to go."),
                            "Check your input is correct",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question)) continue;

                    foreach (string target in targets)
                    {
                        if (string.IsNullOrWhiteSpace(target)) continue;
                        RateLimitException rateLimit = null;

                        do
                        {
                            HashSet<string> targetLists = new HashSet<string>();

                            try
                            {
                                if (target.StartsWith("@"))
                                {
                                    string username = target.Substring(1);
                                    blocklist.Add(TwitterApi.Block(username, true));
                                    GetTargetFollowers(username, rateLimit == null ? "-1" : rateLimit.cursor, targetLists);
                                }
                                else
                                {
                                    if (rateLimit == null)
                                        GetTargetSearchResult(target, true, targetLists);
                                    else
                                        GetTargetSearchResult(rateLimit.target, false, targetLists);
                                }

                                rateLimit = null;
                            }
                            catch (RateLimitException e)
                            {
                                rateLimit = e;
                            }

                            Console.WriteLine("Processing list...");

                            long count = 0;
                            foreach (string s in targetLists)
                            {
                                count++;
                                if (whitelist.Contains(s) || blocklist.Contains(s)) continue;
                                blocklist.Add(TwitterApi.Block(s));
                                Console.WriteLine(
                                    $"Target = {(target.Length < 18 ? target : target.Substring(0, 17) + "...")}, Progress = {count}/{targetLists.Count} ({Math.Round(count * 100 / (double) targetLists.Count, 2)}%), Blocklist = {blocklist.Count}");
                            }

                            if (rateLimit == null) continue;

                            TimeSpan wait = rateLimit.thrownAt.AddMinutes(15) - DateTime.Now;
                            if (wait < TimeSpan.Zero) continue;

                            Console.WriteLine($"Wait {wait:g} for Rate limit...");
                            Thread.Sleep(wait);
                        } while (rateLimit != null);
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
                File.WriteAllText($"blocklist_{DateTime.Now:yyyy-MM-dd_HHmm}.csv", string.Join(",", blocklist));
        }

        private static void GetTargetSearchResult(string target, bool isNewReq, HashSet<string> targetLists)
        {
            Console.WriteLine($"Search {target}...");
            string json = TwitterApi.searchPhase(Uri.EscapeDataString(target), isNewReq);
            if (!string.IsNullOrWhiteSpace(json))
            {
                SearchResultObject result = JsonConvert.DeserializeObject<SearchResultObject>(json);
                while (result.search_metadata.count > 0)
                {
                    targetLists.UnionWith(result.statuses.Select(x => x.user.id_str));
                    result =
                        JsonConvert.DeserializeObject<SearchResultObject>(
                            TwitterApi.searchPhase(result.search_metadata.next_results, false));
                    if (result?.search_metadata.next_results == null) break;
                }
            }
            else
            {
                Console.WriteLine("There is no result.");
            }
        }

        private static void GetTargetFollowers(string username, string cursor, HashSet<string> targetLists)
        {
            Console.WriteLine($"Get {username}'s Followers...");
            string json = TwitterApi.getFollowers(username, cursor);
            if (!string.IsNullOrWhiteSpace(json))
            {
                UserIdsObject result = JsonConvert.DeserializeObject<UserIdsObject>(json);
                while (result != null)
                {
                    targetLists.UnionWith(result.ids);
                    if (result.next_cursor == 0)
                        break;
                    result =
                        JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getFollowers(username,
                            result.next_cursor_str));
                }
            }
            else
            {
                Console.WriteLine("Unable to get target followers.");
            }
        }
    }
}