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
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace UnityPackageAssistant
{
    public interface IPackageView : ITabView
    {
        void SubscribeButtons();
        void UnsubscribeButtons();
        void SetRegistries(List<string> registries);
        void SetAvailablePackages(List<UnityVersionExtended> availablePackages);
        void SetCommonPackages(List<UnityVersionExtended> packages);
        void SetChangedPackages(List<UnityVersionExtended> availablePackages);
        void SetInstallablePackages(List<UnityVersionExtended> availablePackages);
        void SetSuccessStatus(string message);
        void SetErrorStatus(string message);
    }

    public class PackageView : TabView, IPackageView
    {
        private const string kName = "Packages";

        private readonly VisualElement _root = new();
        private readonly VisualElement _headerContainer = new();
        private readonly VisualElement _subHeader = new();
        private readonly DropdownField _registryDropdown = new();
        private readonly Image _refreshIcon = new();
        private readonly Button _refreshButton = new();
        private readonly Button _requestButton = new();
        private readonly Label _statusLabel = new();
        private readonly VisualElement _subContent = new();
        private readonly AvailablePackageListView _availablePackagesListView = new();
        private readonly CommonPackageListView _commonPackagesListView = new();
        private readonly ChangedPackageListView _changedPackagesListView = new();
        private readonly InstallablePackageListView _installablePackagesListView = new();
        private readonly List<IListWithFoldout> _listWithFoldouts;

        private readonly IPackageController _controller = new PackageController();

        public override string Name { get => kName; }

        public PackageView()
        {
            _controller.Init(this);

            _listWithFoldouts = new List<IListWithFoldout>()
            {
                _availablePackagesListView,
                _commonPackagesListView,
                _changedPackagesListView,
                _installablePackagesListView,
            };

            InitializeComponents();
            Compose();
            ApplyStyle();
        }

        public void SubscribeButtons()
        {
            _requestButton.clicked += OnRequestButtonClicked;
            _refreshButton.clicked += OnRefreshButtonClicked;
        }

        public void UnsubscribeButtons()
        {
            _requestButton.clicked -= OnRequestButtonClicked;
            _refreshButton.clicked -= OnRefreshButtonClicked;
        }

        public void SetAvailablePackages(List<UnityVersionExtended> packages)
        {
            _availablePackagesListView.SetItemSource(packages);
        }

        public void SetCommonPackages(List<UnityVersionExtended> packages)
        {
            _commonPackagesListView.SetItemSource(packages);
        }

        public void SetChangedPackages(List<UnityVersionExtended> packages)
        {
            _changedPackagesListView.SetItemSource(packages);
        }

        public void SetInstallablePackages(List<UnityVersionExtended> packages)
        {
            _installablePackagesListView.SetItemSource(packages);
        }

        public void SetRegistries(List<string> registries)
        {
            _registryDropdown.choices = registries;
        }

        public void SetSuccessStatus(string message)
        {
            SetStatus(message, true);
        }

        public void SetErrorStatus(string message)
        {
            SetStatus(message, false);
        }

        private void InitializeComponents()
        {
            _registryDropdown.label = "Scope Registry";
            _registryDropdown.index = 0; // Default to first registry

            _refreshIcon.image = EditorGUIUtility.IconContent("Refresh").image;

            _requestButton.text = "Request";

            _availablePackagesListView.Initialize();
            _availablePackagesListView.OnFoldoutClicked += OnFoldoutClicked;
            _availablePackagesListView.headerTitle = "Available Packages";

            _commonPackagesListView.Initialize();
            _commonPackagesListView.SetInputHandler(_controller);
            _commonPackagesListView.OnFoldoutClicked += OnFoldoutClicked;
            _commonPackagesListView.headerTitle = "Common Packages";

            _changedPackagesListView.Initialize();
            _changedPackagesListView.SetInputHandler(_controller);
            _changedPackagesListView.OnFoldoutClicked += OnFoldoutClicked;
            _changedPackagesListView.headerTitle = "Changed Packages";

            _installablePackagesListView.Initialize();
            _installablePackagesListView.SetInputHandler(_controller);
            _installablePackagesListView.OnFoldoutClicked += OnFoldoutClicked;
            _installablePackagesListView.headerTitle = "Installable Packages";

            _registryDropdown.RegisterValueChangedCallback(OnRegistryChanged);
        }

        private void OnFoldoutClicked(IListWithFoldout listView)
        {
            for (int i = 0, j = _listWithFoldouts.Count; i < j; i++)
            {
                var item = _listWithFoldouts[i];
                if (item == listView)
                {
                    continue;
                }

                var itemFoldout = item.Foldout;
                if (itemFoldout == null)
                {
                    continue;
                }

                itemFoldout.value = false;
            }
        }

        private void Compose()
        {
            _refreshButton.Add(_refreshIcon);

            _subHeader.Add(_registryDropdown);
            _subHeader.Add(_refreshButton);
            _headerContainer.Add(_subHeader);
            _headerContainer.Add(_requestButton);

            _subContent.Add(_availablePackagesListView);
            _subContent.Add(_commonPackagesListView);
            _subContent.Add(_changedPackagesListView);
            _subContent.Add(_installablePackagesListView);

            _root.Add(_headerContainer);
            _root.Add(_subContent);
            _root.Add(_statusLabel);
            Add(_root);
        }

        private void ApplyStyle()
        {
            _root.style.width = new Length(100, LengthUnit.Percent);
            _root.style.height = new Length(100, LengthUnit.Percent);

            _headerContainer.style.flexDirection = FlexDirection.Column;
            _headerContainer.style.flexShrink = 0;
            _headerContainer.style.justifyContent = Justify.SpaceBetween;
            _headerContainer.style.alignItems = Align.Center;
            _headerContainer.style.paddingLeft = 10;
            _headerContainer.style.paddingRight = 10;
            _headerContainer.style.paddingTop = 8;
            _headerContainer.style.paddingBottom = 8;
            _headerContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            _headerContainer.style.borderBottomWidth = 1;
            _headerContainer.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);

            _subHeader.style.flexDirection = FlexDirection.Row;

            _registryDropdown.style.textOverflow = TextOverflow.Ellipsis;

            _refreshIcon.style.height = 20;

            _refreshButton.style.width = 30;
            _refreshButton.style.height = 20;

            _requestButton.style.marginTop = 10;
            _requestButton.style.fontSize = 14;
            _requestButton.style.color = Color.white;
            _requestButton.style.unityFontStyleAndWeight = FontStyle.Bold;

            _statusLabel.style.fontSize = 14;
            _statusLabel.style.paddingTop = 8;
            _statusLabel.style.paddingBottom = 8;
            _statusLabel.style.paddingLeft = 10;
            _statusLabel.style.paddingRight = 10;
            _statusLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            _statusLabel.style.borderTopWidth = 1;
            _statusLabel.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f);
        }

        private void OnRegistryChanged(ChangeEvent<string> evt)
        {
            var registry = evt.newValue;
            _controller.RegistryChanged(registry);
        }

        private void OnRefreshButtonClicked()
        {
            _controller.Refresh();
        }

        private void OnRequestButtonClicked()
        {
            _controller.Request();
        }

        private void SetStatus(string message, bool success)
        {
            _statusLabel.text = message;
            _statusLabel.style.color = success
                ? new Color(0.3f, 0.9f, 0.5f)
                : new Color(0.9f, 0.3f, 0.3f);
        }
    }
}
