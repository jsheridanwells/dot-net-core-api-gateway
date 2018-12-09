using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiGateway
{
    public class Destination
    {
        public string Uri { get; set; }
        public bool AuthRequired { get; set; }
        static HttpClient client = new HttpClient();

        public Destination(string uri, bool authRequired)
        {
            Uri = uri;
            AuthRequired = authRequired;
        }

        public Destination(string path) : this(path, false) {}

        private Destination()
        {
            Uri = "/";
            AuthRequired = false;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequest req)
        {
            string requestContent;
            using (Stream receiveStream = req.Body)
            {
                using(StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestContent = readStream.ReadToEnd();
                }
            }

            using(var newRequest = new HttpRequestMessage(new HttpMethod(req.Method), CreateDestinationUri(req)))
            {
                newRequest.Content = new StringContent(requestContent, Encoding.UTF8, req.ContentType);
                using( var response = await client.SendAsync(newRequest))
                {
                    return response;
                }
            }
        }

        private string CreateDestinationUri(HttpRequest req)
        {
            string requestPath = req.Path.ToString();
            string queryString = req.QueryString.ToString();

            string endpoint = "";
            string[] endpointSplit = requestPath.Substring(1).Split('/');

            if (endpointSplit.Length > 1)
                endpoint = endpointSplit[1];
            
            return Uri + endpoint + queryString;
        }
    }
}
