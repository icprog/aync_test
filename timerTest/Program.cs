using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Timers;

namespace timerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            //timer.Tick += (s, e) =>
            //{
            //    Console.WriteLine("Tick");
            //};
            //timer.Start();

            var timer = new Timer {Interval = 1000};
            timer.Elapsed += (s, e) =>
            {
                Console.WriteLine($"Tick {e.ToString()}.");
            };
            timer.Start();

            while (true)
            {
            }
        }
    }
}
