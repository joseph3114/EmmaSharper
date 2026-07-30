using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Mailing : MailingBase
    {
        [JsonPropertyName("recipient_groups")]
        public List<MailingGroup>? RecipientGroups { get; set; }

        [JsonPropertyName("heads_up_emails")]
        public List<Email>? HeadsUpEmails { get; set; }

        [JsonPropertyName("links")]
        public List<Link>? Links { get; set; }

        [JsonPropertyName("public_webview_url")]
        public string? PublicWebviewUrl { get; set; }

        [JsonPropertyName("recipient_searches")]
        public List<Search>? RecipientSearches { get; set; }

        [JsonPropertyName("recipient_members")]
        public List<Member>? RecipientMembers { get; set; }

        [JsonPropertyName("plaintext")]
        public string? Plaintext { get; set; }

        [JsonPropertyName("html_body")]
        public string? HtmlBody { get; set; }
    }
}
