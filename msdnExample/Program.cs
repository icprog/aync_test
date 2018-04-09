using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace msdnExample
{
    class Program
    {
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Console.WriteLine("program start..");
                GetPageSizeAsync(args[1]).Wait();
                Console.WriteLine("after async call..");
            }
            else
                Console.WriteLine("Enter at least one URL on the command line.");
        }
        private static async Task GetPageSizeAsync(string url)
        {
            var client = new HttpClient();
            var uri = new Uri(Uri.EscapeUriString(url));
            byte[] urlContents = await client.GetByteArrayAsync(uri);
            Console.WriteLine($"{url}: {urlContents.Length / 2:N0} characters");
        }
        // The following call from the command line:
        //    await1 http://docs.microsoft.com
        // the program displays output like the following: 
        //   http://docs.microsoft.com: 7,967 characters
    }
}
