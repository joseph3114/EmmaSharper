using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class SubscriptionImportBulk
    {
        [JsonPropertyName("import_id")]
        public long ImportId { get; set; }
    }
}
