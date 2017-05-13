using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace AquaBot
{
    public class Program
    {
        private static SearchImage si = new Safebooru();

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = "MzEyOTI2NDM3MDk3MDc4Nzg1.C_iviw.lmDn-OfQfuxlrMf3MvKfwu9yMfE"; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
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
        }

        private async void PostTextMessageAsync(SocketMessage message, string messageText)
        {
            await message.Channel.SendMessageAsync(messageText);
        }

        private async Task PostSafebooruAsync(SocketMessage message, string tags)
        {
            ImageInfo result = await si.SearchRandom(new SearchOption(tags));
            await Log(new LogMessage(LogSeverity.Info, "Discord", string.Format("{0} deteced, posting image {1}", message.Content.ToLower(), result.SampleUrl)));
            await message.Channel.SendMessageAsync(result.SampleUrl);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}