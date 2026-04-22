namespace YP.TimedJob.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InvokeAttribute : Attribute
    {
        public string Begin { get; set; }
        public int Interval { get; set; }
        public bool SkipWhileExecuting { get; set; }

        public InvokeAttribute()
        {
            Begin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Interval = 1000;
            SkipWhileExecuting = true;
        }
    }
}