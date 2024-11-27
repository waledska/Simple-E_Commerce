using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace SimpleECommerce.Services
{
    public class TransferPhotosToPathWithStoreService : ITransferPhotosToPathWithStoreService
    {
        // Upload a single image
        public string GetPhotoPath(IFormFile model)
        {
            if (model == null || model.Length == 0)
                return "error, IFormFile model can't be empty";

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
            var imagesFolderName = "Images";

            // Validate file type
            var fileExtension = Path.GetExtension(model.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return "error, invalid file format";

            // Validate file size
            if (model.Length > maxFileSizeInBytes)
                return "error, image size can't be bigger than 10MB";

            // if you want to valid that all images in square dimention
            // try
            // {
            //     // Check if image is square using ImageSharp
            //     using (var image = Image.Load(model.OpenReadStream(), out IImageFormat format))
            //     {
            //         if (image.Width != image.Height)
            //             return "error, image is not square in dimensions";
            //     }
            // }
            // catch (Exception)
            // {
            //     return "error, invalid image file";
            // }

            string uniquePhotoName = Guid.NewGuid() + fileExtension;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolderName, uniquePhotoName);

            try
            {
                // Save the image
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                return "error, something went wrong!";
            }
        }

        // Upload multiple images
        public List<string> GetPhotosPath(List<IFormFile> model)
        {
            var resultPaths = new List<string>();
            if (model == null || !model.Any())
            {
                resultPaths.Add("error, no files provided");
                return resultPaths;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
            var imagesFolderName = "Images";

            foreach (var file in model)
            {
                if (file == null || file.Length == 0)
                {
                    resultPaths.Add("error, empty file");
                    continue;
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    resultPaths.Add("error, invalid file format");
                    continue;
                }

                if (file.Length > maxFileSizeInBytes)
                {
                    resultPaths.Add("error, image size can't be bigger than 10MB");
                    continue;
                }

                // if you want to valid that all images in square dimention
                // try
                // {
                //     using (var image = Image.Load(file.OpenReadStream(), out IImageFormat format))
                //     {
                //         if (image.Width != image.Height)
                //         {
                //             resultPaths.Add("error, image is not square");
                //             continue;
                //         }
                //     }
                // }
                // catch (Exception)
                // {
                //     resultPaths.Add("error, invalid image file");
                //     continue;
                // }

                string uniquePhotoName = Guid.NewGuid() + fileExtension;
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolderName, uniquePhotoName);

                try
                {
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    resultPaths.Add(fullPath);
                }
                catch (Exception ex)
                {
                    resultPaths.Add("error, something went wrong!");
                }
            }

            // if (!resultPaths.Any(path => path.StartsWith("error")))
            //     resultPaths.Add("success");

            return resultPaths;
        }

        // Delete a file
        public bool DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch (Exception ex)
                {
                    // Error deleting file
                    return false;
                }
            }
            // file mot found 
            return false;
        }
    }
}
