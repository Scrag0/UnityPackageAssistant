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
using System.Diagnostics.CodeAnalysis;
using NuGet.Versioning;

namespace UnityPackageAssistant
{
    public class CustomSemanticVersion
    {
        private readonly SemanticVersion _defaultVersion;
        private SemanticVersion _currentVersion;

        private SemanticVersion DefaultVersion { get => new(_defaultVersion); }
        public SemanticVersion CurrentVersion { get => new(_currentVersion); }
        public int Major { get => _currentVersion.Major; }
        public int Minor { get => _currentVersion.Minor; }
        public int Patch { get => _currentVersion.Patch; }

        public CustomSemanticVersion([NotNull] CustomSemanticVersion version)
        {
            _defaultVersion = new SemanticVersion(version.DefaultVersion);
            _currentVersion = new SemanticVersion(version.CurrentVersion);
        }

        public CustomSemanticVersion([NotNull] SemanticVersion version)
        {
            _defaultVersion = new SemanticVersion(version);
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public CustomSemanticVersion(int major, int minor, int patch)
        {
            _defaultVersion = new SemanticVersion(major, minor, patch);
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public CustomSemanticVersion(int major, int minor, int patch, string releaseLabel)
        {
            _defaultVersion = new SemanticVersion(major, minor, patch, releaseLabel);
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public CustomSemanticVersion(int major, int minor, int patch, string releaseLabel, string metadata)
        {
            _defaultVersion = new SemanticVersion(major, minor, patch, releaseLabel, metadata);
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public CustomSemanticVersion(int major, int minor, int patch, IEnumerable<string> releaseLabels, string metadata)
        {
            _defaultVersion = new SemanticVersion(major, minor, patch, releaseLabels, metadata);
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public void IncrementMajor()
        {
            _currentVersion = new SemanticVersion(_currentVersion.Major + 1, 0, 0, _currentVersion.Release, _currentVersion.Metadata);
        }

        public void IncrementMinor()
        {
            _currentVersion = new SemanticVersion(_currentVersion.Major, _currentVersion.Minor + 1, 0, _currentVersion.Release, _currentVersion.Metadata);
        }

        public void IncrementPatch()
        {
            _currentVersion = new SemanticVersion(_currentVersion.Major, _currentVersion.Minor, _currentVersion.Patch + 1, _currentVersion.Release, _currentVersion.Metadata);
        }

        public void DecrementMajor()
        {
            _currentVersion = _currentVersion.Major > 0
                ? new SemanticVersion(_currentVersion.Major - 1, _currentVersion.Minor, _currentVersion.Patch, _currentVersion.Release, _currentVersion.Metadata)
                : _currentVersion;
        }

        public void DecrementMinor()
        {
            _currentVersion = _currentVersion.Minor > 0
                ? new SemanticVersion(_currentVersion.Major, _currentVersion.Minor - 1, _currentVersion.Patch, _currentVersion.Release, _currentVersion.Metadata)
                : _currentVersion;
        }

        public void DecrementPatch()
        {
            _currentVersion = _currentVersion.Patch > 0
                ? new SemanticVersion(_currentVersion.Major, _currentVersion.Minor, _currentVersion.Patch - 1, _currentVersion.Release, _currentVersion.Metadata)
                : _currentVersion;
        }

        public void Reset()
        {
            _currentVersion = new SemanticVersion(_defaultVersion);
        }

        public override string ToString()
        {
            return _currentVersion.ToNormalizedString();
        }
    }
}
