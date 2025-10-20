using System.Net.Http.Headers;
using tas.Domain.CustomModel;

namespace tas.WebAPI.Extensions
{
    public static class FileSaveExtension
    {
        public static FileSaveResultModel SaveFile(this IFormFile file)
        {
            try
            {

                if (file is null || file.Length == 0)
                {
                    FileSaveResultModel fmodel = new FileSaveResultModel();
                    fmodel.FileName = null;
                    fmodel.Status = false;
                    fmodel.Error = "File not found";
                    return fmodel;
                }
                else {
                    var folderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var extension = fileName.Split(".")[1];
                        var newFileName = Guid.NewGuid().ToString() + $".{extension}";
                        var fullPath = Path.Combine(pathToSave, newFileName);
                        var dbPath = Path.Combine(folderName, newFileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        FileSaveResultModel fmodel = new FileSaveResultModel();
                        fmodel.FileName = $@"\{dbPath}";
                        fmodel.Status = true;
                        return fmodel;
                    }
                    else
                    {
                        FileSaveResultModel fmodel = new FileSaveResultModel();
                        fmodel.FileName = null;
                        fmodel.Status = false;
                        fmodel.Error = "File not found";
                        return fmodel;
                    }
                }
          
            }
            catch (Exception ex)
            {
                FileSaveResultModel fmodel = new FileSaveResultModel();
                fmodel.FileName = null;
                fmodel.Status = false;
                fmodel.Error = ex.Message;
                return fmodel;

            }
        }

    }
}
