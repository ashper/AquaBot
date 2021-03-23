using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class DayMessageCommand
    {
        public static async Task CheckAndRun(DiscordSocketClient client, Settings settings)
        {
            settings.LoadSettings();

            var messages = settings.CurrentSettings.DayMessages.Where(x => x.Day == DateTime.Now.DayOfWeek && (DateTime.Now - x.LastRun).TotalDays >= 1);
            if (messages.Count() > 0)
            {
                if (client.GetChannel(settings.CurrentSettings.DayMessageChannel) is IMessageChannel mainChannel)
                {
                    if (DateTime.Now.Hour >= 8)
                    {
                        foreach (var m in messages.Where(x => x.MorningMessage))
                        {
                            Console.WriteLine("Morning message found, sending");

                            await mainChannel.SendMessageAsync("**" + m.Message + "**");
                            await mainChannel.SendMessageAsync(m.ImageLink);
                            m.LastRun = DateTime.Now;
                        }
                    }

                    if (DateTime.Now.Hour >= 20)
                    {
                        foreach (var m in messages.Where(x => !x.MorningMessage))
                        {
                            Console.WriteLine("Evening message found, sending");

                            await mainChannel.SendMessageAsync("**" + m.Message + "**");
                            await mainChannel.SendMessageAsync(m.ImageLink);
                            m.LastRun = DateTime.Now;
                        }
                    }
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
    }
}
