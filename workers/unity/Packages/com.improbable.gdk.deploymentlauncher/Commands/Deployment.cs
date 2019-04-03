using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.Tools;

namespace Improbable.Gdk.DeploymentManager.Commands
{
    public static class Deployment
    {
        private static string DeploymentLauncherProjectPath = Path.GetFullPath(Path.Combine(
            Tools.Common.GetPackagePath("com.improbable.gdk.deploymentlauncher"),
            ".DeploymentLauncher/DeploymentLauncher.csproj"));

        public static WrappedTask<Option<Ipc.Error>, DeploymentConfig> LaunchAsync(DeploymentConfig config)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new List<string>
            {
                config.SimulatedPlayerDeploymentConfig == null ? "create" : "create-sim",
                $"--project_name={config.ProjectName}",
                $"--assembly_name={config.AssemblyName}",
                $"--deployment_name={config.Name}",
                $"--launch_json=\"{Path.Combine(Tools.Common.SpatialProjectRootDir, config.LaunchJson)}\"",
                $"--region={config.Region.ToString()}"
            };

            if (!string.IsNullOrEmpty(config.SnapshotPath))
            {
                args.Add($"--snapshot=\"{Path.Combine(Tools.Common.SpatialProjectRootDir, config.SnapshotPath)}\"");
            }

            if (config.Tags.Count > 0)
            {
                args.Add($"--tags={string.Join(",", config.Tags)}");
            }

            if (config.SimulatedPlayerDeploymentConfig != null)
            {
                args.Add($"--target_deployment={config.SimulatedPlayerDeploymentConfig.TargetDeploymentName}");
                args.Add($"--flag_prefix={config.SimulatedPlayerDeploymentConfig.FlagPrefix}");
                args.Add($"--simulated_coordinator_worker_type={config.SimulatedPlayerDeploymentConfig.WorkerType}");
            }

            var task = RunDeploymentLauncher(args,
                OutputRedirectBehaviour.RedirectStdErr | OutputRedirectBehaviour.RedirectStdOut, token,
                RetrieveIpcError);

            task.Start();

            return new WrappedTask<Option<Ipc.Error>, DeploymentConfig>
            {
                Task = task,
                CancelSource = source,
                Context = config.DeepCopy()
            };
        }

        public static WrappedTask<Option<Ipc.Error>, DeploymentInfo> StopAsync(DeploymentInfo info)
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
                RetrieveIpcError);

            task.Start();

            return new WrappedTask<Option<Ipc.Error>, DeploymentInfo>
            {
                Task = task,
                CancelSource = source,
                Context = info
            };
        }

        public static WrappedTask<(Option<List<DeploymentInfo>>, Option<Ipc.Error>), string> ListAsync(string projectName)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var args = new[]
            {
                "list",
                "--project_name",
                projectName
            };

            var task = RunDeploymentLauncher(args, OutputRedirectBehaviour.RedirectStdErr, token,
                res => RetrieveDeploymentList(res, projectName));

            task.Start();

            return new WrappedTask<(Option<List<DeploymentInfo>>, Option<Ipc.Error>), string>
            {
                Task = task,
                CancelSource = source,
                Context = projectName
            };
        }

        private static async Task<T> RunDeploymentLauncher<T>(IEnumerable<string> programArgs,
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

        private static Option<Ipc.Error> RetrieveIpcError(RedirectedProcessResult result)
        {
            return result.ExitCode == 0
                ? Option<Ipc.Error>.Empty
                : new Option<Ipc.Error>(Ipc.Error.FromStderr(result.Stderr));
        }

        private static (Option<List<DeploymentInfo>>, Option<Ipc.Error>) RetrieveDeploymentList(
            RedirectedProcessResult result, string projectName)
        {
            if (result.ExitCode == 0)
            {
                return (new Option<List<DeploymentInfo>>(Ipc.GetDeploymentInfo(result.Stdout, projectName)),
                    Option<Ipc.Error>.Empty);
            }

            return (Option<List<DeploymentInfo>>.Empty, new Option<Ipc.Error>(Ipc.Error.FromStderr(result.Stderr)));
        }
    }
}
