using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly string _filesDir = Path.Combine(Directory.GetCurrentDirectory(), "files");

    [HttpGet]
    public IActionResult GetFiles()
    {
        try
        {
            if (!Directory.Exists(_filesDir))
            {
                return StatusCode(500, new { error = "Files directory not found" });
            }

            var files = Directory.GetFiles(_filesDir);
            var fileList = new List<object>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.StartsWith(".")) continue; // Exclude hidden files

                fileList.Add(new
                {
                    Name = fileInfo.Name,
                    Size = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime,
                    IsDirectory = false
                });
            }

            return Ok(fileList);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading files directory: {ex}");
            return StatusCode(500, new { error = "Unable to read files directory" });
        }
    }
}
