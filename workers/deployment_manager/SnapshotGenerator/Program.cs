using System;
using System.IO;
using Improbable;
using Improbable.Worker;

namespace SnapshotGenerator
{
    internal class Program
    {
        private static long nextEntityId = 1;
        private static readonly Improbable.Collections.List<Entity> Entities = new Improbable.Collections.List<Entity>();

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Out.WriteLine("Usage: <snapshot filename>");
                Environment.Exit(1);
            }

            var fileName = args[0].Trim('"');
            
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (var stream = new SnapshotOutputStream(fileName))
            {
                foreach (var entity in Entities)
                {
                    WriteEntity(stream, entity);
                }
            }

            Console.Out.WriteLine($"Wrote to {fileName}");
        }

        private static void WriteEntity(SnapshotOutputStream stream, Entity entity)
        {
            var result = stream.WriteEntity(new EntityId(nextEntityId++), entity);
            if (result.HasValue)
            {
                Console.Error.WriteLine(result.Value);
                Environment.Exit(1);
            }
        }        
    }
}
