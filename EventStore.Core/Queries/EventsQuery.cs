namespace EventStore.Core.Queries
{
    public class EventsQuery
    {
        public string StreamName { get; }

        public int StartAtVersion { get; }

        public EventsQuery(string streamName, int startAtVersion)
        {
            StreamName = streamName;
            StartAtVersion = startAtVersion;
        }
    }
}
