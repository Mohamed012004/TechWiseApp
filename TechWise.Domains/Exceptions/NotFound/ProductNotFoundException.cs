namespace TechWise.Domains.Exceptions.NotFound
{
    public class ProductNotFoundException(int id)
        : NotFoundException($"Product with Id {id} was Not Found !!")
    {
    }
}