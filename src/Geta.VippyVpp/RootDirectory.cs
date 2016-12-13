using System;
using System.Collections;
using System.Linq;
using EPiServer.Web.Hosting;

namespace Geta.VippyVpp
{
    /// <summary>
    ///     Vippy root directory - only one directory supported by Vippy
    /// </summary>
    public class RootDirectory : UnifiedDirectory
    {
        private readonly VirtualPathProvider _provider;

        public RootDirectory(VirtualPathProvider provider)
            : base(provider, provider.VirtualPathRoot, null, true)
        {
            _provider = provider;
        }

        /// <summary>
        ///     Root directory is always first level
        /// </summary>
        public override bool IsFirstLevel
        {
            get { return true; }
        }

        /// <summary>
        ///     Root directory does't have parent, returns null
        /// </summary>
        public override UnifiedDirectory Parent
        {
            get { return null; }
        }

        /// <summary>
        ///     Vippy doesn't support subdirectories, returns empty array
        /// </summary>
        /// <returns>Empty array</returns>
        public override UnifiedDirectory[] GetDirectories()
        {
            return new UnifiedDirectory[0];
        }

        /// <summary>
        ///     Vippy has all files in the root directory, returns all video files
        /// </summary>
        /// <returns>Array of video files</returns>
        public override UnifiedFile[] GetFiles()
        {
            var videos = _provider.VippyClient.GetVideos().Result;

            if (videos == null)
            {
                return new UnifiedFile[0];
            }
            return videos
                .Select(video => (new VideoFile(_provider, video) as UnifiedFile))
                .ToArray();
        }

        /// <summary>
        ///     <see cref="GetDirectories"/>
        /// </summary>
        public override IEnumerable Directories
        {
            get { return GetDirectories(); }
        }

        /// <summary>
        ///     <see cref="GetFiles"/>
        /// </summary>
        public override IEnumerable Files
        {
            get { return GetFiles(); }
        }

        /// <summary>
        ///     <see cref="GetDirectories"/>
        ///     <see cref="GetFiles"/>
        /// </summary>
        public override IEnumerable Children
        {
            get { return GetFiles(); }
        }

        public override UnifiedDirectory CreateSubdirectory(string path)
        {
            _provider.ShowResponse("Creation of subdirectories not supported in Vippy");
            return null;
        }

        public override UnifiedFile CreateFile(string name)
        {
            return CreateFile(name, Guid.NewGuid());
        }

        public override UnifiedFile CreateFile(string name, Guid id)
        {
            return new VideoFile(_provider, name);
        }
    }
}