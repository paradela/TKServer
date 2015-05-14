using Owin;
using System.Web.Http;
using Owin.WebSocket.Extensions;
using System.Net.Http;

namespace TKServer
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            ); 
            
            appBuilder.UseWebApi(config);
            appBuilder.MapWebSocketRoute<TKWebSocket>("/ws");
            //appBuilder.MapWebSocketPattern<TKWebSocket>("ws/(?<key>[a-zA-Z0-9]+)")
        }
    }
}
