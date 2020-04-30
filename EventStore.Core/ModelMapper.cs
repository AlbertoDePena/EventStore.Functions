using EventStore.Models;

namespace EventStore.Core
{
    public static class ModelMapper
    {
        public static Stream FromEntity(this Repository.Entities.Stream entity)
            => new Stream { Name = entity.Name, Version = entity.Version, CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt };

        public static Event FromEntity(this Repository.Entities.Event entity)
            => new Event { Type = entity.Type, Data = entity.Data, Version = entity.Version, CreatedAt = entity.CreatedAt };

        public static Snapshot FromEntity(this Repository.Entities.Snapshot entity)
            => new Snapshot { Data = entity.Data, Version = entity.Version, Description = entity.Description, CreatedAt = entity.CreatedAt };
    }
}
