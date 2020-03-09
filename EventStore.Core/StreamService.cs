using EventStore.Core.Contracts;
using EventStore.Models;
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

        public async Task AddSnapshotAsync(AddSnapshot model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<AddSnapshot>().ValidateAndThrowAsync(model);

            var stream = await _unitOfWork.StreamRepository.GetStreamByNameAsync(model.StreamName);

            if (stream == null) throw new EntityNotFoundException($"Stream not found with name {model.StreamName}");

            var snapshot = new Repository.Entities.Snapshot
            {
                StreamId = stream.StreamId,
                Description = model.Description,
                Data = model.Data,
                Version = stream.Version
            };

            await _unitOfWork.StreamRepository.AddSnapshotAsync(snapshot);

            _unitOfWork.Commit();
        }

        public async Task AppendEventsAsync(AppendEvents model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<AppendEvents>().ValidateAndThrowAsync(model);

            var stream = await _unitOfWork.StreamRepository.GetStreamByNameAsync(model.StreamName);

            if (stream == null)
            {
                stream = new Repository.Entities.Stream
                {
                    Name = model.StreamName,
                    Version = 0
                };

                await _unitOfWork.StreamRepository.AddStreamAsync(stream);
            }

            if (stream.Version != model.ExpectedVersion)
                throw new ConcurrencyException($"Concurrency error - expected stream version to be {stream.Version} but got {model.ExpectedVersion}");

            var events =
                model.Events.Select(e => new Repository.Entities.Event
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

        public async Task DeleteSnapshotsAsync(QueryParameters model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<QueryParameters>().ValidateAndThrowAsync(model);

            await _unitOfWork.StreamRepository.DeleteSnapshotsAsync(model.StreamName);

            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Stream>> GetAllStreamsAsync()
        {
            var entities = await _unitOfWork.StreamRepository.GetAllStreamsAsync();

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(QueryParameters model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<QueryParameters>().ValidateAndThrowAsync(model);

            var entities = await _unitOfWork.StreamRepository.GetEventsAsync(model.StreamName, model.StartAtVersion.GetValueOrDefault());

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<IEnumerable<Snapshot>> GetSnapshotsAsync(QueryParameters model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<QueryParameters>().ValidateAndThrowAsync(model);

            var entities = await _unitOfWork.StreamRepository.GetSnapshotsAsync(model.StreamName);

            return entities.Select(Mapper.FromEntity);
        }

        public async Task<Stream> GetStreamAsync(QueryParameters model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _validatorFactory.GetValidator<QueryParameters>().ValidateAndThrowAsync(model);

            var entity = await _unitOfWork.StreamRepository.GetStreamByNameAsync(model.StreamName);

            if (entity == null)
            {
                return null;
            }

            return entity.FromEntity();
        }
    }
}