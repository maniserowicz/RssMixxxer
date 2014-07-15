namespace RssMixxxer.Configuration
{
    public class FeedAggregatorConfig
    {
        public string Title { get; set; }
        public int MaxItems { get; set; }
        public string[] SourceFeeds { get; set; }
        public int SyncInterval_Seconds { get; set; }
        public bool PrefetchHeadRequest { get; set; }
    }
}