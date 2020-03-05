using EventStore.Core.Commands;
using EventStore.Core.DTOs;
using EventStore.Core.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStore.Core.Contracts
{
    public interface IStreamService
    {
        Task AppendEventsAsync(AppendEventsCommand command);

        Task AddSnapshotAsync(AddSnapshotCommand command);

        Task DeleteSnapshotsAsync(DeleteSnapshotsCommand command);

        Task<IEnumerable<Stream>> GetAllStreamsAsync();

        Task<Stream> GetStreamAsync(StreamQuery query);

        Task<IEnumerable<Event>> GetEventsAsync(EventsQuery query);

        Task<IEnumerable<Snapshot>> GetSnapshotsAsync(SnapshotsQuery query);
    }
}