namespace TechWise.Domains.Exceptions.NotFound
{
    public class OrderNotFoundException(int id) : NotFoundException($"Order with Id {id} was Not Found !!")
    {
    }
}
