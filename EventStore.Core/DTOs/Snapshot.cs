using System;

namespace EventStore.Core.DTOs
{
    public class Snapshot
    {
        public int Version { get; set; }

        public string Data { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
