using System.Threading.Tasks;

namespace Beatport.Id3Sync.TagManager
{
    public interface ITagProcessor
    {
        Task ProcessFile(string file);
    }
}