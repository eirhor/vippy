using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using EPiServer.Web.Hosting;

namespace Geta.VippyVpp
{
    /// <summary>
    /// Virtual path provider for Vippy
    /// </summary>
    public class VirtualPathProvider : VirtualPathUnifiedProvider
    {
        private readonly RootDirectory _root;
        private readonly VippyWrapper.VippyWrapper _vippyClient;

        public RootDirectory RootDirectory
        {
            get { return _root; }
        }

        public VippyWrapper.VippyWrapper VippyClient
        {
            get { return _vippyClient; }
        }

        public VirtualPathProvider(string name, NameValueCollection configParameters)
            : base(name, configParameters)
        {
            ValidateAndSetupConfigParams();
            VirtualPathRoot = VirtualPathUtility.ToAbsolute(ConfigurationParameters["virtualPath"]);

            _root = new RootDirectory(this);
            _vippyClient = new VippyWrapper.VippyWrapper(ConfigurationParameters["apiKey"], ConfigurationParameters["secretKey"]);
        }

        public override bool FileExists(string virtualPath)
        {
            Func<string, bool> fileExists = path => GetFileForPath(path) != null;
            return ExecuteForProvider(virtualPath, fileExists, Previous.FileExists);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return ExecuteForProvider(virtualPath, GetFileForPath, Previous.GetFile);
        }

        private VirtualFile GetFileForPath(string virtualPath)
        {
            var videoId = VideoFile.GetVideoIdByVirtualPath(virtualPath);
            if (string.IsNullOrWhiteSpace(videoId))
            {
                return null;
            }

            var video = _vippyClient.GetVideo(videoId).Result;
            if (video == null)
            {
                return null;
            }

            return new VideoFile(this, video);
        }

        /// <summary>
        ///     Vippy does not support directories, so it returns false for Vippy virtual path
        /// </summary>
        /// <param name="virtualDir">Virtual path</param>
        /// <returns>False for Vippy or checks directory existance from previous provider</returns>
        public override bool DirectoryExists(string virtualDir)
        {
            Func<string, bool> directoryExists = IsRootDirectory;
            return ExecuteForProvider(virtualDir, directoryExists, Previous.DirectoryExists);
        }

        /// <summary>
        ///     Vippy does not support directories, so it returns null for Vippy virtual path
        /// </summary>
        /// <param name="virtualDir">Virtual path</param>
        /// <returns>Null for Vippy or path from previous provider</returns>
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return ExecuteForProvider(virtualDir, GetDirectoryForPath, Previous.GetDirectory);
        }

        private VirtualDirectory GetDirectoryForPath(string virtualDir)
        {
            if (IsRootDirectory(virtualDir))
            {
                return _root;
            }

            return null;
        }

        private T ExecuteForProvider<T>(string virtualPath, Func<string, T> task, Func<string, T> defaultTask)
        {
            if (IsProvidersVirtualPath(virtualPath))
            {
                return task(virtualPath);
            }

            return defaultTask(virtualPath);
        }

        private bool IsProvidersVirtualPath(string virtualPath)
        {
            var relRoot = VirtualPathUtility.ToAppRelative(VirtualPathRoot);
            var relPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return relPath.StartsWith(relRoot, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsRootDirectory(string virtualDir)
        {
            return virtualDir.Equals(VirtualPathRoot);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return ExecuteForProvider(virtualPath, path => null,
                path => Previous.GetCacheDependency(path, virtualPathDependencies, utcStart));
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return ExecuteForProvider(virtualPath, path => Guid.NewGuid().ToString(),
                path => Previous.GetFileHash(path, virtualPathDependencies));
        }

        internal void ShowResponse(string message)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Write("<html><head><title>Vippy message</title></head><body><p><a href=\"javascript:history.go(-1);\">&lt; Back</a></p><p>" + message + "</p></body></html>");
            HttpContext.Current.Response.End();
        }
    }
}