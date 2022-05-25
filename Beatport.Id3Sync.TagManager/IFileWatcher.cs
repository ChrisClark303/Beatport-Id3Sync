using System;

namespace Beatport.Id3Sync.TagManager
{
    public interface IFileWatcher
    {
        event EventHandler<FileAddedEventArgs> FileAdded;

        void Start();
    }
}