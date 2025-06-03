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

    [Required]
    [Display(Name = "Book")]
    public Book Book { get; set; } = null!; 
}
