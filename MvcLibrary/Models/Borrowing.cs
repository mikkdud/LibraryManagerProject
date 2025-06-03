using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models;

public class Borrowing
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "User")]
    public required User User { get; set; }

    [Display(Name = "Volume")]
    public required Volume Volume { get; set; }

    [Display(Name = "Borrowed at")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BorrowedAt { get; set; }

    [Display(Name = "Returned at")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? ReturnedAt { get; set; }
}
