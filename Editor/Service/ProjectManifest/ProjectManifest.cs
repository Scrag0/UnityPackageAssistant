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
using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    public class ProjectManifest
    {
        [JsonProperty("dependencies")] public Dictionary<string,string> Dependencies = new();
        [JsonProperty("scopedRegistries")] public List<ScopedRegistry> ScopedRegistries = new();
    }

    public class ScopedRegistry : ICloneable
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("url")] public string Url;
        [JsonProperty("overrideBuiltIns")] public bool OverrideBuiltIns;
        [JsonProperty("scopes")] public string[] Scopes;

        public object Clone()
        {
            var clone = new ScopedRegistry();
            clone.Name = Name;
            clone.Url = Url;
            clone.OverrideBuiltIns = OverrideBuiltIns;
            clone.Scopes = Scopes;
            return clone;
        }
    }
}
