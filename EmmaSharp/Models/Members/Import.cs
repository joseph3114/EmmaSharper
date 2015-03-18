﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EmmaSharp.Models.Members
{
    class Import
    {
        [JsonProperty("import_id")]
        public int ImportId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("import_started")]
        public DateTime? ImportStarted { get; set; }

        [JsonProperty("account_id")]
        public int AccountId { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("num_members_updated")]
        public int NumMembersUpdated { get; set; }

        [JsonProperty("source_filename")]
        public string SourceFilename { get; set; }

        [JsonProperty("fields_updated")]
        public IList<Field> FieldsUpdated { get; set; }

        [JsonProperty("num_fields_added")]
        public int NumFieldsAdded { get; set; }

        [JsonProperty("import_finished")]
        public bool? ImportFinished { get; set; }

        [JsonProperty("groups_updated")]
        public IList<Group> GroupsUpdated { get; set; }

        [JsonProperty("num_skipped")]
        public int NumSkipped { get; set; }

        [JsonProperty("num_duplicates")]
        public int NumDuplicates { get; set; }
    }
}