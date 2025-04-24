using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace DSPC.ConsumerProducer
{
    public class Logger : IDisposable
    {
        private readonly StreamWriter _output;
        private readonly BlockingCollection<string> _queue;
        private readonly Thread _consumerThread;
        private bool _disposed = false;

        public string Filename { get; }

        public Logger()
        {
            Filename = GenerateFileName();
            var logFile = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            _output = new StreamWriter(logFile, Encoding.UTF8);

            _queue = new BlockingCollection<string>(boundedCapacity: 10000);
            _consumerThread = new Thread(Consume) { IsBackground = true };
            _consumerThread.Start();
        }

        public void LogMessage(string logLevel, string message)
        {
            var logMessage = FormatMessage(DateTime.Now, logLevel, message);
            // Додаємо в чергу, не чекаючи запису
            _queue.Add(logMessage);
        }

        private void Consume()
        {
            // GetConsumingEnumerable() чекає, доки CompleteAdding не буде викликаний
            foreach (var logMessage in _queue.GetConsumingEnumerable())
            {
                _output.WriteLine(logMessage);
                // за потреби можна Flush() кожні N записів або за таймером
            }
        }

        private string FormatMessage(DateTime time, string logLevel, string message)
        {
            return $"{time:dd/MM/yyyy HH:mm:ss.fff} [{logLevel}] {message}";
        }

        private string GenerateFileName()
        {
            return $"logfile_{DateTime.Now:ddMM_HHmmss_fff}.log";
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Скажемо споживачу, що більше не буде нових повідомлень
            _queue.CompleteAdding();
            // Дочекаємось, доки всі повідомлення будуть оброблені
            _consumerThread.Join();

            // Фінальний Flush і закриття потоку
            _output.Flush();
            _output.Dispose();
            _queue.Dispose();
        }
    }
}