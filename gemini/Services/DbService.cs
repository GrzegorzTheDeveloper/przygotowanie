using gemini.Data;
using gemini.Models;
using Microsoft.EntityFrameworkCore;

namespace gemini.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<CharacterDto> GetCharacterDetails(int CharacterId)
    {
        if (!await DoesCharacterExist(CharacterId))
            throw new InvalidOperationException("Character doesnt exist");

        Character character = await _context.Characters
            .Include(e => e.Backpacks)
            .Include(e => e.CharacterTitles)
            .ThenInclude(e => e.Title).SingleOrDefaultAsync(e => e.Id == CharacterId);
        return new CharacterDto()
        {
            FirstName = character.FirstName,
            LastName = character.LastName,
            CurrentWeight = character.CurrentWeight,
            MaxWeight = character.MaxWeight,
            BackpackItems = _context.Backpacks.Select(e => new ItemDto()
            {
                ItemName = e.Item.Name,
                ItemWeight = e.Item.Weight,
                amount = e.Item.Backpacks.Count
            }).ToList(),
            Titles = _context.CharacterTitles.Select(e => new TitleDto()
            {
                title = e.Title.Name,
                AcquiredAt = e.AcquiredAt
            }).ToList()
        };
    }

    public async Task<bool> DoesCharacterExist(int CharacterId)
    {
        return await _context.Characters.AnyAsync(e => e.Id == CharacterId);
    }

    public async Task<bool> DoesItemExist(int itemId)
    {
        return await _context.Items.AnyAsync(e => e.Id == itemId);
    }


    public async Task<bool> DoesCharacterHasEnoughMaxWeight(int characterId, List<Item> items)
    {
        var character = await _context.Characters
            .FirstOrDefaultAsync(e => e.Id == characterId);
        var maxWeight = character.MaxWeight;
        var currentWeight = character.CurrentWeight;
        foreach (var item in items)
        {
            currentWeight += item.Weight;
            if (currentWeight > maxWeight)
                throw new InvalidOperationException("Weight limit exceeded");
        }

        return true;
    }


    public async Task<List<BackpackDto>> AddItemsToCharacter(int characterId, List<int> itemIds)
    {
        var character = await _context.Characters
            .Include(c => c.Backpacks)
            .FirstOrDefaultAsync(e => e.Id == characterId);
        if (character == null)
            throw new InvalidOperationException("Character doesn't exist");

        List<BackpackDto> backpacks = new List<BackpackDto>();
        
        var items = await _context.Items
            .Where(item => itemIds.Contains(item.Id))
            .ToListAsync();
        
        if (items.Count != itemIds.Count)
            throw new InvalidOperationException("One or more items do not exist");

        foreach (var item in items)
        {
            character.CurrentWeight += item.Weight;

            
            var backpack = character.Backpacks.FirstOrDefault(e => e.ItemId == item.Id);
            if (backpack == null)
            {
                var newBackpack = new Backpack
                {
                    CharacterId = characterId,
                    ItemId = item.Id,
                    Amount = 1
                };
                _context.Backpacks.Add(newBackpack);
                backpacks.Add(new BackpackDto()
                {
                    characterId = characterId,
                    itemId = item.Id,
                    amount = 1
                });
            }
            else
            {
                backpack.Amount += 1;
                _context.Backpacks.Update(backpack);
                backpacks.Add(new BackpackDto()
                {
                    characterId = characterId,
                    itemId = item.Id,
                    amount = backpack.Amount
                });
            }
        }
        
        _context.Characters.Update(character);
        await _context.SaveChangesAsync();

        return backpacks;
    }
}