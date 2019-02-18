using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace DeploymentManager
{
    /// <summary>
    ///     Runtime options for the CodeGenerator.
    /// </summary>
    public class DeploymentManagerOptions
    {
        public string ReceptionistHost { get; private set; }
        public ushort ReceptionistPort { get; private set; }
        public string WorkerId { get; private set; }
        public string LogFile { get; private set; }
        public string ProjectName { get; private set; }
        public string AssemblyName { get; private set; }
        public string PlayerType { get; private set; }
        public string DeploymentPrefix { get; private set; }
        public int NumberOfDeployments { get; private set; }
        public int MaxPlayers { get; private set; }
        public bool ShouldShowHelp { get; private set; }
        public string HelpText { get; private set; }

        public static DeploymentManagerOptions ParseArguments(ICollection<string> args)
        {
            var options = new DeploymentManagerOptions();
            var optionSet = new OptionSet
            {
                {
                    "receptionist-host=", "REQUIRED: The host of the receptionist that the deployment manager connects to",
                    r => options.ReceptionistHost = r
                },
                {
                    "receptionist-port=", "REQUIRED: The port of the receptionist that the deployment manager connects to",
                    r => options.ReceptionistPort = ushort.Parse(r)
                },
                {
                    "worker-id=", "REQUIRED: The id of the deployment manager",
                    w => options.WorkerId = w
                },
                {
                    "log-file=", "REQUIRED: Path to the log file",
                    l => options.LogFile = l
                },
                {
                    "project-name=", "REQUIRED: The name of the SpatialOS project",
                    p => options.ProjectName = p
                },
                {
                    "assembly-name=", "REQUIRED: The name of the assembly",
                    a => options.AssemblyName = a
                },
                                                {
                    "player-type=", "REQUIRED: The type of player that the deployment manager should track",
                    p => options.PlayerType = p
                },
                                                                {
                    "deployment-prefix=", "REQUIRED: The prefix of the deployments that the deployment manager generates",
                    d => options.DeploymentPrefix = d
                },
                {
                    "number-of-deployments=", "REQUIRED: The number of deployments that the deployment manager should generate",
                    n => options.NumberOfDeployments = int.Parse(n)
                },
                                                                                                {
                    "max-players=", "REQUIRED: The maximum number of players allowed to connect to the generated deployments.",
                    m => options.MaxPlayers = int.Parse(m)
                },
                {
                    "h|help", "show help",
                    h => options.ShouldShowHelp = h != null
                }
            };

            optionSet.Parse(args);

            using (var sw = new StringWriter())
            {
                optionSet.WriteOptionDescriptions(sw);
                options.HelpText = sw.ToString();
            }


            return options;
        }
    }
}
