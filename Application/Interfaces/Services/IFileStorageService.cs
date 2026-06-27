namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string?> SaveFileAsync(IFormFile file, string folderName);
    }
}
