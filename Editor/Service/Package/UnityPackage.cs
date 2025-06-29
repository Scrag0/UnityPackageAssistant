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
using NuGet.Versioning;

namespace UnityPackageAssistant
{
    /// <summary>
    /// Represents a Unity package manifest file (package.json)
    /// </summary>
    [Serializable]
    public class UnityPackage : IEquatable<UnityPackage>, ICloneable
    {
        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("version")]
        [JsonConverter(typeof(SemanticVersionConverter))]
        public SemanticVersion Version { get; set; } = new(0, 0, 0);

        [JsonProperty("displayName")]
        [DefaultValue("")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonProperty("description")]
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("unity")]
        [DefaultValue("")]
        public string Unity { get; set; } = string.Empty;

        [JsonProperty("unityRelease")]
        [DefaultValue("")]
        public string UnityRelease { get; set; } = string.Empty;

        [JsonProperty("documentationUrl")]
        [DefaultValue("")]
        public string DocumentationUrl { get; set; } = string.Empty;

        [JsonProperty("changelogUrl")]
        [DefaultValue("")]
        public string ChangelogUrl { get; set; } = string.Empty;

        [JsonProperty("license")]
        [DefaultValue("")]
        public string License { get; set; } = string.Empty;

        [JsonProperty("licensesUrl")]
        [DefaultValue("")]
        public string LicensesUrl { get; set; } = string.Empty;

        [JsonProperty("dependencies")] public Dictionary<string, string> Dependencies { get; set; } = new();
        [JsonProperty("keywords")] public List<string> Keywords { get; set; } = new();
        [JsonProperty("author")] public Author Author { get; set; } = new();
        [JsonProperty("repository")] public Repository Repository { get; set; } = new();
        [JsonProperty("samples")] public List<Sample> Samples { get; set; } = new();
        [JsonIgnore] public string LocalAssetPath { get; set; } = string.Empty;

        public UnityPackage()
        { }

        protected UnityPackage(UnityPackage copy)
        {
            Name = copy.Name;
            Version = new(copy.Version);
            DisplayName = copy.DisplayName;
            Description = copy.Description;
            Unity = copy.Unity;
            UnityRelease = copy.UnityRelease;
            DocumentationUrl = copy.DocumentationUrl;
            ChangelogUrl = copy.ChangelogUrl;
            License = copy.License;
            LicensesUrl = copy.LicensesUrl;
            Dependencies = new Dictionary<string, string>(copy.Dependencies);
            Keywords = new List<string>(copy.Keywords);
            Author = copy.Author.GetClone();
            Repository = copy.Repository.GetClone();
            Samples = copy.Samples.GetCloneDataList();
            LocalAssetPath = copy.LocalAssetPath;
        }

        public bool ShouldSerializeVersion()
        {
            return Version != null &&
                   !Version.Equals(new SemanticVersion(0, 0, 0));
        }

        public bool ShouldSerializeDependencies()
        {
            return Dependencies != null &&
                   Dependencies.Count > 0 &&
                   Dependencies.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeKeywords()
        {
            return Keywords != null &&
                   Keywords.Count > 0 &&
                   Keywords.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool ShouldSerializeAuthor()
        {
            return Author != null &&
                   !Author.IsEmpty();
        }

        public bool ShouldSerializeRepository()
        {
            return Repository != null &&
                   !Repository.IsEmpty();
        }

        public bool ShouldSerializeSamples()
        {
            return Samples != null &&
                   Samples.Count > 0 &&
                   Samples.Any(x => !x.IsEmpty());
        }

        public bool TryGetLocalAssetPath(out string assetPath)
        {
            assetPath = LocalAssetPath;
            return !string.IsNullOrWhiteSpace(LocalAssetPath);
        }

        public static bool operator ==(UnityPackage left, UnityPackage right) => Equals(left, right);
        public static bool operator !=(UnityPackage left, UnityPackage right) => !Equals(left, right);
        public override bool Equals(object obj) => Equals(obj as UnityPackage);

        public bool Equals(UnityPackage other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name) &&
                   Version.Equals(other.Version) &&
                   DisplayName.Equals(other.DisplayName) &&
                   Description.Equals(other.Description) &&
                   Unity.Equals(other.Unity) &&
                   UnityRelease.Equals(other.UnityRelease) &&
                   DocumentationUrl.Equals(other.DocumentationUrl) &&
                   ChangelogUrl.Equals(other.ChangelogUrl) &&
                   License.Equals(other.License) &&
                   LicensesUrl.Equals(other.LicensesUrl) &&
                   Dependencies.DictionarySequenceEqual(other.Dependencies) &&
                   Keywords.SequenceEqual(other.Keywords) &&
                   Author.Equals(other.Author) &&
                   Author == other.Author &&
                   Repository.Equals(other.Repository) &&
                   Samples.SequenceEqual(other.Samples);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Name);
            hash.Add(Version.ToNormalizedString());
            hash.Add(DisplayName);
            hash.Add(Description);
            hash.Add(Unity);
            hash.Add(UnityRelease);
            hash.Add(DocumentationUrl);
            hash.Add(ChangelogUrl);
            hash.Add(License);
            hash.Add(LicensesUrl);
            hash.Add(Dependencies.DictionaryToHashCode());
            hash.Add(Keywords.ListToHashCode());
            hash.Add(Author);
            hash.Add(Repository);
            hash.Add(Samples.ListToHashCode());
            return hash.ToHashCode();
        }

        public virtual object Clone()
        {
            return new UnityPackage(this);
        }

        public virtual bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Name) &&
                   Version.Equals(new SemanticVersion(0, 0, 0)) &&
                   string.IsNullOrWhiteSpace(DisplayName) &&
                   string.IsNullOrWhiteSpace(Description) &&
                   string.IsNullOrWhiteSpace(Unity) &&
                   string.IsNullOrWhiteSpace(UnityRelease) &&
                   string.IsNullOrWhiteSpace(DocumentationUrl) &&
                   string.IsNullOrWhiteSpace(ChangelogUrl) &&
                   string.IsNullOrWhiteSpace(License) &&
                   string.IsNullOrWhiteSpace(LicensesUrl) &&
                   !ShouldSerializeDependencies() &&
                   !ShouldSerializeKeywords() &&
                   !ShouldSerializeAuthor() &&
                   !ShouldSerializeRepository() &&
                   !ShouldSerializeSamples();
        }
    }
}
