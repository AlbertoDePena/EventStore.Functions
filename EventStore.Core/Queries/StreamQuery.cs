namespace EventStore.Core.Queries
{
    public class StreamQuery
    {
        public string StreamName { get; }

        public StreamQuery(string streamName)
        {
            StreamName = streamName;
        }
    }
}
