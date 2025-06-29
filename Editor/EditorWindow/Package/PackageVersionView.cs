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
using NuGet.Versioning;
using UnityEngine.UIElements;

namespace UnityPackageAssistant
{
    public class PackageVersionView : VisualElement
    {
        public event Action<SemanticVersion> OnVersionUpdated;

        private readonly VisualElement _root = new();
        private readonly Label _versionLabel = new();
        private readonly VisualElement _versionElementsContainer = new();
        private readonly VersionNumberElement _majorNumberElement = new();
        private readonly VersionNumberElement _minorNumberElement = new();
        private readonly VersionNumberElement _patchNumberElement = new();
        private CustomSemanticVersion _versionComponents;

        public SemanticVersion Version { get => _versionComponents.CurrentVersion; }

        public PackageVersionView()
        {
            InitializeComponents();
            Compose();
            ApplyStyle();
            SetValuesDefault();
        }

        public void Init(SemanticVersion version)
        {
            _versionComponents = new CustomSemanticVersion(version);

            _majorNumberElement.Init(0, _versionComponents.Major);
            _minorNumberElement.Init(1, _versionComponents.Minor);
            _patchNumberElement.Init(2, _versionComponents.Patch);
        }

        public void SetActiveVersionChange(bool state)
        {
            _majorNumberElement.SetActiveButtons(state);
            _minorNumberElement.SetActiveButtons(state);
            _patchNumberElement.SetActiveButtons(state);
        }

        private void InitializeComponents()
        {
            _versionLabel.text = "Version:";
            _majorNumberElement.OnIncrementClicked += OnVersionIncrement;
            _majorNumberElement.OnDecrementClicked += OnVersionDecrement;

            _minorNumberElement.OnIncrementClicked += OnVersionIncrement;
            _minorNumberElement.OnDecrementClicked += OnVersionDecrement;

            _patchNumberElement.OnIncrementClicked += OnVersionIncrement;
            _patchNumberElement.OnDecrementClicked += OnVersionDecrement;
        }

        private void Compose()
        {
            _root.Add(_versionLabel);
            _versionElementsContainer.Add(_majorNumberElement);

            var separator1 = new Label(".");
            separator1.AddToClassList("version-separator");
            _versionElementsContainer.Add(separator1);
            _versionElementsContainer.Add(_minorNumberElement);

            var separator2 = new Label(".");
            separator2.AddToClassList("version-separator");
            _versionElementsContainer.Add(separator2);
            _versionElementsContainer.Add(_patchNumberElement);

            _root.Add(_versionElementsContainer);
            Add(_root);
        }

        private void ApplyStyle()
        {
            // Version container styles
            _root.style.flexDirection = FlexDirection.Row;
            _root.style.alignItems = Align.Center;
            _root.style.borderBottomLeftRadius = 4;
            _root.style.borderBottomRightRadius = 4;
            _root.style.borderTopLeftRadius = 4;
            _root.style.borderTopRightRadius = 4;

            // Version label styles
            _versionLabel.style.width = 60;

            // Version controls container styles
            _versionElementsContainer.style.flexDirection = FlexDirection.Row;
            _versionElementsContainer.style.alignItems = Align.Center;
        }

        private void SetValuesDefault()
        {
            _majorNumberElement.Init(0, 0);
            _minorNumberElement.Init(1, 0);
            _patchNumberElement.Init(2, 0);
        }

        private void OnVersionIncrement(int component)
        {
            switch (component)
            {
                case 0: // major
                    _versionComponents.IncrementMajor();
                    break;

                case 1: // minor
                    _versionComponents.IncrementMinor();
                    break;

                case 2: // patch
                    _versionComponents.IncrementPatch();
                    break;
            }

            _majorNumberElement.SetValue(_versionComponents.Major);
            _minorNumberElement.SetValue(_versionComponents.Minor);
            _patchNumberElement.SetValue(_versionComponents.Patch);
            OnVersionUpdated?.Invoke(Version);
        }

        private void OnVersionDecrement(int component)
        {
            switch (component)
            {
                case 0: // major
                    _versionComponents.DecrementMajor();
                    break;

                case 1: // minor
                    _versionComponents.DecrementMinor();
                    break;

                case 2: // patch
                    _versionComponents.DecrementPatch();
                    break;
            }

            _majorNumberElement.SetValue(_versionComponents.Major);
            _minorNumberElement.SetValue(_versionComponents.Minor);
            _patchNumberElement.SetValue(_versionComponents.Patch);
            OnVersionUpdated?.Invoke(Version);
        }
    }
}
