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
using NuGet.Versioning;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Verdaccio.CustomModels;

namespace UnityPackageAssistant
{
    public class BasePackageElement : VisualElement
    {
        public event Action<UnityVersionExtended, SemanticVersion> OnPublishClicked;
        public event Action<UnityManifest, SemanticVersion> OnUnpublishClicked;

        public event Action<SemanticVersion> OnVersionUpdated;

        // UI components
        protected readonly VisualElement _root = new();
        protected readonly Label _nameLabel = new();
        protected readonly VisualElement _detailsContainer = new();
        protected readonly VisualElement _warningContainer = new();
        protected readonly PackageVersionView _packageVersionView = new();
        protected readonly Label _warningLabel = new();
        protected readonly Image _warningIcon = new();
        private readonly VisualElement _publishContainer = new();
        private readonly Button _publishButton = new();

        private readonly VisualElement _unpublishContainer = new();
        private readonly Label _versionLabel = new();
        private readonly Button _unpublishButton = new();
        private readonly DropdownField _remoteVersions = new();
        private Dictionary<string, SemanticVersion> _remoteVersionsDictionary = new();
        protected UnityVersionExtended _package = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            InitializeComponents();
            Compose();
            ApplyStyle();
            DoOnInitialize();
            _isInitialized = true;
        }

        public void SetData(UnityVersionExtended package)
        {
            _package = package;

            _packageVersionView.Init(package.Version);

            // Initialize basic components
            _nameLabel.text = _package.Name;

            if (!string.IsNullOrEmpty(_package.Description))
            {
                _nameLabel.tooltip = _package.Description;
            }

            var differentRegistries = _package.DifferentRegistries;
            if (differentRegistries != 0 && differentRegistries != 1)
            {
                _warningContainer.style.display = DisplayStyle.Flex;
                _warningLabel.text = $"In {differentRegistries} different registries";
            }

            var warningIcon = EditorGUIUtility.IconContent("console.warnicon").image;
            _warningIcon.image = warningIcon;

            _packageVersionView.OnVersionUpdated -= OnVersionUpdated;
            _packageVersionView.OnVersionUpdated += OnVersionUpdated;

            DoOnSetData(package);
        }

        protected void SetActivePublish(bool state)
        {
            _packageVersionView.SetActiveVersionChange(state);
            _publishButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }

        protected void SetActiveUnpublish(bool state)
        {
            _unpublishContainer.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }

        protected virtual void InitializeComponents()
        {
            _publishButton.text = "Publish";
            _unpublishButton.text = "Unpublish";
            _versionLabel.text = "Version:";
            _remoteVersions.index = 0;
        }

        protected virtual void Compose()
        {
            _publishContainer.Add(_packageVersionView);
            _publishContainer.Add(_publishButton);

            _unpublishContainer.Add(_versionLabel);
            _unpublishContainer.Add(_remoteVersions);
            _unpublishContainer.Add(_unpublishButton);

            _detailsContainer.Add(_publishContainer);
            _detailsContainer.Add(_unpublishContainer);

            _warningContainer.Add(_warningIcon);
            _warningContainer.Add(_warningLabel);

            _root.Add(_nameLabel);
            _root.Add(_warningContainer);
            _root.Add(_detailsContainer);
            Add(_root);
        }

        protected virtual void ApplyStyle()
        {
            // Package container styles
            _root.style.marginTop = 8;
            _root.style.marginBottom = 8;
            _root.style.marginLeft = 10;
            _root.style.marginRight = 10;
            _root.style.paddingTop = 10;
            _root.style.paddingBottom = 10;
            _root.style.paddingLeft = 12;
            _root.style.paddingRight = 12;
            _root.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);

            // Name label styles
            _nameLabel.style.fontSize = 14;
            _nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _nameLabel.style.marginBottom = 4;

            _warningContainer.style.display = DisplayStyle.None;
            _warningContainer.style.flexDirection = FlexDirection.Row;
            _warningContainer.style.alignItems = Align.Center;
            _warningContainer.style.alignSelf = Align.FlexStart;

            _warningIcon.scaleMode = ScaleMode.ScaleToFit;
            _warningIcon.style.width = 24;
            _warningIcon.style.height = 24;
            _warningIcon.style.marginRight = 5;

            _warningLabel.style.color = Color.yellow;

            // Details container styles
            _detailsContainer.style.marginLeft = 10;

            _publishContainer.style.flexDirection = FlexDirection.Row;
            _publishContainer.style.alignItems = Align.Center;
            _publishContainer.style.marginTop = 10;
            _publishContainer.style.marginBottom = 10;

            _unpublishContainer.style.flexDirection = FlexDirection.Row;
            _unpublishContainer.style.alignItems = Align.Center;
            _unpublishContainer.style.marginTop = 10;
            _unpublishContainer.style.marginBottom = 10;
            _versionLabel.style.width = 60;

            _remoteVersions.style.textOverflow = TextOverflow.Ellipsis;
            _remoteVersions.style.height = 25;

            StyleButton(_publishButton);
            StyleButton(_unpublishButton);
        }

        protected virtual void DoOnInitialize()
        { }

        protected virtual void DoOnSetData(UnityVersionExtended package)
        {
            _remoteVersionsDictionary = package
                .RemoteVersions
                .ToDictionary(x => x.ToString());

            _remoteVersions.choices = _remoteVersionsDictionary.Keys.ToList();
            _remoteVersions.index = 0;

            _publishButton.clicked -= OnPublishButtonClick;
            _publishButton.clicked += OnPublishButtonClick;
            _unpublishButton.clicked -= OnUnpublishButtonClick;
            _unpublishButton.clicked += OnUnpublishButtonClick;
        }

        protected virtual void StyleButton(Button button)
        {
            button.style.height = 30;
            button.style.paddingLeft = 12;
            button.style.paddingRight = 12;
            button.style.fontSize = 12;
            button.style.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
            button.style.color = Color.white;
        }

        private void OnPublishButtonClick()
        {
            OnPublishClicked?.Invoke(_package, _packageVersionView.Version);
        }

        private void OnUnpublishButtonClick()
        {
            var remoteVersion = _remoteVersions.value;
            var semanticVersion = _remoteVersionsDictionary[remoteVersion];
            var manifest = _package.Manifest;
            OnUnpublishClicked?.Invoke(manifest, semanticVersion);
        }
    }
}
