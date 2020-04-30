using EventStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStore.Core.Contracts
{
    public interface IStreamService
    {
        Task AppendEventsAsync(AppendEvents model);

        Task AddSnapshotAsync(AddSnapshot model);

        Task DeleteSnapshotsAsync(string streamName);

        Task<IEnumerable<Stream>> GetAllStreamsAsync();

        Task<Stream> GetStreamAsync(string streamName);

        Task<IEnumerable<Event>> GetEventsAsync(string streamName, int? startAtVersion);

        Task<IEnumerable<Snapshot>> GetSnapshotsAsync(string streamName);
    }
}