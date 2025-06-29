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
using Extensions;
using Verdaccio.Models;

namespace Verdaccio.CustomModels
{
    public static class UnityManifestExtensions
    {
        public const string kLatestDistTag = "latest";

        public static string GetLatestDistTag(this UnityManifest manifest)
        {
            if (manifest == null || manifest.DistTags == null)
            {
                return kLatestDistTag;
            }

            return manifest.DistTags.GetValueOrDefault(kLatestDistTag, kLatestDistTag);
        }

        public static void SetLatestDistTag(this UnityManifest manifest, string value)
        {
            if (manifest == null)
            {
                return;
            }

            if (manifest.DistTags == null)
            {
                manifest.DistTags = new GenericBody();
            }

            manifest.DistTags[kLatestDistTag] = value;
        }

        internal static bool TryGetLatestVersion(this UnityManifest manifest, out UnityVersion unityVersion)
        {
            var latestVersion = manifest.GetLatestDistTag();
            var originalVersion = manifest.Versions.GetValueOrDefault(latestVersion, null);
            unityVersion = originalVersion?.GetClone();
            return unityVersion != null;
        }

        public static string GetLatestDistTag(this UnityUnpublishManifest manifest)
        {
            if (manifest == null || manifest.DistTags == null)
            {
                return kLatestDistTag;
            }

            return manifest.DistTags.GetValueOrDefault(kLatestDistTag, kLatestDistTag);
        }

        public static void SetLatestDistTag(this UnityUnpublishManifest manifest, string value)
        {
            if (manifest == null)
            {
                return;
            }

            if (manifest.DistTags == null)
            {
                manifest.DistTags = new GenericBody();
            }

            manifest.DistTags[kLatestDistTag] = value;
        }

        internal static bool TryGetLatestVersion(this UnityUnpublishManifest manifest, out UnityVersion unityVersion)
        {
            var latestVersion = manifest.GetLatestDistTag();
            var originalVersion = manifest.Versions.GetValueOrDefault(latestVersion, null);
            unityVersion = originalVersion?.GetClone();
            return unityVersion != null;
        }

        public static UnityUnpublishManifest GetUnpublishManifest(this UnityManifest manifest)
        {
            if (manifest == null)
            {
                return null;
            }

            var unpublishManifest = new UnityUnpublishManifest();
            unpublishManifest.Id = manifest.Id;
            unpublishManifest.Rev = manifest.Rev;
            unpublishManifest.Name = manifest.Name;
            unpublishManifest.Description = manifest.Description;
            unpublishManifest.DistTags = new GenericBody(manifest.DistTags);
            unpublishManifest.Time = new GenericBody(manifest.Time);
            unpublishManifest.Versions = new UnityVersions(manifest.Versions.GetCloneDictionary());
            unpublishManifest.Maintainers = new List<Author>(manifest.Maintainers.GetCloneData());
            unpublishManifest.Contributors = new List<Author>(manifest.Contributors.GetCloneData());
            unpublishManifest.Readme = manifest.Readme;
            unpublishManifest.Users = new PackageUsers(manifest.Users);
            unpublishManifest.Bugs = manifest.Bugs.GetClone();
            unpublishManifest.License = manifest.License;
            unpublishManifest.Homepage = manifest.Homepage;
            unpublishManifest.Repository = manifest.Repository.GetClone();
            unpublishManifest.Keywords = new List<string>(manifest.Keywords);
            unpublishManifest.Author = manifest.Author.GetClone();
            return unpublishManifest;
        }
    }
}
