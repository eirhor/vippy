namespace Geta.VippyWrapper.Requests
{
    public class GetEmbedCodeRequest
    {
        public string VideoId { get; set; }

        public string PlayerId { get; set; }

        /// <summary>
        /// Example: 640x480
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Should show embed funcion on player
        /// </summary>
        public bool ShowEmbedCode { get; set; }

        public bool EnableFacebookSharing { get; set; }

        public bool EnableTwitterSharing { get; set; }

        public bool EnableLinkedInSharing { get; set; }

        /// <summary>
        /// You need to specify a valid logo, this is the ID of one of your uploaded logos
        /// </summary>
        public string LogoId { get; set; }
    }
}