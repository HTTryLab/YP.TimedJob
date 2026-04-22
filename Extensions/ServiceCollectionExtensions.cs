using Microsoft.Extensions.DependencyInjection;
using YP.TimedJob.Core;

namespace YP.TimedJob.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTimedJob(this IServiceCollection services)
        {
            services.AddSingleton<IJobLogger, ConsoleJobLogger>();
            services.AddSingleton<JobScheduler>();
            return services;
        }
    }
}