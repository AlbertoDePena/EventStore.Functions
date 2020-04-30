using EventStore.Repository.Contracts;
using Numaka.Common.Contracts;
using Numaka.Common;
using System;
using System.Threading.Tasks;

namespace EventStore.Repository
{
    public class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        private IStreamRepository _streamRepository;
        private readonly ISqlServerGuidFactory _guidFactory;

        public UnitOfWork(IDbConnectionFactory dbConnectionFactory, ISqlServerGuidFactory guidFactory) : base(dbConnectionFactory) 
        {
            _guidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
        }

        public IStreamRepository StreamRepository
        {
            get
            {
                return _streamRepository ?? (_streamRepository = new StreamRepository(GetDbConnection, GetDbTransaction, _guidFactory));
            }
        }

        /// <inheritdoc/>
        public async Task CommitOrUndoAsync(Func<Task> task)
        {
            try
            {
                await task();
                Commit();
            }
            catch
            {
                Undo();
                throw;
            }
        }
    }
}