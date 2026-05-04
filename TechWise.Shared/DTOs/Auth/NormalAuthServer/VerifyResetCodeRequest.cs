using System.ComponentModel.DataAnnotations;

public class VerifyResetCodeRequest
{
    [Required(ErrorMessage = "Code is required")]
    [StringLength(6, MinimumLength = 6,
        ErrorMessage = "Code must be 6 digits")]
    public string Code { get; set; }
}