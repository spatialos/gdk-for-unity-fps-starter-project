using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Improbable.Gdk.Tools;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public static class Authentication
    {
        public static WrappedTask<RedirectedProcessResult, int> Authenticate()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new List<string>
            {
                "auth",
                "login",
                "--json_output"
            };

            var task = Task.Run(async () => await RedirectedProcess.Command(Tools.Common.SpatialBinary)
                .InDirectory(Tools.Common.SpatialProjectRootDir)
                .WithArgs(args.ToArray())
                .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdOut |
                    OutputRedirectBehaviour.RedirectStdErr | OutputRedirectBehaviour.ProcessSpatialOutput)
                .RunAsync(token));

            return new WrappedTask<RedirectedProcessResult, int>
            {
                Task = task,
                CancelSource = source,
                Context = 0
            };
        }
    }
}
