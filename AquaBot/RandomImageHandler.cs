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

        private static string KampaiImageLocation = @"Images\Kampais";
        private static string AbuseImageLocation = @"Images\Abuses";
        private static int LastKampai = 999;
        private static int LastAbuse = 999;

        public async static Task AquaKampai(SocketMessage message)
        {
            var imageResult = PickImage(KampaiImageLocation, LastKampai);
            LastKampai = imageResult.imageIndex;
            await message.Channel.SendFileAsync(imageResult.imageURI, "K A M P A I !");
        }

        public async static Task AquaAbuse(SocketMessage message)
        {
            var imageResult = PickImage(AbuseImageLocation, LastAbuse);
            LastAbuse = imageResult.imageIndex;
            await message.Channel.SendFileAsync(imageResult.imageURI, "Waaaaaaaaa!");
        }

        private static (string imageURI, int imageIndex) PickImage(string FolderDirectory, int lastRandomImage)
        {
            var allImages = new DirectoryInfo(FolderDirectory).GetFiles();
            var imageCount = allImages.Count();
            var rand = new Random();
            var selectedImage = rand.Next(0, imageCount - 1);
            while (selectedImage == lastRandomImage)
            {
                selectedImage = rand.Next(0, imageCount - 1);
            }
            return (allImages[selectedImage].FullName, selectedImage);
        }
    }
}