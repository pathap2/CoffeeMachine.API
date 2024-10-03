using System.ComponentModel.DataAnnotations;

namespace CoffeeMachine.API.Models.Request;

public class CoffeeRequest
{
    [Key]
    public int Id { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Request count must be a non-negative number.")]
    public int RequestCount { get; set; }

    [Required(ErrorMessage = "The LastRequestDate field is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:sszzz}", ApplyFormatInEditMode = true)]
    public DateTime LastRequestDate { get; set; }
}