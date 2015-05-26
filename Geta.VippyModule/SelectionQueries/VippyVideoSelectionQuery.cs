using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using Geta.VippyWrapper.Responses;
using log4net;

namespace Geta.VippyModule.SelectionQueries
{
    [ServiceConfiguration(typeof(ISelectionQuery))]
    public class VippyVideoSelectionQuery : ISelectionQuery
    {
        readonly ILog _logger = LogManager.GetLogger(typeof(VippyVideoSelectionQuery));

        public ISelectItem GetItemByValue(string value)
        {
            var video = GetVideoById(value);

            if (video != null)
            {
                return new SelectItem
                {
                    Text = video.Title,
                    Value = video.VideoId
                };
            }

            return null;
        }

        public Video GetVideoById(string id)
        {
            var videos = GetVideos();
            return videos.FirstOrDefault(x => x.VideoId == id);
        }

        public IEnumerable<ISelectItem> GetItems(string query)
        {
            var matches = GetVideos();

            if (IsNotEmpty(query))
            {
                matches = FilterByName(query, matches);
            }

            return matches
                .Take(20)
                .Select(x => new SelectItem
                {
                    Text = x.Title,
                    Value = x.VideoId
                });
        }

        private static bool IsNotEmpty(string query)
        {
            return !string.IsNullOrEmpty(query) && !string.Equals(query, "*", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<Video> FilterByName(string name, IEnumerable<Video> matches)
        {
            //Remove * at the end of name                
            var n = name.Substring(0, name.Length - 1);
            return matches.Where(e => e.Title.StartsWith(n, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Video> GetVideos()
        {
            string cacheKey = "vippyvideos";

            var videos = CacheManager.Get(cacheKey) as List<Video>;

            if (videos == null)
            {
                var wrapper = new VippyWrapper.VippyWrapper(VippyConfiguration.ApiKey, VippyConfiguration.SecretKey);

                videos = (wrapper.GetVideos().Result).ToList();

                CacheManager.Insert(cacheKey, videos, new CacheEvictionPolicy(null, null, null, TimeSpan.FromMinutes(1), CacheTimeoutType.Absolute));
            }

            return videos;
        }
    }
}