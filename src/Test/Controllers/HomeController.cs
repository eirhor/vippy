using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Geta.VippyWrapper;
using Geta.VippyWrapper.Requests;
using Geta.VippyWrapper.Responses;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var vippyWrapper = new VippyWrapper(apiKey: "{apiKey}", secretKey: "{apiSecret}");

            //// Test
            //// Delete video
            //// post video
            //// put video
            //var newVideoRequest = new NewVideoRequest();

            //using (FileStream file = System.IO.File.OpenRead(@"C:\Projects\second.mp4"))
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        file.CopyTo(memoryStream);
            //        newVideoRequest.Data = memoryStream.ToArray();
            //        newVideoRequest.ContentLength = memoryStream.Length;
            //    }
            //}

            //newVideoRequest.Title = "second.mp4 title";
            //newVideoRequest.VideoPath = "second.mp4";
            //newVideoRequest.ContentType = "video/mp4";

            //var newVideoResponse = await vippyWrapper.PutVideo(newVideoRequest);

            IHtmlString embedCode = await vippyWrapper.GetEmbedCode(new GetEmbedCodeRequest()
            {
                VideoId = "9176"
            });

            Logo logo = await vippyWrapper.GetLogo(logoId: "50");

            IEnumerable<Logo> logos = await vippyWrapper.GetLogos();

            IEnumerable<Player> players = await vippyWrapper.GetPlayers();

            Video video = await vippyWrapper.GetVideo(videoId: "9176", withStatistics: true);

            var videoThumbnails = await vippyWrapper.GetVideoThumbnails(new[] { "9176", "9177" });

            IEnumerable<Video> videos = await vippyWrapper.GetVideos(true);

            // Login to vippy.co, tools -> Archives, at the bottom you have the archive number.
            IEnumerable<Tag> tags = await vippyWrapper.GetTags("9176");

            //var presentation = await vippyWrapper.GetPresentation("presentationId");

            //var presentations = await vippyWrapper.GetPresentations();

            return View();
        }
    }
}