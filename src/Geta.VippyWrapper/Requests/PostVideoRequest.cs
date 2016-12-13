namespace Geta.VippyWrapper.Requests
{
    public class PostVideoRequest
    {
        public string VideoId { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional, comma separated list of new tags, this will delete all existing tags on this resource
        /// </summary>
        public string Tags { get; set; }
    }
}