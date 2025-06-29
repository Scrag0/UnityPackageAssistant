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
using System.Linq;
using Cysharp.Threading.Tasks;
using NuGet.Versioning;
using UnityEngine;
using Verdaccio.CustomModels;

namespace UnityPackageAssistant
{
    public interface IPublishHandler
    {
        void PublishPackage(UnityVersionExtended package, SemanticVersion version);
    }

    public interface IUnpublishHandler
    {
        void UnpublishPackage(UnityManifest manifest, SemanticVersion version);
    }

    public interface IPackageHandler : IPublishHandler, IUnpublishHandler
    { }

    public interface IPackageController : IPackageHandler
    {
        void Init(IPackageView view);
        void Refresh();
        void Request();
        void RegistryChanged(string registry);
    }

    public class PackageController : IPackageController
    {
        private IUPMService _upmService;
        private PackagesModel _model;
        private IPackageView _view;

        public void Init(IPackageView view)
        {
            _upmService = ServiceLocator.Get<IUPMService>();
            _model = BuildModel();
            _view = view;
            InitView(view);
            view.SubscribeButtons();
            SetAnalysisResult();
        }

        public void Refresh()
        {
            _view.UnsubscribeButtons();
            try
            {
                _upmService.Initialize();
                _model = BuildModel();
                InitView(_view);
                _view.SetSuccessStatus("Successful refresh");
            }
            catch (Exception ex)
            {
                _view.SetErrorStatus($"Error refreshing: {ex.Message}");
                Debug.LogError($"Error refreshing: {ex}");
            }
            finally
            {
                _view.SubscribeButtons();
            }
        }

        public async void Request()
        {
            _view.UnsubscribeButtons();
            try
            {
                _model.RegistryPackageGroups = await _upmService.GetChangedPackages();
                SetAnalysisResult();
                _view.SetSuccessStatus("Packages request successful");
            }
            catch (Exception ex)
            {
                _view.SetErrorStatus($"Error requesting packages: {ex.Message}");
                Debug.LogError($"Error requesting packages: {ex}");
            }
            finally
            {
                _view.SubscribeButtons();
            }
        }

        public void RegistryChanged(string registry)
        {
            if (!_model.Registries.Keys.Contains(registry))
            {
                return;
            }

            _model.CurrentRegistry = registry;
            SetAnalysisResult();
        }

        public async void PublishPackage(UnityVersionExtended package, SemanticVersion version)
        {
            UniTaskCompletionSource<bool> tcs = new();
            ConfirmationPopup.Show("Are you sure you want to publish this item?", result =>
            {
                tcs.TrySetResult(result);
            });

            var result = await tcs.Task;

            if (!result)
            {
                return;
            }

            try
            {
                if (version < package.RemoteVersion)
                {
                    _view.SetErrorStatus($"Current version is less then remote version >>{package.RemoteVersion}<<");
                    Debug.LogError($"Error publishing package: version is less then remote version >>{package.RemoteVersion}<<");
                    return;
                }

                await UpdatePackageVersion(package, version);
                var currentRegistry = _model.CurrentRegistry;
                var registry = _model.Registries[currentRegistry];
                await _upmService.PublishPackage(registry.Url, registry.Token, package, OnSuccess, OnError);

                _view.SetSuccessStatus($"Publishing {package.Name}@{package.Version} to registry...");
            }
            catch (Exception ex)
            {
                _view.SetErrorStatus($"Error publishing package: {ex.Message}");
                Debug.LogError($"Error publishing package: {ex}");
            }

            void OnSuccess(string message)
            {
                _view.SetSuccessStatus(message);
            }

            void OnError(string message)
            {
                _view.SetErrorStatus(message);
            }
        }

        public async void UnpublishPackage(UnityManifest manifest, SemanticVersion version)
        {
            UniTaskCompletionSource<bool> tcs = new();
            ConfirmationPopup.Show("Are you sure you want to delete this item?", result =>
            {
                tcs.TrySetResult(result);
            });

            var result = await tcs.Task;

            if (!result)
            {
                return;
            }

            try
            {
                var currentRegistry = _model.CurrentRegistry;
                var registry = _model.Registries[currentRegistry];
                await _upmService.UnpublishPackage(registry.Url, registry.Token, manifest, version, OnSuccess, OnError);

                _view.SetSuccessStatus($"Unpublishing {manifest.Name}@{version.ToNormalizedString()} from registry...");
            }
            catch (Exception ex)
            {
                _view.SetErrorStatus($"Error unpublishing package: {ex.Message}");
                Debug.LogError($"Error unpublishing package: {ex}");
            }

            void OnSuccess(string message)
            {
                _view.SetSuccessStatus(message);
            }

            void OnError(string message)
            {
                _view.SetErrorStatus(message);
            }
        }

        private PackagesModel BuildModel()
        {
            var model = new PackagesModel();
            var scopedRegistries = _upmService.Registries;

            Dictionary<string, RegistryData> registries = new();

            for (int i = 0, j = scopedRegistries.Count; i < j; i++)
            {
                var registry = scopedRegistries[i];
                var registryName = $"{registry.Name} ({registry.Url})";
                registryName = registryName.Contains("http") ? registryName.Replace("http://", string.Empty).Replace("https://", string.Empty) : registryName;
                var registryData = new RegistryData()
                {
                    Name = registry.Name,
                    Url = registry.Url,
                    Token = registry.Token,
                };
                registries.TryAdd(registryName, registryData);
            }

            model.Registries = registries;
            model.CurrentRegistry = registries.Keys.FirstOrDefault() ?? string.Empty;

            if (_upmService.TryRestoreChangedPackages(out var restoredPackages))
            {
                model.RegistryPackageGroups = restoredPackages;
            }

            return model;
        }

        private void InitView(IPackageView view)
        {
            var registries = _model.Registries.Keys.ToList();
            view.SetRegistries(registries);
        }

        private void SetAnalysisResult()
        {
            var currentRegistry = _model.CurrentRegistry;
            var registry = _model.Registries[currentRegistry];
            var registryPackageGroup = _model.RegistryPackageGroups.FirstOrDefault(x => x.Registry.Name.Equals(registry.Name));
            var availablePackages = registryPackageGroup?.AvailablePackages;
            var commonPackages = registryPackageGroup?.CommonPackages;
            var changedPackages = registryPackageGroup?.ChangedPackages;
            var installablePackages = registryPackageGroup?.InstallablePackages;
            _view.SetAvailablePackages(availablePackages);
            _view.SetCommonPackages(commonPackages);
            _view.SetChangedPackages(changedPackages);
            _view.SetInstallablePackages(installablePackages);
        }

        private async UniTask UpdatePackageVersion(UnityPackage package, SemanticVersion version)
        {
            try
            {
                _upmService.ChangePackageVersion(package, version);
                var updatedPackage = await _upmService.GetPackage(package);

                Debug.Log($"Updating package {package.Name} to version {updatedPackage.Version}");
                _view.SetSuccessStatus($"Updated {package.Name} to version {updatedPackage.Version}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating version: {ex}");
                _view.SetErrorStatus($"Error updating version: {ex.Message}");
            }
        }
    }

    public sealed class PackagesModel
    {
        public Dictionary<string, RegistryData> Registries { get; set; } = new();
        public List<RegistryPackageGroup> RegistryPackageGroups { get; set; } = new();
        public string CurrentRegistry { get; set; }
    }

    public sealed class RegistryData
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }
    }
}
