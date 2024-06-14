using gemini.Models;

namespace gemini.Services;

public interface IDbService
{
    Task<CharacterDto> GetCharacterDetails(int CharacterId);
    Task<bool> DoesCharacterExist(int CharacterId);

    Task<List<BackpackDto>> AddItemsToCharacter(int characterId, List<int> itemIds);

    Task<bool> DoesItemExist(int itemId);

    Task<bool> DoesCharacterHasEnoughMaxWeight(int characterId, List<Item> items);
    
}