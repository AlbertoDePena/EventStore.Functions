using EventStore.Core.Commands;
using EventStore.Core.Contracts;
using EventStore.Core.DTOs;
using EventStore.Core.Queries;
using EventStore.Repository.Contracts;
using FluentValidation;
using Numaka.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventStore.Core
{
    public class StreamService : IStreamService
    {
        private readonly Contracts.IValidatorFactory _validatorFactory;
        private readonly IUnitOfWork _unitOfWork;

        public StreamService(IUnitOfWork unitOfWork, Contracts.IValidatorFactory validatorFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
        }

        public async Task AddSnapshotAsync(AddSnapshotCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            await _validatorFactory.GetValidator<AddSnapshotCommand>().ValidateAndThrowAsync(command);

            var stream = await _unitOfWork.StreamRepository.GetStreamByNameAsync(command.StreamName);

            if (stream == null) throw new KeyNotFoundException($"Stream not found with name {command.StreamName}");

            var snapshot = new Repository.Entities.Snapshot
            {
                StreamId = stream.StreamId,
                Description = command.Description,
                Data = command.Data,
                Version = stream.Version
            };

            await _unitOfWork.StreamRepository.AddSnapshotAsync(snapshot);

            _unitOfWork.Commit();
        }

        public async Task AppendEventsAsync(AppendEventsCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            await _validatorFactory.GetValidator<AppendEventsCommand>().ValidateAndThrowAsync(command);

            var stream = await _unitOfWork.StreamRepository.GetStreamByNameAsync(command.StreamName);

            if (stream == null)
            {
                stream = new Repository.Entities.Stream
                {
                    Name = command.StreamName,
                    Version = 0
                };

                await _unitOfWork.StreamRepository.AddStreamAsync(stream);
            }

            if (stream.Version != command.ExpectedVersion)
                throw new ConcurrencyException($"Concurrency error - expected stream version to be {stream.Version} but got {command.ExpectedVersion}");

            var events =
                command.Events.Select(e => new Repository.Entities.Event
                {
                    StreamId = stream.StreamId,
                    Type = e.Type,
                    Data = e.Data,
                    Version = ++stream.Version
                }).ToList();

            await _unitOfWork.StreamRepository.AddEventsAsync(events);
            await _unitOfWork.StreamRepository.UpdateStreamAsync(stream);

            _unitOfWork.Commit();
        }

        public async Task DeleteSnapshotsAsync(DeleteSnapshotsCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            await _validatorFactory.GetValidator<DeleteSnapshotsCommand>().ValidateAndThrowAsync(command);

            await _unitOfWork.StreamRepository.DeleteSnapshotsAsync(command.StreamName);

            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Stream>> GetAllStreamsAsync()
        {
            var entities = await _unitOfWork.StreamRepository.GetAllStreamsAsync();

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(EventsQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            await _validatorFactory.GetValidator<EventsQuery>().ValidateAndThrowAsync(query);

            var entities = await _unitOfWork.StreamRepository.GetEventsAsync(query.StreamName, query.StartAtVersion);

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<IEnumerable<Snapshot>> GetSnapshotsAsync(SnapshotsQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            await _validatorFactory.GetValidator<SnapshotsQuery>().ValidateAndThrowAsync(query);

            var entities = await _unitOfWork.StreamRepository.GetSnapshotsAsync(query.StreamName);

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<Stream> GetStreamAsync(StreamQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            await _validatorFactory.GetValidator<StreamQuery>().ValidateAndThrowAsync(query);

            var entity = await _unitOfWork.StreamRepository.GetStreamByNameAsync(query.StreamName);

            if (entity == null)
            {
                return null;
            }

            return entity.FromEntity();
        }
    }
}