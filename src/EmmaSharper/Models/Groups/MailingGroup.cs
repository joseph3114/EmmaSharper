using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MailingGroup : Group
    {
        //Ugh. API names GroupName differently in Mailings than elsewhere.
        [JsonPropertyName("name")]
        public new string GroupName { get; set; }
    }
}
