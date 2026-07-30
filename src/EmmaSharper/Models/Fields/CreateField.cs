using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class CreateField : BaseField
    {
        [JsonPropertyName("shortcut_name")]
        public string? ShortcutName { get; set; }

        public CreateField()
        {
            // `ShortcutName = ShortcutName` and `DisplayName = DisplayName` were here. Both are
            // no-ops - this constructor takes no parameters, so there was nothing to assign from.
            // Flagged by CodeQL as cs/self-assignment; removing them changes no behaviour.
            FieldType = FieldType.Text;
            WidgetType = WidgetType.Text;
            ColumnOrder = 0;
        }
    }
}
