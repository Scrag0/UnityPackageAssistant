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

namespace Extensions
{
    public static class ICLoneableExtensions
    {
        public static IEnumerable<T> GetCloneData<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.Select(x => (T)x.Clone()).ToList();
        }

        public static List<T> GetCloneDataList<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.GetCloneData().ToList();
        }

        public static Dictionary<TKey, TValue> GetCloneDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TValue : ICloneable
        {
            return dictionary.ToDictionary(
                kvp => kvp.Key,
                kvp => (TValue)kvp.Value.Clone()
            );
        }

        public static T GetClone<T>(this T instance) where T : ICloneable
        {
            return (T)instance.Clone();
        }
    }
}
