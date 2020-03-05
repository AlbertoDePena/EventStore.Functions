namespace EventStore.Core.Queries
{
    public class SnapshotsQuery
    {
        public string StreamName { get; }

        public SnapshotsQuery(string streamName)
        {
            StreamName = streamName;
        }
    }
}
