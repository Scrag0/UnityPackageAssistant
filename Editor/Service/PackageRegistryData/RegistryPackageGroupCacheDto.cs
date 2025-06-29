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

namespace UnityPackageAssistant
{
    public class RegistryPackageGroupCacheDto
    {
        public ScopedRegistry Registry { get; set; } = new();
        public List<UnityVersionExtendedCacheDto> AvailablePackages { get; set; } = new();
        public List<UnityVersionExtendedCacheDto> CommonPackages { get; set; } = new();
        public List<UnityVersionExtendedCacheDto> ChangedPackages { get; set; } = new();
        public List<UnityVersionExtendedCacheDto> InstallablePackages { get; set; } = new();

        public RegistryPackageGroupCacheDto()
        { }

        public RegistryPackageGroupCacheDto(RegistryPackageGroup registryPackageGroup)
        {
            Registry = registryPackageGroup.Registry.GetClone();
            AvailablePackages = registryPackageGroup.AvailablePackages.Select(x => new UnityVersionExtendedCacheDto(x)).ToList();
            CommonPackages = registryPackageGroup.CommonPackages.Select(x => new UnityVersionExtendedCacheDto(x)).ToList();
            ChangedPackages = registryPackageGroup.ChangedPackages.Select(x => new UnityVersionExtendedCacheDto(x)).ToList();
            InstallablePackages = registryPackageGroup.InstallablePackages.Select(x => new UnityVersionExtendedCacheDto(x)).ToList();
        }

        public RegistryPackageGroup ToRegistryPackageGroup()
        {
            var instance = new RegistryPackageGroup();
            instance.Registry = Registry.GetClone();
            instance.AvailablePackages = AvailablePackages.Select(x => x.ToUnityVersionExtended()).ToList();
            instance.CommonPackages = CommonPackages.Select(x => x.ToUnityVersionExtended()).ToList();
            instance.ChangedPackages = ChangedPackages.Select(x => x.ToUnityVersionExtended()).ToList();
            instance.InstallablePackages = InstallablePackages.Select(x => x.ToUnityVersionExtended()).ToList();
            return instance;
        }
    }
}
