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
using Verdaccio.Models;

namespace Verdaccio.CustomModels
{
    /// <summary>
    /// Based on <see cref="Verdaccio.Models.Manifest"/>.
    /// Type of field <see cref="Verdaccio.Models.Manifest.Versions"/> changed to <see cref="UnityVersions"/>
    /// </summary>
    public class UnityManifest : ICloneable
    {
        [JsonProperty("_id")]
        [DefaultValue("")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("_rev")]
        [DefaultValue("")]
        public string Rev { get; set; } = string.Empty;

        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("dist-tags")] public GenericBody DistTags { get; set; } = new();
        [JsonProperty("time")] public GenericBody Time { get; set; } = new();
        [JsonProperty("versions")] internal UnityVersions Versions { get; set; } = new();
        [JsonProperty("maintainers")] public List<Author> Maintainers { get; set; } = new();
        [JsonProperty("contributors")] public List<Author> Contributors { get; set; } = new();

        [JsonProperty("readme")]
        [DefaultValue("")]
        public string Readme { get; set; } = string.Empty;

        [JsonProperty("users")] public PackageUsers Users { get; set; } = new();
        [JsonProperty("bugs")] public BugsInfo Bugs { get; set; } = new();

        [JsonProperty("license")]
        [DefaultValue("")]
        public string License { get; set; } = string.Empty;

        [JsonProperty("homepage")]
        [DefaultValue("")]
        public string Homepage { get; set; } = string.Empty;

        [JsonProperty("repository")] public RepositoryInfo Repository { get; set; } = new();
        [JsonProperty("keywords")] public List<string> Keywords { get; set; } = new();
        [JsonProperty("author")] public Author Author { get; set; } = new();
        [JsonProperty("_distfiles")] public DistFiles DistFiles { get; set; } = new();
        [JsonProperty("_uplinks")] public UpLinks UpLinks { get; set; } = new();
        [JsonProperty("_attachments")] public Attachments Attachments { get; set; } = new();

        public object Clone()
        {
            var clone = new UnityManifest();
            clone.Id = Id;
            clone.Rev = Rev;
            clone.Name = Name;
            clone.Description = Description;
            clone.DistTags = new GenericBody(DistTags);
            clone.Time = new GenericBody(Time);;
            clone.Versions = new UnityVersions(Versions.GetCloneDictionary());
            clone.Maintainers = Maintainers.GetCloneDataList();
            clone.Contributors = Contributors.GetCloneDataList();
            clone.Readme = Readme;
            clone.Users = new PackageUsers(Users);
            clone.Bugs = Bugs.GetClone();
            clone.License = License;
            clone.Homepage = Homepage;
            clone.Repository = Repository.GetClone();
            clone.Keywords = new List<string>(Keywords);
            clone.Author = Author.GetClone();
            clone.DistFiles = new DistFiles(DistFiles.GetCloneDictionary());
            clone.UpLinks = new UpLinks(UpLinks.GetCloneDictionary());
            clone.Attachments = new Attachments(Attachments.GetCloneDictionary());
            return clone;
        }

        public bool ShouldSerializeDistTags()
        {
            return DistTags != null &&
                   DistTags.Count > 0 &&
                   DistTags.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeTime()
        {
            return Time != null &&
                   Time.Count > 0 &&
                   Time.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value));
        }

        public bool ShouldSerializeVersions()
        {
            return Versions != null &&
                   Versions.Count > 0 &&
                   Versions.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value.IsEmpty());
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

        public bool ShouldSerializeUsers()
        {
            return Users != null &&
                   Users.Count > 0 &&
                   Users.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value == false);
        }

        public bool ShouldSerializeBugs()
        {
            return Bugs != null &&
                   !Bugs.IsEmpty();
        }

        public bool ShouldSerializeRepository()
        {
            return Repository != null &&
                   !Repository.IsEmpty();
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

        public bool ShouldSerializeDistFiles()
        {
            return DistFiles != null &&
                   DistFiles.Count > 0 &&
                   DistFiles.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value.IsEmpty());
        }

        public bool ShouldSerializeUpLinks()
        {
            return UpLinks != null &&
                   UpLinks.Count > 0 &&
                   UpLinks.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value.IsEmpty());
        }

        public bool ShouldSerializeAttachments()
        {
            return Attachments != null &&
                   Attachments.Count > 0 &&
                   Attachments.Any(pair => !string.IsNullOrWhiteSpace(pair.Key) && !pair.Value.IsEmpty());
        }
    }
}
