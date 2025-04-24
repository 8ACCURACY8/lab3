using System;
using System.IO;

namespace DSPC.ConsumerProducer
{
    class Program
    {
        static void Main()
        {
            using (var logger = new Logger())
            {
                Console.WriteLine($"Log file: {Path.GetFullPath(logger.Filename)}");

                var producers = new[]
                {
                    new Producer(logger, "A"),
                    new Producer(logger, "B"),
                    new Producer(logger, "C"),
                };

                foreach (var p in producers) p.StartProduce();

                Console.WriteLine("Press <Enter> to stop...");
                Console.ReadLine();

                foreach (var p in producers) p.StopProduce();

                Console.WriteLine("Finishing log writes...");
            } 
        }
    }
}
