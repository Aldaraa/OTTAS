using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.HostedService
{
    public class JobScopedService
    {


        public async Task DoWork()
        {
            // This method represents the work you want to perform at scheduled intervals.
            Console.WriteLine("MyScopedService is doing some work.");

            // Simulate some work
            await Task.Delay(TimeSpan.FromSeconds(2));

            Console.WriteLine("MyScopedService has completed its work.");
        }

    }
}
