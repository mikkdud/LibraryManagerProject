using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Login")]
    public string Login { get; set; } = null!;

    [ScaffoldColumn(false)] // ukrywa pole w formularzach i walidacji edycji
    public string? PasswordHash { get; set; }

    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Is administrator")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Borrowings")]
    public ICollection<Borrowing>? Borrowings { get; set; }
}
