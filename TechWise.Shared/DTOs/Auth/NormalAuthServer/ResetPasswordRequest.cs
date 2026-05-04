using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required]
    public string ResetToken { get; set; }

    [Required]
    [MinLength(7)]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmNewPassword { get; set; }
}