using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Serialization;
using System.Threading.Tasks;
using WOPIHost.Models;
using WOPIHost.Services;
using Microsoft.Extensions.Configuration; // Add this for IConfiguration

public class WopiDiscoveryService
{
    private readonly IConfiguration _configuration;
    private readonly FileService _fileService;  // Declare a private FileService field
    private readonly HttpClient _httpClient;    // Declare a private HttpClient field

    // Constructor now accepts FileService and HttpClient as dependencies
    public WopiDiscoveryService(IConfiguration configuration, FileService fileService, HttpClient httpClient)
    {
        _configuration = configuration;
        _fileService = fileService;  // Initialize the FileService
        _httpClient = httpClient;    // Initialize HttpClient
    }

    public string GenerateDocumentUrl(string fileId, string fileUrl)
    {
        // Get the file extension dynamically from the file ID using FileService
        var fileExtension = _fileService.GetFileExtension(fileId);

        if (string.IsNullOrEmpty(fileExtension))
            return null;

        // Load WOPI discovery data from the remote URL
        var discoveryData = GetDiscoveryDataAsync().Result; // Await async operation synchronously (caution: can block thread)

        var app = discoveryData.NetZone.Apps
            .FirstOrDefault(a => a.Actions.Any(action => action.Extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)));

        if (app == null)
            return null;

        // Find the corresponding action URL for the file extension
        var action = app.Actions.FirstOrDefault(a => a.Extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

        if (action != null)
        {
            // Generate the document URL
            return $"{action.Urlsrc}?WOPISrc={fileUrl}&access_token=sample-token";
        }

        return null;
    }

    private async Task<WopiDiscovery> GetDiscoveryDataAsync()
    {
        // The URL for the WOPI discovery XML file
        string discoveryUrl = _configuration["WopiDiscoveryUrl"] ?? "http://192.168.0.109:9980/hosting/discovery"; // You can store the URL in appsettings.json

        // Fetch the XML content using HttpClient
        var xmlContent = await _httpClient.GetStringAsync(discoveryUrl);

        // Initialize an XmlSerializer for WopiDiscovery
        var serializer = new XmlSerializer(typeof(WopiDiscovery));

        using (var reader = new StringReader(xmlContent))
        {
            // Deserialize the XML data into a WopiDiscovery object
            return (WopiDiscovery)serializer.Deserialize(reader);
        }
    }
}
