using System;

namespace EventStore.Core.DTOs
{
    public class Event
    {
        public int Version { get; set; }

        public string Type { get; set; }

        public string Data { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
