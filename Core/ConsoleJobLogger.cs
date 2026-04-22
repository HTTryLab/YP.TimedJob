namespace YP.TimedJob.Core
{
    public class ConsoleJobLogger : IJobLogger
    {
        public void Information(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[WARN] {DateTime.Now}: {message}");
        }

        public void Error(string message, Exception? exception = null)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            if (exception != null)
            {
                Console.WriteLine($"[ERROR] Exception: {exception.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {exception.StackTrace}");
            }
        }
    }
}