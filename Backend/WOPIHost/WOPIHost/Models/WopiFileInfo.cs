// WopiFileInfo.cs
using System.Text.Json.Serialization;

namespace WOPIHost.Models
{
    public class WopiFileInfo
    {
        [JsonPropertyName("BaseFileName")]
        public string BaseFileName { get; set; }
        [JsonPropertyName("Size")]
        public long Size { get; set; }
        [JsonPropertyName("OwnerId")]
        public string OwnerId { get; set; }
        [JsonPropertyName("Version")]
        public string Version { get; set; }
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }
        [JsonPropertyName("UserFriendlyName")]
        public string UserFriendlyName { get; set; }

        [JsonPropertyName("LastModifiedTime")]
        public string LastModifiedTime { get; set; }
        [JsonPropertyName("UserCanWrite")]
        public bool UserCanWrite { get; set; }
    }
}
