using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace RssMixxxer
{
    public static class SyndicationFeedExtensions
    {
        public static string GetRssString(this SyndicationFeed @this)
        {
            var stringBuilder = new StringBuilder();

            using (var writer = XmlWriter.Create(stringBuilder))
            {
                @this.SaveAsRss20(writer);
            }

            return stringBuilder.ToString();
        }
    }
}