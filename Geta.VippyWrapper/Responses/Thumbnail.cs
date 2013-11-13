using Newtonsoft.Json;

namespace Geta.VippyWrapper.Responses
{
    public class Thumbnail
    {
        public string VideoId { get; set; }

        [JsonProperty("thumbnail")]
        public string ThumbnailUrl { get; set; }
    }
}