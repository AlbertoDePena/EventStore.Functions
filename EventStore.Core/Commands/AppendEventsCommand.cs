using System.Collections.Generic;
using System.Linq;

namespace EventStore.Core.Commands
{
    public class AppendEventsCommand
    {
        public int ExpectedVersion { get; }

        public IEnumerable<NewEventCommand> Events { get; }

        public string StreamName { get; }

        public AppendEventsCommand(string streamName, int expectedVersion, IEnumerable<NewEventCommand> events)
        {
            StreamName = streamName;
            ExpectedVersion = expectedVersion;
            Events = events ?? Enumerable.Empty<NewEventCommand>();
        }
    }
}
