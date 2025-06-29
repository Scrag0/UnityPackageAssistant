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
    public class Sample : IEquatable<Sample>, ICloneable
    {
        [JsonProperty("displayName")]
        [DefaultValue("")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonProperty("description")]
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("path")]
        [DefaultValue("")]
        public string Path { get; set; } = string.Empty;

        public static bool operator ==(Sample left, Sample right) => Equals(left, right);
        public static bool operator !=(Sample left, Sample right) => !Equals(left, right);
        public override bool Equals(object obj) => Equals(obj as Sample);

        public bool Equals(Sample other)
        {
            if (other == null) return false;
            return DisplayName.Equals(other.DisplayName) &&
                   Description.Equals(other.Description) &&
                   Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(DisplayName);
            hash.Add(Description);
            hash.Add(Path);
            return hash.ToHashCode();
        }

        public object Clone()
        {
            var clone = new Sample();
            clone.DisplayName = DisplayName;
            clone.Description = Description;
            clone.Path = Path;
            return clone;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(DisplayName) &&
                   string.IsNullOrWhiteSpace(Description) &&
                   string.IsNullOrWhiteSpace(Path);
        }
    }
}
