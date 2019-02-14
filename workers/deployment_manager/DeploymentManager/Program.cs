using System;
using System.IO;
using System.Reflection;
using Improbable.Worker;

namespace DeploymentManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Read arguments
            var host = args[0];
            var port = ushort.Parse(args[1]);
            var workerId = args.Length >= 3 ? args[2] : $"{DeploymentManager.WorkerType}-{Guid.NewGuid().ToString()}";
            var logFileName = args.Length >= 4 ? args[3] : string.Empty;
            var projectName = args[4];
            var assemblyName = args[5];
            var clientType = args[6];
            var deploymentNamePrefix = args[7];
            var numberOfDeployments = int.Parse(args[8]);
            var maxPlayerCount = int.Parse(args[9]);

            // Prepare Logger
            if (string.IsNullOrEmpty(logFileName))
            {
                logFileName = Path.Combine(Environment.CurrentDirectory, workerId + ".log");
            }

            Log.Init(logFileName);
            Log.Print(LogLevel.Debug, $"Opened logfile {logFileName}");

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Log.Print(LogLevel.Fatal, eventArgs.ExceptionObject.ToString());

                if (eventArgs.IsTerminating)
                {
                    Log.Shutdown();
                }
            };

            // Load GeneratedCode assembly
            var generatedCode = Assembly.Load("GeneratedCode");
            Log.Print(LogLevel.Debug, $"Loaded generated code from {generatedCode.Location}");
            Log.Print(LogLevel.Info, $"Connecting to {host}:{port} as {workerId}");

            // Start deployment manager
            var deploymentManager = new DeploymentManager(host, port, workerId, projectName, clientType, assemblyName, deploymentNamePrefix, numberOfDeployments, maxPlayerCount);
            deploymentManager.ObserveDeployments();

            Log.Shutdown();
        }

    }
}
