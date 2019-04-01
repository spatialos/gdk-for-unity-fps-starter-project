using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Improbable.Gdk.Tools;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    internal static class DeploymentTasks
    {
        private static readonly string DotNetProjectPath =
            Path.GetFullPath(Path.Combine(Tools.Common.GetPackagePath("com.improbable.gdk.deploymentlauncher"),
                ".DeploymentLauncher/DeploymentLauncher.csproj"));

        public class WrappedTask<TResult> : IDisposable
        {
            public Task<TResult> Task;
            public CancellationTokenSource CancelSource;

            public void Dispose()
            {
                Task?.Dispose();
                CancelSource?.Dispose();
            }
        }

        public static WrappedTask<bool> TriggerUploadAssemblyAsync(string[] arguments, Action onSuccess,
            Action onFailure)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var projectRootPath = Path.Combine(Application.dataPath, "../../../");

            var task = Task.Run(async () =>
            {
                if (!Tools.Common.CheckDependencies())
                {
                    return false;
                }

                var processResult = await RedirectedProcess.Command(Tools.Common.SpatialBinary).WithArgs(arguments)
                    .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdOut |
                        OutputRedirectBehaviour.RedirectStdErr | OutputRedirectBehaviour.ProcessSpatialOutput)
                    .InDirectory(projectRootPath)
                    .RunAsync(token);

                if (processResult.ExitCode == 0)
                {
                    onSuccess();
                    return true;
                }

                onFailure();
                return false;
            });

            return new WrappedTask<bool>
            {
                Task = task,
                CancelSource = source
            };
        }

        public static WrappedTask<bool> TriggerLaunchDeploymentAsync(List<string> arguments, Action onSuccess)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var dotNetWorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

            EditorApplication.LockReloadAssemblies();

            var task = Task.Run(async () =>
                {
                    if (!Tools.Common.CheckDependencies())
                    {
                        return false;
                    }

                    var processResult = await RedirectedProcess.Command(Tools.Common.DotNetBinary)
                        .WithArgs(ConstructArguments(arguments))
                        .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdOut |
                            OutputRedirectBehaviour.RedirectStdErr)
                        .InDirectory(dotNetWorkingDirectory)
                        .RunAsync(token);

                    if (processResult.ExitCode == 0)
                    {
                        onSuccess();
                    }

                    return processResult.ExitCode != 0;
                }
            );

            return new WrappedTask<bool>
            {
                Task = task,
                CancelSource = source
            };
        }

        public static WrappedTask<bool> TriggerStopDeploymentAsync(List<string> arguments, Action onSuccess, Action<RedirectedProcessResult> onFailure)
        {
            if (!Tools.Common.CheckDependencies())
            {
                return null;
            }

            var dotNetWorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

            var source = new CancellationTokenSource();
            var token = source.Token;

            var task = Task.Run(async () =>
            {
                var processResult = await RunDeploymentLauncherHelperAsync(token, dotNetWorkingDirectory, arguments, true);
                if (processResult.ExitCode != 0)
                {
                    onFailure(processResult);
                    return false;
                }

                onSuccess();
                return true;
            });

            return new WrappedTask<bool>
            {
                Task = task,
                CancelSource = source
            };
        }

        public static WrappedTask<List<DeploymentManager.DeploymentInfo>> TriggerListDeploymentsAsync(List<string> args,
            bool redirectStdout = false)
        {
            if (!Tools.Common.CheckDependencies())
            {
                return null;
            }

            var dotNetWorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

            var source = new CancellationTokenSource();
            var token = source.Token;

            var task = Task.Run(async () =>
            {
                var processResult = await RunDeploymentLauncherHelperAsync(token, dotNetWorkingDirectory, args, redirectStdout);
                if (processResult.ExitCode != 0)
                {
                    return null;
                }

                // Get deployments from output.
                var deploymentList = new List<DeploymentManager.DeploymentInfo>();
                foreach (var line in processResult.Stdout)
                {
                    var tokens = line.Split(' ');
                    if (tokens.Length != 3)
                    {
                        continue;
                    }

                    if (tokens[0] != "<deployment>")
                    {
                        continue;
                    }

                    deploymentList.Add(new DeploymentManager.DeploymentInfo
                    {
                        Id = tokens[1],
                        Name = tokens[2]
                    });
                }

                deploymentList.Sort((item1, item2) => string.Compare(item1.Name, item2.Name, StringComparison.Ordinal));

                return deploymentList;
            });

            return new WrappedTask<List<DeploymentManager.DeploymentInfo>>
            {
                Task = task,
                CancelSource = source
            };
        }

        public static async Task<RedirectedProcessResult> RunDeploymentLauncherHelperAsync(CancellationToken token,
            string workingDirectory,
            List<string> args,
            bool redirectStdout = false)
        {
            var outputOptions = OutputRedirectBehaviour.RedirectStdErr;
            if (redirectStdout)
            {
                outputOptions |= OutputRedirectBehaviour.RedirectStdOut;
            }

            var processResult = await RedirectedProcess.Command(Tools.Common.DotNetBinary)
                .WithArgs(ConstructArguments(args))
                .RedirectOutputOptions(outputOptions)
                .InDirectory(workingDirectory)
                .RunAsync(token);

            if (processResult.ExitCode == 0 || token.IsCancellationRequested)
            {
                return processResult;
            }

            // Examine the failure reason.
            var failureReason = processResult.Stdout.Count > 0 ? processResult.Stdout[0] : "";
            if (failureReason == "<error:unauthenticated>")
            {
                // The reason this task failed is because we are authenticated. Try authenticating.
                Debug.Log(
                    "Failed to connect to the SpatialOS platform due to being unauthenticated. Running `spatial auth login` then retrying the last operation...");
                var spatialAuthLoginResult = await RedirectedProcess.Command(Tools.Common.SpatialBinary)
                    .WithArgs(new string[] { "auth", "login", "--json_output" })
                    .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdErr |
                        OutputRedirectBehaviour.ProcessSpatialOutput)
                    .InDirectory(workingDirectory)
                    .RunAsync(token);

                if (spatialAuthLoginResult.ExitCode == 0)
                {
                    // Re-run the task.
                    processResult = await RedirectedProcess.Command(Tools.Common.DotNetBinary)
                        .WithArgs(ConstructArguments(args))
                        .RedirectOutputOptions(OutputRedirectBehaviour.RedirectStdErr)
                        .InDirectory(workingDirectory)
                        .RunAsync(token);
                }
                else
                {
                    Debug.Log("Failed to run `spatial auth login`.");
                }
            }

            return processResult;
        }

        private static string[] ConstructArguments(List<string> args)
        {
            var baseArgs = new List<string>
            {
                "run",
                "-p",
                $"\"{DotNetProjectPath}\"",
                "--",
            };
            baseArgs.AddRange(args.Select(arg => $"\"{arg}\""));
            return baseArgs.ToArray();
        }
    }
}
