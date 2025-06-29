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
using Newtonsoft.Json;
using NuGet.Versioning;

namespace UnityPackageAssistant
{
    public class SemanticVersionConverter : JsonConverter<SemanticVersion>
    {
        private const string kDefaultValue = "0.0.0";

        public override void WriteJson(JsonWriter writer, SemanticVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToNormalizedString());
        }

        public override SemanticVersion ReadJson(JsonReader reader, Type objectType, SemanticVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var str = (string)reader.Value;
            if (string.IsNullOrWhiteSpace(str))
            {
                str = kDefaultValue;
            }

            if (SemanticVersion.TryParse(str, out SemanticVersion semanticVersion))
            {
                return semanticVersion;
            }

            return SemanticVersion.Parse(kDefaultValue);
        }
    }
}
