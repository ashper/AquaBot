namespace AquaBot
{
    public class Megumin : IBotAvatar
    {
        public AvatarID ID => AvatarID.Megumin;

        public Discord.Image Avatar => new Discord.Image("MeguminAvatar.jpg");

        public string Name => "Megumin";

        public string LeaveMessage => "Okay, bye ;_;";

        public string JoinMessage => "EXPLOSION!";

        public string AlreadyHere => "I'm... I'm already here";

        public string ChangeUnknown => "Errrrrr, who?";

        public string DiscordWontLetMe => "Discord said no...";
    }
}