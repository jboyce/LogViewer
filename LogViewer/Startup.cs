using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace LogViewer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //var fileSystem = new PhysicalFileSystem("..\\..\\Site");
            var fileSystem = new EmbeddedResourceFileSystem("LogViewer.Site");
            var options = new FileServerOptions
            {
                FileSystem = fileSystem
            };
            app.UseFileServer(options);

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            app.UseWebApi(config);
        }
    }
}
