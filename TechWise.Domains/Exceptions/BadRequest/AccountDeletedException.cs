
namespace TechWise.Domains.Exceptions.BadRequest
{
    public class AccountDeletedException()
        : BadRequestException("This account has been deleted")
    {
    }
}