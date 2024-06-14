using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace gemini.Models;
[Table("backpacks")]
[PrimaryKey(nameof(CharacterId), nameof(ItemId))]
public class Backpack
{
    
    public int CharacterId { get; set; }

    public int ItemId { get; set; }
    
    public int Amount { get; set; }
    [ForeignKey(nameof(CharacterId))]
    public Character Characters  { get; set; }
    [ForeignKey(nameof(ItemId))]
    public Item Item { get; set; }
}