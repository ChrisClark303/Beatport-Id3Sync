using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.TagManager
{
    public class BeatportFileWatcher : IFileWatcher
    {
        private readonly string _watchPath;
        private readonly ILogger _logger;
        private FileSystemWatcher _watcher;

        public BeatportFileWatcher(ITagProcessorOptions options, ILogger logger)
        {
            _watchPath = options.SourcePath;
            _logger = logger;
        }

        public event EventHandler<FileAddedEventArgs> FileAdded;

        private List<string> _createdFiles = new List<string>();

        public void Start()
        {
            _watcher = new FileSystemWatcher
            {
                Path = _watchPath,
                Filter = "*.mp3",
                IncludeSubdirectories = true,
                InternalBufferSize = (16384 * 8)
            };
            _watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                try
                {
                    _logger.Debug("File created: {@FileSystemEventArgs}", e);
                    _createdFiles.Add(e.Name);
                    //FileAdded?.Invoke(this, new FileAddedEventArgs(e.FullPath));
                    //var waitTask = Task.Delay(1000)
                    //    .ContinueWith((t) => ProcessFile(e.FullPath));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"An error occurred handling a file created event : {ex.Message}");
                }
            };

            _watcher.Changed += async (object sender, FileSystemEventArgs e) =>
            {
                _logger.Debug("File changed: {@FileSystemEventArgs}", e);
                if (_createdFiles.Contains(e.Name))
                {
                    var waitTask = Task.Delay(1000)
                        .ContinueWith((t) =>
                        {
                            FileAdded?.Invoke(this, new FileAddedEventArgs(e.FullPath));
                            _createdFiles.Remove(e.Name);
                        });
                }
            };

            //_logger.Information("Starting tag processor");
            _watcher.EnableRaisingEvents = true;
            _logger.Information("Listening for file changes");
        }
    }
}
