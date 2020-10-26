using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class DayMessageCommand
    {
        public static async Task CheckAndRun(DiscordSocketClient client, Settings settings)
        {
            settings.LoadSettings();

            // Only run between 5pm and midnight
            if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 24)
            {
                if (settings.CurrentSettings.DayMessageLastRun == null || (DateTime.Now - settings.CurrentSettings.DayMessageLastRun).TotalHours > 18)
                {
                    if (settings.CurrentSettings.DayMessages.TryGetValue(DateTime.Now.DayOfWeek, out DayMessage day))
                    {
                        if (client.GetChannel(settings.CurrentSettings.DayMessageChannel) is IMessageChannel mainChannel)
                        {
                            Console.WriteLine("Day message found, sending");

                            await mainChannel.SendMessageAsync("**" + day.Message + "**");
                            await mainChannel.SendMessageAsync(day.ImageLink);

                            settings.CurrentSettings.DayMessageLastRun = DateTime.Now;
                            settings.SaveSettings();
                        }
                        else
                        {
                            Console.WriteLine("No day message channel found, stopping check");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No day message found, stopping check");
                    }
                }
                else
                {
                    Console.WriteLine("Day message previously run, stopping check");
                }
            }
            else
            {
                Console.WriteLine($"DateTime.Now.Hour = { DateTime.Now.Hour }, not time to run");
            }
        }

        public static async Task Run(DiscordSocketClient client, Settings settings)
        {
            settings.LoadSettings();

            if (settings.CurrentSettings.DayMessages.TryGetValue(DateTime.Now.DayOfWeek, out DayMessage day))
            {
                if (client.GetChannel(settings.CurrentSettings.DayMessageChannel) is IMessageChannel mainChannel)
                {
                    await mainChannel.SendMessageAsync("**" + day.Message + "**");
                    await mainChannel.SendMessageAsync(day.ImageLink);
                }
            }
        }

        public static async Task Handle(DiscordSocketClient client, SocketMessage message, Settings settings)
        {
            switch (message.Content)
            {
                case string a when a.ToLowerInvariant().Contains("-setchannel"):
                    await SetChannel(message, settings);
                    break;

                case string a when a.ToLowerInvariant().Contains("-run"):
                    await Run(client, settings);
                    break;

                case string a when a.ToLowerInvariant().Contains("-setday"):
                    await SetDay(message, settings);
                    break;

                default:
                    break;
            }
        }

        private static Task SetDay(SocketMessage socketMessage, Settings settings)
        {
            var m = socketMessage.Content.Replace("!daymessage", "").Replace("!daym", "");

            var splitCommand = SplitCommandLine(m).ToArray();

            if (splitCommand.Length != 4)
            {
                socketMessage.Channel.SendMessageAsync("Unable to parse daymessage command, expected: -setday DayOfWeek Message Link, don't forget quotes around spaces!");
            }

            var day = splitCommand[1].ToLower();
            var message = splitCommand[2];
            var link = splitCommand[3];

            DayOfWeek? dayOfWeek = null;

            switch (day)
            {
                case "monday":
                case "m":
                    dayOfWeek = DayOfWeek.Monday;
                    break;

                case "tuesday":
                case "tu":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;

                case "wednesday":
                case "w":
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;

                case "thursday":
                case "th":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;

                case "friday":
                case "f":
                    dayOfWeek = DayOfWeek.Friday;
                    break;

                case "saturday":
                case "sa":
                    dayOfWeek = DayOfWeek.Saturday;
                    break;

                case "sunday":
                case "su":
                    dayOfWeek = DayOfWeek.Sunday;
                    break;
            }

            if (dayOfWeek == null)
            {
                socketMessage.Channel.SendMessageAsync("Unable to parse day of week");
            }

            settings.LoadSettings();

            if (settings.CurrentSettings.DayMessages.TryGetValue(dayOfWeek.Value, out DayMessage value))
            {
                value.Message = message;
                value.ImageLink = link;
            }
            else
            {
                settings.CurrentSettings.DayMessages.Add(dayOfWeek.Value, new DayMessage() { ImageLink = link, Message = message });
            }
            settings.SaveSettings();

            socketMessage.Channel.SendMessageAsync("Day message saved");

            return Task.CompletedTask;
        }

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
            .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
            .Where(arg => !string.IsNullOrEmpty(arg));
        }

        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str[nextPiece..c];
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) && (input[0] == quote) && (input[^1] == quote))
            {
                return input[1..^1];
            }

            return input;
        }

        private static async Task SetChannel(SocketMessage message, Settings settings)
        {
            if (message.MentionedChannels.Count() == 1)
            {
                var c = message.MentionedChannels.First();
                settings.CurrentSettings.DayMessageChannel = c.Id;
                settings.SaveSettings();
                await message.Channel.SendMessageAsync("Message channel saved");
            }
            else
            {
                await message.Channel.SendMessageAsync("Error, please mention one channel");
            }
        }
    }
}