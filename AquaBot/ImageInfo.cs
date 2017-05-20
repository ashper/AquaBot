namespace AquaBot
{
    public class ImageInfo
    {
        public string ImageId { get; internal set; }

        public string Source { get; internal set; }

        public string OrigUrl { get; internal set; }
        public int OrigWidth { get; internal set; }
        public int OrigHeight { get; internal set; }
        public long OrigFileSize { get; internal set; }

        public string SampleUrl { get; internal set; }
        public long SampleFileSize { get; internal set; }
        public int SampleWidth { get; internal set; }
        public int SampleHeight { get; internal set; }

        public string ThumbUrl { get; internal set; }
        public int ThumbWidth { get; internal set; }
        public int ThumbHeight { get; internal set; }
    }
}