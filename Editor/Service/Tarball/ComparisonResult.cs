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

namespace UnityPackageAssistant
{
    [Flags]
    public enum ComparisonResult
    {
        Identical = 0,
        PackageJsonDiffers = 1 << 0,
        FileCountDiffers = 1 << 1,
        FileNamesDiffer = 1 << 2,
        FileContentsDiffer = 1 << 3,
        MetadataDiffers = 1 << 4,
    }
}
