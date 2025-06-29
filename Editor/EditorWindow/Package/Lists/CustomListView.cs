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

namespace UnityPackageAssistant
{
    public class CustomListView<TView, TData> : ListView
        where TView : VisualElement, new()
    {
        public void Initialize()
        {
            InitializeComponents();
            Compose();
            ApplyStyle();
            DoOnInitialize();
        }

        public void SetItemSource(List<TData> list)
        {
            itemsSource = list;
        }

        public TData GetItem(int index)
        {
            return (TData)itemsSource[index];
        }

        protected virtual void DoOnInitialize()
        { }

        protected virtual void OnBindItem(TView item, int index)
        { }

        protected virtual void OnUnbindItem(TView item, int index)
        { }

        protected virtual void InitializeComponents()
        {
            headerTitle = "List of packages";

            makeItem = () =>
            {
                var packageElement = new TView();
                packageElement.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    packageElement.style.backgroundColor = Color.clear;
                });

                packageElement.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    packageElement.style.backgroundColor = Color.clear;
                });
                return packageElement;
            };
            bindItem = (element, i) =>
            {
                if (element is not TView item)
                {
                    return;
                }

                OnBindItem(item, i);
            };
            unbindItem = (element, i) =>
            {
                if (element is not TView item)
                {
                    return;
                }

                OnUnbindItem(item, i);
            };
        }

        protected virtual void Compose()
        { }

        protected virtual void ApplyStyle()
        {
            fixedItemHeight = 60;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            style.flexGrow = 1;
            showFoldoutHeader = true;
            showBoundCollectionSize = false;
        }
    }
}
