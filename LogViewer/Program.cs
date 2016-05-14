using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace LogViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaultHostUrl = "http://localhost:9000";
            string hostUrl = ConfigurationManager.AppSettings["HostUrl"] ?? defaultHostUrl;
            
            using (WebApp.Start<Startup>(hostUrl))
            {
                Console.WriteLine("Goto {0} to view log.", hostUrl);
                Console.WriteLine("Listening for log events at {0}/log", hostUrl);
                Console.WriteLine("Hit enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
