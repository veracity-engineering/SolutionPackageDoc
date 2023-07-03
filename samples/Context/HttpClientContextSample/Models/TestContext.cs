namespace HttpClientContextSample.Models
{
    public class TestContext
    {
        public Identity? Payload { get; set; }

        public string? CorrelationId { get; set; }

        public Dictionary<string, object>? Items { get; set; }
    }
}
