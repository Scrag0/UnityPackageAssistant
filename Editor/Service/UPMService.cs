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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using NuGet.Versioning;
using UnityEditor;
using UnityEngine;
using Verdaccio.CustomModels;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UnityPackageAssistant
{
    internal interface IUPMService : IService
    {
        List<ScopedRegistryExtended> Registries { get; }
        void Initialize();
        bool TryRestoreCurrentTab(out int index);
        void SetRestoreCurrentTab(int index);
        bool TryRestoreChangedPackages(out List<RegistryPackageGroup> registryPackageGroups);
        void SetRestoreChangedPackages(List<RegistryPackageGroup> registryPackageGroups);
        void SaveRegistryExtended(ScopedRegistryExtended registry);
        void RemoveRegistry(ScopedRegistry registry);
        UniTask Login(string registryUrl, string username, string password, bool rememberMe, Action<string> onSuccess, Action<string> onError);
        UniTask SignUp(string registryUrl, string username, string password, string email, bool alwaysAuth, Action<string> onSuccess, Action<string> onError);
        UniTask<bool> PublishPackage(string registryUrl, string authToken, UnityPackage package, Action<string> onSuccess, Action<string> onError);
        UniTask<bool> UnpublishPackage(string registryUrl, string authToken, UnityManifest manifest, SemanticVersion version, Action<string> onSuccess, Action<string> onError);
        UniTask<ComparePackageResult> ComparePackage(string registryUrl, string authToken, PackageInfo package);
        UniTask<ComparePackageResult> ComparePackage(string authToken, UnityVersion package);
        UniTask<List<RegistryPackageGroup>> GetChangedPackages();
        void ChangePackageVersion(UnityPackage package, SemanticVersion version);
        UniTask<UnityPackage> GetPackage(UnityPackage packageName);
        void UpdateUnityPackageManager();
    }

    public class UPMService : IUPMService
    {
        private const string kCacheFolder = "UnityPackageAssistant";
        private const string kUsernameKey = "UsernameKey";
        private const string kRegistryPackageGroupsKey = "kRegistryPackageGroupsKey";

        private readonly INPMWebRequests _npmWebRequests = new NPMWebRequests();
        private readonly ICredentialProvider _credentialProvider = new CredentialProvider();
        private readonly IProjectManifestProvider _projectManifestProvider = new ProjectManifestProvider();
        private readonly IPackageDataProvider _packageDataProvider = new PackageDataProvider();
        private readonly ICachedDataProvider _cachedDataProvider = new CachedDataProvider();
        private readonly List<ScopedRegistryExtended> _registries = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public List<ScopedRegistryExtended> Registries { get => new(_registries); }

        public UPMService()
        {
            Initialize();
        }

        public void Initialize()
        {
            _credentialProvider.Initialize();
            _projectManifestProvider.Initialize();
            _packageDataProvider.Initialize();
            var scopedRegistries = _projectManifestProvider.Registries;

            if (scopedRegistries == null)
            {
                UnityEngine.Debug.Log("No scoped registries set");
                return;
            }

            _registries.Clear();
            for (int i = 0, j = scopedRegistries.Count; i < j; i++)
            {
                var scopedRegistry = scopedRegistries[i];
                _registries.Add(LoadRegistry(scopedRegistry));
            }

            _cachedDataProvider.Initialize(kCacheFolder);
        }

        public bool TryRestoreCurrentTab(out int index)
        {
            var result = _cachedDataProvider.TryGetData(kUsernameKey, _cacheDuration, out index);
            return result;
        }

        public void SetRestoreCurrentTab(int index)
        {
            _cachedDataProvider.SetData(kUsernameKey, index);
        }

        public bool TryRestoreChangedPackages(out List<RegistryPackageGroup> registryPackageGroups)
        {
            var result = _cachedDataProvider.TryGetData(kRegistryPackageGroupsKey, _cacheDuration, out List<RegistryPackageGroupCacheDto> cacheDTOs);
            if (cacheDTOs == null)
            {
                registryPackageGroups = null;
                return false;
            }

            registryPackageGroups = cacheDTOs.Select(x => x.ToRegistryPackageGroup()).ToList();
            return result;
        }

        public void SetRestoreChangedPackages(List<RegistryPackageGroup> registryPackageGroups)
        {
            var cacheDTOs = registryPackageGroups.Select(x => new RegistryPackageGroupCacheDto(x)).ToList();
            _cachedDataProvider.SetData(kRegistryPackageGroupsKey, cacheDTOs);
        }

        public void SaveRegistryExtended(ScopedRegistryExtended registry)
        {
            _projectManifestProvider.Save(registry);

            if (!string.IsNullOrEmpty(registry.Token))
            {
                _credentialProvider.SetCredential(registry.Url, registry.Auth, registry.Token);
            }
            else
            {
                _credentialProvider.RemoveCredentialForRegistry(registry.Url);
            }
        }

        public void RemoveRegistry(ScopedRegistry registry)
        {
            _projectManifestProvider.Remove(registry);
        }

        private ScopedRegistryExtended LoadRegistry(ScopedRegistry scopedRegistry)
        {
            ScopedRegistryExtended registry = new ScopedRegistryExtended();
            registry.Name = scopedRegistry.Name;
            registry.Url = scopedRegistry.Url;
            registry.Scopes = scopedRegistry.Scopes;

            if (!_credentialProvider.TryGetCredential(registry.Url, out var credential))
            {
                return registry;
            }

            registry.Auth = credential.AlwaysAuth;
            registry.Token = credential.Token;
            return registry;
        }

        public async UniTask Login(string registryUrl, string username, string password, bool alwaysAuth, Action<string> onSuccess, Action<string> onError)
        {
            string token = await _npmWebRequests.Login(registryUrl, username, password, onSuccess, onError);
            _credentialProvider.SetCredential(registryUrl, alwaysAuth, token);
            UpdateUnityPackageManager();
        }

        public async UniTask SignUp(string registryUrl, string username, string password, string email, bool alwaysAuth, Action<string> onSuccess, Action<string> onError)
        {
            var token = await _npmWebRequests.AddUser(registryUrl, username, password, email, onSuccess, onError);
            _credentialProvider.SetCredential(registryUrl, alwaysAuth, token);
            UpdateUnityPackageManager();
        }

        public async UniTask<bool> PublishPackage(string registryUrl, string authToken, UnityPackage package, Action<string> onSuccess, Action<string> onError)
        {
            var result = await _npmWebRequests.PublishPackage(registryUrl, authToken, package, onSuccess, onError);
            UpdateUnityPackageManager();
            return result;
        }

        public async UniTask<bool> UnpublishPackage(string registryUrl, string authToken, UnityManifest manifest, SemanticVersion version, Action<string> onSuccess, Action<string> onError)
        {
            var result = manifest.Versions.Count == 1 && manifest.Versions.ContainsKey(version.ToNormalizedString())
                ? await _npmWebRequests.DeletePackage(registryUrl, authToken, manifest, onSuccess, onError)
                : await _npmWebRequests.UnpublishPackage(registryUrl, authToken, manifest, version, onSuccess, onError);
            UpdateUnityPackageManager();
            return result;
        }

        public async UniTask<ComparePackageResult> ComparePackage(string registryUrl, string authToken, PackageInfo package)
        {
            var result = new ComparePackageResult();
            var tarballOperationResult = await _npmWebRequests.GetLatestTarballData(registryUrl, authToken, package, null, null);
            result.LatestVersion = tarballOperationResult.LatestVersion;

            if (!tarballOperationResult.Success)
            {
                result.AreIdentical = false;
                return result;
            }

            var latestTarball = tarballOperationResult.Data;
            var localTarball = await Tarball.GetBytes(package.assetPath);

            var comparisonReport = GtzPackageComparator.ComparePackages(localTarball, latestTarball);

            result.AreIdentical = comparisonReport.Result == ComparisonResult.Identical;
            return result;
        }

        public async UniTask<ComparePackageResult> ComparePackage(string authToken, UnityVersion package)
        {
            var result = new ComparePackageResult();
            var tarballOperationResult = await _npmWebRequests.GetTarballData(authToken, package, null, null);
            result.LatestVersion = tarballOperationResult.LatestVersion;

            if (!tarballOperationResult.Success)
            {
                result.AreIdentical = false;
                return result;
            }

            if (!_packageDataProvider.TryGetAssetPath(package, out string assetPath))
            {
                result.AreIdentical = false;
                return result;
            }

            var latestTarball = tarballOperationResult.Data;
            var localTarball = await Tarball.GetBytes(assetPath);

            var comparisonReport = GtzPackageComparator.ComparePackages(localTarball, latestTarball);

            result.AreIdentical = comparisonReport.Result == ComparisonResult.Identical;
            return result;
        }

        public async UniTask<List<RegistryPackageGroup>> GetChangedPackages()
        {
            List<UnityPackage> embeddedPackagesManifests = _packageDataProvider.EmbeddedPackagesManifests;
            List<UnityVersionExtended> embeddedPackagesExtended = embeddedPackagesManifests.Select(x => new UnityVersionExtended(x)).ToList();
            List<PackageInfo> allAvailablePackages = await _packageDataProvider.GetAllAvailablePackages(false);
            List<ScopedRegistryExtended> scopedRegistries = new(_registries);

            Dictionary<ScopedRegistryExtended, List<PackageInfo>> packageInfoByRegistry = scopedRegistries
                .GroupJoin(
                    allAvailablePackages,
                    reg => (url: reg.Url, name: reg.Name),
                    pkg => (pkg.registry.url, pkg.registry.name),
                    (reg, pkg) => (Registry: reg, Packages: pkg.ToList())
                )
                .ToDictionary(
                    x => x.Registry,
                    x => x.Packages
                );

            ConcurrentDictionary<ScopedRegistryExtended, List<UnityVersionExtended>> concurrentGroupedByRegistry = new();
            var registryTasks = packageInfoByRegistry.Select(async group =>
            {
                var scopedRegistry = group.Key;
                var packageInfos = group.Value;

                var manifestTasks = packageInfos.Select(async packageInfo =>
                {
                    UnityManifest manifest = await _npmWebRequests.GetManifest(scopedRegistry.Url, scopedRegistry.Token, packageInfo, null, null);

                    if (!manifest.TryGetLatestVersion(out var latestVersion))
                    {
                        return null;
                    }

                    var version = new UnityVersionExtended(latestVersion);
                    version.Manifest = manifest;
                    return version;
                });

                var results = await UniTask.WhenAll(manifestTasks);
                var validManifests = results.Where(m => m != null).ToList();
                concurrentGroupedByRegistry[scopedRegistry] = validManifests;
            });
            await UniTask.WhenAll(registryTasks);

            Dictionary<string, int> packageIdRegistryCollisions = concurrentGroupedByRegistry
                .SelectMany(kvp => kvp.Value.Select(pkg => new
                {
                    pkg.Id,
                    RegistryUrl = kvp.Key.Url,
                }))
                .GroupBy(x => x.Id)
                .Select(g => new
                {
                    Id = g.Key,
                    DistinctRegistryCount = g.Select(x => x.RegistryUrl).Distinct().Count(),
                })
                .Where(x => x.DistinctRegistryCount > 1)
                .ToDictionary(
                    x => x.Id,
                    x => x.DistinctRegistryCount
                );

            Dictionary<ScopedRegistryExtended, List<UnityVersionExtended>> groupedByRegistry = concurrentGroupedByRegistry
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value
                        .Select(package =>
                        {
                            if (packageIdRegistryCollisions.TryGetValue(package.Id, out var count))
                            {
                                package.DifferentRegistries = count;
                            }

                            return package;
                        }).ToList()
                );

            var registryGroups = new List<RegistryPackageGroup>();
            var analysisTasks = groupedByRegistry.Select(async group =>
            {
                var scopedRegistry = group.Key;
                var unityVersions = group.Value;

                var comparer = new PackageNameComparer();
                var availablePackages = unityVersions.Except(embeddedPackagesExtended, comparer).ToList();
                var installablePackages = embeddedPackagesExtended.Except(unityVersions, comparer).ToList();
                var token = scopedRegistry.Token;

                var registryGroup = new RegistryPackageGroup();
                registryGroup.Registry = scopedRegistry;
                registryGroup.AvailablePackages = availablePackages;
                registryGroup.InstallablePackages = installablePackages;

                var commonPackagesData = unityVersions
                    .Where(data => embeddedPackagesExtended.Contains(data, comparer))
                    .Select(unityVersion =>
                    {
                        var embeddedPackage = embeddedPackagesExtended.FirstOrDefault(x => comparer.Equals(unityVersion, x));
                        unityVersion.LocalAssetPath = embeddedPackage?.LocalAssetPath;
                        return unityVersion;
                    })
                    .ToList();
                for (int i = 0, j = commonPackagesData.Count; i < j; i++)
                {
                    var package = commonPackagesData[i];

                    try
                    {
                        var compareResult = await ComparePackage(token, package);
                        package.RemoteVersion = compareResult.LatestVersion;
                        if (compareResult.AreIdentical)
                        {
                            registryGroup.CommonPackages.Add(package);
                        }
                        else
                        {
                            registryGroup.ChangedPackages.Add(package);
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"GetChangedPackages().\nException: {exception}");
                        registryGroup.AvailablePackages.Add(package);
                    }
                }

                registryGroups.Add(registryGroup);
            });
            await UniTask.WhenAll(analysisTasks);

            SetRestoreChangedPackages(registryGroups);
            return registryGroups;
        }

        public void ChangePackageVersion(UnityPackage package, SemanticVersion version)
        {
            _packageDataProvider.ChangePackageVersion(package, version);
        }

        public UniTask<UnityPackage> GetPackage(UnityPackage packageName)
        {
            UpdateUnityPackageManager();
            _packageDataProvider.Initialize();
            var packages = _packageDataProvider.EmbeddedPackagesManifests;
            var result = packages.FirstOrDefault(package => package.Name == packageName.Name);
            return UniTask.FromResult(result);
        }

        public void UpdateUnityPackageManager()
        {
            // Ensure Package Manager window is open
            EditorApplication.ExecuteMenuItem("Window/Package Manager");

            // Wait one frame
            EditorApplication.delayCall += RefreshUpmList;
        }

        private static void RefreshUpmList()
        {
            var packageManagerWindowType = Type.GetType("UnityEditor.PackageManager.UI.PackageManagerWindow,UnityEditor");
            if (packageManagerWindowType == null)
            {
                Debug.LogError("Could not find PackageManagerWindow type");
                return;
            }

            var instanceProperty = packageManagerWindowType.GetProperty("instance", BindingFlags.NonPublic | BindingFlags.Static);
            if (instanceProperty == null)
            {
                Debug.LogError("Could not find 'instance' property");
                return;
            }

            // Get the value of the 'instance' property
            var instanceValue = instanceProperty.GetValue(null);
            if (instanceValue == null)
            {
                Debug.Log("PackageManagerWindow instance is null. The window might not be open.");
            }

            // Get the m_Root field from PackageManagerWindow
            var rootField = packageManagerWindowType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null)
            {
                Debug.LogError("Could not find m_Root field");
                return;
            }

            // Get the value of m_Root field
            var root = rootField.GetValue(instanceValue);
            if (root == null)
            {
                Debug.LogError("m_Root is null");
                return;
            }

            // Get the type of PackageManagerWindowRoot
            var rootType = root.GetType();
            // Get the m_PageManager field from PackageManagerWindowRoot
            var pageManagerField = rootType.GetField("m_PageManager", BindingFlags.NonPublic | BindingFlags.Instance);
            if (pageManagerField == null)
            {
                Debug.LogError("Could not find m_PageManager field");
                return;
            }

            // Get the value of m_PageManager field
            var pageManager = pageManagerField.GetValue(root);
            if (pageManager == null)
            {
                Debug.LogError("m_PageManager is null");
                return;
            }

            var pageManagerType = pageManager.GetType();
            var refreshOptionsType = Type.GetType("UnityEditor.PackageManager.UI.Internal.RefreshOptions,UnityEditor");

            MethodInfo refreshMethod = null;

            // Get all methods named "Refresh"
            var refreshMethods = pageManagerType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "Refresh")
                .ToArray();

            // Look for one that takes a RefreshOptions parameter
            foreach (var method in refreshMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != refreshOptionsType)
                {
                    continue;
                }

                refreshMethod = method;
                break;
            }

            if (refreshMethod == null)
            {
                Debug.LogError("Could not find Refresh method with RefreshOptions parameter");
                return;
            }

            // Create a RefreshOptions enum value
            var refreshOptions = Enum.Parse(refreshOptionsType, "UpmAny");

            // Call the Refresh method
            refreshMethod.Invoke(pageManager, new[] { refreshOptions });
            Debug.Log("Successfully refreshed Package Manager");
        }
    }
}
