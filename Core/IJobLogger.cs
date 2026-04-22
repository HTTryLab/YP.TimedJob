namespace YP.TimedJob.Core
{
    public interface IJobLogger
    {
        void Information(string message);
        void Warning(string message);
        void Error(string message, Exception? exception = null);
    }
}