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

namespace UnityPackageAssistant
{
    public static class HashCodeExtensions
    {
        public static int ListToHashCode<T>(this IList<T> list)
        {
            HashCode hash = new();

            if (list == null)
            {
                hash.Add(0);
                return hash.ToHashCode();
            }

            if (list.Count == 0)
            {
                hash.Add(1);
                return hash.ToHashCode();
            }

            hash.Add(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                hash.Add(list[i]);
            }

            return hash.ToHashCode();
        }

        public static int DictionaryToHashCode<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            HashCode hash = new();

            if (dictionary == null)
            {
                hash.Add(0);
                return hash.ToHashCode();
            }

            if (dictionary.Count == 0)
            {
                hash.Add(1);
                return hash.ToHashCode();
            }

            hash.Add(dictionary.Count);

            // Create a sorted list of keys to ensure consistent ordering
            var sortedKeys = new List<TKey>(dictionary.Keys);
            sortedKeys.Sort((a, b) => Comparer<TKey>.Default.Compare(a, b));

            foreach (var key in sortedKeys)
            {
                hash.Add(key);
                hash.Add(dictionary[key]);
            }

            return hash.ToHashCode();
        }
    }
}
