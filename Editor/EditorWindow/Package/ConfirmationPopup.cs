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

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityPackageAssistant
{
    public class ConfirmationPopup : EditorWindow
    {
        private VisualElement Root { get => rootVisualElement; }
        private Label _label;
        private VisualElement _buttonHolder;
        private Button _confirmButton;
        private Button _cancelButton;
        private System.Action<bool> _onResult;
        private string _message;

        public static void Show(string message, System.Action<bool> onResult)
        {
            ConfirmationPopup window = CreateInstance<ConfirmationPopup>();
            window.titleContent = new GUIContent("Confirmation");
            window._message = message;
            window._onResult = onResult;
            window.minSize = new Vector2(250, 100);
            window.maxSize = new Vector2(250, 100);
            window.ShowUtility();
        }

        public void CreateGUI()
        {
            InitializeComponents();
            Compose();
            ApplyStyle();
        }

        private void InitializeComponents()
        {
            _label = new Label();
            _buttonHolder = new VisualElement();
            _confirmButton = new Button();
            _cancelButton = new Button();

            _label.text = _message;

            _confirmButton.text = "Confirm";
            _cancelButton.text = "Cancel";

            _confirmButton.clicked += () => CloseWithResult(true);
            _cancelButton.clicked += () => CloseWithResult(false);
        }

        private void Compose()
        {
            _buttonHolder.Add(_cancelButton);
            _buttonHolder.Add(_confirmButton);

            Root.Add(_label);
            Root.Add(_buttonHolder);
        }

        private void ApplyStyle()
        {
            Root.style.paddingTop = 15;
            Root.style.paddingBottom = 15;
            Root.style.paddingLeft = 20;
            Root.style.paddingRight = 20;
            Root.style.flexDirection = FlexDirection.Column;
            Root.style.alignItems = Align.Center;

            _label.style.unityTextAlign = TextAnchor.MiddleCenter;
            _label.style.whiteSpace = WhiteSpace.Normal;
            _label.style.marginBottom = 5;
            _label.style.fontSize = 14;
            _label.style.flexGrow = 1;
            _label.style.flexShrink = 1;
            _label.style.unityFontStyleAndWeight = FontStyle.Bold;
            _label.style.color = new Color(0.278f, 0.981f, 0.199f);

            _buttonHolder.style.flexDirection = FlexDirection.Row;
            _buttonHolder.style.justifyContent = Justify.SpaceBetween;
            _buttonHolder.style.width = new Length(100, LengthUnit.Percent);
            _buttonHolder.style.marginTop = 10;
            _buttonHolder.style.unityParagraphSpacing = 10;

            StyleButton(_confirmButton, new Color(0.2f, 0.6f, 0.2f)); // green
            StyleButton(_cancelButton, new Color(0.8f, 0.2f, 0.2f)); // red
        }

        private void StyleButton(Button button, Color bgColor)
        {
            button.style.backgroundColor = bgColor;
            button.style.color = Color.white;
            button.style.borderTopLeftRadius = 5;
            button.style.borderTopRightRadius = 5;
            button.style.borderBottomLeftRadius = 5;
            button.style.borderBottomRightRadius = 5;
            button.style.paddingTop = 5;
            button.style.paddingBottom = 5;
            button.style.paddingLeft = 10;
            button.style.paddingRight = 10;
            button.style.flexGrow = 1;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private void CloseWithResult(bool result)
        {
            _onResult?.Invoke(result);
            Close();
        }
    }
}
