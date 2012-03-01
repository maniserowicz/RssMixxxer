using System.Net;

namespace RssMixxxer.Remote
{
    public static class WebRequestExtensions
    {
        /// <summary>
        /// Does not throw exception when getting status other than OK
        /// </summary>
        /// <remarks>Particularly useful for NotModified response, which is used extensively in this project.</remarks>
        public static WebResponse GetResponseEx(this WebRequest @this)
        {
            try
            {
                return @this.GetResponse();
            }
            catch (WebException exc)
            {
                if (exc.Response != null)
                {
                    return exc.Response;
                }
                throw;
            }
        }
    }
}