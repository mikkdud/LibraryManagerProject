using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class Volume
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Inventory number")]
    public string InventoryNumber { get; set; } = null!;

    [Display(Name = "Is available")]
    public bool IsAvailable { get; set; } = true;

    // NIE wymagaj Book – to tylko na potrzeby EF (Include itp.)
    public Book? Book { get; set; }

    [Required]
    [Display(Name = "BookID")]
    public int BookId { get; set; } 
}
