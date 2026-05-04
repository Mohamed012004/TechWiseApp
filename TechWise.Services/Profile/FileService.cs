using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TechWise.Services.Abstractions.Files;
using TechWise.Shared.Settings;

namespace TechWise.Services.Files
{
    public class FileService(IOptions<CloudinarySettings> options) : IFileService
    {
        private readonly Cloudinary _cloudinary = new Cloudinary(new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret));

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, imageStream),
                Folder = "TechWise/Profiles",
                Transformation = new Transformation()
                    .Width(300).Height(300).Crop("fill")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                throw new Exception(result.Error.Message);

            return result.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}