using System.Text.Json.Serialization;

namespace HttpClientContextSample.Models
{
    public class TestContext
    {
        [JsonPropertyName("payload")]
        public Identity? Payload { get; set; }

        [JsonPropertyName("correlationId")]
        public string? CorrelationId { get; set; }

        [JsonPropertyName("items")]
        public Dictionary<string, object>? Items { get; set; }
    }
}
