using System;

namespace EventStore.Core.DTOs
{
    public class Stream
    {
        public int Version { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
