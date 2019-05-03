using System;
using System.Threading;
using System.Threading.Tasks;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public class WrappedTask<TResult, TContext> : IDisposable, IWrappedTask
    {
        public Task<TResult> Task;
        public CancellationTokenSource CancelSource;
        public TContext Context;

        public void Dispose()
        {
            Task?.Dispose();
            CancelSource?.Dispose();
        }

        public bool IsDone()
        {
            return Task.IsCompleted;
        }

        public void Cancel()
        {
            CancelSource.Cancel();
        }

        public void Wait()
        {
            Task.Wait();
        }
    }

    public interface IWrappedTask
    {
        bool IsDone();
        void Cancel();
        void Wait();
    }
}
