using System;

namespace AquaBot
{
    [Serializable]
    public class BotSettings
    {
        public DateTime FirstNameChangeTime { get; set; }
        public DateTime SecondNameChangeTime { get; set; }
        public AvatarID CurrentAvatarID { get; set; }
    }
}