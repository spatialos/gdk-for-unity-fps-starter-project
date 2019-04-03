using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Improbable.Gdk.Tools;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public static class Deployment
    {
        private static string DeploymentLauncherProjectPath = Path.GetFullPath(Path.Combine(
            Tools.Common.GetPackagePath("com.improbable.gdk.deploymentlauncher"),
            ".DeploymentLauncher/DeploymentLauncher.csproj"));

        public static WrappedTask<bool, DeploymentConfig> LaunchAsync(DeploymentConfig config)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            // TODO: Allow for no snapshot
            var args = new[]
            {
                "create",
                "--project_name",
                config.ProjectName,
                "--assembly_name",
                config.AssemblyName,
                "--deployment_name",
                config.Name,
                "--launch_json",
                Path.Combine(Tools.Common.SpatialProjectRootDir, config.LaunchJson),
                "--snapshot",
                Path.Combine(Tools.Common.SpatialProjectRootDir, config.SnapshotPath),
                "--region",
                config.Region.ToString()
            };

            var task = RunDeploymentLauncher(args,
                OutputRedirectBehaviour.RedirectStdErr | OutputRedirectBehaviour.RedirectStdOut, token,
                result => result.ExitCode == 0);

            task.Start();

            return new WrappedTask<bool, DeploymentConfig>
            {
                Task = task,
                CancelSource = source,
                Context = config.DeepCopy()
            };
        }

        public static WrappedTask<bool, DeploymentInfo> StopAsync(DeploymentInfo info)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new[]
            {
                "stop",
                "--project_name",
                info.ProjectName,
                "--deployment_id",
                info.Id
            };

            var task = RunDeploymentLauncher(args,
                OutputRedirectBehaviour.RedirectStdOut | OutputRedirectBehaviour.RedirectStdErr,
                token,
                result => result.ExitCode == 0);

            task.Start();

            return new WrappedTask<bool, DeploymentInfo>
            {
                Task = task,
                CancelSource = source,
                Context = info
            };
        }

        public static WrappedTask<List<DeploymentInfo>, string> ListAsync(string projectName)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new[]
            {
                "list",
                "--project_name",
                projectName
            };

            // TODO: Handle result.
            var task = RunDeploymentLauncher(args, OutputRedirectBehaviour.RedirectStdErr, token,
                result => new List<DeploymentInfo>());

            task.Start();

            return new WrappedTask<List<DeploymentInfo>, string>
            {
                Task = task,
                CancelSource = source,
                Context = projectName
            };
        }

        private static async Task<T> RunDeploymentLauncher<T>(string[] programArgs,
            OutputRedirectBehaviour redirectBehaviour,
            CancellationToken token,
            Func<RedirectedProcessResult, T> resultHandler)
        {
            var wrappedArgs = new[] { "run", "-p", $"\"{DeploymentLauncherProjectPath}\"" }
                .Concat(programArgs)
                .ToArray();

            var processResult = await RedirectedProcess.Command(Tools.Common.DotNetBinary)
                .InDirectory("") // TODO: Figure out where this should run.
                .WithArgs(wrappedArgs)
                .RedirectOutputOptions(redirectBehaviour)
                .WithTimeout(new TimeSpan(0, 0, 25, 0))
                .RunAsync(token);

            return resultHandler(processResult);
        }
    }
}
