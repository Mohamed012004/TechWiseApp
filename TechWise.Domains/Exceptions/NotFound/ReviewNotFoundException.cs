namespace TechWise.Domains.Exceptions.NotFound
{
    public class ReviewNotFoundException() : NotFoundException($"Review not found")
    {
    }
}
