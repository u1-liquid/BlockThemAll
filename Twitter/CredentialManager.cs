using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using BlockThemAll.Base;
using BlockThemAll.Properties;

namespace BlockThemAll.Twitter
{
    internal sealed class CredentialManager
    {
        private static volatile CredentialManager instance;
        private static readonly object syncRoot = new object();

        public static CredentialManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CredentialManager();
                    }
                }

                return instance;
            }
        }

        public SortedDictionary<string, TwitterApi> Credentials { get; } = new SortedDictionary<string, TwitterApi>(
            Comparer<string>.Create((p1, p2) =>
            {
                if (string.Equals(p1, p2)) return 0;
                if (string.Equals(p1, "Default")) return -1;
                return string.Compare(p1, p2, StringComparison.Ordinal);
            })
        );
        private string currentKey { get; set; }
        public TwitterApi Current { get; private set; }
        public UserStatus Status => Current?.Status ?? UserStatus.INVALID_CREDITIONAL;
        public UserInfoObject MyUserInfo => Current?.MyUserInfo;

        public void AddCredential(string name, TwitterApi credential)
        {
            if (Current == null)
                Current = credential;
            else if (!string.Equals(Current?.MyUserInfo?.id_str, credential?.MyUserInfo?.id_str))
                throw new InvalidCredentialException("Unable to add Twitter Credential : Different Account");

            Credentials.Add(name, credential);
        }
        
        public void RemoveCredential(string name)
        {
            TwitterApi target;
            if (!Credentials.TryGetValue(name, out target)) return;

            Credentials.Remove(name);

            if (!ReferenceEquals(Current, target)) return;
            KeyValuePair<string, TwitterApi> pairDefault = Credentials.FirstOrDefault();
            currentKey = pairDefault.Key;
            Current = pairDefault.Value;
        }

        public void SelectCredential(string name)
        {
            TwitterApi tw;
            if (Credentials.TryGetValue(name, out tw))
            {
                currentKey = name;
                Current = tw;
            }
            else
            {
                KeyValuePair<string, TwitterApi> pairDefault = Credentials.FirstOrDefault();
                currentKey = pairDefault.Key;
                Current = pairDefault.Value;
            }
        }

        private void SelectNextCredential()
        {
            try
            {
                KeyValuePair<string, TwitterApi> pairNext = Credentials.SkipWhile(p => !string.Equals(p.Key, currentKey)).Skip(1).Take(1).ToArray()[0];
                currentKey = pairNext.Key;
                Current = pairNext.Value;
            }
            catch (Exception)
            {
                KeyValuePair<string, TwitterApi> pairDefault = Credentials.FirstOrDefault();
                currentKey = pairDefault.Key;
                Current = pairDefault.Value;
            }
        }

        public UserIdsObject getMyFriends(string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getMyFriends(cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserIdsObject getMyFollowers(string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getMyFollowers(cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserIdsObject getMyBlockList(string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getMyBlockList(cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserIdsObject getMyMuteList(string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getMyMuteList(cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserIdsObject getFollowers(string username, string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getFollowers(username, cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserIdsObject getRetweeters(string tweetid, string cursor)
        {
            UserIdsObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.getRetweeters(tweetid, cursor);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public List<UserInfoObject> lookupUsers(string[] targets, bool isScreenName = false)
        {
            List<UserInfoObject> result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.lookupUsers(targets, isScreenName);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public SearchResultObject searchPhase(string phase, bool newReq)
        {
            SearchResultObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.searchPhase(phase, newReq);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserInfoObject Block(string id, bool isScreenName = false)
        {
            UserInfoObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.Block(id, isScreenName);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserInfoObject UnBlock(string id, bool isScreenName = false)
        {
            UserInfoObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.UnBlock(id, isScreenName);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserInfoObject Mute(string id)
        {
            UserInfoObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.Mute(id);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }

        public UserInfoObject UnMute(string id)
        {
            UserInfoObject result = null;
            int RetryCount = Credentials.Count;
            while (RetryCount-- > 0)
            {
                try
                {
                    result = Current.UnMute(id);
                    break;
                }
                catch (RateLimitException)
                {
                    if (RetryCount == 0) throw;
                    if (!Settings.Default.AutoSwitchCredApiLimit) throw;
                    SelectNextCredential();
                }
            }
            return result;
        }
    }
}