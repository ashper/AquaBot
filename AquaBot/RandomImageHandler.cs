using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquaBot
{
    public static class RandomImageHandler
    {
        private static string KampaiImageLocation = @"Images\Kampais";
        private static string AbuseImageLocation = @"Images\Abuses";

        public async static Task AquaKampai(SocketMessage message)
        {
            await message.Channel.SendFileAsync(PickImage(KampaiImageLocation), "K A M P A I !");
        }

        public async static Task AquaAbuse(SocketMessage message)
        {
            await message.Channel.SendFileAsync(PickImage(AbuseImageLocation), "Waaaaaaaaa!");
        }

        private static string PickImage(string FolderDirectory)
        {
            var allImages = new DirectoryInfo(FolderDirectory).GetFiles();
            var imageCount = allImages.Count();
            var rand = new Random();
            var selectedImage = rand.Next(0, imageCount - 1);
            return allImages[selectedImage].FullName;
        }
    }
}