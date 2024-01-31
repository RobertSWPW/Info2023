﻿using System.Drawing;
using LazZiya.ImageResize;

namespace Info.Infrastructure
{
    public class ImageFileUpload
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public ImageFileUpload(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public FileSendResult SendFile(IFormFile picture, string destination, int width)
        {
            FileSendResult SendingFile = new();

            string extension = Path.GetExtension(picture.FileName);
            if (!FileTypeCheck(extension))
            {
                SendingFile.Name = Path.GetFileName(picture.FileName);
                SendingFile.Success = false;
                SendingFile.Error = "Niepoprawny typ pliku graficznego.";
                return SendingFile;
            }

            //wygenerowanie nazwy i ustalenie ścieżki docelowej
            SendingFile.Name = Guid.NewGuid().ToString() + extension;
            var upload = Path.Combine(hostingEnvironment.WebRootPath, destination);
            var filePath = Path.Combine(upload, SendingFile.Name);

            //przesłanie pliku na serwer
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                picture.CopyTo(fileStream);
            }

            //Skalowanie obrazka i zapisanie w podkatalogu
            string path = Path.Combine(filePath);

            using (var imgFile = Image.FromFile(path))
            {
                var miniFile = imgFile.ScaleByWidth(width);
                upload = Path.Combine(hostingEnvironment.WebRootPath, destination + "\\mini");
                filePath = Path.Combine(upload, SendingFile.Name);
                miniFile.SaveAs(filePath);
            }

            SendingFile.Success = true;
            return SendingFile;
        }

        private static bool FileTypeCheck(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".png" or ".gif" => true,
                _ => false,
            };
        }
    }
}
