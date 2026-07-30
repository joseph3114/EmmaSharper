using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class BaseField
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("field_type")]
        public FieldType FieldType { get; set; }

        [JsonPropertyName("widget_type")]
        public WidgetType WidgetType { get; set; }

        [JsonPropertyName("column_order")]
        public int? ColumnOrder { get; set; }
    }
}
