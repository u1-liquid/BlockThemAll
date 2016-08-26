using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BlockThemAll
{
    internal class Program
    {
        public static string ini_file = "BlockThemAll.ini";

        [STAThread]
        private static void Main()
        {
            TwitterApi.Login(new IniSettings(new FileInfo(ini_file)));
            if (TwitterApi.TwitterOAuth.User.Token == null) return;

            HashSet<string> whitelist = new HashSet<string>();
            HashSet<string> blocklist = new HashSet<string>();

            Console.WriteLine("Loading login info...");
            string myId = TwitterApi.getMyId();

            string readLine;
            if (!string.IsNullOrEmpty(myId))
            {
                Console.WriteLine("Get My Friends...");
                UserIdsObject result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFriends(myId, "-1"));
                while (result != null)
                {
                    whitelist.UnionWith(result.ids);
                    if (result.next_cursor != 0)
                        result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFriends(myId, result.next_cursor_str));
                    else
                        break;
                }

                Console.WriteLine("Get My Followers...");
                result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFollowers(myId, "-1"));
                while (result != null)
                {
                    whitelist.UnionWith(result.ids);
                    if (result.next_cursor != 0)
                        result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyFollowers(myId, result.next_cursor_str));
                    else
                        break;
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
                    result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyBlockList("-1"));
                    while (result != null)
                    {
                        blocklist.UnionWith(result.ids);
                        if (result.next_cursor != 0)
                            result = JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getMyBlockList(result.next_cursor_str));
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

                        if (string.IsNullOrWhiteSpace(target)) continue;

                        if (target.StartsWith("@"))
                        {
                            string username = target.Substring(1);
                            Console.WriteLine("Get " + target + "'s Followers...");
                            string json = TwitterApi.getFollowers(username, "-1");
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                UserIdsObject result = JsonConvert.DeserializeObject<UserIdsObject>(json);
                                while (result != null)
                                {
                                    targetLists.AddRange(result.ids);
                                    if (result.next_cursor != 0)
                                        result =
                                            JsonConvert.DeserializeObject<UserIdsObject>(TwitterApi.getFollowers(username,
                                                result.next_cursor_str));
                                    else
                                        break;
                                }

                                blocklist.Add(TwitterApi.Block(username, true));
                            }
                            else
                            {
                                Console.WriteLine("Unable to get target followers.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Search " + target + "...");
                            string json = TwitterApi.searchPhase(Uri.EscapeDataString(target), true);
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                SearchResultObject result = JsonConvert.DeserializeObject<SearchResultObject>(json);
                                while (result.search_metadata.count > 0)
                                {
                                    targetLists.AddRange(result.statuses.Select(x => x.user.id_str));
                                    result =
                                        JsonConvert.DeserializeObject<SearchResultObject>(
                                            TwitterApi.searchPhase(result.search_metadata.next_results, false));
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
                            if (whitelist.Contains(s) || blocklist.Contains(s)) continue;
                            blocklist.Add(TwitterApi.Block(s));
                            Console.WriteLine("Target = {0}, Progress = {1}/{2} ({3}%), Blocklist = {4}",
                                target.Length < 18 ? target : target.Substring(0, 17) + "...", count,
                                targetLists.Count, Math.Round(count * 100 / (double) targetLists.Count, 2), blocklist.Count);
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
}