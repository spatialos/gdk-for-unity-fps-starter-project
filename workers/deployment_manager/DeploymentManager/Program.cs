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
            try
            {
                var options = DeploymentManagerOptions.ParseArguments(args);

                // Prepare Logger
                var logFileName = options.LogFile;
                if (string.IsNullOrEmpty(logFileName))
                {
                    logFileName = Path.Combine(Environment.CurrentDirectory, $"{options.WorkerId}.log");
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

                // Start deployment manager
                var deploymentManager = new DeploymentManager(options);
                deploymentManager.ObserveDeployments();
            }
            catch (Exception e)
            {
                Log.Print(LogLevel.Debug, $"Failed to run deployment manager with exception: {e.Message} - {e.StackTrace}");
            }
            finally
            {
                Log.Shutdown();
            }
        }

    }
}
