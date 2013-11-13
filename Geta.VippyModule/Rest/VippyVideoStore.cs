using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using EPiServer.Shell.Services.Rest;
using Geta.VippyWrapper.Responses;

namespace Geta.VippyModule.Rest
{
    [RestStore("vippyvideo")]
    public class VippyVideoStore : RestControllerBase
    {
        private async Task<IEnumerable<Video>> GetVideos()
        {
            string cacheKey = "vippyvideos";

            var videos = HttpRuntime.Cache.Get(cacheKey) as List<Video>;

            if (videos == null)
            {
                var wrapper = new VippyWrapper.VippyWrapper(VippyConfiguration.ApiKey,
                    VippyConfiguration.SecretKey);

                videos = (await wrapper.GetVideos()).ToList();

                HttpRuntime.Cache.Insert(cacheKey, videos, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
            }

            return videos;
        }

        public async Task<RestResult> Get(string name)
        {
            IEnumerable<Video> matches;
            if (string.IsNullOrEmpty(name) || string.Equals(name, "*", StringComparison.OrdinalIgnoreCase))
            {
                matches = await GetVideos();
            }
            else
            {
                //Remove * at the end of name                
                name = name.Substring(0, name.Length - 1);
                matches = (await GetVideos()).Where(e => e.Title.StartsWith(name, StringComparison.OrdinalIgnoreCase));
            }

            var result = matches
                .Take(10)
                .Select(m => new { Name = m.Title, Id = m.VideoId })
                .ToList();


            result.Insert(0, new { Name = string.Empty, Id = string.Empty });

            return Rest(result);
        }
    }
}