using System.Text.Json.Serialization;

namespace HttpClientContextSample.Models
{
    public class Identity
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }
    }
}