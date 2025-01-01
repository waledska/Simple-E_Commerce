namespace SimpleECommerce.Services
{
    public interface ITransferPhotosToPathWithStoreService
    {
        /// <summary>
        /// Asynchronously stores the input image in project 's files
        /// </summary>
        /// <param name="model">that is the IFormFile for the image</param>
        /// <returns>the new stored image's path</returns>
        Task<string> GetPhotoPathAsync(IFormFile model);
        /// <summary>
        /// Asynchronously stores the input List of images in project 's files
        /// </summary>
        /// <param name="model">that is List of IFormFile for the images</param>
        /// <returns>the new stored image's pathes in List of strings</returns>
        Task<List<string>> GetPhotosPathAsync(List<IFormFile> model);
        /// <summary>
        /// Asynchronously deletes a file at the specified path.
        /// </summary>
        /// <param name="path">image path to delete</param>
        /// <returns>true when the file is deleted, false when it is not</returns>
        Task<bool> DeleteFileAsync(string path);
    }
}
