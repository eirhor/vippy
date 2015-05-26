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
        private readonly VippyWrapper.VippyWrapper _vippyWrapper;
        readonly ILog _logger = LogManager.GetLogger(typeof(VippyVideoSelectionQuery));

        public VippyVideoSelectionQuery(VippyWrapper.VippyWrapper vippyWrapper)
        {
            if (vippyWrapper == null) throw new ArgumentNullException("vippyWrapper");
            _vippyWrapper = vippyWrapper;
        }

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
            return _vippyWrapper.GetVideo(id, false).Result;
        }

        public IEnumerable<ISelectItem> GetItems(string query)
        {
            var matches = GetVideos();

            if (IsNotEmpty(query))
            {
                matches = FilterByName(query, matches);
            }

            var list = matches
                .Take(20)
                .Select(x => new SelectItem
                {
                    Text = x.Title,
                    Value = x.VideoId
                })
                .ToList();

            list.Insert(0, new SelectItem
            {
                Text = string.Empty,
                Value = string.Empty
            });

            return list;
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

            var videos = CacheManager.Get(cacheKey) as IEnumerable<Video>;

            if (videos == null)
            {
                videos = _vippyWrapper.GetVideos().Result;

                CacheManager.Insert(cacheKey, videos, new CacheEvictionPolicy(null, null, null, TimeSpan.FromMinutes(2), CacheTimeoutType.Absolute));
            }

            return videos;
        }
    }
}