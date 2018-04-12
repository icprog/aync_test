using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SequentiallyExecutedTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            
            programCoreAsync();
            Console.ReadLine();
        }

        static async Task programCoreAsync()
        {
            try
            {
                int n = 1000;
                for (int i = 0; i < n; i++)
                {
                    var timeout1 = new CancellationTokenSource(2100);
                    await FirstThingAsync(timeout1.Token);

                    var timeout2 = new CancellationTokenSource(2200);
                    await SecondThingAsync(timeout2.Token);

                    var timeout3 = new CancellationTokenSource(2300);
                    await ThirdThingAsync(timeout3.Token);
                }
            }
            catch (OperationCanceledException canceledException )
            {
                Console.WriteLine(canceledException);
            }
        }

        private static async Task FirstThingAsync(CancellationToken token)
        {
            Console.WriteLine("Enter first thing async");
            Thread.Sleep(3000);
            // Do something here
        }

        private static async Task SecondThingAsync(CancellationToken token)
        {
            Console.WriteLine("Enter second thing async");
            Thread.Sleep(1000);
            // Do something here
        }

        private static async Task ThirdThingAsync(CancellationToken token)
        {
            Console.WriteLine("Enter third thing async");
            Thread.Sleep(1000);
            // Do something here
        }
    }
}
