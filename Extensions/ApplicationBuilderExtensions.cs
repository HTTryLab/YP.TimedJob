using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using YP.TimedJob.Core;

namespace YP.TimedJob.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTimedJob(this IApplicationBuilder app)
        {
            var scheduler = app.ApplicationServices.GetRequiredService<JobScheduler>();
            
            // 扫描所有加载的程序集，而不仅仅是入口程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                scheduler.RegisterJobs(assembly);
            }
            
            scheduler.Start();
            return app;
        }
    }
}