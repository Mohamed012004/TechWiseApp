namespace TechWise.Domains.Exceptions.NotFound
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string Message) : base(Message)
        {

        }
    }
}
