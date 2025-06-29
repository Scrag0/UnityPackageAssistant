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

namespace UnityPackageAssistant.Login
{
    public interface IAuthenticationController
    {
        void Init(IAuthenticationView authenticationView);
        void ChangeRegistry(string registry);
        void Login(string username, string password, bool rememberMe, Action<string> onSuccess, Action<string> onError);
        void SignUp(string username, string email, string password, string confirmPassword, bool rememberMe, Action<string> onSuccess, Action<string> onError);
        void OnRefreshClick();
    }

    public class AuthenticationController : IAuthenticationController
    {
        private IUPMService _upmService;
        private AuthenticationModel _model;
        private IAuthenticationView _view;

        public void Init(IAuthenticationView authenticationView)
        {
            _upmService = ServiceLocator.Get<IUPMService>();
            _model = BuildModel();
            _view = authenticationView;
            InitView(authenticationView);
            _view.SubscribeButtons();
        }

        public void ChangeRegistry(string registry)
        {
            _model.CurrentRegistry = registry;
        }

        public void Login(string username, string password, bool rememberMe, Action<string> onSuccess, Action<string> onError)
        {
            var registry = _model.CurrentRegistry;
            if (string.IsNullOrWhiteSpace(registry))
            {
                onError?.Invoke("Please add scoped registry to project settings");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                onError?.Invoke("Please enter both username and password");
                return;
            }

            var registryUrl = _model.Registries[registry];

            _upmService.Login(registryUrl, username, password, rememberMe, onSuccess, onError);
        }

        public void SignUp(string username, string email, string password, string confirmPassword, bool rememberMe, Action<string> onSuccess, Action<string> onError)
        {
            var registry = _model.CurrentRegistry;
            if (string.IsNullOrWhiteSpace(registry))
            {
                onError?.Invoke("Please add scoped registry to project settings");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                onError?.Invoke("All fields are required");
                return;
            }

            if (password != confirmPassword)
            {
                onError?.Invoke("Passwords do not match");
                return;
            }

            _upmService.SignUp(registry, username, password, email, rememberMe, onSuccess, onError);
        }

        public void OnRefreshClick()
        {
            _view.UnsubscribeButtons();
            try
            {
                _upmService.Initialize();
                _model = BuildModel();
                InitView(_view);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error refreshing: {ex}");
            }
            finally
            {
                _view.SubscribeButtons();
            }
        }

        private AuthenticationModel BuildModel()
        {
            var model = new AuthenticationModel();
            var scopedRegistries = _upmService.Registries;

            Dictionary<string, string> registries = new();

            for (int i = 0, j = scopedRegistries.Count; i < j; i++)
            {
                var registry = scopedRegistries[i];
                var registryName = $"{registry.Name} ({registry.Url})";
                registryName = registryName.Contains("http") ? registryName.Replace("http://", string.Empty).Replace("https://", string.Empty) : registryName;
                registries.TryAdd(registryName, registry.Url);
            }

            model.Registries = registries;
            model.CurrentRegistry = registries.Keys.FirstOrDefault();
            return model;
        }

        private void InitView(IAuthenticationView view)
        {
            var registries = _model.Registries.Keys.ToList();
            view.SetRegistries(registries);
        }
    }

    public class AuthenticationModel
    {
        public Dictionary<string, string> Registries { get; set; } = new();
        public string CurrentRegistry { get; set; }
    }
}
