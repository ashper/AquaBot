using System;
using System.Collections.Generic;

namespace AquaBot
{
    public class SearchResult
    {
        public bool Success { get; internal set; }
        public IList<ImageInfo> Result { get; internal set; }
        public Exception Error { get; internal set; }
    }
}