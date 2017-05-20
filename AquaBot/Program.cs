using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AquaBot
{
    public class Program
    {
        private static Safebooru si = new Safebooru();
        private DiscordSocketClient Client;
        private IBotAvatar CurrentAvatar;
        private const string SettingFileName = "settings.json";
        private BotSettings CurrentSettings;

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            Client = new DiscordSocketClient();
            LoadSettings();

            Client.Log += Log;
            Client.MessageReceived += MessageReceived;

            string token = "MzE1MDcxMjUxMDMzMDk2MTk0.DAHWWg.vMNzJR7GTwIFHkRJbIx6kTaT7L0"; // Remember to keep this private!
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingFileName))
            {
                using (StreamReader re = File.OpenText(SettingFileName))
                {
                    JsonSerializer se = new JsonSerializer();
                    JsonTextReader reader = new JsonTextReader(re);
                    CurrentSettings = se.Deserialize<BotSettings>(reader);
                    switch (CurrentSettings.CurrentAvatarID)
                    {
                        case AvatarID.Aqua:
                            CurrentAvatar = new Aqua();
                            break;

                        case AvatarID.Darkness:
                            CurrentAvatar = new Darkness();
                            break;

                        case AvatarID.Megumin:
                            CurrentAvatar = new Megumin();
                            break;
                    }
                }
            }
            else
            {
                CurrentAvatar = new Aqua();
                CurrentSettings = new BotSettings() { CurrentAvatarID = AvatarID.Aqua, FirstNameChangeTime = DateTime.Now, SecondNameChangeTime = DateTime.Now };
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            using (StreamWriter file = File.CreateText(SettingFileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, CurrentSettings);
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.ToLower() == "!aqua")
            {
                await PostSafebooruAsync(message, "aqua_(konosuba) 1girl");
            }
            else if (message.Content.ToLower() == "!megumin")
            {
                await PostSafebooruAsync(message, "megumin 1girl");
            }
            else if (message.Content.ToLower() == "!darkness")
            {
                await PostSafebooruAsync(message, "darkness_(konosuba) 1girl");
            }
            else if (message.Content.ToLower() == "!konosuba")
            {
                await PostSafebooruAsync(message, "kono_subarashii_sekai_ni_shukufuku_wo! -1girl -1boy");
            }
            else if (message.Content.ToLower().IndexOf("!konocustom") >= 0)
            {
                await PostCustomSafebooruAsync(message);
            }
            else if (message.Content.ToLower().IndexOf("!change") >= 0)
            {
                await ChangeBotAvatar(message);
            }
        }

        private async Task PostSafebooruAsync(SocketMessage message, string tags)
        {
            ImageInfo result = await si.SearchRandom(new SearchOption(tags));
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, posting image {result.SampleUrl}"));
            await message.Channel.SendMessageAsync(result.SampleUrl);
        }

        private async Task PostCustomSafebooruAsync(SocketMessage message)
        {
            var tags = message.Content.ToLower().Replace("!konocustom", "").Trim();
            await PostSafebooruAsync(message, tags);
        }

        private async Task ChangeBotAvatar(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"Change request detected {message.Content}"));
            if (CanIChangeUser() == false)
            {
                await message.Channel.SendMessageAsync(CurrentAvatar.DiscordWontLetMe);
                return;
            }
            var newAvatarString = message.Content.ToLower().Replace("!change", "").Trim();
            IBotAvatar newAvatar = null;
            switch (newAvatarString)
            {
                case "aqua":
                    newAvatar = new Aqua();
                    break;

                case "megumin":
                    newAvatar = new Megumin();
                    break;

                case "darkness":
                    newAvatar = new Darkness();
                    break;
            }
            if (newAvatar == null)
            {
                await message.Channel.SendMessageAsync(CurrentAvatar.ChangeUnknown);
            }
            else if (newAvatar.ID == CurrentAvatar.ID)
            {
                await message.Channel.SendMessageAsync(CurrentAvatar.AlreadyHere);
            }
            else
            {
                try
                {
                    await message.Channel.SendMessageAsync(CurrentAvatar.LeaveMessage);
                    await Client.CurrentUser.ModifyAsync(x => { x.Avatar = newAvatar.Avatar; x.Username = $"[KonoBot]{newAvatar.Name}"; });
                    await Task.Delay(1000, System.Threading.CancellationToken.None);
                    await message.Channel.SendMessageAsync(newAvatar.JoinMessage);
                    CurrentAvatar = newAvatar;
                    SaveUserChange();
                }
                catch (Exception)
                {
                    await message.Channel.SendMessageAsync(CurrentAvatar.DiscordWontLetMe);
                }
            }
        }

        private void SaveUserChange()
        {
            CurrentSettings.SecondNameChangeTime = CurrentSettings.FirstNameChangeTime;
            CurrentSettings.FirstNameChangeTime = DateTime.Now;
            CurrentSettings.CurrentAvatarID = CurrentAvatar.ID;
            SaveSettings();
        }

        private bool CanIChangeUser()
        {
            DateTime now = DateTime.Now;
            if ((CurrentSettings.FirstNameChangeTime > now.AddHours(-1) && CurrentSettings.FirstNameChangeTime <= now)
                && (CurrentSettings.SecondNameChangeTime > now.AddHours(-1) && CurrentSettings.SecondNameChangeTime <= now))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}