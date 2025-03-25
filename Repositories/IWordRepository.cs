using System.Net.Mime;
using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordRepository : IBaseRepository<WordEntity>
{
    Task<List<WordEntity>> GetReviewingWordsByGuidsAsync(List<Guid> reviewingWordIds);
    Task<List<WordEntity>> GetAllWordsAsync(int index, int size, Guid? keyword);
    Task<int> GetTotalWordsAsync(Guid? keyword);
    Task<MemoryStream> GetWordMnemonicImage(string fileName);
    Task<MemoryStream> GetWordAudio(string fileName);
}