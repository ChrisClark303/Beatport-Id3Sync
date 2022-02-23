using Id3Lib;
using Microsoft.Extensions.Hosting;
using Mp3Lib;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.TagManager
{
    public class TagProcessor : BackgroundService
    {
        private readonly ITagProcessorOptions _options;
        private readonly ILogger _logger;

        public TagProcessor(ITagProcessorOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = _options.SourcePath,
                Filter = "*.mp3",
                IncludeSubdirectories = true,
                InternalBufferSize = (16384 * 8)
            };
            watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                try
                {
                    _logger.Debug("File created: {@FileSystemEventArgs}", e);
                    var waitTask = Task.Delay(1000)
                        .ContinueWith((t) => ProcessFile(e.FullPath));
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, $"An error occurred handling a file created event : {ex.Message}");
                }
            };

            _logger.Information("Starting tag processor");
            watcher.EnableRaisingEvents = true;
            _logger.Information("Listening for file changes");
        }

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
            string fileName = $"{artist} - {tagHandler.Title}.mp3";
            copyFilePath = CreateSafePathForFile(artist, album, fileName);
            return copyFilePath;
        }

        private string CreateSafePathForFile(string artist, string album, string fileName)
        {
            var pathSafeArtist = artist;
            var pathSafeAlbum = album;
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                pathSafeAlbum = pathSafeAlbum.Replace(invalidChar, '_');
                pathSafeArtist = pathSafeArtist.Replace(invalidChar, '_');
            }
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
