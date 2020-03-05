using System;

namespace EventStore.Repository.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IStreamRepository StreamRepository { get; }

        void Commit();
    }
}