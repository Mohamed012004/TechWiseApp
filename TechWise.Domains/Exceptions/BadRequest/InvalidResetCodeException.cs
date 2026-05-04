
namespace TechWise.Domains.Exceptions.BadRequest
{
    public class InvalidResetCodeException()
        : BadRequestException("Invalid or expired reset code")
    {
    }
}