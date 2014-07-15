using Simple.Data;

namespace RssMixxxer.LocalCache
{
    public interface ILocalFeedsProvider
    {
        dynamic Db();
    }

    public class LocalFeedsProvider : ILocalFeedsProvider
    {
         public dynamic Db()
         {
             return Database.OpenNamedConnection("feed_cache");
         }
    }
}