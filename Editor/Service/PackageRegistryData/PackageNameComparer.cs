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
    public class PackageNameComparer : IEqualityComparer<UnityVersionExtended>
    {
        public bool Equals(UnityVersionExtended left, UnityVersionExtended right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            return left.Name.Equals(right.Name);
        }

        public int GetHashCode(UnityVersionExtended instance)
        {
            if (instance.Name == null)
                return 0;

            HashCode hash = new();
            hash.Add(instance.Name);
            return hash.ToHashCode();
        }
    }
}
