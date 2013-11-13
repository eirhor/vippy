using Geta.VippyWrapper.Responses;

namespace Geta.VippyWrapper.Requests
{
    public class NewVideoRequest
    {
        /// <summary>
        /// Content-Type (required, content type of your video)
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Content-Length (required, the size of your video)
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// x-vpy-video (required, path to the video you are uploading)
        /// </summary>
        public string VideoPath { get; set; }

        /// <summary>
        /// x-vpy-title (required, the title which Vippy will create your video under)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content-MD5 (optional, md5 hash of your video file)
        /// </summary>
        public string ContentMD5Hash { get; set; }

        /// <summary>
        /// x-vpy-description (optional, sets the description on your video)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Expect (optional, useful if you plan to upload large files to Vippy)
        /// </summary>
        public bool Expect { get; set; }

        /// <summary>
        /// x-vpy-uploadedby (optional, name of the person which is uploading the file)
        /// </summary>
        public string UploadedBy { get; set; }

        /// <summary>
        /// x-vpy-tags (optional, comma separated list of tags)
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// x-vpy-quality (optional, defines the quality Vippy will encode your video in. Can be set to lower, normal or higher)
        /// </summary>
        public VideoQuality? VideoQuality { get; set; }

        /// <summary>
        /// x-vpy-notify (optional, you can set an url which Vippy will do a HTTP post to when the upload is complete, the HTTP post will include videoId and status of your video)
        /// </summary>
        public string NotifyUrl { get; set; }

        public byte[] Data { get; set; }
    }
}