using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Geta.VippyWrapper.Helpers;
using Geta.VippyWrapper.Requests;
using Geta.VippyWrapper.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Geta.VippyWrapper
{
    /// <summary>
    /// Wrapper for Vippy REST API 
    /// For more information see: http://vippy.co/developers/rest
    /// </summary>
    public class VippyWrapper
    {
        private readonly string _apiKey;
        private readonly string _secretKey;

        /// <summary>
        /// Instantiating of wrapper. We recommend that you use an IoC of some so you don't have to instansiate a new instance of this wrapper all the time.
        /// </summary>
        /// <param name="apiKey">Api key for Vippy</param>
        /// <param name="secretKey">Secret key for Vippy</param>
        public VippyWrapper(string apiKey, string secretKey)
        {
            this._apiKey = apiKey;
            this._secretKey = secretKey;
        }

        /// <summary>
        /// Used to get all the tags from this Vippy archive.
        /// Vippy endpoint: GET http://rest.vippy.co/archivetags
        /// </summary>
        /// <param name="archiveId">Login to vippy.co, tools -> Archives, at the bottom you have the archive number.</param>
        /// <returns>Collection with all tags <see cref="Tag"/></returns>
        public async Task<IEnumerable<Tag>> GetTags(string archiveId)
        {
            var client = this.GetHttpClient();

            var response = await client.GetAsync(string.Format("/archivetags?archive={0}", HttpUtility.UrlEncode(archiveId))).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return VippyDeserializer.Deserialize<ArchiveTag>(responseContent);
        }

        /// <summary>
        /// Used to remove video from Vippy.
        /// Vippy endpoint: DELETE http://rest.vippy.co/video
        /// </summary>
        /// <param name="videoId">Video id</param>
        public async void DeleteVideo(string videoId)
        {
            var client = this.GetHttpClient();

            var response = await client.DeleteAsync(string.Format("/video?videoId={0}", videoId)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        /// Used to get HTML code for embedding
        /// Vippy endpoint: GET http://rest.vippy.co/embedvideo
        /// </summary>
        /// <param name="getEmbedCodeRequest"><see cref="GetEmbedCodeRequest"/></param>
        /// <returns>HTML string with embed code</returns>
        public async Task<IHtmlString> GetEmbedCode(GetEmbedCodeRequest getEmbedCodeRequest)
        {
            var client = this.GetHttpClient();

            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["videoId"] = getEmbedCodeRequest.VideoId;

            if (!string.IsNullOrWhiteSpace(getEmbedCodeRequest.PlayerId))
            {
                queryString["playerId"] = getEmbedCodeRequest.PlayerId;
            }

            if (!string.IsNullOrWhiteSpace(getEmbedCodeRequest.Size))
            {
                queryString["size"] = getEmbedCodeRequest.Size;
            }

            queryString["embedcode"] = ToVippyBool(getEmbedCodeRequest.ShowEmbedCode);
            queryString["facebook"] = ToVippyBool(getEmbedCodeRequest.EnableFacebookSharing);
            queryString["twitter"] = ToVippyBool(getEmbedCodeRequest.EnableTwitterSharing);
            queryString["linkedin"] = ToVippyBool(getEmbedCodeRequest.EnableLinkedInSharing);

            if (!string.IsNullOrWhiteSpace(getEmbedCodeRequest.LogoId))
            {
                queryString["logo"] = getEmbedCodeRequest.LogoId;
            }

            var response = await client.GetAsync("/embedvideo?" + queryString).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HtmlString(HttpUtility.HtmlDecode(VippyDeserializer.DeserializeItem<string>(responseContent)));
        }

        // TODO: refactor and check how it should play together with different players
        private static string ToVippyBool(bool condition)
        {
            return condition ? "1" : "0";
        }

        /// <summary>
        /// Used to get logo from Vippy
        /// Vippy endpoint: GET http://rest.vippy.co/logo
        /// </summary>
        /// <param name="logoId">Logo id</param>
        /// <returns>Logo <see cref="Logo"/></returns>
        public async Task<Logo> GetLogo(string logoId)
        {
            var client = this.GetHttpClient();

            var response = await client.GetAsync(string.Format("/logo?logoId={0}", HttpUtility.UrlEncode(logoId))).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return VippyDeserializer.Deserialize<Logo>(responseContent).FirstOrDefault();
        }

        /// <summary>
        /// Used to get all the logos from Vippy.
        /// Vippy endpoint: GET http://rest.vippy.co/logos
        /// </summary>
        /// <returns>Collection with all logos <see cref="Logo"/></returns>
        public async Task<IEnumerable<Logo>> GetLogos()
        {
            var client = this.GetHttpClient();

            var response = await client.GetAsync("/logos").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return VippyDeserializer.Deserialize<Logo>(responseContent);
        }

        /// <summary>
        /// Used to get all the players from Vippy.
        /// Vippy endpoint: GET http://rest.vippy.co/players
        /// </summary>
        /// <returns>Collection with all players <see cref="Player"/></returns>
        public async Task<IEnumerable<Player>> GetPlayers()
        {
            var client = this.GetHttpClient();

            var response = await client.GetAsync("/players").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return VippyDeserializer.Deserialize<Player>(responseContent);
        }

        /// <summary>
        /// Used to get video from Vippy.
        /// Vippy endpoint: GET http://rest.vippy.co/video
        /// </summary>
        /// <param name="videoId">Video id</param>
        /// <param name="withStatistics">Use true to get information about plays, views ..., when false default values returned</param>
        /// <returns>Video <see cref="Video"/></returns>
        public async Task<Video> GetVideo(string videoId, bool withStatistics = false)
        {
            HttpClient client = this.GetHttpClient();
            var response = await client.GetAsync(string.Format("/video?videoId={0}&statistics={1}", HttpUtility.UrlEncode(videoId), withStatistics ? "1" : "0")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return VippyDeserializer.Deserialize<Video>(responseContent).FirstOrDefault();
        }

        /// <summary>
        /// Used to get all the thumbnails for provided videos.
        /// Vippy endpoint: GET  http://rest.vippy.co/videothumbnails
        /// </summary>
        /// <param name="videoIds">Array of video ids</param>
        /// <returns>Collection with all thumbnails <see cref="Thumbnail"/></returns>
        public async Task<IEnumerable<Thumbnail>> GetVideoThumbnails(string[] videoIds)
        {
            HttpClient client = this.GetHttpClient();

            var queryString = new StringBuilder();

            for (int i = 0; i < videoIds.Length; i++)
            {
                queryString.AppendFormat("&videoId%5B{0}%5D={1}", i, videoIds[i]);
            }

            var requestString = string.Format("/videothumbnails?{0}", queryString.ToString().TrimStart('&'));
            var response = await client.GetAsync(requestString).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return VippyDeserializer.Deserialize<Thumbnail>(responseContent);
        }

        /// <summary>
        /// Used to get all the video from Vippy.
        /// Vippy endpoint: GET http://rest.vippy.co/videos
        /// </summary>
        /// <param name="withStatistics">Use true to get information about plays, views ..., when false default values returned</param>
        /// <returns>Collection with all videos <see cref="Video"/></returns>
        public async Task<IEnumerable<Video>> GetVideos(bool withStatistics = false)
        {
            HttpClient client = this.GetHttpClient();
            var response = await client.GetAsync(string.Format("/videos?statistics={0}", withStatistics ? "1" : "0")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return VippyDeserializer.Deserialize<Video>(responseContent);
        }

        /// <summary>
        /// Used to update information of existing video.
        /// Vippy endpoint: POST http://rest.vippy.co/video
        /// </summary>
        /// <param name="postVideoRequest"><see cref="PostVideoRequest"/></param>
        public async void PostVideo(PostVideoRequest postVideoRequest)
        {
            var client = this.GetHttpClient();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(postVideoRequest, settings), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/video", httpContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Used to upload new video.
        /// Vippy endpoint: PUT http://rest.vippy.co/video
        /// </summary>
        /// <param name="newVideoRequest"><see cref="NewVideoRequest"/></param>
        /// <returns>Video <see cref="Video"/></returns>
        // TODO: create strongly typed return type 
        public async Task<dynamic> PutVideo(NewVideoRequest newVideoRequest)
        {
            var client = this.GetHttpClient();

            var dataContent = new ByteArrayContent(newVideoRequest.Data);
            dataContent.Headers.ContentType = MediaTypeHeaderValue.Parse(newVideoRequest.ContentType);
            client.DefaultRequestHeaders.Add("x-vpy-video", newVideoRequest.VideoPath);
            client.DefaultRequestHeaders.Add("x-vpy-title", newVideoRequest.Title);
            client.DefaultRequestHeaders.Authorization = this.GenerateAuthorizationHeader(client, dataContent.Headers);

            var response = await client.PutAsync("/video", dataContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        /// <summary>
        /// Used to get presentation from Vippy
        /// Vippy endpoint: GET http://rest.vippy.co/presentation
        /// </summary>
        /// <param name="presentationId">Presentation id</param>
        /// <returns>Presentation</returns>
        internal async Task<dynamic> GetPresentation(string presentationId)
        {
            var client = this.GetHttpClient();

            var response = await client.GetAsync(string.Format("/presentation?presentation={0}", HttpUtility.UrlEncode(presentationId))).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        /// <summary>
        /// Used to get all the presentations from Vippy
        /// Vippy endpoint: GET http://rest.vippy.co/presentations
        /// </summary>
        /// <returns>Collection of presentations</returns>
        internal async Task<dynamic> GetPresentations()
        {
            HttpClient client = this.GetHttpClient();
            var response = await client.GetAsync("/presentations").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        private HttpClient GetHttpClient()
        {
            var baseUrl = new Uri("http://rest.vippy.co");

            var client = new HttpClient { BaseAddress = baseUrl };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Date = DateTime.UtcNow;
            client.DefaultRequestHeaders.Add("x-vpy-version", "1.1");

            client.DefaultRequestHeaders.Authorization = this.GenerateAuthorizationHeader(client);

            return client;
        }

        private AuthenticationHeaderValue GenerateAuthorizationHeader(HttpClient httpClient, HttpContentHeaders extraContentHeaders = null)
        {
            var signature = new StringBuilder();

            var headers = httpClient.DefaultRequestHeaders.ToDictionary(requestHeader => requestHeader.Key, requestHeader => requestHeader.Value);

            if (extraContentHeaders != null)
            {
                foreach (var contentHeader in extraContentHeaders)
                {
                    headers.Add(contentHeader.Key, contentHeader.Value);
                }
            }

            foreach (var httpRequestHeader in headers.OrderBy(header => header.Key))
            {
                if (httpRequestHeader.Key == "Accept" || httpRequestHeader.Key == "Authorization")
                {
                    continue;
                }

                foreach (string value in httpRequestHeader.Value)
                {
                    signature.Append(value.ToLowerInvariant().Replace(" ", string.Empty));
                }
            }

            return new AuthenticationHeaderValue("Vippy", string.Format("{0}:{1}", this._apiKey, GetHashedSignature(this._secretKey, signature.ToString())));
        }

        private static string GetHashedSignature(string apiSecretKey, string signature)
        {
            var hmacshai1 = new HMACSHA1(Encoding.UTF8.GetBytes(apiSecretKey), true);
            byte[] hashedSignature = hmacshai1.ComputeHash(Encoding.UTF8.GetBytes(signature));
            return Convert.ToBase64String(hashedSignature);
        }
    }
}