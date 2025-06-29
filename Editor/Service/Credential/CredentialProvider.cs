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
using System.IO;
using System.Linq;
using UnityEngine;
using Tomlyn;
using Tomlyn.Model;
using Tomlyn.Syntax;

namespace UnityPackageAssistant
{
    public class Credential
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public bool AlwaysAuth { get; set; }
    }

    public interface ICredentialProvider : IProvider
    {
        List<Credential> CredentialSet { get; }
        String[] Registries { get; }
        void SetCredential(string url, bool alwaysAuth, string token);
        void RemoveCredentialForRegistry(string url);
        bool HasRegistry(string url);
        bool TryGetCredential(string url, out Credential credential);
    }

    public class CredentialProvider : ICredentialProvider
    {
        private const string kNpmAuthKey = "npmAuth";
        private const string kTokenKey = "token";
        private const string kAlwaysAuthKey = "alwaysAuth";

        private readonly string _upmConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".upmconfig.toml");
        private readonly List<Credential> _credentials = new();

        public List<Credential> CredentialSet { get => new(_credentials); }

        public String[] Registries
        {
            get
            {
                var credentialsCount = _credentials.Count;
                String[] urls = new String[credentialsCount];
                for (int i = 0; i < credentialsCount; i++)
                {
                    urls[i] = _credentials[i].Url;
                }

                return urls;
            }
        }

        public void Initialize()
        {
            if (!File.Exists(_upmConfigFile))
            {
                return;
            }

            _credentials.Clear();

            var upmConfigFileData = File.ReadAllText(_upmConfigFile);
            var upmConfig = Toml.Parse(upmConfigFileData);
            if (upmConfig.HasErrors)
            {
                Debug.LogError("Cannot load upmconfig, invalid format");
                return;
            }

            TomlTable table = upmConfig.ToModel();

            if (!table.TryGetValue(kNpmAuthKey, out var npmAuth))
            {
                return;
            }

            TomlTable auth = (TomlTable)npmAuth;

            foreach (var registry in auth)
            {
                Credential cred = new Credential();
                cred.Url = registry.Key;
                TomlTable value = (TomlTable)registry.Value;
                cred.Token = (string)value[kTokenKey];
                cred.AlwaysAuth = (bool)value[kAlwaysAuthKey];

                _credentials.Add(cred);
            }
        }

        public void SetCredential(string url, bool alwaysAuth, string token)
        {
            if (!TryGetCredential(url, out var cred))
            {
                cred = new Credential();
                _credentials.Add(cred);
            }

            cred.Url = url;
            cred.AlwaysAuth = alwaysAuth;
            cred.Token = token;

            Save();
        }

        public void RemoveCredentialForRegistry(string url)
        {
            _credentials.RemoveAll(x => x.Url.Equals(url, StringComparison.Ordinal));
            Save();
        }

        public bool HasRegistry(string url)
        {
            return _credentials.Any(x => x.Url.Equals(url, StringComparison.Ordinal));
        }

        public bool TryGetCredential(string url, out Credential credential)
        {
            credential = _credentials.FirstOrDefault(x => !string.IsNullOrEmpty(x.Url) && x.Url.Equals(url, StringComparison.Ordinal));
            return credential != null;
        }

        private void Save()
        {
            var documentSyntax = new DocumentSyntax();
            for (int i = 0, j = _credentials.Count; i < j; i++)
            {
                var credential = _credentials[i];
                if (string.IsNullOrEmpty(credential.Token))
                {
                    credential.Token = "";
                }

                var keySyntax = new KeySyntax(kNpmAuthKey, credential.Url);
                var tableSyntax = new TableSyntax(keySyntax)
                {
                    Items =
                    {
                        { kTokenKey, credential.Token },
                        { kAlwaysAuthKey, credential.AlwaysAuth }
                    },
                };
                documentSyntax.Tables.Add(tableSyntax);
            }

            File.WriteAllText(_upmConfigFile, documentSyntax.ToString());
        }
    }
}
