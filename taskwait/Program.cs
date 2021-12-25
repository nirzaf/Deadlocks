using System;
using System.Threading;
using System.Threading.Tasks;

namespace threadpool
{
    class Program
    {
        static async Task Main()
        {
            await TestAsync();
            TestSync();
        }

        static async Task TestAsync()
        {
            await Task.Run(() => {
                Thread.Sleep(1000);
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] task 1");
            });
        }

        static void TestSync()
        {
            var tcs = new TaskCompletionSource();

            var t = Task.Run(() => {
                Thread.Sleep(1000);
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] task 2");
                tcs.SetResult();
            });

            tcs.Task.Wait();
        }

    }
}
