using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Add this for ILogger
using WOPIHost.Models;
using WOPIHost.Services;
using System;
using System.IO;
using System.Linq;

namespace WOPIHost.Controllers
{
    [ApiController]
    [Route("wopi/files")]
    public class WopiController : ControllerBase
    {
        private readonly FileService _fileService;
        private readonly WopiDiscoveryService _wopiDiscoveryService;
        private readonly ILogger<WopiController> _logger;  // Declare ILogger for logging

        // Constructor accepts ILogger as a dependency
        public WopiController(FileService fileService, WopiDiscoveryService wopiDiscoveryService, ILogger<WopiController> logger)
        {
            _fileService = fileService;
            _wopiDiscoveryService = wopiDiscoveryService;
            _logger = logger;
        }

        // Token Validation with logging
        private bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is null or empty.");
                return false;
            }

            if (token != "sample-token")
            {
                _logger.LogWarning($"Token validation failed. Expected: 'sample-token', Found: '{token}'");
                return false;
            }

            return true;
        }

        // Step 1: CheckFileInfo with error handling and logging
        [HttpGet("{fileId}")]
        public IActionResult CheckFileInfo(string fileId, [FromQuery] string access_token)
        {
            try
            {
                _logger.LogInformation($"Checking file info for fileId: {fileId}");

                if (!ValidateToken(access_token))
                {
                    _logger.LogWarning("Unauthorized access attempt.");
                    return Unauthorized();
                }

                if (!_fileService.FileExists(fileId))
                {
                    _logger.LogWarning($"File not found: {fileId}");
                    return NotFound();
                }

                var fileInfo = _fileService.GetFileInfo(fileId);
                _logger.LogInformation($"File info retrieved: {fileInfo.BaseFileName}");
                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking file info.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Step 2: GetFile (Download) with error handling and logging
        [HttpGet("{fileId}/contents")]
        public IActionResult GetFileContents(string fileId, [FromQuery] string access_token)
        {
            try
            {
                _logger.LogInformation($"Downloading file contents for fileId: {fileId}");

                if (!ValidateToken(access_token))
                {
                    _logger.LogWarning("Unauthorized access attempt.");
                    return Unauthorized();
                }

                var data = _fileService.ReadFile(fileId);
                if (data == null)
                {
                    _logger.LogWarning($"File not found: {fileId}");
                    return NotFound();
                }

                _logger.LogInformation($"File downloaded successfully: {fileId}");
                return File(data, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading file contents.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Step 3: PutFile (Upload) with error handling and logging
        [HttpPost("{fileId}/contents")]
        public async Task<IActionResult> PutFileContents(string fileId, [FromQuery] string access_token)
        {
            try
            {
                _logger.LogInformation($"Uploading file contents for fileId: {fileId}");

                if (!ValidateToken(access_token))
                {
                    _logger.LogWarning("Unauthorized access attempt.");
                    return Unauthorized();
                }

                using var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);  // Use async copy
                _fileService.WriteFile(fileId, ms.ToArray());

                _logger.LogInformation($"File uploaded successfully: {fileId}");
                return Ok(new { message = "File uploaded successfully", lastModified = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file contents.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Step 4: RenameFile (Optional) with error handling and logging
        [HttpPost("{fileId}")]
        public IActionResult RenameFile(string fileId, [FromQuery] string access_token, [FromHeader] string X_WOPI_Override, [FromHeader] string X_WOPI_RequestedName)
        {
            try
            {
                _logger.LogInformation($"Renaming file: {fileId} to {X_WOPI_RequestedName}");

                if (X_WOPI_Override != "RENAME_FILE")
                {
                    _logger.LogWarning("Invalid operation for renaming.");
                    return BadRequest("Invalid operation.");
                }

                if (!ValidateToken(access_token))
                {
                    _logger.LogWarning("Unauthorized access attempt.");
                    return Unauthorized();
                }

                _fileService.RenameFile(fileId, X_WOPI_RequestedName);

                _logger.LogInformation($"File renamed successfully: {fileId} to {X_WOPI_RequestedName}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while renaming the file.");
                return StatusCode(500, "Internal server error");
            }
        }

        // New Endpoint: Generate Document URL with error handling and logging
        [HttpGet("generate-url")]
        public IActionResult GenerateDocumentUrl([FromQuery] string fileId, [FromQuery] string access_token)
        {
            try
            {
                _logger.LogInformation($"Generating document URL for fileId: {fileId}");

                if (!ValidateToken(access_token))
                {
                    _logger.LogWarning("Unauthorized access attempt.");
                    return Unauthorized();
                }

                // Construct the base URL
                var fileUrl = $"http://192.168.0.109:5050/wopi/files/{fileId}";

                // Generate the document URL based on the file extension
                var url = _wopiDiscoveryService.GenerateDocumentUrl(fileId, fileUrl);

                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogWarning($"No matching action found for file extension for fileId: {fileId}");
                    return NotFound("No matching action found for the file extension.");
                }

                _logger.LogInformation($"Generated document URL: {url}");
                return Ok(new { documentUrl = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating document URL.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
