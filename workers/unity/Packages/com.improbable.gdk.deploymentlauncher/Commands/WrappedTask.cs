using System;
using System.Threading;
using System.Threading.Tasks;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public class WrappedTask<TResult, TContext> : IDisposable
    {
        public Task<TResult> Task;
        public CancellationTokenSource CancelSource;
        public TContext Context;

        public void Dispose()
        {
            Task?.Dispose();
            CancelSource?.Dispose();
        }
    }
}
