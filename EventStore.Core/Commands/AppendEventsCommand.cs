namespace EventStore.Core.Commands
{
    public class AppendEventsCommand
    {
        public int ExpectedVersion { get; }

        public NewEventCommand[] Events { get; }

        public string StreamName { get; }

        public AppendEventsCommand(string streamName, int expectedVersion, NewEventCommand[] events)
        {
            StreamName = streamName;
            ExpectedVersion = expectedVersion;
            Events = events;
        }
    }
}
