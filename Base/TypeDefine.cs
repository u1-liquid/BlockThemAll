namespace BlockThemAll.Base
{
    public enum EntityType
    {
        UNDEFINED = 0,
        SINGLE_BLOCK,
        FOLLOWER_BLOCK,
        FOLLOWING_BLOCK,
        KEYWORD_BLOCK,
        REPLY_BLOCK,
        RETWEET_BLOCK,
        SINGLE_UNBLOCK,
        FOLLOWER_UNBLOCK,
        FOLLOWING_UNBLOCK,
        KEYWORD_UNBLOCK,
        REPLY_UNBLOCK,
        RETWEET_UNBLOCK,
        BLOCK_TO_MUTE = 20,
        SINGLE_MUTE,
        FOLLOWER_MUTE,
        FOLLOWING_MUTE,
        KEYWORD_MUTE,
        REPLY_MUTE,
        RETWEET_MUTE,
        SINGLE_UNMUTE,
        FOLLOWER_UNMUTE,
        FOLLOWING_UNMUTE,
        KEYWORD_UNMUTE,
        REPLY_UNMUTE,
        RETWEET_UNMUTE,
        SKIP = 999
    }

    public class TreeEntity
    {
        public EntityType Type { get; set; }
        public string Target { get; set; }

        public TreeEntity(EntityType type, string target)
        {
            Type = type;
            Target = target;
        }
    }
}
