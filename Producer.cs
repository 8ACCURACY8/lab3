using System;
using System.Threading;

namespace DSPC.ConsumerProducer
{
    public class Producer
    {
        private readonly Logger _logger;
        private readonly string _name;
        private readonly Random _random;
        private bool _cancelled;

        public Producer(Logger logger, string name)
        {
            _logger = logger;
            _name = name;
            _random = new Random();
        }

        public void StartProduce()
        {
            var thread = new Thread(Produce) { IsBackground = true };
            thread.Start();
        }

        public void StopProduce()
        {
            _cancelled = true;
        }

        private void Produce()
        {
            while (!_cancelled)
            {
                _logger.LogMessage("Debug", $"Message from Producer {_name}");
                Thread.Sleep(_random.Next(50, 100));
            }
        }
    }
}
