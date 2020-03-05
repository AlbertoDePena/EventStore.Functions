using System;

namespace EventStore.Repository.Entities
{
    public class Event
    {
        public Guid EventId { get; set; }

        public Guid StreamId { get; set; }

        public int Version { get; set; }

        public string Type { get; set; }

        public string Data { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
