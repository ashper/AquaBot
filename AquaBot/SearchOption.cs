namespace AquaBot
{
    public class SearchOption
    {
        public SearchOption()
        : this(null, null, null)
        { }

        public SearchOption(string tags)
            : this(tags, null, null)
        { }

        public SearchOption(string tags, int page)
            : this(tags, page, null)
        { }

        public SearchOption(string tags, int? page)
            : this(tags, page, null)
        { }

        public SearchOption(string tags, int? page, int? limit)
        {
            this.Tags = tags;
            this.Limit = limit;
            this.Page = page;
        }

        public string Tags { get; set; }
        public int? Limit { get; set; }
        public int? Page { get; set; }
    }
}