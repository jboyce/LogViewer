using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace LogViewer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var fileSystem = new PhysicalFileSystem("..\\..\\Site");
            var options = new FileServerOptions
            {
                FileSystem = fileSystem
            };
            app.UseFileServer(options);

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            app.UseWebApi(config);
        }
    }
}
