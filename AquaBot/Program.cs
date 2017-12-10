using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AquaBot
{
    public class Program
    {
        // TODO: A lot of refactoring needed, maybe the boorus should be an interface?
        // TODO: This is a mess >.>
        private static Safebooru sb = new Safebooru();

        private static Gelbooru gb = new Gelbooru();
        private static WikipediaClient wiki = new WikipediaClient();
        private DiscordSocketClient Client;

        private Settings Settings;

        private Timer tm = null;

        private AutoResetEvent autoEvent = null;

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            Settings = new Settings();
            Settings.LoadSettings();

            autoEvent = new AutoResetEvent(false);
            tm = new Timer(Execute, autoEvent, 0, 600000);

            await Task.Delay(-1);
        }

        public async void Execute(Object stateInfo)
        {
            try
            {
                if (Client == null || Client.ConnectionState != ConnectionState.Connected)
                {
                    await NewAquaBot();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error detected in Execute Method {e.Message} - { e.StackTrace }");
            }
        }

        public async Task NewAquaBot()
        {
            try
            {
                Client = new DiscordSocketClient();
                Client.Log += Log;
                Client.MessageReceived += MessageReceived;

                await Client.LoginAsync(TokenType.Bot, Settings.CurrentSettings.LiveToken);
                await Client.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error detected in NewAquaBot {e.Message} - { e.StackTrace }");
            }
        }

        // TODO - This feels wrong being in here? Maybe some better way to handle a large if like this?
        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == 267118970094485505 && message.Content == "nice")
            {
                await HandleYuudachiNice(message);
            }
            else if (message.Source != MessageSource.Bot)
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
                }
                else if (message.Content.ToLower() == "!flip")
                {
                    await FilpHeadsTails(message);
                }
                else if (message.Content.ToLower().Replace(" ", "").IndexOf("kampai") >= 0 && message.Content.Length <= 20)
                {
                    await RandomImageHandler.AquaKampai(message);
                }
                else if (message.Content.ToLower().IndexOf("aqua") >= 0)
                {
                    await AquaAbuseHandler.HandlePotentialAbuse(message);
                }
                else if (message.Content.ToLower().IndexOf("!wiki") == 0)
                {
                    await WikiSearch(message);
                }
            }
        }

        private async Task WikiSearch(SocketMessage message)
        {
            var searchTerm = message.Content.Substring(5).Trim();
            var pageUrl = await wiki.SearchForPage(searchTerm);
            if (pageUrl == "")
            {
                await message.Channel.SendMessageAsync("Nothing found.");
            }
            else if (pageUrl == "BROKE")
            {
                await message.Channel.SendMessageAsync("Wikipedia search broke :joy:");
            }
            else
            {
                await message.Channel.SendMessageAsync(pageUrl);
            }
        }

        private async Task HandleYuudachiNice(SocketMessage message)
        {
            await Task.Run(async () =>
            {
                var sentMessage = await message.Channel.SendMessageAsync("<:amusedaqua:281585444335124493> This is Aqua's server, bad Yuudachi ");
                await Task.Delay(5000);
                await message.DeleteAsync();
                await sentMessage.DeleteAsync();
            });
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