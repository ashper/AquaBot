using System;

namespace AquaBot
{
    public class Aqua : IBotAvatar
    {
        public AvatarID ID => AvatarID.Aqua;

        public Discord.Image Avatar => new Discord.Image("AquaAvatar.jpg");

        public string Name => "Aqua";

        public string LeaveMessage => "Hmmph, fine!";

        public string JoinMessage => "You owe me more praise!";

        public string AlreadyHere => "Baka, I'm right here";

        public string ChangeUnknown => "Who?!";

        public string DiscordWontLetMe => "Discord is a big meanie and stopped me";
    }
}