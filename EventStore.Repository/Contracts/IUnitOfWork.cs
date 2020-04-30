using System;
using System.Threading.Tasks;

namespace EventStore.Repository.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IStreamRepository StreamRepository { get; }

        /// <summary>
        /// Execute an async task and commit any changes.
        /// Undo any changes if the task throws an exception.
        /// </summary>
        /// <param name="task"></param>
        Task CommitOrUndoAsync(Func<Task> task);
    }
}