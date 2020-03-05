using EventStore.Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStore.Repository.Contracts
{
    public interface IStreamRepository
    {
        Task<IEnumerable<Stream>> GetAllStreamsAsync();

        Task<Stream> GetStreamByNameAsync(string streamName);

        Task<IEnumerable<Snapshot>> GetSnapshotsAsync(string streamName);

        Task<IEnumerable<Event>> GetEventsAsync(string streamName, int startAtVersion);

        Task DeleteSnapshotsAsync(string streamName);

        Task AddSnapshotAsync(Snapshot model);

        Task AddStreamAsync(Stream model);

        Task UpdateStreamAsync(Stream model);

        Task AddEventsAsync(IEnumerable<Event> models);
    }
}