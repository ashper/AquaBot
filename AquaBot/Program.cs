using Discord;
using Discord.WebSocket;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AquaBot
{
    public class Program
    {
        // TODO: A lot of refactoring needed, maybe the boorus should be an interface?
        // TODO: This is a mess >.>

        private static readonly Safebooru safeBooruImages = new Safebooru();
        private static readonly Gelbooru gelbooruImages = new Gelbooru();
        private static readonly WikipediaClient wiki = new WikipediaClient();
        private static Random rnd = new Random();
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

                await Client.LoginAsync(TokenType.Bot, Settings.CurrentSettings.TestingToken);
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
            if (message.Author.IsBot == false)
            {
                if (string.Equals(message.Content, "!aqua", StringComparison.OrdinalIgnoreCase))
                {
                    await PostImageSearchAsync(message, "aqua_(konosuba) 1girl", safeBooruImages);
                }
                else if (string.Equals(message.Content, "!megumin", StringComparison.OrdinalIgnoreCase))
                {
                    await PostImageSearchAsync(message, "megumin 1girl", safeBooruImages);
                }
                else if (string.Equals(message.Content, "!darkness", StringComparison.OrdinalIgnoreCase))
                {
                    await PostImageSearchAsync(message, "darkness_(konosuba) 1girl", safeBooruImages);
                }
                else if (string.Equals(message.Content, "!konosuba", StringComparison.OrdinalIgnoreCase))
                {
                    await PostImageSearchAsync(message, "kono_subarashii_sekai_ni_shukufuku_wo! -1girl -1boy", safeBooruImages);
                }
                else if (message.Content.IndexOf("!safebooru", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    await PostSafebooruAsync(message);
                }
                else if (message.Content.IndexOf("!gelbooru", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    await PostGelbooruAsync(message);
                }
                else if (string.Equals(message.Content, "!bugmatty", StringComparison.OrdinalIgnoreCase))
                {
                    await BugMatty(message);
                }
                else if (string.Equals(message.Content, "!flip", StringComparison.OrdinalIgnoreCase))
                {
                    await FilpHeadsTails(message);
                }
                // spaces could be between the letters, k a m p a i, because walp does that
                else if (message.Content.Replace(" ", "").IndexOf("kampai", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    await RandomImageHandler.AquaKampai(message, Log, true);
                }
                else if (message.Content.IndexOf("aqua", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    await AquaAbuseHandler.HandlePotentialAbuse(message, Log);
                }
                else if (message.Content.IndexOf("!wiki", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await WikiSearch(message);
                }
                else if (message.Content.IndexOf("!drinking", StringComparison.OrdinalIgnoreCase) == 0 || message.Content.IndexOf("!drinkers", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await DrinkingHandler.SetDrinkers(message, Settings, Log);
                }
                else if (message.Content.IndexOf("!drink", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await DrinkingHandler.Drink(message, Settings, Client, Log);
                }
                else if (message.Content.IndexOf("!d", StringComparison.OrdinalIgnoreCase) == 0 && Regex.Match(message.Content.ToLower().Replace(" ", ""), @"!d\d+").Success)
                {
                    await RollDX(message);
                }
                else if (message.Content.IndexOf("!tuck", StringComparison.OrdinalIgnoreCase) == 0 && message.MentionedUsers.Count > 0)
                {
                    await Tuck(message);
                }
                else if (message.Content.Replace("!", "").Contains(Client.CurrentUser.Mention.Replace("!", "")))
                {
                    if (message.Content.IndexOf("or", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        await MakeDecision(message);
                    }
                    else if (message.Content.LastIndexOf('?') == message.Content.Length - 1)
                    {
                        await Magic8Ball.Answer(message, Log);
                    }
                }
            }
        }

        private async Task MakeDecision(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, making a choice"));

            var cleanedMessage = message.Content.Replace(Client.CurrentUser.Mention, "").Replace(Client.CurrentUser.Mention.Replace("!", ""), "");
            var splitOrs = cleanedMessage.ToLower().Split("or", StringSplitOptions.RemoveEmptyEntries);
            var choiceIndex = rnd.Next(0, splitOrs.Length);
            var choice = splitOrs[choiceIndex];

            await message.Channel.SendMessageAsync(choice);
        }

        private async Task Tuck(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, tucking people in"));
            foreach (var user in message.MentionedUsers)
            {
                await message.Channel.SendMessageAsync($"Tucking { user.Mention } into bed :point_right: :bed:, night night!");
            }
        }

        private async Task WikiSearch(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, searching wikipedia"));

            var searchTerm = message.Content.Substring(5).Trim();
            var pageUrl = await wiki.SearchForPage(searchTerm);
            if (pageUrl?.Length == 0)
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

        private async Task FilpHeadsTails(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, flipping coin"));

            var rnd = new Random();
            var headsOrTails = rnd.Next(2);

            if (headsOrTails == 0)
            {
                await message.Channel.SendMessageAsync("Tails");
            }
            else if (headsOrTails == 1)
            {
                await message.Channel.SendMessageAsync("Heads");
            }
        }

        private async Task RollDX(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, rolling dice"));

            if (Int32.TryParse(message.Content.Replace(" ", "").ToLower().Replace("!d", "").Trim(), out var parsedRoll))
            {
                var rnd = new Random();
                var randomRoll = rnd.Next(1, parsedRoll + 1);
                await message.Channel.SendMessageAsync(randomRoll.ToString());
            }
        }

        private async Task PostImageSearchAsync(SocketMessage message, string tags, SearchImage searchingEngine)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, posting image"));

            ImageInfo result = await searchingEngine.SearchRandom(new SearchOption(tags));
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
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, searching gelbooru"));

            if (CheckNFSWChannel(message) == false)
            {
                await message.Channel.SendMessageAsync("Baka! Hentai! This channel isn't NSFW");
                return;
            }

            var tags = $"{message.Content.ToLower().Replace("!gelbooru", "").Trim()} sort:score:desc";
            await PostImageSearchAsync(message, tags, gelbooruImages);
        }

        private async Task PostSafebooruAsync(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, searching safebooru"));

            var tags = message.Content.ToLower().Replace("!safebooru", "").Trim();
            await PostImageSearchAsync(message, tags, safeBooruImages);
        }

        private async Task BugMatty(SocketMessage message)
        {
            await Log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, bugging matty"));

            var user = Client.GetUser(108696446991085568);
            if (user != null)
            {
                await message.Channel.SendMessageAsync($"{ user.Mention } stupid");
            }
        }

        private bool CheckNFSWChannel(SocketMessage message)
        {
            return true;
            //This has been removed, need to find an alternative, let it through for now.
            //return message.Channel.IsNsfw;
        }

        // Could move this out to it's own class and also log to a file?
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}