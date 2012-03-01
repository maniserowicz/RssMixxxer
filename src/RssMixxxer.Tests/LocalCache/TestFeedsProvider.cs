using Simple.Data;
using RssMixxxer.LocalCache;

namespace RssMixxxer.Tests.LocalCache
{
    public class TestFeedsProvider : ILocalFeedsProvider
    {
        private dynamic _db;

        public dynamic Db()
        {
            var inMemoryAdapter = new InMemoryAdapter();
            inMemoryAdapter.SetKeyColumn("LocalFeedInfo", "Id");

            Database.UseMockAdapter(inMemoryAdapter);

            if (_db == null)
            {
                _db = Database.Open();
            }

            return _db;
        }
    }
}