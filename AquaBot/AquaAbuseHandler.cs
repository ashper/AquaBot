using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class AquaAbuseHandler
    {
        // Naughty words... giggle
        private static List<string> AbuseTerms = new List<string>()
        {
            "sucks","isshit","crap","useless","uselessgoddess",
            "trash","ashit","thot","fuck","slag","whore","shit",
            "pointless","waste","poopoo","idiot","stinky", "stupid"
        };

        // At this point we know the message contains the text Aqua, lets see if we can find any other text from our abuse list
        public static async Task HandlePotentialAbuse(SocketMessage message, Func<LogMessage, Task> log)
        {
            foreach(var abuse in AbuseTerms)
            {
                if(message.Content.ToLower().Replace(" ", "").IndexOf(abuse) >= 0)
                {
                    await RandomImageHandler.AquaAbuse(message, log);
                    break;
                }
            }
        }

    }
}
