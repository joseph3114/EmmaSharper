using System;
using System.Collections.Generic;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Trigger
    {
        [JsonPropertyName("parent_mailing")]
        public MailingTrigger ParentMailing { get; set; }

        [JsonPropertyName("surveys")]
        public string Surveys { get; set; }

        [JsonPropertyName("event_type")]
        public string EventType { get; set; }

        // Need more info from Emma
        [JsonPropertyName("links")]
        public string Links { get; set; }

        [JsonPropertyName("field_id")]
        public long? FieldId { get; set; }

        // Need more info from Emma
        [JsonPropertyName("signup_integrations")]
        public string SignupIntegrations { get; set; }

        // Need more info from Emma
        [JsonPropertyName("push_offest_units")]
        public string PushOffsetUnits { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("start_timestamp")]
        public DateTime? StartTimestamp { get; set; }

        [JsonPropertyName("trigger_id")]
        public long? TriggerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        // Need more info from Emma
        [JsonPropertyName("signups")]
        public int?[] Signups { get; set; }

        // Need more info from Emma
        [JsonPropertyName("push_offset")]
        public string PushOffset { get; set; }

        // Need more info from Emma
        [JsonPropertyName("groups")]
        public List<Group> Groups { get; set; }

        [JsonPropertyName("parent_mailing_id")]
        public long? ParentMailingId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("is_disabled")]
        public bool IsDisabled { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }
    }
}
