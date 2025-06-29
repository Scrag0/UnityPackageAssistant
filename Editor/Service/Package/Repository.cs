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
using System.ComponentModel;
using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    public class Repository : IEquatable<Repository>, ICloneable
    {
        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("type")]
        [DefaultValue("")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("revision")]
        [DefaultValue("")]
        public string Revision { get; set; } = string.Empty;

        public static bool operator ==(Repository left, Repository right) => Equals(left, right);
        public static bool operator !=(Repository left, Repository right) => !Equals(left, right);
        public override bool Equals(object obj) => Equals(obj as Repository);

        public bool Equals(Repository other)
        {
            if (other == null) return false;
            return Url.Equals(other.Url) &&
                   Type.Equals(other.Type) &&
                   Revision.Equals(other.Revision);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Url);
            hash.Add(Type);
            hash.Add(Revision);
            return hash.ToHashCode();
        }

        public object Clone()
        {
            var clone = new Repository();
            clone.Url = Url;
            clone.Type = Type;
            clone.Revision = Revision;
            return clone;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Url) &&
                   string.IsNullOrWhiteSpace(Type) &&
                   string.IsNullOrWhiteSpace(Revision);
        }
    }
}
