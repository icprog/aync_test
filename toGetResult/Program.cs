using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace toGetResult
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Only RUN the task as needed.  FooGet 
            // still allows you to generalize your task.
            Task.Factory.StartNew(() =>
            {
                dynamic value = FooGet();
                value.RunSynchronously();
                Console.WriteLine(value.Result.Result.ToString());

                //Task<object> task = FooGet();
                //task.RunSynchronously();
                //Console.WriteLine(task.Result.Result.ToString());
            }).Wait();

            Console.WriteLine("Task is done.");
            Console.ReadLine();
        }

        private static Task<object> FooGet()
        {
            var task = new Task<object>(asyncBar1);

            return task;
        }

        private async static Task<object> asyncBar()
        {
            // do work!
            return "Hello, world!";
        }

        private static Task<string> asyncBar1()
        {
            var task = new Task<string>(() => "Hello, async world!");
            return task;
        }
    }
}
