using MonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonProcessorTests
{
    class Program
    {
        static void Main(string[] args)
        {
            MonRequest req = new MonRequest
            {
                Method = "CMD",
                Path = "netstat",
                Query = "-t -o"
            };
            MonitoringProcessor processor = new MonitoringProcessor();
            object res = processor.Invoke(req);
            Console.WriteLine(res);

            Console.ReadLine();
        }
    }
}
