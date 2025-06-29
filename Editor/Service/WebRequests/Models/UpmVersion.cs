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
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Verdaccio.Models
{
    internal class Versions : Dictionary<string, UpmVersion>
    {
        public Versions()
        { }

        public Versions(IDictionary<string, UpmVersion> dictionary) : base(dictionary)
        { }
    }

    /// <remarks>Fields with two types:
    /// <list type="bullet">
    /// <item>
    ///   <description><see cref="Author"/>. Types: string | Author</description>
    /// </item>
    /// <item>
    ///   <description><see cref="Repository"/>.Types: string | any</description>
    /// </item>
    /// <item>
    ///   <description><see cref="Keywords"/>.Types: string | string[]</description>
    /// </item>
    /// </list>
    /// </remarks>
    internal class UpmVersion
    {
        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("version")]
        [DefaultValue("")]
        public string Version { get; set; } = string.Empty;

        [JsonProperty("directories")]
        [DefaultValue(null)]
        public object Directories { get; set; }

        [JsonProperty("dist")] public Dist Dist { get; set; } = new();

        [JsonProperty("author")] public Author Author { get; set; } = new();

        [JsonProperty("main")]
        [DefaultValue("")]
        public string Main { get; set; } = string.Empty;

        [JsonProperty("homemage")]
        [DefaultValue("")]
        public string Homemage { get; set; } = string.Empty;

        [JsonProperty("license")]
        [DefaultValue("")]
        public string License { get; set; } = string.Empty;

        [JsonProperty("readme")]
        [DefaultValue("")]
        public string Readme { get; set; } = string.Empty;

        [JsonProperty("readmeFileName")]
        [DefaultValue("")]
        public string ReadmeFileName { get; set; } = string.Empty;

        [JsonProperty("readmeFilename")]
        [DefaultValue("")]
        public string ReadmeFilename { get; set; } = string.Empty;

        [JsonProperty("description")]
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("bin")]
        [DefaultValue("")]
        public string Bin { get; set; } = string.Empty;

        [JsonProperty("bugs")]
        [DefaultValue(null)]
        public object Bugs { get; set; }

        [JsonProperty("files")] public List<string> Files { get; set; } = new();

        [JsonProperty("gitHead")]
        [DefaultValue("")]
        public string GitHead { get; set; } = string.Empty;

        [JsonProperty("maintainers")] public List<Author> Maintainers { get; set; } = new();
        [JsonProperty("contributors")] public List<Author> Contributors { get; set; } = new();

        [JsonProperty("repository")]
        [DefaultValue(null)]
        public object Repository { get; set; }

        [JsonProperty("scripts")]
        [DefaultValue(null)]
        public object Scripts { get; set; }

        [JsonProperty("homepage")]
        [DefaultValue("")]
        public string Homepage { get; set; } = string.Empty;

        [JsonProperty("etag")]
        [DefaultValue("")]
        public string Etag { get; set; } = string.Empty;

        [JsonProperty("dependencies")] public Dependencies Dependencies { get; set; } = new();
        [JsonProperty("peerDependencies")] public Dependencies PeerDependencies { get; set; } = new();
        [JsonProperty("devDependencies")] public Dependencies DevDependencies { get; set; } = new();
        [JsonProperty("optionalDependencies")] public Dependencies OptionalDependencies { get; set; } = new();
        [JsonProperty("peerDependenciesMeta")] public PeerDependenciesMeta PeerDependenciesMeta { get; set; } = new();
        [JsonProperty("bundleDependencies")] public List<string> BundleDependencies { get; set; } = new();
        [JsonProperty("acceptDependencies")] public Dependencies AcceptDependencies { get; set; } = new();
        [JsonProperty("keywords")] public List<string> Keywords { get; set; } = new();

        [JsonProperty("nodeVersion")]
        [DefaultValue("")]
        public string NodeVersion { get; set; } = string.Empty;

        [JsonProperty("_id")]
        [DefaultValue("")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("_npmVersion")]
        [DefaultValue("")]
        public string NpmVersion { get; set; } = string.Empty;

        [JsonProperty("_npmUser")] public Author NpmUser { get; set; } = new();

        [JsonProperty("_hasShrinkwrap")]
        [DefaultValue(null)]
        public bool? HasShrinkwrap { get; set; }

        [JsonProperty("deprecated")]
        [DefaultValue("")]
        public string Deprecated { get; set; } = string.Empty;

        [JsonProperty("funding")] public Funding Funding { get; set; } = new();
        [JsonProperty("engines")] public Engines Engines { get; set; } = new();

        [JsonProperty("hasInstallScript")]
        [DefaultValue(null)]
        public bool? HasInstallScript { get; set; }

        [JsonProperty("cpu")] public List<string> Cpu { get; set; } = new();
        [JsonProperty("os")] public List<string> Os { get; set; } = new();

        public bool ShouldSerializeDist()
        {
            return Dist != null &&
                   !Dist.IsEmpty();
        }

        public bool ShouldSerializeAuthor()
        {
            return Author != null &&
                   !Author.IsEmpty();
        }

        public bool ShouldSerializeFiles()
        {
            return Files != null &&
                   Files.Count > 0 &&
                   Files.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool ShouldSerializeMaintainers()
        {
            return Maintainers != null &&
                   Maintainers.Count > 0 &&
                   Maintainers.Any(x => !x.IsEmpty());
        }

        public bool ShouldSerializeContributors()
        {
            return Contributors != null &&
                   Contributors.Count > 0 &&
                   Contributors.Any(x => !x.IsEmpty());
        }

        public bool ShouldSerializeDependencies()
        {
            return Dependencies != null &&
                   Dependencies.Count > 0 &&
                   Dependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializePeerDependencies()
        {
            return PeerDependencies != null &&
                   PeerDependencies.Count > 0 &&
                   PeerDependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeDevDependencies()
        {
            return DevDependencies != null &&
                   DevDependencies.Count > 0 &&
                   DevDependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeOptionalDependencies()
        {
            return OptionalDependencies != null &&
                   OptionalDependencies.Count > 0 &&
                   OptionalDependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializePeerDependenciesMeta()
        {
            return PeerDependenciesMeta != null &&
                   PeerDependenciesMeta.Count > 0 &&
                   PeerDependenciesMeta.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value.IsEmpty());
        }

        public bool ShouldSerializeBundleDependencies()
        {
            return BundleDependencies != null &&
                   BundleDependencies.Count > 0 &&
                   BundleDependencies.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool ShouldSerializeAcceptDependencies()
        {
            return AcceptDependencies != null &&
                   AcceptDependencies.Count > 0 &&
                   AcceptDependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeKeywords()
        {
            return Keywords != null &&
                   Keywords.Count > 0 &&
                   Keywords.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool ShouldSerializeNpmUser()
        {
            return NpmUser != null &&
                   !NpmUser.IsEmpty();
        }

        public bool ShouldSerializeFunding()
        {
            return Funding != null &&
                   !Funding.IsEmpty();
        }

        public bool ShouldSerializeEngines()
        {
            return Engines != null &&
                   Engines.Count > 0 &&
                   Engines.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeCpu()
        {
            return Cpu != null &&
                   Cpu.Count > 0 &&
                   Cpu.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool ShouldSerializeOs()
        {
            return Os != null &&
                   Os.Count > 0 &&
                   Os.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Name) &&
                   string.IsNullOrWhiteSpace(Version) &&
                   Directories == null &&
                   !ShouldSerializeDist() &&
                   !ShouldSerializeAuthor() &&
                   string.IsNullOrWhiteSpace(Main) &&
                   string.IsNullOrWhiteSpace(Homemage) &&
                   string.IsNullOrWhiteSpace(License) &&
                   string.IsNullOrWhiteSpace(Readme) &&
                   string.IsNullOrWhiteSpace(ReadmeFileName) &&
                   string.IsNullOrWhiteSpace(ReadmeFilename) &&
                   string.IsNullOrWhiteSpace(Description) &&
                   string.IsNullOrWhiteSpace(Bin) &&
                   Bugs == null &&
                   !ShouldSerializeFiles() &&
                   string.IsNullOrWhiteSpace(GitHead) &&
                   !ShouldSerializeMaintainers() &&
                   !ShouldSerializeContributors() &&
                   Repository == null &&
                   Scripts == null &&
                   string.IsNullOrWhiteSpace(Homepage) &&
                   string.IsNullOrWhiteSpace(Etag) &&
                   !ShouldSerializeDependencies() &&
                   !ShouldSerializePeerDependencies() &&
                   !ShouldSerializeDevDependencies() &&
                   !ShouldSerializeOptionalDependencies() &&
                   !ShouldSerializePeerDependenciesMeta() &&
                   !ShouldSerializeBundleDependencies() &&
                   !ShouldSerializeAcceptDependencies() &&
                   !ShouldSerializeKeywords() &&
                   string.IsNullOrWhiteSpace(NodeVersion) &&
                   string.IsNullOrWhiteSpace(Id) &&
                   string.IsNullOrWhiteSpace(NpmVersion) &&
                   !ShouldSerializeNpmUser() &&
                   HasShrinkwrap == null &&
                   string.IsNullOrWhiteSpace(Deprecated) &&
                   !ShouldSerializeFunding() &&
                   !ShouldSerializeEngines() &&
                   HasInstallScript == null &&
                   !ShouldSerializeCpu() &&
                   !ShouldSerializeOs();
        }
    }
}
