using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace RssMixxxer
{
    public static class SyndicationFeedExtensions
    {
        /// <summary>
        /// Returns feed content as UTF8-encoded string
        /// </summary>
        public static string GetRssString(this SyndicationFeed @this)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream))
                {
                    @this.SaveAsRss20(writer);
                }

                var bytes = memoryStream.ToArray();

                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}