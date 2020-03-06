using EventStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStore.Core.Contracts
{
    public interface IStreamService
    {
        Task AppendEventsAsync(AppendEvents model);

        Task AddSnapshotAsync(AddSnapshot model);

        Task DeleteSnapshotsAsync(QueryParameters model);

        Task<IEnumerable<Stream>> GetAllStreamsAsync();

        Task<Stream> GetStreamAsync(QueryParameters query);

        Task<IEnumerable<Event>> GetEventsAsync(QueryParameters query);

        Task<IEnumerable<Snapshot>> GetSnapshotsAsync(QueryParameters query);
    }
}