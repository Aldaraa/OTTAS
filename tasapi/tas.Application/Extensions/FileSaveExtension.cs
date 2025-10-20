using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.CustomModel;

namespace tas.Application.Extensions
{



    public static class FileSaveExtension
    {
        public static FileSaveResultModel SaveImageFile(this IFormFile file)
        {
            try
            {
                  string[] AllowedExtensions = { "jpg", "jpeg", "png", "gif", "bmp" };
                if (file is null || file.Length == 0)
                {
                    FileSaveResultModel fmodel = new FileSaveResultModel();
                    fmodel.FileName = null;
                    fmodel.Status = false;
                    fmodel.Error = "File not found";
                    return fmodel;
                }
                else
                {
                    var folderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var extension = fileName.Split(".")[1];
                        var newFileName = Guid.NewGuid().ToString() +$"_{fileName.Trim()}";
                        if (AllowedExtensions.Contains(extension.ToLower()))
                        {
                            var fullPath = Path.Combine(pathToSave, newFileName);
                            var dbPath = Path.Combine(folderName, newFileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            FileSaveResultModel fmodel = new FileSaveResultModel();
                            fmodel.FileName = "/" +dbPath;
                            fmodel.Status = true;
                            return fmodel;
                        }
                        else {
                            //.
                            FileSaveResultModel fmodel = new FileSaveResultModel();
                            fmodel.FileName = null;
                            fmodel.Status = false;
                            fmodel.Error = "Invalid file type";
                            return fmodel;
                        }

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


        public static FileSaveResultModel SaveFile(this IFormFile file)
        {
            string[] allowedExtensions = {
                    "doc", "docx", "pdf", "txt", "rtf", "ppt", "pptx", "xls", "xlsx",
                    "jpg", "jpeg", "png", "gif", "bmp", "tiff", "svg", "eml", "msg"
                };
            var result = new FileSaveResultModel();

            if (file == null || file.Length == 0)
            {
                result.FileName = null;
                result.Status = false;
                result.Error = "File not found";
                return result;
            }

            try
            {
                // Generate a sanitized filename and validate the extension
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
                var newFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";

                if (!allowedExtensions.Contains(extension))
                {
                    result.FileName = null;
                    result.Status = false;
                    result.Error = "Not Allowed file. Unable to upload. Allowed extensions: " + string.Join(", ", allowedExtensions);
                    return result;
                }

                // Validate MIME type
                var mimeType = file.ContentType;
                if (!IsValidMimeType(mimeType, extension))
                {
                    result.FileName = null;
                    result.Status = false;
                    result.Error = "Invalid file type. Unable to upload";
                    return result;
                }

                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fullPath = Path.Combine(pathToSave, newFileName);
                var dbPath = Path.Combine(folderName, newFileName);


                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                result.FileName = $"\\{dbPath}";
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.FileName = null;
                result.Status = false;
                result.Error = ex.Message;
            }

            return result;
        }


        private static bool IsValidMimeType(string mimeType, string extension)
        {
            var mimeTypes = new Dictionary<string, List<string>>
            {
                { "doc", new List<string> { "application/msword" } },
                { "docx", new List<string> { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
                { "pdf", new List<string> { "application/pdf" } },
                { "txt", new List<string> { "text/plain" } },
                { "rtf", new List<string> { "application/rtf" } },
                { "ppt", new List<string> { "application/vnd.ms-powerpoint" } },
                { "pptx", new List<string> { "application/vnd.openxmlformats-officedocument.presentationml.presentation" } },
                { "xls", new List<string> { "application/vnd.ms-excel" } },
                { "xlsx", new List<string> { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { "jpg", new List<string> { "image/jpeg" } },
                { "jpeg", new List<string> { "image/jpeg" } },
                { "png", new List<string> { "image/png" } },
                { "gif", new List<string> { "image/gif" } },
                { "bmp", new List<string> { "image/bmp" } },
                { "tiff", new List<string> { "image/tiff" } },
                { "svg", new List<string> { "image/svg+xml" } },
                { "eml", new List<string> { "message/rfc822" } },
                { "msg", new List<string>
                    {
                        "application/vnd.ms-outlook",
                        "application/msoutlook",
                        "application/x-msg",
                        "application/octet-stream"
                    }
                },
            };

            return mimeTypes.TryGetValue(extension, out var expectedMimeTypes) && expectedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);
        }



    }
}
