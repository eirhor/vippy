# .NET wrapper for Vippy

![](http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_Vippy_VippyDebug)/statusIcon)

What is Vippy?
------------------------------
Vippy is a mediabank with support for videos, presentations and documents. For more information see: http://vippy.co.

What is vippy project?
------------------------------
This is a .NET wrapper around Vippy's public REST API. This wrapper contains all the public endpoints available at: http://vippy.co/developers/rest.

How to get started?
------------------------------
Start by installing NuGet package:

    Install-Package Geta.VippyWrapper
    
This will add one dll file that contains the wrapper client. To use it simply create a new instance of VippyWrapper. There you have all the methods available that Vippy's REST API has available. For example, getting all videos:

    var wrapper = new VippyWrapper("apikey", "secretkey");
    var allVideos = wrapper.GetVideos();

This will return a collection of all the videos available for this account. To get the API and Secret key login to http://vippy.co and under Account you will see your keys.

All the public methods of the VippyWrapper class are async that means you need to take this under consideration when using the wrapper.

Example of how this would look in an MVC controller's action method:

    public async Task<JsonResult> Index()
    {
      var wrapper = new VippyWrapper("apikey", "secretkey");
      var allVideos = await wrapper.GetVideos();
      return JsonContent(allVideos);
    }

Example of how to do uploading:

    var wrapper = new VippyWrapper("apikey", "secretkey");
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

    var response = wrapper.PutVideo(newVideoRequest);

Feedback
------------------------------
If you have any feedback, want to contribute or find any bugs feel free to contact us or use the issue tracker to create issues.

Source code
------------------------------
In the source code you have four projects. VippyWrapper is the main project for consuming the Vippy API. In addition we have two EPiServer specific projects that are still work in progress. These two EPiServer projects will allow editors to easily add, delete, and update videos from within EPiServer.

Thanks
------------------------------
Thanks to customer NHO for letting us open source this module.
