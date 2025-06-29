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
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    public interface IProjectManifestProvider : IProvider
    {
        List<ScopedRegistry> Registries { get; }
        Dictionary<string, string> Dependencies { get; }
        void Save(ScopedRegistry registry);
        void Remove(ScopedRegistry registry);
    }

    public class ProjectManifestProvider : IProjectManifestProvider
    {
        private readonly string _manifest = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
        private ProjectManifest _projectManifest;

        public List<ScopedRegistry> Registries { get => new List<ScopedRegistry>(_projectManifest.ScopedRegistries); }
        public Dictionary<string, string> Dependencies { get => new Dictionary<string, string>(_projectManifest.Dependencies); }

        public void Initialize()
        {
            var data = File.ReadAllText(_manifest);
            _projectManifest = JsonConvert.DeserializeObject<ProjectManifest>(data);
        }

        public void Save(ScopedRegistry registry)
        {
            var hasRegistry = false;
            var registryIndex = -1;
            var scopedRegistries = _projectManifest.ScopedRegistries;
            for (int i = 0, j = scopedRegistries.Count; i < j; i++)
            {
                var scopedRegistry = scopedRegistries[i];
                if (scopedRegistry.Name == null || scopedRegistry.Url == null ||
                    !scopedRegistry.Name.Equals(registry.Name, StringComparison.Ordinal) ||
                    !scopedRegistry.Url.Equals(registry.Url, StringComparison.Ordinal))
                {
                    continue;
                }

                hasRegistry = true;
                registryIndex = i;
                break;
            }

            if (!hasRegistry)
            {
                _projectManifest.ScopedRegistries.Add(registry);
            }
            else
            {
                _projectManifest.ScopedRegistries[registryIndex].OverrideBuiltIns = registry.OverrideBuiltIns;
                _projectManifest.ScopedRegistries[registryIndex].Scopes = new List<string>(registry.Scopes).ToArray();
            }

            SaveManifest();
        }

        public void Remove(ScopedRegistry registry)
        {
            var scopedRegistries = _projectManifest.ScopedRegistries;
            for (int i = 0, j = scopedRegistries.Count; i < j; i++)
            {
                var scopedRegistry = scopedRegistries[i];
                if (scopedRegistry.Name == null || scopedRegistry.Url == null ||
                    !scopedRegistry.Name.Equals(registry.Name, StringComparison.Ordinal) ||
                    !scopedRegistry.Url.Equals(registry.Url, StringComparison.Ordinal))
                {
                    continue;
                }

                scopedRegistries.RemoveAt(i);
                break;
            }

            SaveManifest();
        }

        private void SaveManifest()
        {
            var json = JsonConvert.SerializeObject(_projectManifest);
            File.WriteAllText(_manifest, json);
            AssetDatabase.Refresh();
        }
    }
}
