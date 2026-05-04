namespace TechWise.Domains.Exceptions.NotFound
{
    public class UserNotFoundException(string email) : NotFoundException($"User with Email {email} was Not Found !!")
    {

    }
}
