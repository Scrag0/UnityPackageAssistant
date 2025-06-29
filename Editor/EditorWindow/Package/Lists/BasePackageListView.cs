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
using UnityEngine.UIElements;

namespace UnityPackageAssistant
{
    public interface IListWithFoldout
    {
        event Action<IListWithFoldout> OnFoldoutClicked;
        Foldout Foldout { get; }
    }

    public abstract class BasePackageListView<TView> : CustomListView<TView, UnityVersionExtended>, IListWithFoldout
        where TView : BasePackageElement, new()
    {
        private Foldout _headerFoldout;

        public event Action<IListWithFoldout> OnFoldoutClicked;
        public Foldout Foldout { get => _headerFoldout; }

        protected override void DoOnInitialize()
        {
            base.DoOnInitialize();
            _headerFoldout = this.Q<Foldout>(className: foldoutHeaderUssClassName);
            if (_headerFoldout == null)
            {
                return;
            }

            _headerFoldout.value = false;

            _headerFoldout.UnregisterCallback<ClickEvent>(ProcessFoldoutClick);
            _headerFoldout.RegisterCallback<ClickEvent>(ProcessFoldoutClick);
        }

        protected override void OnBindItem(TView item, int index)
        {
            var data = GetItem(index);
            item.Initialize();
            item.SetData(data);
        }

        protected override void OnUnbindItem(TView item, int index)
        { }

        private void ProcessFoldoutClick(ClickEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            OnFoldoutClicked?.Invoke(this);
        }
    }
}
