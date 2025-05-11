// FileService.cs
using WOPIHost.Models;
using System.IO;
using System;

namespace WOPIHost.Services
{
    public class FileService
    {
        private readonly string fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

        public FileService()
        {
            Directory.CreateDirectory(fileDirectory);
        }

        public byte[] ReadFile(string fileId)
        {
            string path = GetFilePath(fileId);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public void WriteFile(string fileId, byte[] data)
        {
            string path = GetFilePath(fileId);
            File.WriteAllBytes(path, data);
        }

        public WopiFileInfo GetFileInfo(string fileId)
        {
            string path = GetFilePath(fileId);

            if (!File.Exists(path))
                return null;

            var fileInfo = new FileInfo(path);
            return new WopiFileInfo
            {
                BaseFileName = fileInfo.Name,
                Size = fileInfo.Length,
                OwnerId = "ali",  // Implement logic for file owner
                UserId = "ali",    // Implement logic for current user
                UserFriendlyName = "Ali",
                UserCanWrite = true,
                Version = fileInfo.LastWriteTimeUtc.Ticks.ToString(),
                LastModifiedTime = fileInfo.LastWriteTimeUtc.ToString("o")
            };
        }

        public bool FileExists(string fileId)
        {
            return File.Exists(GetFilePath(fileId));
        }

        public void RenameFile(string fileId, string newFileName)
        {
            string oldPath = GetFilePath(fileId);
            string newPath = Path.Combine(fileDirectory, newFileName);

            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }
        }

        // Extract the file extension from the fileId (which could be the file name)
        public string GetFileExtension(string fileId)
        {
            return Path.GetExtension(fileId)?.TrimStart('.')?.ToLower(); // Remove the leading '.' and ensure lowercase
        }



        private string GetFilePath(string fileId)
        {
            return Path.Combine(fileDirectory, fileId);
        }
    }
}
