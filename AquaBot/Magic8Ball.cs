using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class Magic8Ball
    {
        private static readonly Random rnd = new Random();

        private static readonly string[] Answers = new string[] {
             "As I see it, yes.             ",
             "Ask again later.              ",
             "Better not tell you now.      ",
             "Cannot predict now.           ",
             "Concentrate and ask again.    ",
             "Don’t count on it.            ",
             "It is certain.                ",
             "It is decidedly so.           ",
             "Most likely.                  ",
             "My reply is no.               ",
             "My sources say no.            ",
             "Outlook not so good.          ",
             "Outlook good.                 ",
             "Reply hazy, try again.        ",
             "Signs point to yes.           ",
             "Very doubtful.                ",
             "Without a doubt.              ",
             "Yes.                          ",
             "Yes – definitely.             ",
             "You may rely on it.           "
        };

        public async static Task Answer(SocketMessage message, Func<LogMessage, Task> log)
        {
            await log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, rolling magic 8 ball"));
            var answerIndex = rnd.Next(0, Answers.Length);
            await message.Channel.SendMessageAsync(Answers[answerIndex]);
        }
    }
}