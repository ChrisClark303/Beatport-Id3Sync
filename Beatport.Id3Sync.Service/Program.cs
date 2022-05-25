using Beatport.Id3Sync.TagManager;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.Service
{
    class Program
    {
        //static async Task<int> StartWithTopShelf()
        //{

        //}

        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (opts) =>
                {
                    await CreateHostBuilder(args, opts).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineOptions options)
        {
            SetupLogger();

            Log.Logger.Information("Starting host builder with options {@Options}", options);
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(cfg => cfg.AddProvider(new SerilogLoggerProvider(Log.Logger)))
                .ConfigureServices((hostContext, services) =>
                {
                    SetupDI(options, services);
                })
                .UseWindowsService();
        }

        private static void SetupDI(CommandLineOptions options, IServiceCollection services)
        {
            services.AddSingleton<ITagProcessorOptions>(options);
            services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);
            services.AddSingleton<IFileWatcher,BeatportFileWatcher>();
            services.AddSingleton<ITagProcessor, TagProcessor>();
            services.AddHostedService<TagProcessManager>();
        }

        private static void SetupLogger()
        {
            string logPath = CreateFilepathForLogging();
            Log.Logger = new LoggerConfiguration()
                             .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, shared: true)
                             .MinimumLevel.Verbose()
                             .CreateLogger();
        }

        private static string CreateFilepathForLogging()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appRoot = Path.Combine(appData, "BeatPort_Id3Sync", "logs");
            var logPath = @$"{appRoot}\log.txt";
            return logPath;
        }
    }
}
