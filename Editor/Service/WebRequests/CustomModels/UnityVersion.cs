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
using Extensions;
using Newtonsoft.Json;
using UnityPackageAssistant;
using Verdaccio.Models;
using Author = Verdaccio.Models.Author;

namespace Verdaccio.CustomModels
{
    internal class UnityVersions : Dictionary<string, UnityVersion>
    {
        public UnityVersions()
        { }

        public UnityVersions(IDictionary<string, UnityVersion> dictionary) : base(dictionary)
        { }
    }

    /// <remarks>Fields removed of <see cref="UpmVersion"/>:
    /// <list type="bullet">
    /// <item>
    ///   <description><see cref="UpmVersion.Name"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Version"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Author"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.License"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Description"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Repository"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Dependencies"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Keywords"/></description>
    /// </item>
    /// </list>
    /// Fields removed as their type is an object as in <see cref="UpmVersion"/>:
    /// <list type="bullet">
    /// <item>
    ///   <description><see cref="UpmVersion.Directories"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Bugs"/></description>
    /// </item>
    /// <item>
    ///   <description><see cref="UpmVersion.Scripts"/></description>
    /// </item>
    /// </list>
    /// </remarks>
    public class UnityVersion : UnityPackage
    {
        [JsonProperty("dist")] public Dist Dist { get; set; } = new();

        [JsonProperty("main")]
        [DefaultValue("")]
        public string Main { get; set; } = string.Empty;

        [JsonProperty("homemage")]
        [DefaultValue("")]
        public string Homemage { get; set; } = string.Empty;

        [JsonProperty("readme")]
        [DefaultValue("")]
        public string Readme { get; set; } = string.Empty;

        [JsonProperty("readmeFileName")]
        [DefaultValue("")]
        public string ReadmeFileName { get; set; } = string.Empty;

        [JsonProperty("readmeFilename")]
        [DefaultValue("")]
        public string ReadmeFilename { get; set; } = string.Empty;

        [JsonProperty("bin")]
        [DefaultValue("")]
        public string Bin { get; set; } = string.Empty;

        [JsonProperty("files")] public List<string> Files { get; set; } = new();

        [JsonProperty("gitHead")]
        [DefaultValue("")]
        public string GitHead { get; set; } = string.Empty;

        [JsonProperty("maintainers")] public List<Author> Maintainers { get; set; } = new();
        [JsonProperty("contributors")] public List<Author> Contributors { get; set; } = new();

        [JsonProperty("homepage")]
        [DefaultValue("")]
        public string Homepage { get; set; } = string.Empty;

        [JsonProperty("etag")]
        [DefaultValue("")]
        public string Etag { get; set; } = string.Empty;

        // TODO: Maybe create custom Dependencies with SemanticVersion and change string version to SemanticVersion
        [JsonProperty("peerDependencies")] public Dependencies PeerDependencies { get; set; } = new();
        [JsonProperty("devDependencies")] public Dependencies DevDependencies { get; set; } = new();
        [JsonProperty("optionalDependencies")] public Dependencies OptionalDependencies { get; set; } = new();
        [JsonProperty("peerDependenciesMeta")] public PeerDependenciesMeta PeerDependenciesMeta { get; set; } = new();
        [JsonProperty("bundleDependencies")] public List<string> BundleDependencies { get; set; } = new();
        [JsonProperty("acceptDependencies")] public Dependencies AcceptDependencies { get; set; } = new();

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

        public UnityVersion()
        { }

        public UnityVersion(UnityPackage copy) : base(copy)
        { }

        public UnityVersion(UnityVersion copy) : base(copy)
        {
            Dist = copy.Dist.GetClone();
            Main = copy.Main;
            Homemage = copy.Homemage;
            Readme = copy.Readme;
            ReadmeFileName = copy.ReadmeFileName;
            ReadmeFilename = copy.ReadmeFilename;
            Bin = copy.Bin;
            Files = new List<string>(copy.Files);
            GitHead = copy.GitHead;
            Maintainers = copy.Maintainers.GetCloneDataList();
            Contributors = copy.Contributors.GetCloneDataList();
            Homepage = copy.Homepage;
            Etag = copy.Etag;
            PeerDependencies = new Dependencies(copy.PeerDependencies);
            DevDependencies = new Dependencies(copy.DevDependencies);
            OptionalDependencies = new Dependencies(copy.OptionalDependencies);
            PeerDependenciesMeta = new PeerDependenciesMeta(copy.PeerDependenciesMeta.GetCloneDictionary());
            BundleDependencies = new List<string>(copy.BundleDependencies);
            AcceptDependencies = new Dependencies(copy.AcceptDependencies);
            NodeVersion = copy.NodeVersion;
            Id = copy.Id;
            NpmVersion = copy.NpmVersion;
            NpmUser = copy.NpmUser.GetClone();
            HasShrinkwrap = copy.HasShrinkwrap;
            Deprecated = copy.Deprecated;
            Funding = copy.Funding.GetClone();
            Engines = new Engines(copy.Engines);
            HasInstallScript = copy.HasInstallScript;
            Cpu = new List<string>(copy.Cpu);
            Os = new List<string>(copy.Os);
        }

        public override object Clone()
        {
            return new UnityVersion(this);
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() &&
                   !ShouldSerializeDist() &&
                   string.IsNullOrWhiteSpace(Main) &&
                   string.IsNullOrWhiteSpace(Homemage) &&
                   string.IsNullOrWhiteSpace(Readme) &&
                   string.IsNullOrWhiteSpace(ReadmeFileName) &&
                   string.IsNullOrWhiteSpace(ReadmeFilename) &&
                   string.IsNullOrWhiteSpace(Bin) &&
                   !ShouldSerializeFiles() &&
                   string.IsNullOrWhiteSpace(GitHead) &&
                   !ShouldSerializeMaintainers() &&
                   !ShouldSerializeContributors() &&
                   string.IsNullOrWhiteSpace(Homepage) &&
                   string.IsNullOrWhiteSpace(Etag) &&
                   !ShouldSerializePeerDependencies() &&
                   !ShouldSerializeDevDependencies() &&
                   !ShouldSerializeOptionalDependencies() &&
                   !ShouldSerializePeerDependenciesMeta() &&
                   !ShouldSerializeBundleDependencies() &&
                   !ShouldSerializeAcceptDependencies() &&
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

        public bool ShouldSerializeDist()
        {
            return Dist != null &&
                   !Dist.IsEmpty();
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
    }
}
