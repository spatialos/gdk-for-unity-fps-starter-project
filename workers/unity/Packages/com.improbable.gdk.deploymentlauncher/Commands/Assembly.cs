using System.Threading;
using System.Threading.Tasks;
using Improbable.Gdk.Tools;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public static class Assembly
    {
        public static WrappedTask<bool, AssemblyConfig> UploadAsync(AssemblyConfig config,
            bool force = false)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new[]
            {
                "cloud",
                "upload",
                config.AssemblyName,
                "--project_name",
                config.ProjectName,
                force ? "--json_output --force" : "--json_output"
            };

            var task = Task.Run(async () =>
            {
                var processResult = await RedirectedProcess.Command(Tools.Common.SpatialBinary)
                    .InDirectory(Tools.Common.SpatialProjectRootDir)
                    .WithArgs(args)
                    .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdOut |
                        OutputRedirectBehaviour.RedirectStdErr | OutputRedirectBehaviour.ProcessSpatialOutput)
                    .RunAsync(token);

                return processResult.ExitCode == 0;
            });

            return new WrappedTask<bool, AssemblyConfig>
            {
                Task = task,
                CancelSource = source,
                Context = config.DeepCopy()
            };
        }
    }
}
