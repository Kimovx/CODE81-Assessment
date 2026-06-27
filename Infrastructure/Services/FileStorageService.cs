using CODE81_Assessment.Application.Interfaces.Services;

namespace CODE81_Assessment.Infrastructure.Services
{
    public class FileStorageService(IWebHostEnvironment env) : IFileStorageService
    {
        private readonly IWebHostEnvironment _env = env;

        public async Task<string?> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folderName}/{fileName}";
        }
    }
}
