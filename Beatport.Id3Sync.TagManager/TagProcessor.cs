using Id3Lib;
using Microsoft.Extensions.Hosting;
using Mp3Lib;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.TagManager
{
    public class FileAddedEventArgs : EventArgs
    {
        public string FilePath { get; }

        public FileAddedEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }

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

    public class TagProcessManager : BackgroundService
    {
        private readonly IFileWatcher _fileWatcher;
        private readonly ILogger _logger;
        private readonly ITagProcessor _tagProcessor;

        public TagProcessManager(IFileWatcher fileWatcher, ILogger logger, ITagProcessor tagProcessor)
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
        }
    }


    public class TagProcessor : ITagProcessor
    {
        private readonly ITagProcessorOptions _options;
        private readonly ILogger _logger;
        private readonly char[] _invalidChars;

        public TagProcessor(ITagProcessorOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
            var invalidFileChars = Path.GetInvalidFileNameChars();
            var invalidPathChars = Path.GetInvalidPathChars();
            _invalidChars = invalidPathChars.Union(invalidFileChars).ToArray();
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    FileSystemWatcher watcher = new FileSystemWatcher
        //    {
        //        Path = _options.SourcePath,
        //        Filter = "*.mp3",
        //        IncludeSubdirectories = true,
        //        InternalBufferSize = (16384 * 8)
        //    };
        //    watcher.Created += async (object sender, FileSystemEventArgs e) =>
        //    {
        //        try
        //        {
        //            _logger.Debug("File created: {@FileSystemEventArgs}", e);
        //            var waitTask = Task.Delay(1000)
        //                .ContinueWith((t) => ProcessFile(e.FullPath));
        //        }
        //        catch(Exception ex)
        //        {
        //            _logger.Error(ex, $"An error occurred handling a file created event : {ex.Message}");
        //        }
        //    };

        //    _logger.Information("Starting tag processor");
        //    watcher.EnableRaisingEvents = true;
        //    _logger.Information("Listening for file changes");
        //}

        public async Task ProcessFile(string file)
        {
            _logger.Information($"Processing file {file}");
            TagHandler tagHandler = GetTagHandlerForFile(file);

            string copyFilePath;
            string artist = tagHandler.Artist;
            var album = tagHandler.Album;
            try
            {
                copyFilePath = CreateDirectoriesForProcessedMp3(tagHandler, artist, album);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error has occurred creating directories for {file}");
                throw;
            }

            try
            {
                CopyMp3(file, tagHandler, copyFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occurred copying file: {tagHandler.Title}");
                throw;
            }

            tagHandler = UpdateId3Tags(tagHandler, copyFilePath, artist);

            try
            {
                DeleteOriginalFile(file);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred deleting file: {file}", ex);
            }
        }

        private void DeleteOriginalFile(string file)
        {
            _logger.Information($"Deleting original file {file}.");
            File.Delete(file);
        }

        private TagHandler UpdateId3Tags(TagHandler tagHandler, string copyFilePath, string artist)
        {
            try
            {
                _logger.Information($"Updating tags for {tagHandler.Title}");
                var mp3 = new Mp3File(copyFilePath);
                tagHandler = mp3.TagHandler;
                tagHandler.AlbumArtist = artist;
                tagHandler.Publisher = "";
                tagHandler.ContentGroup = "";
                mp3.Update();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occurred updating tags on file: {copyFilePath}");
                throw;
            }

            return tagHandler;
        }

        private void CopyMp3(string file, TagHandler tagHandler, string copyFilePath)
        {
            _logger.Information($"Copying {tagHandler.Title} to {copyFilePath}");
            File.Copy(file, copyFilePath, true);
        }

        private string CreateDirectoriesForProcessedMp3(TagHandler tagHandler, string artist, string album)
        {
            string copyFilePath;
            copyFilePath = CreateSafePathForFile(artist, album, tagHandler.Title);
            return copyFilePath;
        }

        private string CreateSafePathForFile(string artist, string album, string title)
        {
            var pathSafeArtist = artist;
            var pathSafeAlbum = album;
            var pathSafeTitle = title;

            foreach (var invalidChar in _invalidChars)
            {
                pathSafeAlbum = pathSafeAlbum.Replace(invalidChar, '_');
                pathSafeArtist = pathSafeArtist.Replace(invalidChar, '_');
                pathSafeTitle = pathSafeTitle.Replace(invalidChar, '_');
            }
            string fileName = $"{pathSafeArtist} - {pathSafeTitle}.mp3";
            var artistDir = Directory.CreateDirectory(Path.Combine(_options.OutputPath, pathSafeArtist));
            var albumDir = Directory.CreateDirectory(Path.Combine(artistDir.FullName, pathSafeAlbum));
            return Path.Combine(albumDir.FullName, fileName);
        }

        private TagHandler GetTagHandlerForFile(string file)
        {
            try
            {
                var mp3 = new Mp3File(file);
                var tagHandler = mp3.TagHandler;
                Console.WriteLine($"{nameof(tagHandler.Artist)}: {tagHandler.Artist}");
                Console.WriteLine($"{nameof(tagHandler.Composer)}: {tagHandler.Composer}");
                Console.WriteLine($"{nameof(tagHandler.Album)}: {tagHandler.Album}");
                Console.WriteLine($"{nameof(tagHandler.Title)}: {tagHandler.Title}");
                Console.WriteLine($"{nameof(tagHandler.Song)}: {tagHandler.Song}");
                Console.WriteLine($"{nameof(tagHandler.Comment)}: {tagHandler.Comment}");
                Console.WriteLine($"{nameof(tagHandler.Publisher)}: {tagHandler.Publisher}");
                Console.WriteLine($"{nameof(tagHandler.ContentGroup)}: {tagHandler.ContentGroup}");
                Console.WriteLine("-------------------------------");

                return tagHandler;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error has occurred creating the taghandler for {file}");
                throw;
            }
        }
    }

    public interface ITagProcessorOptions
    {
        string SourcePath { get; set; }
        string OutputPath { get; set; }
    }
}
