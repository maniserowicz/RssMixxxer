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
            return content
                .Trim()
            ;
        }
    }
}