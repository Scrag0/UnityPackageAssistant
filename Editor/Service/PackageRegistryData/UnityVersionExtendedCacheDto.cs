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

using Extensions;
using Verdaccio.CustomModels;

namespace UnityPackageAssistant
{
    public class UnityVersionExtendedCacheDto
    {
        public UnityVersionExtended UnityVersionExtended { get; set; } = new();
        public UnityManifest UnityManifest { get; set; } = new();
        public string LocalAssetPath { get; set; } = string.Empty;

        public UnityVersionExtendedCacheDto()
        { }

        public UnityVersionExtendedCacheDto(UnityVersionExtended package)
        {
            UnityVersionExtended = package.GetClone();
            UnityManifest = package.Manifest.GetClone();
            LocalAssetPath = package.LocalAssetPath;
        }
 
        public UnityVersionExtended ToUnityVersionExtended()
        {
            var clone = UnityVersionExtended.GetClone();
            clone.Manifest = UnityManifest.GetClone();
            clone.LocalAssetPath = LocalAssetPath;
            return clone;
        }
    }
}
