namespace gemini.Models;

public class CharacterDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CurrentWeight { get; set; }
    public int MaxWeight { get; set; }
    public List<ItemDto> BackpackItems { get; set; }
    public List<TitleDto> Titles { get; set; }
}