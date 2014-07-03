using System.Net;

namespace RssMixxxer.Remote
{
    public interface IHttpRequestFactory
    {
        HttpWebRequest CreateRequest(string uri);
    }

    public class HttpRequestFactory : IHttpRequestFactory
    {
        public HttpWebRequest CreateRequest(string uri)
        {
            return (HttpWebRequest) WebRequest.Create(uri);
        }
    }
}