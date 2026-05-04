namespace TechWise.Domains.Exceptions.NotFound
{
    public class AdminNotFoundException(int id) : NotFoundException($"Admin with Id {id} was Not Found !!")
    {

    }
}
