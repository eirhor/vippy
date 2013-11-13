using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer.Security;
using EPiServer.Web;
using EPiServer.Web.Hosting;
using Geta.VippyWrapper.Requests;
using Geta.VippyWrapper.Responses;

namespace Geta.VippyVpp
{
    /// <summary>
    ///     File for Vippy video
    /// </summary>
    public class VideoFile : UnifiedFile
    {
        private const string VideoStatusCompleted = "7";
        private readonly VirtualPathProvider _provider;
        private readonly Video _vippyVideo;
        private readonly VideoSummary _videoSummary;

        public VideoFile(VirtualPathProvider provider, Video vippyVideo)
            : base(provider.RootDirectory, provider, CreateVideoPath(provider, vippyVideo), true)
        {
            _provider = provider;
            _vippyVideo = vippyVideo;
            _videoSummary = new VideoSummary(this, vippyVideo);
        }

        public VideoFile(VirtualPathProvider provider, string filename)
            : base(provider.RootDirectory, provider, CreateVideoPathFromFileName(provider, filename), true)
        {
            _provider = provider;
            _videoSummary = new VideoSummary(this, filename);
        }

        private static string CreateVideoPathFromFileName(VirtualPathProvider provider, string filename)
        {
            return provider.CombineVirtualPaths(provider.RootDirectory.VirtualPath, filename);
        }

        /// <summary>
        ///     Vippy video associated fith virtual file
        /// </summary>
        public Video Video
        {
            get
            {
                return _vippyVideo;
            }
        }

        /// <summary>
        ///     Returns AccessLevel for querying
        /// </summary>
        /// <returns>AccessLevel.Read</returns>
        public override AccessLevel QueryAccess()
        {
            // TODO: now read access, but check if other needed
            return AccessLevel.Read;
        }

        /// <summary>
        ///     Vippy files are not local, returns null.
        /// </summary>
        public override string LocalPath
        {
            // This is local path, not URL
            // Return null otherwise handler will check local path existance with File.Exists
            get { return null; }
        }

        /// <summary>
        ///     Link to the original video file
        /// </summary>
        public override string PermanentLinkVirtualPath
        {
            get { return _vippyVideo.OriginalUrl; }
        }

        /// <summary>
        ///     Upload date/time to Vippy
        /// </summary>
        public override DateTime Created
        {
            get
            {
                return
                 _vippyVideo.Uploaded.HasValue
                 ? _vippyVideo.Uploaded.Value
                 : DateTime.Now;
            }
        }

        /// <summary>
        ///     <see cref="Created"/>
        /// </summary>
        public override DateTime Changed
        {
            get { return Created; }
        }

        /// <summary>
        ///     Extension of original video
        /// </summary>
        public override string Extension
        {
            get { return VirtualPathUtilityEx.GetExtension(_vippyVideo.OriginalUrl); }
        }

        /// <summary>
        ///     Size of file
        /// </summary>
        public override long Length
        {
            get { return _vippyVideo.Size.HasValue ? _vippyVideo.Size.Value : 0; }
        }

        /// <summary>
        ///     Opens file for read/write. Current implementation supports only reads.
        /// </summary>
        /// <param name="mode">FileMode</param>
        /// <param name="access">FileAccess</param>
        /// <param name="share">FileShare</param>
        /// <returns>Stream for file access</returns>
        public override Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            if (mode == FileMode.Open)
            {
                return OpenVideo();
            }

            if (new[] {FileMode.CreateNew, FileMode.Create, FileMode.OpenOrCreate}.Contains(mode))
            {
                return UploadVideo();
            }

            return new MemoryStream();
        }

        private Stream UploadVideo()
        {
            var file = HttpContext.Current.Request.Files[0];
            var fileStream = file.InputStream;
            var contentType = file.ContentType;
            var filename = file.FileName;
            long contentLength;

            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                fileStream.Position = 0;
                fileStream.CopyTo(memoryStream);
                contentLength = memoryStream.Length;
                data = memoryStream.ToArray();
            }

            var newVideoRequest = new NewVideoRequest
            {
                Data = data,
                ContentLength = contentLength,
                Title = Path.GetFileNameWithoutExtension(filename),
                VideoPath = filename,
                ContentType = contentType
            };

            var response = _provider.VippyClient.PutVideo(newVideoRequest).Result;
            
            return new MemoryStream();
        }

        private Stream OpenVideo()
        {
            if (VideoIsAvailable())
            {
                var client = new WebClient();
                try
                {
                    var fileByte = client.DownloadData(_vippyVideo.OriginalUrl);
                    return new MemoryStream(fileByte);
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        if (statusCode == HttpStatusCode.Forbidden)
                        {
                            _provider.ShowResponse(string.Format("Video is not available yet: Completing"));
                        }
                    }
                }
            }

            _provider.ShowResponse(string.Format("Video is not available yet: {0}", _vippyVideo.StatusText));

            return new MemoryStream();
        }

        private bool VideoIsAvailable()
        {
            return _vippyVideo.StatusCode == VideoStatusCompleted;
        }

        /// <summary>
        ///     Vippy's root directory
        /// </summary>
        public override UnifiedDirectory Parent
        {
            get { return _provider.RootDirectory; }
        }

        /// <summary>
        ///     Vippy video file summary
        /// </summary>
        public override IUnifiedSummary Summary
        {
            get { return _videoSummary; }
        }

        public override void Delete()
        {
            base.Delete();
        }

        public override void CopyTo(string newVirtualPath)
        {
            base.CopyTo(newVirtualPath);
        }

        public override void CopyTo(string newVirtualPath, Guid fileId)
        {
            base.CopyTo(newVirtualPath, fileId);
        }

        public override void MoveTo(string newVirtualPath)
        {
            base.MoveTo(newVirtualPath);
        }

        public static string GetVideoIdByVirtualPath(string virtualPath)
        {
            var path = VirtualPathUtilityEx.ToAppRelative(virtualPath);
            var segments = path.Split('/');
            if (segments.Length < 3)
            {
                return string.Empty;
            }

            var fileName = segments[2];
            var idxOfSeparator = fileName.IndexOf('-');
            var id = fileName.Length > idxOfSeparator && idxOfSeparator > 0 
                        ? fileName.Substring(0, idxOfSeparator) 
                        : string.Empty;

            int x;
            if (int.TryParse(id, out x))
            {
                return id;
            }

            return string.Empty;
        }

        private static string CreateFileName(Video video)
        {
            var name = CleanString(video.Title);
            var extension = VirtualPathUtilityEx.GetExtension(video.OriginalUrl);
            return string.Format("{0}-{1}{2}", video.VideoId, name, extension);
        }

        private static string CleanString(string input)
        {
            return Regex.Replace(input, @"[^\ \w\.@-]", string.Empty, RegexOptions.None);
        }

        /// <summary>
        ///     Creates virtual path for video file
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="video"></param>
        /// <returns>Virtual path to file</returns>
        private static string CreateVideoPath(VirtualPathUnifiedProvider provider, Video video)
        {
            return string.Format("{0}{1}", provider.VirtualPathRoot, CreateFileName(video));
        }
    }
}