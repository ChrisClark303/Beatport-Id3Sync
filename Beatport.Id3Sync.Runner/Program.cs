using Beatport.Id3Sync.TagManager;
using Mp3Lib;
using System;
using System.Collections.Generic;
using Utils;

namespace Beatport.Id3Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> browseEnum = FileIterator.GetFiles(args[0], "*.mp3");
            foreach (var file in browseEnum)
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
                Console.WriteLine("-------------------------------");
            }

            Console.Read();
        }
    }
}
