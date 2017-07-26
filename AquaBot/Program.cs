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
        private static Gelbooru gb = new Gelbooru();
        private DiscordSocketClient Client;
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

            string token = "MzI3ODgyNDU5MjEzNzI1Njk4.DFoeSg.NpS9OUJPeA32NUwHAGgP9_qGipg"; // Remember to keep this private!
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
                  
                }
            }
            else
            {
              
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

        }

        private async Task PostSafebooruAsync(SocketMessage message, string tags)
        {
            ImageInfo result = await gb.SearchRandom(new SearchOption(tags));
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, posting image {result.SampleUrl}"));
            if (result == null || string.IsNullOrWhiteSpace(result.SampleUrl))
            {
                await message.Channel.SendMessageAsync("Hahahaha nothing found, loser");
            }
            else
            {
                await message.Channel.SendMessageAsync(result.SampleUrl);
            }
        }

        private async Task PostCustomSafebooruAsync(SocketMessage message)
        {
            var tags = message.Content.ToLower().Replace("!konocustom", "").Trim();
            await PostSafebooruAsync(message, tags);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}