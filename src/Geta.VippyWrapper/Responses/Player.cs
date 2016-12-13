using Geta.VippyWrapper.Helpers;
using Newtonsoft.Json;

namespace Geta.VippyWrapper.Responses
{
    public class Player
    {
        public string PlayerId { get; set; }

        public string Title { get; set; }

        public string Size { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool AutoPlay { get; set; }

        [JsonProperty("default")]
        [JsonConverter(typeof(BoolConverter))]
        public bool IsDefaultPlayer { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool EmbedCode { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Facebook { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Twitter { get; set; }

        public string LogoId { get; set; }

        public string LogoDescription { get; set; }

        public string LogoUrl { get; set; }
    }
}