using gemini.Data;
using gemini.Models;
using gemini.Services;
using Microsoft.AspNetCore.Mvc;

namespace gemini.Controllers;
[ApiController]
[Route("api/characters")]
public class DbController : ControllerBase
{
    private readonly IDbService _dbService;
    private readonly DatabaseContext _context;

    public DbController(IDbService dbService, DatabaseContext context)
    {
        _dbService = dbService;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacterDetails(int id)
    {
        try
        {
            return Ok(await _dbService.GetCharacterDetails(id));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{characterId}/backpacks")]
    public async Task<IActionResult> AddItemsToBackpack(int characterId, [FromBody] List<int> itemIds)
    {
        if (!await _dbService.DoesCharacterExist(characterId))
            return NotFound("Character doesn't exist");
        List<Item> items = new List<Item>();
        foreach (var id in itemIds)
        {
            if (await _dbService.DoesItemExist(id))
            {
                items.Add(await _context.Items.FindAsync(id));
            }
            else
            {
                return NotFound($"Item with id - {id} doesn't exist");
            }
        }

        if (!await _dbService.DoesCharacterHasEnoughMaxWeight(characterId, items))
            return BadRequest($"Character with id {characterId} doesn't have enough capacity");

        return Ok(await _dbService.AddItemsToCharacter(characterId, itemIds));
    }
}