using EventStore.Repository.Contracts;
using Numaka.Common.Contracts;
using Numaka.Common.Data;

namespace EventStore.Repository
{
    public class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        private IStreamRepository _streamRepository;

        public UnitOfWork(IDbConnectionFactory dbConnectionFactory, IGuidFactory guidFactory) : base(dbConnectionFactory, guidFactory) { }

        public IStreamRepository StreamRepository
        {
            get
            {
                return _streamRepository ?? (_streamRepository = new StreamRepository(GetDbConnection, GetDbTransaction, GuidFactory));
            }
        }
    }
}