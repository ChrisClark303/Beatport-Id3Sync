using Beatport.Id3Sync.TagManager;
using System;

namespace Beatport.Id3Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new TagDataManager();
            //manager.WriteTagData(args[0]);

            Console.Read();
        }
    }
}
