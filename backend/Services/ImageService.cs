using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Inventory.Services
{
    public class ImageService : IImageService
    {
        private readonly string _wwwrootPath;

        public ImageService(IWebHostEnvironment env)
        {
            _wwwrootPath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"));
        }

        public async Task<string?> UploadImageAsync(IFormFile imageFile, string folderName = "images")
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Validate file type (optional, but good practice)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only image files are allowed.");

            // Create unique filename
            var fileName = Guid.NewGuid().ToString() + extension;
            var folderPath = Path.Combine(_wwwrootPath, folderName);
            Directory.CreateDirectory(folderPath); // Ensure folder exists

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative path for API serving
            return Path.Combine(folderName, fileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(_wwwrootPath, imagePath.Replace("/", "\\"));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public Task<string?> GetImagePathAsync(string imageName)
        {
            // Assuming imageName is the relative path
            return Task.FromResult<string?>(imageName);
        }
    }
}