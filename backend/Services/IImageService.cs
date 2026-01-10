using Microsoft.AspNetCore.Http;

namespace Inventory.Services
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile imageFile, string folderName = "images");
        Task<bool> DeleteImageAsync(string imagePath);
        Task<string?> GetImagePathAsync(string imageName);
    }
}