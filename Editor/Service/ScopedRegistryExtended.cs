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
using UnityEngine;
using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    public class ScopedRegistryExtended : ScopedRegistry
    {
        public bool Auth { get; set; }
        public string Token { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsValidCredential()
        {
            if (string.IsNullOrWhiteSpace(Url) || !Uri.IsWellFormedUriString(Url, UriKind.Absolute))
            {
                return false;
            }

            if (!Auth)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(Token);
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            if (Scopes.Length < 1)
            {
                return false;
            }

            var clearedScopes = new List<string>(Scopes);
            clearedScopes.RemoveAll(string.IsNullOrWhiteSpace);
            Scopes = clearedScopes.ToArray();

            for (int i = 0, j = Scopes.Length; i < j; i++)
            {
                var scope = Scopes[i];
                if (Uri.CheckHostName(scope) == UriHostNameType.Dns)
                {
                    continue;
                }

                Debug.LogWarning("Invalid scope " + scope);
                return false;
            }

            return IsValidCredential();
        }
    }
}
