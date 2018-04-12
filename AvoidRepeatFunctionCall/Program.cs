using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvoidRepeatFunctionCall
{
    class Program
    {
        private static bool _functionJustCalled = false;

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Program start");

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    
                    RepeatFunction();
                }
            });

            //RepeatFunction();
            /*while (true)
            {
                Task.Delay(1000).ContinueWith(delay => RepeatFunction());
            }*/
            Console.ReadLine();
            /*check if there's any same call within 5 min time. If yes, skip it. */
        }

        private static void RepeatFunction()
        {
            if (_functionJustCalled)
            {
                Console.WriteLine("Skip");
                return;
            }
            Console.WriteLine($"This is a repeated function. Task ID: {Task.CurrentId}");
            _functionJustCalled = true;
            Task.Delay(5000).ContinueWith(a => _functionJustCalled = false);

        }
    }
}
