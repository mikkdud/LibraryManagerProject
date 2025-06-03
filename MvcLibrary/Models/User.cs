using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Login")]
    public string Login { get; set; } = null!; // wycisza warning kompilatora

    [Required]
    [Display(Name = "Password hash")]
    public string PasswordHash { get; set; } = null!; 

    [Display(Name = "API token")]
    public string Token { get; set; } = "";

    [Display(Name = "Is administrator")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Borrowings")]
    public ICollection<Borrowing>? Borrowings { get; set; }
}
