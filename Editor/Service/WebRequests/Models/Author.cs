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
using System.ComponentModel;
using Newtonsoft.Json;

namespace Verdaccio.Models
{
    public class Author : ICloneable
    {
        [JsonProperty("username")]
        [DefaultValue("")]
        public string Username { get; set; } = string.Empty;

        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("email")]
        [DefaultValue("")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url { get; set; } = string.Empty;

        public object Clone()
        {
            var clone = new Author();
            clone.Username = Username;
            clone.Name = Name;
            clone.Email = Email;
            clone.Url = Url;
            return clone;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Username) &&
                   string.IsNullOrWhiteSpace(Name) &&
                   string.IsNullOrWhiteSpace(Email) &&
                   string.IsNullOrWhiteSpace(Url);
        }
    }
}
