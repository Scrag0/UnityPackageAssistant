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

using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using Cysharp.Threading.Tasks;

namespace UnityPackageAssistant
{
    public static class Tarball
    {
        public static async UniTask<byte[]> GetBytes(string packagePath)
        {
            string folder = FileUtil.GetUniqueTempPathInProject();
            var result = Client.Pack(packagePath, folder);
            await UniTask.WaitUntil(() => result.IsCompleted);
            var outputFilePath = result.Result.tarballPath;
            byte[] bytes = await File.ReadAllBytesAsync(outputFilePath);
            File.Delete(outputFilePath);
            Directory.Delete(folder);
            return bytes;
        }
    }
}
