using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.ServiceAccount.V1Alpha1;
using System;
using System.IO;

namespace ServiceAccountGenerator
{

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Out.WriteLine("Usage: <service account token path>");
                Environment.Exit(1);
            }

            var path = args[0].Trim('"');
            var projectName = args[1];

            Console.WriteLine(Path.GetFullPath(Path.Combine(path, "ServiceAccountToken.txt")));

            var perm = new Permission
            {
                Parts = { new RepeatedField<string> { "prj", projectName, "*" } },
                Verbs =
                {
                    new RepeatedField<Permission.Types.Verb>
                    {
                        Permission.Types.Verb.Read,
                        Permission.Types.Verb.Write
                    }
                }
            };

            var perm2 = new Permission
            {
                Parts = { new RepeatedField<string> { "srv", "bundles"} },
                Verbs =
                {
                    new RepeatedField<Permission.Types.Verb>
                    {
                        Permission.Types.Verb.Read
                    }
                }
            };

            var resp = ServiceAccountServiceClient.Create().CreateServiceAccount(new CreateServiceAccountRequest
            {
                Name = "GDKService",
                ProjectName = projectName,
                Permissions = { new RepeatedField<Permission> { perm, perm2 } },
                Lifetime = Duration.FromTimeSpan(new TimeSpan(1, 0, 0, 0)) // Let this service account live for one day
            });

            var writer = new StreamWriter(Path.GetFullPath(Path.Combine(path, "ServiceAccountToken.txt")));
            writer.WriteLine(resp.Token);
            writer.Close();
        }
    }
}
