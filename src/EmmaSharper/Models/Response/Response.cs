using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class ResponseBase
    {
        [JsonPropertyName("sent")]
        public int? Sent { get; set; }

        [JsonPropertyName("delivered")]
        public int? Delivered { get; set; }

        [JsonPropertyName("bounced")]
        public int? Bounced { get; set; }

        [JsonPropertyName("opened")]
        public int? Opened { get; set; }

        [JsonPropertyName("clicked")]
        public int? Clicked { get; set; }

        [JsonPropertyName("clicked_unique")]
        public int? ClickedUnique { get; set; }

        [JsonPropertyName("forwarded")]
        public int? Forwarded { get; set; }

        [JsonPropertyName("shared")]
        public int? Shared { get; set; }

        [JsonPropertyName("share_clicked")]
        public int? ShareClicked { get; set; }

        [JsonPropertyName("webview_shared")]
        public int? WebviewShared { get; set; }

        [JsonPropertyName("webview_share_clicked")]
        public int? WebviewShareClicked { get; set; }

        [JsonPropertyName("signed_up")]
        public int? SignedUp { get; set; }

        [JsonPropertyName("opted_out")]
        public int? OptedOut { get; set; }

        [JsonPropertyName("count_purchased")]
        public int? CountPurchased { get; set; }

        [JsonPropertyName("sum_purchased")]
        public decimal? SumPurchased { get; set; }
    }

    public class Response : ResponseBase
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("in_progress")]
        public int? InProgress { get; set; }

        [JsonPropertyName("recipient_count")]
        public int? RecipientCount { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        //[JsonPropertyName("purchase_metrics")]
        //public List<T> PurchaseMetrics { get; set; }
    }

    public class ResponseSummary : ResponseBase
    {
        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("month")]
        public int? Month { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("mailings")]
        public int? Mailings { get; set; }

    }
}
