// Copyright 2025 Bohdan Yavhusishyn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Verdaccio.Models
{
    public class Attachments : Dictionary<string, AttachmentItem>
    {
        public Attachments()
        { }

        public Attachments(IDictionary<string, AttachmentItem> dictionary) : base(dictionary)
        { }
    }

    // TODO: Maybe create custom AttachmentItem with SemanticVersion
    public class AttachmentItem : ICloneable
    {
        [JsonProperty("content_type")]
        [DefaultValue("")]
        public string ContentType { get; set; } = string.Empty;

        [JsonProperty("data")]
        [DefaultValue("")]
        public string Data { get; set; } = string.Empty;

        [JsonProperty("length")]
        [DefaultValue(null)]
        public int? Length { get; set; }

        [JsonProperty("shasum")]
        [DefaultValue("")]
        public string Shasum { get; set; } = string.Empty;

        [JsonProperty("version")]
        [DefaultValue("")]
        public string Version { get; set; } = string.Empty;

        public object Clone()
        {
            var clone = new AttachmentItem();
            clone.ContentType = ContentType;
            clone.Data = Data;
            clone.Length = Length;
            clone.Shasum = Shasum;
            clone.Version = Version;
            return clone;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(ContentType) &&
                   string.IsNullOrWhiteSpace(Data) &&
                   Length == null &&
                   string.IsNullOrWhiteSpace(Shasum) &&
                   string.IsNullOrWhiteSpace(Version);
        }
    }
}
