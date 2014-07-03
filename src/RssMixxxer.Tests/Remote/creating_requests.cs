using RssMixxxer.Remote;
using Xunit;

namespace RssMixxxer.Tests.Remote
{
    public class creating_requests
    {
        private HttpRequestFactory _httpRequestFactory;

        public creating_requests()
        {
            _httpRequestFactory = new HttpRequestFactory();
        }

        [Fact]
        public void sets_user_agent_string_on_new_request___some_servers_return_Forbidden_when_user_agent_string_missing()
        {
            var request = _httpRequestFactory.CreateRequest("http://some-url.com");

            Assert.NotEmpty(request.UserAgent);

            Assert.Equal("rssmixxxer/user_agent", request.UserAgent);
        }
    }
}