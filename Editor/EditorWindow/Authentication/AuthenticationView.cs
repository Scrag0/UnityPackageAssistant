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
using UnityPackageAssistant.Login;

namespace UnityPackageAssistant
{
    public interface IAuthenticationView : ITabView
    {
        void SetRegistries(List<string> registries);
        void SubscribeButtons();
        void UnsubscribeButtons();
    }

    public class AuthenticationView : TabView, IAuthenticationView
    {
        private const string kName = "Account";

        private readonly VisualElement _root = new();
        private readonly VisualElement _tabContainer = new();
        private readonly VisualElement _scopeContainer = new();
        private readonly Button _loginTabButton = new();
        private readonly Button _signupTabButton = new();
        private readonly VisualElement _contentContainer = new();
        private readonly DropdownField _registryDropdown = new();
        private readonly Image _refreshIcon = new();
        private readonly Button _refreshButton = new();
        private readonly Label _statusLabel = new();

        private readonly VisualElement _loginContainer = new();
        private readonly TextField _loginUsernameField = new();
        private readonly TextField _loginPasswordField = new();
        private readonly Button _loginButton = new();
        private readonly Toggle _rememberMeLoginToggle = new();
        private readonly Toggle _rememberMeSignUpToggle = new();

        private readonly VisualElement _signupContainer = new();
        private readonly TextField _signupUsernameField = new();
        private readonly TextField _signupEmailField = new();
        private readonly TextField _signupPasswordField = new();
        private readonly TextField _signupConfirmPasswordField = new();
        private readonly Button _signupButton = new();

        private readonly IAuthenticationController _controller = new AuthenticationController();

        public override string Name { get => kName; }

        public AuthenticationView()
        {
            _controller.Init(this);
            InitializeComponents();
            Compose();
            ApplyStyle();
            SwitchTab(true);
        }

        public void SetRegistries(List<string> registries)
        {
            _registryDropdown.choices = registries;
        }

        public void SubscribeButtons()
        {
            _refreshButton.clicked += OnRefreshButtonClicked;
        }

        public void UnsubscribeButtons()
        {
            _refreshButton.clicked -= OnRefreshButtonClicked;
        }

        private void InitializeComponents()
        {
            _loginTabButton.text = "Login";
            _signupTabButton.text = "Sign Up";

            _registryDropdown.label = "Scope Registry";
            _registryDropdown.index = 0;

            _refreshIcon.image = EditorGUIUtility.IconContent("Refresh").image;

            _loginUsernameField.label = "Username";
            _loginPasswordField.label = "Password";
            _loginPasswordField.isPasswordField = true;

            _rememberMeLoginToggle.label = "Remember me";
            _rememberMeLoginToggle.value = true;
            _loginButton.text = "Login";

            _signupUsernameField.label = "Username";
            _signupEmailField.label = "Email";
            _signupPasswordField.label = "Password";
            _signupPasswordField.isPasswordField = true;
            _signupConfirmPasswordField.label = "Confirm Password";
            _signupConfirmPasswordField.isPasswordField = true;
            _rememberMeSignUpToggle.label = "Remember me";
            _rememberMeSignUpToggle.value = true;
            _signupButton.text = "Create Account";

            _registryDropdown.RegisterValueChangedCallback(OnRegistryChanged);
            _refreshButton.clicked += OnRefreshButtonClicked;
            _loginTabButton.clicked += () => SwitchTab(true);
            _signupTabButton.clicked += () => SwitchTab(false);
            _loginButton.clicked += OnLoginButtonClicked;
            _signupButton.clicked += OnSignupButtonClicked;
        }

        private void Compose()
        {
            _refreshButton.Add(_refreshIcon);
            _scopeContainer.Add(_registryDropdown);
            _scopeContainer.Add(_refreshButton);

            _tabContainer.Add(_loginTabButton);
            _tabContainer.Add(_signupTabButton);

            _loginContainer.Add(_loginUsernameField);
            _loginContainer.Add(_loginPasswordField);
            _loginContainer.Add(_rememberMeLoginToggle);
            _loginContainer.Add(_loginButton);

            _signupContainer.Add(_signupUsernameField);
            _signupContainer.Add(_signupEmailField);
            _signupContainer.Add(_signupPasswordField);
            _signupContainer.Add(_signupConfirmPasswordField);
            _signupContainer.Add(_rememberMeSignUpToggle);
            _signupContainer.Add(_signupButton);

            _contentContainer.Add(_loginContainer);
            _contentContainer.Add(_signupContainer);

            _root.Add(_scopeContainer);
            _root.Add(_tabContainer);
            _root.Add(_contentContainer);
            _root.Add(_statusLabel);
            Add(_root);
        }

        private void ApplyStyle()
        {
            _root.style.width = new Length(100, LengthUnit.Percent);
            _root.style.maxWidth = 400;
            _root.style.alignSelf = Align.Center;
            _root.style.marginTop = 30;
            _root.style.marginBottom = 30;

            _scopeContainer.style.flexDirection = FlexDirection.Row;
            _scopeContainer.style.marginBottom = 0;
            _scopeContainer.style.height = 40;
            _scopeContainer.style.alignItems = Align.Center;
            _scopeContainer.style.alignSelf = Align.Center;

            _tabContainer.style.flexDirection = FlexDirection.Row;
            _tabContainer.style.marginBottom = 0;
            _tabContainer.style.height = 40;

            _contentContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            _contentContainer.style.borderBottomLeftRadius = 5;
            _contentContainer.style.borderBottomRightRadius = 5;
            _contentContainer.style.paddingTop = 25;
            _contentContainer.style.paddingBottom = 25;
            _contentContainer.style.paddingLeft = 20;
            _contentContainer.style.paddingRight = 20;

            StyleTab(_loginTabButton);
            StyleTab(_signupTabButton);
            _loginTabButton.style.borderTopLeftRadius = 5;
            _signupTabButton.style.borderTopRightRadius = 5;

            _rememberMeLoginToggle.style.marginTop = 5;
            _rememberMeLoginToggle.style.marginBottom = 15;

            _refreshButton.style.flexDirection = FlexDirection.Column;
            _refreshButton.style.flexGrow = 0;
            _refreshButton.style.flexShrink = 1;
            _refreshButton.style.width = 30;
            _refreshButton.style.height = 20;

            _refreshIcon.style.height = 20;

            StyleFormContainer(_loginContainer);
            StyleFormContainer(_signupContainer);

            StyleTextField(_loginUsernameField);
            StyleTextField(_loginPasswordField);
            StyleButton(_loginButton, true);
            StyleDropdown(_registryDropdown);

            _rememberMeSignUpToggle.style.marginTop = 5;
            _rememberMeSignUpToggle.style.marginBottom = 15;

            StyleTextField(_signupUsernameField);
            StyleTextField(_signupEmailField);
            StyleTextField(_signupPasswordField);
            StyleTextField(_signupConfirmPasswordField);
            StyleButton(_signupButton, true);

            _statusLabel.style.fontSize = 14;
            _statusLabel.style.marginTop = 15;
            _statusLabel.style.alignSelf = Align.Center;
            _statusLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private void SwitchTab(bool showLogin)
        {
            _statusLabel.text = string.Empty;

            _loginContainer.style.display = showLogin ? DisplayStyle.Flex : DisplayStyle.None;
            _signupContainer.style.display = showLogin ? DisplayStyle.None : DisplayStyle.Flex;

            if (showLogin)
            {
                StyleActiveTab(_loginTabButton);
                StyleInactiveTab(_signupTabButton);
            }
            else
            {
                StyleActiveTab(_signupTabButton);
                StyleInactiveTab(_loginTabButton);
            }
        }

        private void OnLoginButtonClicked()
        {
            string username = _loginUsernameField.value;
            string password = _loginPasswordField.value;
            bool rememberMe = _rememberMeLoginToggle.value;
            _controller.Login(username, password, rememberMe, OnSuccess, OnError);

            void OnSuccess(string message)
            {
                SetSuccessStatus(message);
            }

            void OnError(string message)
            {
                SetErrorStatus(message);
            }
        }

        private void OnSignupButtonClicked()
        {
            string username = _signupUsernameField.value;
            string email = _signupEmailField.value;
            string password = _signupPasswordField.value;
            string confirmPassword = _signupConfirmPasswordField.value;
            bool rememberMe = _rememberMeSignUpToggle.value;

            _controller.SignUp(username, email, password, confirmPassword, rememberMe, OnSuccess, OnError);

            void OnSuccess(string message)
            {
                SetSuccessStatus(message);
            }

            void OnError(string message)
            {
                SetErrorStatus(message);
            }
        }

        private void OnRegistryChanged(ChangeEvent<string> evt)
        {
            var registry = evt.newValue;
            _controller.ChangeRegistry(registry);
        }

        private void OnRefreshButtonClicked()
        {
            _controller.OnRefreshClick();
        }

        private void SetErrorStatus(string message)
        {
            _statusLabel.text = message;
            _statusLabel.style.color = new Color(0.9f, 0.3f, 0.3f); // Red color
        }

        private void SetSuccessStatus(string message)
        {
            _statusLabel.text = message;
            _statusLabel.style.color = new Color(0.3f, 0.9f, 0.5f); // Green color
        }

        private void StyleTab(Button tabButton)
        {
            tabButton.tooltip = "";
            tabButton.style.flexGrow = 1;
            tabButton.style.height = 40;
            tabButton.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            tabButton.style.borderBottomWidth = 0;
            tabButton.style.borderTopWidth = 1;
            tabButton.style.borderLeftWidth = 1;
            tabButton.style.borderRightWidth = 1;

            var color = new Color(0.3f, 0.3f, 0.3f);
            tabButton.style.borderBottomColor = color;
            tabButton.style.borderLeftColor = color;
            tabButton.style.borderTopColor = color;
            tabButton.style.borderRightColor = color;

            var width = 1;
            tabButton.style.borderBottomWidth = width;
            tabButton.style.borderLeftWidth = width;
            tabButton.style.borderTopWidth = width;
            tabButton.style.borderRightWidth = width;

            tabButton.style.fontSize = 14;
            tabButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            tabButton.style.color = Color.white;
        }

        private void StyleActiveTab(Button tabButton)
        {
            tabButton.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            tabButton.style.color = Color.white;
        }

        private void StyleInactiveTab(Button tabButton)
        {
            tabButton.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            tabButton.style.color = new Color(0.7f, 0.7f, 0.7f);
        }

        private void StyleFormContainer(VisualElement container)
        {
            container.style.flexDirection = FlexDirection.Column;
        }

        private void StyleTextField(TextField field)
        {
            field.style.marginBottom = 10;
        }

        private void StyleDropdown(DropdownField dropdown)
        {
            dropdown.style.textOverflow = TextOverflow.Ellipsis;
        }

        private void StyleButton(Button button, bool isPrimary)
        {
            button.style.height = 40;
            button.style.marginTop = 5;
            button.style.marginBottom = 5;
            button.style.fontSize = 14;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;

            if (!isPrimary)
            {
                return;
            }

            button.style.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
            button.style.color = Color.white;
        }
    }
}
