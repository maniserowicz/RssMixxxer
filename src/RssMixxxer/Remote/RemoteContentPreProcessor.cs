namespace RssMixxxer.Remote
{
    public interface IRemoteContentPreProcessor
    {
        string PreProcess(string content);
    }

    public class RemoteContentPreProcessor : IRemoteContentPreProcessor
    {
        public string PreProcess(string content)
        {
            string nbsp = ((char)160).ToString();

            return content
                .Trim()
                .Replace(nbsp, string.Empty)
            ;
        }
    }
}