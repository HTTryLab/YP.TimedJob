using System.Reflection;
using System.Threading.Tasks;

namespace YP.TimedJob.Core
{
    public class JobScheduler
    {
        private readonly List<JobTask> _tasks = new List<JobTask>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IJobLogger _logger;

        public JobScheduler(IJobLogger logger)
        {
            _logger = logger ?? new ConsoleJobLogger();
        }

        public JobScheduler() : this(new ConsoleJobLogger())
        {
        }

        public void Start()
        {
            Task.Run(async () => await RunScheduledTasks(), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void RegisterJobs(Assembly assembly)
        {
            var jobTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Job)) && !t.IsAbstract);
            foreach (var jobType in jobTypes)
            {
                var methods = jobType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var invokeAttr = method.GetCustomAttribute<InvokeAttribute>();
                    if (invokeAttr != null)
                    {
                        var jobInstance = Activator.CreateInstance(jobType);
                        if (jobInstance != null)
                        {
                            var task = new JobTask
                            {
                                JobInstance = jobInstance,
                                Method = method,
                                Begin = DateTime.Parse(invokeAttr.Begin),
                                Interval = invokeAttr.Interval,
                                SkipWhileExecuting = invokeAttr.SkipWhileExecuting,
                                LastExecuteTime = DateTime.MinValue
                            };
                            _tasks.Add(task);
                        }
                    }
                }
            }
        }

        private async Task RunScheduledTasks()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var now = DateTime.Now;
                foreach (var task in _tasks)
                {
                    if (now >= task.Begin && (now - task.LastExecuteTime).TotalMilliseconds >= task.Interval)
                    {
                        if (!task.IsRunning || !task.SkipWhileExecuting)
                        {
                            task.IsRunning = true;
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await ExecuteTask(task);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error($"Task execution error: {ex.Message}", ex);
                                }
                                finally
                                {
                                    task.IsRunning = false;
                                    task.LastExecuteTime = DateTime.Now;
                                }
                            });
                        }
                    }
                }
                await Task.Delay(100, _cancellationTokenSource.Token);
            }
        }

        private async Task ExecuteTask(JobTask task)
        {
            try
            {
                var result = task.Method.Invoke(task.JobInstance, null);
                if (result is Task taskResult)
                {
                    await taskResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing job task: {ex.Message}", ex);
            }
        }

        private class JobTask
        {
            public required object JobInstance { get; set; }
            public required MethodInfo Method { get; set; }
            public DateTime Begin { get; set; }
            public int Interval { get; set; }
            public bool SkipWhileExecuting { get; set; }
            public DateTime LastExecuteTime { get; set; }
            public bool IsRunning { get; set; }
        }
    }
}