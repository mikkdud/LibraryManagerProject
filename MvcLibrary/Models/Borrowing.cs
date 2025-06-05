using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class Borrowing
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public int VolumeId { get; set; }
    public Volume Volume { get; set; } = null!;

    [Display(Name = "Borrowed at")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BorrowedAt { get; set; }

    [Display(Name = "Returned at")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? ReturnedAt { get; set; }
}
