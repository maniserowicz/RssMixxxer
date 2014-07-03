using System.Net;

namespace RssMixxxer.Remote
{
    public interface IHttpRequestFactory
    {
        HttpWebRequest CreateRequest(string uri);
    }

    public class HttpRequestFactory : IHttpRequestFactory
    {
        private const string USER_AGENT_STRING = "rssmixxxer/user_agent";

        public HttpWebRequest CreateRequest(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.UserAgent = USER_AGENT_STRING;

            return request;
        }
    }
}