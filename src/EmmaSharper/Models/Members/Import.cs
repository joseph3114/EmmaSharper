using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class Import
    {
        [JsonPropertyName("import_id")]
        public long? ImportId { get; set; }

        [JsonPropertyName("status")]
        public ImportStatus? Status { get; set; }

        [JsonPropertyName("style")]
        public string? Style { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("import_started")]
        public DateTime? ImportStarted { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("num_members_updated")]
        public int? NumMembersUpdated { get; set; }

        [JsonPropertyName("source_filename")]
        public string? SourceFilename { get; set; }

        [JsonPropertyName("fields_updated")]
        public List<Field>? FieldsUpdated { get; set; }

        [JsonPropertyName("num_members_added")]
        public int? NumMembersAdded { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("import_finished")]
        public DateTime? ImportFinished { get; set; }

        [JsonPropertyName("groups_updated")]
        public List<Group>? GroupsUpdated { get; set; }

        [JsonPropertyName("num_skipped")]
        public int? NumSkipped { get; set; }

        [JsonPropertyName("num_duplicates")]
        public int? NumDuplicates { get; set; }
    }
}
