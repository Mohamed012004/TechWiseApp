
namespace TechWise.Services.Abstractions.Files
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
        Task DeleteImageAsync(string publicId);
    }
}