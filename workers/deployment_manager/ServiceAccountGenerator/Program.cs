using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.ServiceAccount.V1Alpha1;
using System;
using System.IO;

namespace ServiceAccountGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Out.WriteLine("Usage: <service account token path> <SpatialOS project name>");
                Environment.Exit(1);
            }

            var path = args[0].Trim('"');
            var projectName = args[1];

            Console.WriteLine(Path.GetFullPath(Path.Combine(path, "ServiceAccountToken.txt")));

            var perm = new Permission
            {
                Parts = { "prj", projectName, "*" },
                Verbs =
                {
                    Permission.Types.Verb.Read,
                    Permission.Types.Verb.Write
                }
            };

            var perm2 = new Permission
            {
                Parts = { "srv", "bundles" },
                Verbs = { Permission.Types.Verb.Read }
            };

            var resp = ServiceAccountServiceClient.Create().CreateServiceAccount(new CreateServiceAccountRequest
            {
                Name = "GDKService",
                ProjectName = projectName,
                Permissions = { perm, perm2 },
                Lifetime = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0))
            });

            using (var writer = new StreamWriter(Path.GetFullPath(Path.Combine(path, "ServiceAccountToken.txt"))))
            {
                writer.WriteLine(resp.Token);
            }
        }
    }
}
