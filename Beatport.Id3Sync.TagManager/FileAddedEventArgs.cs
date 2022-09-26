using System;

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
}
