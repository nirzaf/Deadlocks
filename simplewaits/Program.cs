using System.Diagnostics;
using System.Threading;

namespace simplewaits
{
    class Program
    {
        static readonly object lck = new();
        static readonly ManualResetEvent ev = new(false);

        static void Main()
        {
            Trace.Listeners.Add(new ConsoleTraceListener() { 
                TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ThreadId
            });

            var thr = new Thread(WaitAndRun);

            lock (lck)
            {
                Trace.TraceInformation($"Starting thread");
                thr.Start();
                Trace.TraceInformation($"Sleeping");
                Thread.Sleep(1000);
            }
            Trace.TraceInformation("Waiting for mutex");
            ev.WaitOne();
            Trace.TraceInformation("Finished");

            thr.Join();
        }

        static void WaitAndRun()
        {
            Trace.TraceInformation($"Waiting for lock");
            lock (lck)
            {
                Trace.TraceInformation($"Sleeping");
                Thread.Sleep(1000);
                ev.Set();
            }
            Thread.Sleep(1000);
            Trace.TraceInformation("Finished");
        }
    }
}
