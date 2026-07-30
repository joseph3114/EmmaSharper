namespace EmmaSharper
{
    public class UpdateField : BaseField
    {
        public UpdateField()
        {
            // `DisplayName = DisplayName` was here - a no-op, since this constructor takes no
            // parameters. Flagged by CodeQL as cs/self-assignment.
            FieldType = FieldType.Text;
            WidgetType = WidgetType.Text;
            ColumnOrder = 0;
        }
    }
}
