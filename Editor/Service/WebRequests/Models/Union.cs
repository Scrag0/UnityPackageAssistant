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

using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    [JsonConverter(typeof(UnionConverter))]
    public class Union<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public bool IsT1 { get => Item1 != null; }
        public bool IsT2 { get => Item2 != null; }

        public Union()
        { }

        public Union(T1 value) => Item1 = value;
        public Union(T2 value) => Item2 = value;

        public bool TryGetT1(out T1 value)
        {
            value = Item1;
            return Item1 != null;
        }

        public bool TryGetT2(out T2 value)
        {
            value = Item2;
            return Item2 != null;
        }

        public static implicit operator Union<T1, T2>(T1 value) => new(value);
        public static implicit operator Union<T1, T2>(T2 value) => new(value);
    }
}
