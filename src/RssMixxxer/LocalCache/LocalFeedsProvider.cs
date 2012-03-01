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
             return Database.OpenFile("local-feeds.sdf");
         }
    }
}