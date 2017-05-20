namespace AquaBot
{
    public interface IBotAvatar
    {
        AvatarID ID { get; }
        Discord.Image Avatar { get; }
        string Name { get; }
        string LeaveMessage { get; }
        string JoinMessage { get; }
        string AlreadyHere { get; }
        string ChangeUnknown { get; }
        string DiscordWontLetMe { get; }
        string NothingFound { get; }
    }
}