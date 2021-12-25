using System;
using System.Threading;
using System.Threading.Tasks;

namespace deadlock
{
    class Program
    {
        private static object lck1 = new object();
        private static Mutex lck2 = new Mutex();

        static async Task Main(string[] args)
        {
            var t1 = Task.Run(Lock1);
            var t2 = Task.Run(Lock2);

            await Task.WhenAny(t1, t2);
        }

        static void Lock1() {
            lock (lck1) {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                lck2.WaitOne();
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - locked");
            }
        }

        static void Lock2() {
            lck2.WaitOne();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000);
            lock (lck1) {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - locked");
            }
        }
    }
}
