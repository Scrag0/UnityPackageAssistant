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
using System.Linq;
using Extensions;
using Newtonsoft.Json;

namespace Verdaccio.Models
{
    public class Dist : ICloneable
    {
        [JsonProperty("npm-signature")]
        [DefaultValue("")]
        public string NpmSignature { get; set; } = string.Empty;

        [JsonProperty("signatures")] public List<Signature> Signatures { get; set; } = new();

        [JsonProperty("fileCount")]
        [DefaultValue(null)]
        public int? FileCount { get; set; }

        [JsonProperty("integrity")]
        [DefaultValue("")]
        public string Integrity { get; set; } = string.Empty;

        [JsonProperty("shasum")]
        [DefaultValue("")]
        public string Shasum { get; set; } = string.Empty;

        [JsonProperty("unpackedSize")]
        [DefaultValue(null)]
        public long? UnpackedSize { get; set; }

        [JsonProperty("tarball")]
        [DefaultValue("")]
        public string Tarball { get; set; } = string.Empty;

        public object Clone()
        {
            var clone = new Dist();
            clone.NpmSignature = NpmSignature;
            clone.Signatures.AddRange(Signatures.GetCloneData());
            clone.FileCount = FileCount;
            clone.Integrity = Integrity;
            clone.Shasum = Shasum;
            clone.UnpackedSize = UnpackedSize;
            clone.Tarball = Tarball;
            return clone;
        }

        public bool ShouldSerializeSignatures()
        {
            return Signatures != null &&
                   Signatures.Count > 0 &&
                   Signatures.Any(x => !x.IsEmpty());
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(NpmSignature) &&
                   (Signatures == null || Signatures.Count > 0) &&
                   FileCount == null &&
                   string.IsNullOrWhiteSpace(Integrity) &&
                   string.IsNullOrWhiteSpace(Shasum) &&
                   UnpackedSize == null &&
                   string.IsNullOrWhiteSpace(Tarball);
        }
    }
}
