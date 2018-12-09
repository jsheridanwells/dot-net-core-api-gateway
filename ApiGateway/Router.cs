using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace ApiGateway
{
    public class Router
    {
        public List<Route> Routes { get; set; }
        public Destination AuthenticationService { get; set; }

        public Router(string routeConfigPath)
        {
            dynamic router = JsonLoader.LoadFromFile<dynamic>(routeConfigPath);
            Routes = JsonLoader.Deserialize<List<Route>>(Convert.ToString(router.routes));
            AuthenticationService = JsonLoader.Deserialize<Destination>(Convert.ToString(router.AuthenticationService));
        }

        public async Task<HttpResponseMessage> RouteRequest(HttpRequest req)
        {
            string path = req.Path.ToString();
            string basePath = '/' + path.Split('/')[1];
            Destination destination;

            try
            {
                destination = Routes.First(r => r.Endpoint.Equals(basePath)).Destination;
            }
            catch
            {
                return ConstructErrorMessage("The path could not be found.");
            }

            if (destination.AuthRequired)
            {
                string token = req.Headers["token"];
                req.Query.Append(new KeyValuePair<string, StringValues>("token", new StringValues(token)));
                HttpResponseMessage authResponse = await AuthenticationService.SendRequest(req);
                if (!authResponse.IsSuccessStatusCode) 
                    return ConstructErrorMessage("Authentication failed.    ");
            }

            return await destination.SendRequest(req);
        }

        private HttpResponseMessage ConstructErrorMessage(string error)
        {
            HttpResponseMessage message = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(error)
            };
            return message;
        }
    }
}
