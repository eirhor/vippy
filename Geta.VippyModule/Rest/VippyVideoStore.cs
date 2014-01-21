using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using EPiServer.Shell.Services.Rest;
using Geta.VippyWrapper.Responses;

namespace Geta.VippyModule.Rest
{
    [RestStore("vippyvideo")]
    public class VippyVideoStore : RestControllerBase
    {
        public RestResult Get(string name)
        {
            var id = GetId(name);
            if (!string.IsNullOrEmpty(id))
            {
                return RestVideoById(id);
            }

            var matches = GetVideos();
            if (IsNotEmpty(name))
            {
                matches = FilterByName(name, matches);
            }

            return RestVideos(matches);
        }

        private RestResult RestVideos(IEnumerable<Video> matches)
        {
            var result = matches
                .Take(10)
                .Select(m => new {Name = m.Title, Id = m.VideoId})
                .ToList();
           
            result.Add(new {Name = string.Empty, Id = string.Empty});

            return Rest(result);
        }

        private static IEnumerable<Video> FilterByName(string name, IEnumerable<Video> matches)
        {
            //Remove * at the end of name                
            var n = name.Substring(0, name.Length - 1);
            return matches.Where(e => e.Title.StartsWith(n, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsNotEmpty(string name)
        {
            return !string.IsNullOrEmpty(name) && !string.Equals(name, "*", StringComparison.OrdinalIgnoreCase);
        }

        private RestResult RestVideoById(string id)
        {
            var videos = GetVideos();
            var video = videos.FirstOrDefault(x => x.VideoId == id);
            if (video != null)
            {
                return Rest(new
                {
                    Name = video.Title,
                    Id = video.VideoId
                });
            }

            return Rest(new {Name = string.Empty, Id = string.Empty});
        }

        private string GetId(string name)
        {
            if (string.IsNullOrWhiteSpace(name) && RouteData.Values.Count > 0)
            {
                return RouteData.Values.First().Value as string;
            }

            return string.Empty;
        }

        private IEnumerable<Video> GetVideos()
        {
            string cacheKey = "vippyvideos";

            var videos = HttpRuntime.Cache.Get(cacheKey) as List<Video>;

            if (videos == null)
            {
                var wrapper = new VippyWrapper.VippyWrapper(VippyConfiguration.ApiKey,
                    VippyConfiguration.SecretKey);

                videos = (wrapper.GetVideos().Result).ToList();

                HttpRuntime.Cache.Insert(cacheKey, videos, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
            }

            return videos;
        }
    }
}