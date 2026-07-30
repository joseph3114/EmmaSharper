using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class CreateField : BaseField
    {
        [JsonPropertyName("shortcut_name")]
        public string ShortcutName { get; set; }

        public CreateField()
        {
            ShortcutName = ShortcutName;
            DisplayName = DisplayName;
            FieldType = FieldType.Text;
            WidgetType = WidgetType.Text;
            ColumnOrder = 0;
        }
    }
}
