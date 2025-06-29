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
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NuGet.Versioning;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UnityPackageAssistant
{
    internal interface IPackageDataProvider : IProvider
    {
        List<UnityPackage> EmbeddedPackagesManifests { get; }
        UniTask<List<PackageInfo>> GetAllAvailablePackages(bool offlineMode = true);
        UniTask<List<PackageInfo>> GetPackagesFromScope(string scope, bool offlineMode = true);
        void ChangePackageVersion(UnityPackage package, SemanticVersion version);
        bool TryGetAssetPath(UnityPackage package, out string assetPath);
    }

    internal class PackageDataProvider : IPackageDataProvider
    {
        private const string kPackageJsonFile = "package.json";

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        private List<PackageInfo> _embeddedPackages;
        private List<UnityPackage> _embeddedPackagesManifests;

        public List<UnityPackage> EmbeddedPackagesManifests { get => new(_embeddedPackagesManifests); }

        public void Initialize()
        {
            var packages = PackageInfo.GetAllRegisteredPackages();
            _embeddedPackages = packages.Where(x => x.source is PackageSource.Embedded).ToList();

            var packagesCount = _embeddedPackages.Count;
            _embeddedPackagesManifests = new List<UnityPackage>(packagesCount);
            for (int i = 0; i < packagesCount; i++)
            {
                var embeddedPackage = _embeddedPackages[i];
                var packageManifest = GetUnityPackage(embeddedPackage);
                packageManifest.LocalAssetPath = embeddedPackage.assetPath;
                _embeddedPackagesManifests.Add(packageManifest);
            }
        }

        public async UniTask<List<PackageInfo>> GetAllAvailablePackages(bool offlineMode = true)
        {
            var listRequest = Client.SearchAll(offlineMode);
            await UniTask.WaitUntil(() => listRequest.IsCompleted);
            var packages = listRequest?.Result ?? Array.Empty<PackageInfo>();
            var result = packages.ToList();
            return result;
        }

        public async UniTask<List<PackageInfo>> GetPackagesFromScope(string scope, bool offlineMode = true)
        {
            var packages = await GetAllAvailablePackages(offlineMode);
            var result = packages
                .Where(x => x.packageId.Contains(scope))
                .ToList();
            return result;
        }

        public void ChangePackageVersion(UnityPackage package, SemanticVersion version)
        {
            if (!TryGetPackageInfo(package, out PackageInfo packageInfo))
            {
                return;
            }

            package.Version = new SemanticVersion(version);
            SavePackageManifest(packageInfo, package);
        }

        public bool TryGetAssetPath(UnityPackage package, out string assetPath)
        {
            if (!TryGetPackageInfo(package, out PackageInfo packageInfo))
            {
                assetPath = string.Empty;
                return false;
            }

            assetPath = packageInfo.assetPath;
            return true;
        }

        private bool TryGetPackageInfo(UnityPackage package, out PackageInfo packageInfo)
        {
            packageInfo = _embeddedPackages.FirstOrDefault(x => x.name.Equals(package.Name));
            return packageInfo != null;
        }

        private UnityPackage GetUnityPackage(PackageInfo packageInfo)
        {
            string packageJsonPath = GetPackageJsonPath(packageInfo);
            var data = File.ReadAllText(packageJsonPath);
            var packageManifest = JsonConvert.DeserializeObject<UnityPackage>(data);
            return packageManifest;
        }

        private void SavePackageManifest(PackageInfo packageInfo, UnityPackage package)
        {
            var serializedData = JsonConvert.SerializeObject(package, _serializerSettings);
            string packageJsonPath = GetPackageJsonPath(packageInfo);
            File.WriteAllText(packageJsonPath, serializedData);
        }

        private string GetPackageJsonPath(PackageInfo packageInfo)
        {
            var packageFolderPath = packageInfo.assetPath;
            string packageJsonPath = Path.Combine(packageFolderPath, kPackageJsonFile);
            return packageJsonPath;
        }
    }
}
