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
        // TODO: A lot of refactoring needed, maybe the boorus should be an interface?
        private static Safebooru sb = new Safebooru();

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
            //LoadSettings();

            Client.Log += Log;
            Client.MessageReceived += MessageReceived;

            var token = "MzEyOTI2NDM3MDk3MDc4Nzg1.DMvM1A.lYcl4RxYtL2Deawle1UIi2kkUTQ"; 
          //  string token = "MzE1MDcxMjUxMDMzMDk2MTk0.DFqFTw.xILmbk3dEdvw-0wJVUvFH4WphTQ"; // testing token
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

        // TODO - This feels wrong being in here
        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.ToLower() == "!aqua")
            {
                await PostImageSearchAsync(message, "aqua_(konosuba) 1girl", sb);
            }
            else if (message.Content.ToLower() == "!megumin")
            {
                await PostImageSearchAsync(message, "megumin 1girl", sb);
            }
            else if (message.Content.ToLower() == "!darkness")
            {
                await PostImageSearchAsync(message, "darkness_(konosuba) 1girl", sb);
            }
            else if (message.Content.ToLower() == "!konosuba")
            {
                await PostImageSearchAsync(message, "kono_subarashii_sekai_ni_shukufuku_wo! -1girl -1boy", sb);
            }
            else if (message.Content.ToLower().IndexOf("!safebooru") >= 0)
            {
                await PostSafebooruAsync(message);
            }
            else if (message.Content.ToLower().IndexOf("!gelbooru") >= 0)
            {
                await PostGelbooruAsync(message);
            }
            else if (message.Content.ToLower() == "!bugmatty")
            {
                await BugMatty(message);
            }
            else if (message.Content.ToLower().IndexOf("!roll") >= 0)
            {
                await RollDX(message);
            } else if(message.Content.ToLower() == "!flip")
            {
                await FilpHeadsTails(message);
            }
        }

        private async Task FilpHeadsTails(SocketMessage message)
        {
            var rnd = new Random();
            var headsOrTails = rnd.Next(2);

            if (headsOrTails == 0)
            {
                await message.Channel.SendMessageAsync("True");
            }
            else if (headsOrTails == 1)
            {
                await message.Channel.SendMessageAsync("False");
            }
        }

        private async Task RollDX(SocketMessage message)
        {
            int maxRoll = 0;
            if (Int32.TryParse(message.Content.ToLower().Replace("!roll", "").Trim(), out maxRoll))
            {
                var rnd = new Random();
                var randomRoll = rnd.Next(1, maxRoll);
                await message.Channel.SendMessageAsync(randomRoll.ToString());
            }
            else
            {
                await message.Channel.SendMessageAsync("You didn't do it right dummy!");
            }
        }

        private async Task PostImageSearchAsync(SocketMessage message, string tags, SearchImage searchingEngine)
        {
            ImageInfo result = await searchingEngine.SearchRandom(new SearchOption(tags));
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

        private async Task PostGelbooruAsync(SocketMessage message)
        {
            if (CheckNFSWChannel(message) == false)
            {
                await message.Channel.SendMessageAsync("Baka! Hentai! This channel isn't NSFW");
                return;
            }

            var tags = $"{message.Content.ToLower().Replace("!gelbooru", "").Trim()} sort:score:desc";
            await PostImageSearchAsync(message, tags, gb);
        }

        private async Task PostSafebooruAsync(SocketMessage message)
        {
            var tags = message.Content.ToLower().Replace("!safebooru", "").Trim();
            await PostImageSearchAsync(message, tags, sb);
        }

        private async Task BugMatty(SocketMessage message)
        {
            var user = Client.GetUser(108696446991085568);
            if (user != null)
            {
                await message.Channel.SendMessageAsync($"{ user.Mention } stupid");
            }
        }

        private bool CheckNFSWChannel(SocketMessage message)
        {
            return message.Channel.IsNsfw;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}