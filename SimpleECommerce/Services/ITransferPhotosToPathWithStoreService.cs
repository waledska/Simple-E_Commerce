namespace SimpleECommerce.Services
{
    public interface ITransferPhotosToPathWithStoreService
    {
        string GetPhotoPath(IFormFile model);
        List<string> GetPhotosPath(List<IFormFile> model);
        bool DeleteFile(string path);
    }
}
