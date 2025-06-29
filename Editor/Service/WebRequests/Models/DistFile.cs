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
    public class DistFiles : Dictionary<string, DistFile>
    {
        public DistFiles()
        { }

        public DistFiles(IDictionary<string, DistFile> dictionary) : base(dictionary)
        { }
    }

    public class DistFile : ICloneable
    {
        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("sha")]
        [DefaultValue("")]
        public string Sha { get; set; } = string.Empty;

        [JsonProperty("registry")]
        [DefaultValue("")]
        public string Registry { get; set; } = string.Empty;

        public object Clone()
        {
            var clone = new DistFile();
            clone.Url = Url;
            clone.Sha = Sha;
            clone.Registry = Registry;
            return clone;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Url) &&
                   string.IsNullOrWhiteSpace(Sha) &&
                   string.IsNullOrWhiteSpace(Registry);
        }
    }
}
