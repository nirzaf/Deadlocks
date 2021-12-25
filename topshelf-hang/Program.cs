using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Threading;
using Topshelf;
using Topshelf.Logging;

namespace TopshelfHang
{
    class TestWorker : ServiceControl
    {
        private static readonly LogWriter logger = HostLogger.Get<TestWorker>();
        public static bool ShouldStop { get; private set; }
        private WaitHandle handle;

        public bool Start(HostControl hostControl)
        {
            logger.Info("Starting test worker...");

            handle = new ManualResetEvent(false);
            logger.Info("Starting worker threads...");

            // start the listenening thread
            ThreadPool.QueueUserWorkItem(DoWork, handle);
            return true;
        }

        private static void DoWork(Object state)
        {
            var h = (ManualResetEvent)state;
            try
            {
                throw new Exception();
            }
            finally
            {
                logger.InfoFormat("Releasing the handle");
                h.Set();
            }
        }

        public bool Stop(HostControl hostControl)
        {
            ShouldStop = true;
            logger.Info("Stopping test worker...");
            // wait for all threads to finish
            handle.WaitOne();

            return true;
        }
    }

    class Program
    {
        static void Main()
        {
            var config = new LoggingConfiguration();
            config.AddRule(LogLevel.Info, LogLevel.Fatal, new ConsoleTarget(), "TopshelfHang.TestWorker");

            HostFactory.Run(hc => {
                hc.UseNLog(new LogFactory(config));
                // service is constructed using its default constructor
                hc.Service<TestWorker>();
                // sets service properties
                hc.SetServiceName(typeof(TestWorker).Namespace);
                hc.SetDisplayName(typeof(TestWorker).Namespace);
                hc.SetDescription("Test worker");
            });
        }
    }
}
