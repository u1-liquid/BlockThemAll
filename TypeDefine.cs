using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockThemAll
{
    public enum EntityType
    {
        UNDEFINED,
        SINGLE_BLOCK,
        FOLLOWER_BLOCK,
        FOLLOWING_BLOCK,
        KEYWORD_BLOCK,
        SINGLE_UNBLOCK,
        FOLLOWER_UNBLOCK,
        FOLLOWING_UNBLOCK,
        KEYWORD_UNBLOCK,
        SINGLE_MUTE,
        FOLLOWER_MUTE,
        FOLLOWING_MUTE,
        KEYWORD_MUTE,
        SINGLE_UNMUTE,
        FOLLOWER_UNMUTE,
        FOLLOWING_UNMUTE,
        KEYWORD_UNMUTE,
        SKIP
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
