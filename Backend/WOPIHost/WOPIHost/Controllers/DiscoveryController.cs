// DiscoveryController.cs
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WOPIHost.Controllers
{
    [ApiController]
    [Route("hosting/discovery")]
    public class DiscoveryController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetDiscoveryFile()
        {
            var discoveryXml = @"
<?xml version=""1.0"" encoding=""UTF-8""?>
<wopi-discovery xmlns=""http://schemas.microsoft.com/wopidiscovery"">
    <net-zone name=""external"">
        <urlsrc name=""web-app"">
            http://192.168.0.109:9980/loleaflet/dist/loleaflet.html?WOPISrc={0}&access_token={1}
        </urlsrc>
    </net-zone>
</wopi-discovery>";

            // URL encode the WOPISrc and access token
            string wopiSrc = Uri.EscapeDataString("http://192.168.0.109:5050/wopi/files/{file_id}");
            string accessToken = Uri.EscapeDataString("sample-token");

            // Replace the placeholders with the actual encoded URLs
            discoveryXml = string.Format(discoveryXml, wopiSrc, accessToken);

            return Content(discoveryXml, "application/xml", Encoding.UTF8);
        }
    }
}
