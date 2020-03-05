using System;

namespace EventStore.Repository.Entities
{
    public class Snapshot
    {
        public Guid SnapshotId { get; set; }

        public Guid StreamId { get; set; }

        public int Version { get; set; }

        public string Data { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
