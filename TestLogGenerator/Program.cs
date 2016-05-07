using System;
using NLog;

namespace LogClient
{
    class Program
    {
        private static Logger _logger;

        static void Main(string[] args)
        {
            _logger = LogManager.GetCurrentClassLogger();
            
            _logger.Info("blah");
            Console.ReadLine();
        }
    }
}
