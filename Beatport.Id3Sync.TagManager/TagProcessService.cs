using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.TagManager
{
    public class TagProcessService : BackgroundService
    {
        private readonly IFileWatcher _fileWatcher;
        private readonly ILogger _logger;
        private readonly ITagProcessor _tagProcessor;

        public TagProcessService(IFileWatcher fileWatcher, ILogger logger, ITagProcessor tagProcessor)
        {
            _fileWatcher = fileWatcher;
            _logger = logger;
            _tagProcessor = tagProcessor;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _fileWatcher.FileAdded += (s, e) => 
            {
                _logger.Information("Handled file added: {@FileAddedEventArgs}", e);
                _tagProcessor.ProcessFile(e.FilePath);
            };
            _fileWatcher.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
