using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IDbService
    {
        Task<bool> DoesPersonExists(int characterId);
        Task<IEnumerable<CharacterInfoDto>> GetPersonInfo(int characterId);
        Task AddItemsToPerson(int characterId, List<int> itemIds);
        Task<bool> CanPersonCarryMore(int characterId, List<int> itemIds);
        Task<bool> DoesItemExist(int itemId);
        Task UpdatePersonsWeight(int characterId, List<int> itemIds);
    }
}