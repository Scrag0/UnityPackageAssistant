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
    public class UnityPackageAssistantWindow : EditorWindow
    {
        private const int kDefaultTabIndex = 0;

        private VisualElement _tabContainer;
        private VisualElement _contentContainer;
        private List<Button> _tabButtons;
        private List<TabView> _tabViews;

        private int _currentPage;
        private UPMService _upmService;

        [MenuItem("Tools/Unity Package Assistant")]
        public static void OpenWindow()
        {
            UnityPackageAssistantWindow wnd = GetWindow<UnityPackageAssistantWindow>();
            wnd.titleContent = new GUIContent("UnityPackageAssistant");
        }

        public void CreateGUI()
        {
            InitializeServiceLocator();
            InitializeComponents();
            Compose();
            ApplyStyle();
            var firstShowTab = _upmService.TryRestoreCurrentTab(out int index) ? index : kDefaultTabIndex;
            FirstShowTab(firstShowTab);
        }

        private void InitializeServiceLocator()
        {
            _upmService = new UPMService();
            ServiceLocator.Register<IUPMService>(_upmService);
        }

        private void InitializeComponents()
        {
            _tabContainer = new();
            _contentContainer = new();
            _tabButtons = new();
            _tabViews = new()
            {
                new PackageView(),
                new AuthenticationView(),
                new AboutView(),
            };

            for (int i = 0, j = _tabViews.Count; i < j; i++)
            {
                int tabIndex = i;
                var view = _tabViews[tabIndex];
                var tabButton = new Button();
                tabButton.text = view.Name;
                tabButton.clicked += () => ShowTab(tabIndex);
                _tabButtons.Add(tabButton);
            }
        }


        private void Compose()
        {
            var root = rootVisualElement;

            for (int i = 0, j = _tabButtons.Count; i < j; i++)
            {
                var tabButton = _tabButtons[i];
                _tabContainer.Add(tabButton);
            }

            for (int i = 0, j = _tabViews.Count; i < j; i++)
            {
                var tabView = _tabViews[i];
                _contentContainer.Add(tabView);
            }

            // Add to root
            root.Add(_tabContainer);
            root.Add(_contentContainer);
        }

        private void ApplyStyle()
        {
            _tabContainer.style.flexDirection = FlexDirection.Row;
            _tabContainer.style.marginTop = 5;
            _tabContainer.style.marginBottom = 5;

            _contentContainer.style.flexGrow = 1;

            for (int i = 0, j = _tabButtons.Count; i < j; i++)
            {
                var tabButton = _tabButtons[i];
                tabButton.style.flexGrow = 1;
            }

            for (int i = 0, j = _tabViews.Count; i < j; i++)
            {
                var tabView = _tabViews[i];
                tabView.style.display = DisplayStyle.None;
            }
        }

        private void ShowTab(int index)
        {
            if (_currentPage == index)
            {
                return;
            }

            if (_tabViews.Count <= index)
            {
                return;
            }

            _upmService.SetRestoreCurrentTab(index);
            _tabViews[_currentPage].style.display = DisplayStyle.None;
            _tabViews[index].style.display = DisplayStyle.Flex;
            _currentPage = index;
        }

        private void FirstShowTab(int index)
        {
            for (int i = 0, j = _tabViews.Count; i < j; i++)
            {
                _tabViews[i].style.display = (i == index) ? DisplayStyle.Flex : DisplayStyle.None;
            }

            _currentPage = index;
        }
    }
}
