using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Rollbar;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                RollbarLocator.RollbarInstance.Configure(new RollbarConfig("bbc088aa9f874a9b9aa0e63045912409"));
                RollbarLocator.RollbarInstance.Info("Rollbar is configured properly.");

                logger.Debug("init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets(appAssembly, optional: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .UseStartup<Startup>();
    }
}
