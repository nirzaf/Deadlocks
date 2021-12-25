using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace hang
{
    static class AppFolder
    {
        private static readonly string appFolderPath;

        static AppFolder()
        {
            var logger = new TraceSource(nameof(AppFolder));

            appFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data");

            if (Directory.Exists(appFolderPath))
            {
                logger.TraceInformation($"App folder already exists: {appFolderPath}");
            }
            else
            {
                Directory.CreateDirectory(appFolderPath);
                logger.TraceInformation($"App folder created: {appFolderPath}");
            }
        }

        public static string AppFolderPath => appFolderPath;
    }

    class CustomFileTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            var log = Path.Combine(AppFolder.AppFolderPath, "app.log");
            File.AppendAllText(log, message);
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }
    }

    static class Program
    {
        static void Main()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new CustomFileTraceListener());

            var thr = new Thread(DoingSomeWork);
            thr.Start();

            Trace.TraceInformation("Application started");

            thr.Join();

            Trace.TraceInformation("Application finished");
        }

        static void DoingSomeWork(object state)
        {
            var outputFile = Path.Combine(AppFolder.AppFolderPath, "data.txt");

            File.WriteAllText(outputFile, "some calculations");
        }
    }
}
