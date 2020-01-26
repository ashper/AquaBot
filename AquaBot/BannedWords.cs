using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class BannedWords
    {
        private static List<string> CharsToRemove = new List<string>() { "|", "*", "_" };

        public static Task AddBannedWord(SocketMessage message, string word, Settings settings)
        {
            if (!settings.CurrentSettings.BannedWords.Contains(word.ToLower()))
            {
                settings.CurrentSettings.BannedWords.Add(word);
                settings.SaveSettings();
                return message.Channel.SendMessageAsync($@"{message.Author.Mention} word added");
            }
            return message.Channel.SendMessageAsync($@"{message.Author.Mention} word already in list");
        }

        public static bool CheckMessage(SocketMessage message, List<string> bannedWords)
        {
            var cleanedMessage = message.Content.RemoveStrings(CharsToRemove);

            foreach (var word in bannedWords)
            {
                if (cleanedMessage.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static Task HandleUseOfBannedWords(List<string> bannedWords, SocketMessage message)
        {
            Console.WriteLine("Banned word detected, fixing");

            var cleanedMessage = message.Content.RemoveStrings(CharsToRemove);

            foreach (var word in bannedWords)
            {
                cleanedMessage = cleanedMessage.Replace(word, "water", StringComparison.OrdinalIgnoreCase);
            }

            message.DeleteAsync();
            return message.Channel.SendFileAsync("Images/Purification.gif",
                $@"{message.Author.Mention} Purification! Purification! Purification! `{cleanedMessage}`"
                );
        }

        private static string RemoveStrings(this string input, List<string> CharsToRemove)
        {
            foreach (var c in CharsToRemove)
            {
                input = input.Replace(c, "");
            }
            return input;
        }
    }
}