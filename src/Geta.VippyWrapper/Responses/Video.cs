using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geta.VippyWrapper.Responses
{
    public class Video
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Tag> Tags { get; set; }
        public float? Duration { get; set; }
        public long? Size { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        [JsonProperty("original")]
        public string OriginalUrl { get; set; }

        [JsonProperty("highQuality")]
        public string HighQualityUrl { get; set; }

        [JsonProperty("lowQuality")]
        public string LowQualityUrl { get; set; }
        public string StatusCode { get; set; }
        public string StatusText { get; set; }
        public DateTime? Uploaded { get; set; }
        public long? HighQualitySize { get; set; }
        public long? LowQualitySize { get; set; }

        [JsonProperty("thumbnail")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("open_graph_url")]
        public string OpenGraphUrl { get; set; }
        public int? Views { get; set; }
        public int? Plays { get; set; }
        public int? EndViews { get; set; }
        public string PlaysToImpression { get; set; }
    }
}