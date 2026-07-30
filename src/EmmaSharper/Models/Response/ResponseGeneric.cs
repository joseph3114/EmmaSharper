using System;
using System.Collections.Generic;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class ResponseGeneric
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("member_since")]
        public DateTime? MemberSince { get; set; }

        [JsonPropertyName("email_domain")]
        public string EmailDomain { get; set; }

        [JsonPropertyName("email_user")]
        public string EmailUser { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("member_status_id")]
        public MemberStatusShort MemberStatusId { get; set; }
    }

    public class ResponseClicks : ResponseGeneric
    {
        // int64: Emma link ids exceed Int32.MaxValue. Closes binarypatrick/EmmaSharper#5.
        [JsonPropertyName("link_id")]
        public long? LinkId { get; set; }
    }

    public class ResponseDeliveries : ResponseGeneric
    {
        [JsonPropertyName("delivery_type")]
        public DeliveryType DeliveryType { get; set; }

        [JsonPropertyName("mailing_id")]
        public long? MailingId { get; set; }

        [JsonPropertyName("mailing_name")]
        public string MailingName { get; set; }
    }

    public class ResponseForwards : ResponseGeneric
    {
        [JsonPropertyName("forward_mailing_id")]
        public long? ForwardMailingId { get; set; }
    }

    public class ResponseSignups : ResponseGeneric
    {
        // int64: a member id, so it has the same overflow exposure as MemberId
        // above - missed by the 2019 int32->int64 sweep.
        [JsonPropertyName("ref_member_id")]
        public long? ReferingMemberId { get; set; }

        [JsonPropertyName("mailing_mailing_id")]
        public long? MailingMailingId { get; set; }
    }
}
