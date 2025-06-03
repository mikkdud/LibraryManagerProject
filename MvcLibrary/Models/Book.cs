using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!; 

    [Required]
    [Display(Name = "Author")]
    public string Author { get; set; } = null!; 

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Volumes")]
    public ICollection<Volume>? Volumes { get; set; }
}
