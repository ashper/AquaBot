using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class RandomImageHandler
    {
        // Random but not so random it's perceived as annoying.
        // Cannot repeat an image while it's running.

        private static string KanpaiImageLocation = $"Images{Path.DirectorySeparatorChar}Kanpais";
        private static string AbuseImageLocation = $"Images{Path.DirectorySeparatorChar}Abuses";
        private static int LastKanpai = 999;
        private static int LastAbuse = 999;

        public async static Task AquaKanpai(SocketMessage message, Func<LogMessage, Task> log, bool sendText)
        {
            await log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, K A N P A I !"));

            var imageResult = PickImage(KanpaiImageLocation, LastKanpai);
            LastKanpai = imageResult.imageIndex;
            if (sendText)
            {
                await message.Channel.SendFileAsync(imageResult.imageURI, "K A N P A I !");
            }
            else
            {
                await message.Channel.SendFileAsync(imageResult.imageURI);
            }
        }

        public async static Task AquaAbuse(SocketMessage message, Func<LogMessage, Task> log)
        {
            await log(new LogMessage(LogSeverity.Info, "Discord", $"{message.Content.ToLower()} detected, reacting to abuse"));

            var imageResult = PickImage(AbuseImageLocation, LastAbuse);
            LastAbuse = imageResult.imageIndex;
            await message.Channel.SendFileAsync(imageResult.imageURI, "Waaaaaaaaa!");
        }

        private static (string imageURI, int imageIndex) PickImage(string FolderDirectory, int lastRandomImage)
        {
            var allImages = new DirectoryInfo(FolderDirectory).GetFiles();
            var imageCount = allImages.Count();
            var rand = new Random();
            var selectedImage = rand.Next(0, imageCount);
            while (selectedImage == lastRandomImage)
            {
                selectedImage = rand.Next(0, imageCount);
            }
            return (allImages[selectedImage].FullName, selectedImage);
        }
    }
}