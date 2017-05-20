namespace AquaBot
{
    internal class Darkness : IBotAvatar
    {
        public AvatarID ID => AvatarID.Darkness;

        public Discord.Image Avatar => new Discord.Image("DarknessAvatar.jpg");

        public string Name => "Darkess";

        public string LeaveMessage => "Are you going to punish me?";

        public string JoinMessage => "I will shield you with my body master!";

        public string AlreadyHere => "I'm beside you already master";

        public string ChangeUnknown => "I don't know this person master";

        public string DiscordWontLetMe => "Discord has stopped me master";
    }
}