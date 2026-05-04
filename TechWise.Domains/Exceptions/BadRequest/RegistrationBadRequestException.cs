namespace TechWise.Domains.Exceptions.BadRequest
{
    public class RegistrationBadRequestException(List<string> errors) : BadRequestException(string.Join(",", errors))
    {
    }
}
