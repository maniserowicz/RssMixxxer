using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RssMixxxer.Tests
{
    public static class Resource
    {
        public static string ExtractString(string name)
        {
            string fullName = "RssMixxxer.Tests._testdata." + name;

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(fullName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static IEnumerable<string> read_feeds()
        {
            yield return ExtractString("feed1.xml");
            yield return ExtractString("feed2.xml");
        }
    }
}