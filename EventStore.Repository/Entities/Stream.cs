using System;

namespace EventStore.Repository.Entities
{
    public class Stream
    {
        public Guid StreamId { get; set; }

        public int Version { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
