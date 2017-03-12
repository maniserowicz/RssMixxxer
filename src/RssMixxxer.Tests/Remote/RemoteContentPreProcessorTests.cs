using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using RssMixxxer.Remote;
using Xunit;

namespace RssMixxxer.Tests.Remote
{
    public class RemoteContentPreProcessorTests
    {
        RemoteContentPreProcessor _preprocessor;
        string _content;

        const string VALID_RSS = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
    <channel>
        <item>
        </item>
    </channel>
</rss>";

        public RemoteContentPreProcessorTests()
        {
            _preprocessor = new RemoteContentPreProcessor();
        }

        string execute()
        {
            return _preprocessor.PreProcess(_content);
        }

        [Fact]
        public void trims_content()
        {
            _content = $@"  

{VALID_RSS}
";

            var result = execute();

            assert_can_be_loaded_as_SyndicationFeed(result);
        }

        [Fact]
        public void removes_nonbreaking_spaces()
        {
            string invalid_rss = VALID_RSS.Replace("</item>", "</item>" + (char) 160);

            _content = invalid_rss;

            var result = execute();

            assert_can_be_loaded_as_SyndicationFeed(result);
        }

        void assert_can_be_loaded_as_SyndicationFeed(string content)
        {
            Exception error = null;

            using (var stringReader = new StringReader(content))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    try
                    {
                        var feed = SyndicationFeed.Load(reader);
                    }
                    catch(Exception exc)
                    {
                        error = exc;
                    }
                }
            }

            Assert.Null(error);
        }
    }
}