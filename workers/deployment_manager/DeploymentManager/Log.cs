using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Improbable.Worker;

namespace DeploymentManager
{
    public static class Log
    {
        private static readonly Mutex logMutex = new Mutex();

        private static StreamWriter logStream;

        public static void Print(LogLevel level, string message, Connection connection = null)
        {
            if (logStream != null)
            {
                lock (logStream)
                {
                    var oldColor = Console.ForegroundColor;
                    var msg = $"{level}: {message}";
                    switch (level)
                    {
                        case LogLevel.Debug:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Out.WriteLine(msg);
                            break;
                        case LogLevel.Info:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Out.WriteLine(msg);
                            break;
                        case LogLevel.Warn:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Out.WriteLine(msg);
                            break;
                        case LogLevel.Error:
                        case LogLevel.Fatal:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Error.WriteLine(msg);
                            Environment.ExitCode = 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Did not use a valid LogLevel. LogLevel: {level}");
                    }

                    Console.ForegroundColor = oldColor;

                    Debug.WriteLine(msg);

                    logStream?.WriteLine(Encoding.UTF8.GetChars(Encoding.UTF8.GetBytes(msg)));
                }
            }

            if (connection != null)
            {
                lock (logStream)
                {
                    connection.SendLogMessage(level, DeploymentManager.WorkerType, message);
                }
            }
        }

        public static void Shutdown()
        {
            logStream?.Dispose();
            logStream = null;
        }

        public static void Init(string logFileName)
        {
            if (!string.IsNullOrEmpty(logFileName))
            {
                logStream = new StreamWriter(logFileName) { AutoFlush = true };
            }
        }

        public static void PrintException(Exception e, Connection connection)
        {
            Print(LogLevel.Error, e.Message, connection);
            Print(LogLevel.Error, e.StackTrace, connection);
        }
    }
}
