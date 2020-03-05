using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventStore.Repository.Contracts;
using EventStore.Repository.Entities;
using Numaka.Common.Contracts;
using Numaka.Common.Exceptions;

namespace EventStore.Repository
{
    public class StreamRepository : IStreamRepository
    {
        private readonly Func<IDbConnection> _dbConnectionFunc;
        private readonly Func<IDbTransaction> _dbTransactionFunc;
        private readonly IGuidFactory _guidFactory;

        public StreamRepository(Func<IDbConnection> dbConnectionFunc, Func<IDbTransaction> dbTransactionFunc, IGuidFactory guidFactory)
        {
            _dbConnectionFunc = dbConnectionFunc ?? throw new ArgumentNullException(nameof(dbConnectionFunc));
            _dbTransactionFunc = dbTransactionFunc ?? throw new ArgumentNullException(nameof(dbTransactionFunc));
            _guidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
        }

        public async Task AddEventsAsync(IEnumerable<Event> models)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));

            try
            {
                var transaction = _dbTransactionFunc();

                var param = models.Select(x => new { EventId = _guidFactory.NewSqlServerGuid(), x.StreamId, x.Type, x.Data, x.Version });

                await _dbConnectionFunc().ExecuteAsync("dbo.AddEvent", param, transaction, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task AddSnapshotAsync(Snapshot model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var param = new { SnapshotId = _guidFactory.NewSqlServerGuid(), model.StreamId, model.Description, model.Data, model.Version };

                await _dbConnectionFunc().ExecuteAsync("dbo.AddSnapshot", param, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task AddStreamAsync(Stream model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var transaction = _dbTransactionFunc();

                var param = new { StreamId = _guidFactory.NewSqlServerGuid(), model.Name, model.Version };

                await _dbConnectionFunc().ExecuteAsync("dbo.AddStream", param, transaction, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task DeleteSnapshotsAsync(string streamName)
        {
            if (string.IsNullOrWhiteSpace(streamName)) throw new ArgumentNullException(nameof(streamName));

            try
            {
                await _dbConnectionFunc().ExecuteAsync("dbo.DeleteSnapshots", new { StreamName = streamName }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task<IEnumerable<Stream>> GetAllStreamsAsync()
        {
            try
            {
                return await _dbConnectionFunc().QueryAsync<Stream>("dbo.GetAllStreams", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(string streamName, int startAtVersion)
        {
            if (string.IsNullOrWhiteSpace(streamName)) throw new ArgumentNullException(nameof(streamName));

            try
            {
                return await _dbConnectionFunc().QueryAsync<Event>("dbo.GetEvents", new { StreamName = streamName, StartAtVersion = startAtVersion }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task<IEnumerable<Snapshot>> GetSnapshotsAsync(string streamName)
        {
            if (string.IsNullOrWhiteSpace(streamName)) throw new ArgumentNullException(nameof(streamName));

            try
            {
                return await _dbConnectionFunc().QueryAsync<Snapshot>("dbo.GetSnapshots", new { StreamName = streamName }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task<Stream> GetStreamByNameAsync(string streamName)
        {
            if (string.IsNullOrWhiteSpace(streamName)) throw new ArgumentNullException(nameof(streamName));

            try
            {
                return await _dbConnectionFunc().QuerySingleOrDefaultAsync<Stream>("dbo.GetStreamByName", new { StreamName = streamName }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }

        public async Task UpdateStreamAsync(Stream model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var transaction = _dbTransactionFunc();

                await _dbConnectionFunc().ExecuteAsync("dbo.UpdateStream", new { model.StreamId, model.Version }, transaction, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException e)
            {
                throw new RepositoryException(e.Message, e);
            }
        }
    }
}