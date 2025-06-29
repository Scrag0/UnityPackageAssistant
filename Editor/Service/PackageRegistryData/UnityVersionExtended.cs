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

using System.Collections.Generic;
using System.Linq;
using Extensions;
using Newtonsoft.Json;
using NuGet.Versioning;
using Verdaccio.CustomModels;

namespace UnityPackageAssistant
{
    /// <remarks>Method <see cref="UnityVersionExtended.IsEmpty"/> is not overriden.
    /// Will be the same as in class <see cref="UnityVersion"/></remarks>
    public class UnityVersionExtended : UnityVersion
    {
        [JsonIgnore] public List<SemanticVersion> RemoteVersions { get => Manifest.Versions.Select(x => new SemanticVersion(x.Value.Version)).ToList(); }
        [JsonIgnore] public UnityManifest Manifest { get; set; } = new();
        public SemanticVersion RemoteVersion { get; set; } = new(0, 0, 0);
        public int DifferentRegistries { get; set; } = 0;

        public UnityVersionExtended()
        { }

        public UnityVersionExtended(UnityPackage copy) : base(copy)
        { }

        public UnityVersionExtended(UnityVersion copy) : base(copy)
        { }

        public UnityVersionExtended(UnityVersionExtended copy) : base(copy)
        {
            Manifest = copy.Manifest.GetClone();
            RemoteVersion = new SemanticVersion(copy.RemoteVersion);
            DifferentRegistries = copy.DifferentRegistries;
        }

        public override object Clone()
        {
            return new UnityVersionExtended(this);
        }
    }
}
