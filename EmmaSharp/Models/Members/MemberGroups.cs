﻿using RestSharp.Deserializers;
using System.Collections.Generic;

namespace EmmaSharp.Models.Members
{
    public class MemberGroups
    {
        [DeserializeAs(Name = "groups")]
        public List<Group> Groups { get; set; }
    }
}
