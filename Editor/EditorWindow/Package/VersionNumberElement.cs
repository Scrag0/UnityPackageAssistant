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
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityPackageAssistant
{
    public class VersionNumberElement : VisualElement
    {
        public event Action<int> OnIncrementClicked;
        public event Action<int> OnDecrementClicked;

        private readonly VisualElement _root = new();
        private readonly Button _decrementButton = new();
        private readonly Label _valueLabel = new();
        private readonly Button _incrementButton = new();
        private int _component;

        public VersionNumberElement()
        {
            InitializeComponents();
            Compose();
            ApplyStyle();
        }

        public void Init(int component, int initialValue)
        {
            _component = component;
            SetValue(initialValue);
        }

        public void SetActiveButtons(bool state)
        {
            _decrementButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
            _incrementButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetValue(int value)
        {
            _valueLabel.text = value.ToString();
        }

        private void InitializeComponents()
        {
            _incrementButton.text = "+";
            _incrementButton.clicked += OnIncrement;

            _decrementButton.text = "-";
            _decrementButton.clicked += OnDecrement;
        }

        private void Compose()
        {
            _root.Add(_incrementButton);
            _root.Add(_valueLabel);
            _root.Add(_decrementButton);
            Add(_root);
        }

        private void ApplyStyle()
        {
            _root.style.flexDirection = FlexDirection.Column;
            _root.style.alignItems = Align.Center;

            _decrementButton.style.width = 20;
            _decrementButton.style.height = 20;
            _decrementButton.style.paddingTop = 0;
            _decrementButton.style.paddingBottom = 0;
            _decrementButton.style.paddingLeft = 0;
            _decrementButton.style.paddingRight = 0;
            _decrementButton.style.unityTextAlign = TextAnchor.MiddleCenter;

            _valueLabel.style.width = 20;
            _valueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            _incrementButton.style.width = 20;
            _incrementButton.style.height = 20;
            _incrementButton.style.paddingTop = 0;
            _incrementButton.style.paddingBottom = 0;
            _incrementButton.style.paddingLeft = 0;
            _incrementButton.style.paddingRight = 0;
            _incrementButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private void OnIncrement()
        {
            OnIncrementClicked?.Invoke(_component);
        }

        private void OnDecrement()
        {
            OnDecrementClicked?.Invoke(_component);
        }
    }
}
