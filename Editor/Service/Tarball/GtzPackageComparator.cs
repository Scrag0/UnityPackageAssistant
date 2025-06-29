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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Unity.SharpZipLib.Tar;
using Unity.SharpZipLib.GZip;
using Newtonsoft.Json;

namespace UnityPackageAssistant
{
    public static class GtzPackageComparator
    {
        private const string kPackageFolder = "package";
        private const string kPackageJsonFile = "package.json";

        public static ComparisonReport ComparePackages(byte[] packageOne, byte[] packageTwo)
        {
            ComparisonResult result = ComparisonResult.Identical;
            var detailedDifferences = new List<string>();
            var differentFiles = new List<string>();

            string tempFolder1 = FileUtil.GetUniqueTempPathInProject();
            string tempFolder2 = FileUtil.GetUniqueTempPathInProject();

            try
            {
                ExtractGtzPackage(packageOne, tempFolder1);
                ExtractGtzPackage(packageTwo, tempFolder2);

                result = ComparePackageJson(tempFolder1, tempFolder2, result, detailedDifferences);

                var files1 = GetFileNamesInDirectory(tempFolder1);
                var files2 = GetFileNamesInDirectory(tempFolder2);

                if (files1.Count != files2.Count)
                {
                    result |= ComparisonResult.FileCountDiffers;
                    detailedDifferences.Add($"File count differs: {files1.Count} vs {files2.Count}");
                }

                result = CompareFileStructures(files1, files2, result, detailedDifferences, differentFiles);

                result = CompareCommonFiles(files1, files2, tempFolder1, tempFolder2, result, detailedDifferences, differentFiles);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error comparing packages: {ex.Message}\n{ex.StackTrace}");
                detailedDifferences.Add($"Error during comparison: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempFolder1))
                    {
                        Directory.Delete(tempFolder1, true);
                    }

                    if (Directory.Exists(tempFolder2))
                    {
                        Directory.Delete(tempFolder2, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error cleaning up temporary folders: {ex.Message}");
                }
            }

            return new ComparisonReport
            {
                Result = result,
                DetailedDifferences = detailedDifferences,
                DifferentFiles = differentFiles,
            };
        }

        private static List<string> GetFileNamesInDirectory(string tempFolder2)
        {
            return Directory
                .GetFiles(tempFolder2, "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(tempFolder2.Length).TrimStart(Path.DirectorySeparatorChar))
                .ToList();
        }

        private static ComparisonResult CompareCommonFiles(List<string> files1, List<string> files2, string tempFolder1, string tempFolder2, ComparisonResult result, List<string> detailedDifferences, List<string> differentFiles)
        {
            var commonFiles = files1.Intersect(files2).ToList();
            foreach (var file in commonFiles)
            {
                string fullPath1 = Path.Combine(tempFolder1, file);
                string fullPath2 = Path.Combine(tempFolder2, file);

                // Compare file content
                if (!AreFilesEqual(fullPath1, fullPath2))
                {
                    result |= ComparisonResult.FileContentsDiffer;
                    detailedDifferences.Add($"File content differs: {file}");
                    differentFiles.Add(file);
                }

                // Compare file metadata
                FileInfo fileInfo1 = new FileInfo(fullPath1);
                FileInfo fileInfo2 = new FileInfo(fullPath2);

                if (fileInfo1.Length != fileInfo2.Length)
                {
                    result |= ComparisonResult.MetadataDiffers;
                    detailedDifferences.Add($"File size differs for {file}: {fileInfo1.Length} vs {fileInfo2.Length}");
                }

                // Compare file attributes
                if (fileInfo1.Attributes != fileInfo2.Attributes)
                {
                    result |= ComparisonResult.MetadataDiffers;
                    detailedDifferences.Add($"File attributes differ for {file}: {fileInfo1.Attributes} vs {fileInfo2.Attributes}");
                }
            }

            return result;
        }

        private static ComparisonResult CompareFileStructures(List<string> files1, List<string> files2, ComparisonResult result, List<string> detailedDifferences, List<string> differentFiles)
        {
            var onlyInFirst = files1.Except(files2).ToList();
            var onlyInSecond = files2.Except(files1).ToList();

            if (onlyInFirst.Count <= 0 && onlyInSecond.Count <= 0)
            {
                return result;
            }

            result |= ComparisonResult.FileNamesDiffer;

            foreach (var file in onlyInFirst)
            {
                detailedDifferences.Add($"File exists only in first package: {file}");
                differentFiles.Add(file);
            }

            foreach (var file in onlyInSecond)
            {
                detailedDifferences.Add($"File exists only in second package: {file}");
                differentFiles.Add(file);
            }

            return result;
        }

        private static ComparisonResult ComparePackageJson(string tempFolder1, string tempFolder2, ComparisonResult result, List<string> detailedDifferences)
        {
            string packageJson1Path = Path.Combine(tempFolder1, kPackageFolder, kPackageJsonFile);
            string packageJson2Path = Path.Combine(tempFolder2, kPackageFolder, kPackageJsonFile);

            if (!File.Exists(packageJson1Path) && !File.Exists(packageJson2Path))
            {
                result |= ComparisonResult.PackageJsonDiffers;
                detailedDifferences.Add("Package.json doesn't exist in both packages");
                return result;
            }

            if (!File.Exists(packageJson1Path) || !File.Exists(packageJson2Path))
            {
                result |= ComparisonResult.PackageJsonDiffers;
                detailedDifferences.Add("Package.json exists in only one of the packages");
                return result;
            }

            var packageTextOne = File.ReadAllText(packageJson1Path);
            var packageTextTwo = File.ReadAllText(packageJson2Path);

            var packageOne = JsonConvert.DeserializeObject<UnityPackage>(packageTextOne);
            var packageTwo = JsonConvert.DeserializeObject<UnityPackage>(packageTextTwo);

            if (packageOne.Equals(packageTwo))
            {
                return result;
            }

            result |= ComparisonResult.PackageJsonDiffers;

            // TODO: Check if needs this much details
            if (packageOne.Name != packageTwo.Name)
            {
                detailedDifferences.Add($"Package name differs: '{packageOne.Name}' vs '{packageTwo.Name}'");
            }

            if (packageOne.Version != packageTwo.Version)
            {
                detailedDifferences.Add($"Package version differs: '{packageOne.Version}' vs '{packageTwo.Version}'");
            }

            if (packageOne.Dependencies.Count != packageTwo.Dependencies.Count)
            {
                detailedDifferences.Add("Dependencies count differs");
            }
            else
            {
                foreach (var dep in packageOne.Dependencies)
                {
                    if (packageTwo.Dependencies.TryGetValue(dep.Key, out string version) && version == dep.Value)
                    {
                        continue;
                    }

                    detailedDifferences.Add($"Dependency '{dep.Key}' differs: '{dep.Value}' vs '{(version ?? "not found")}'");
                }
            }

            return result;
        }

        private static void ExtractGtzPackage(byte[] packageData, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            using var memoryStream = new MemoryStream(packageData);
            using var gzipStream = new GZipInputStream(memoryStream);
            using var tarInput = new TarInputStream(gzipStream, Encoding.UTF8);
            while (tarInput.GetNextEntry() is { } entry)
            {
                string outPath = Path.Combine(destinationPath, entry.Name.Replace('/', Path.DirectorySeparatorChar));

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(outPath);
                    continue;
                }

                // Ensure directory exists
                var directoryName = Path.GetDirectoryName(outPath);
                if (string.IsNullOrEmpty(directoryName))
                {
                    continue;
                }

                Directory.CreateDirectory(directoryName);

                // Extract file
                using FileStream outStream = File.Create(outPath);
                tarInput.CopyEntryContents(outStream);
            }
        }

        private static bool AreFilesEqual(string firstFile, string secondFile)
        {
            const int bytesToRead = 8192;

            // Check if the files are the same object
            if (string.Equals(firstFile, secondFile, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Check if files have different lengths
            FileInfo firstFileInfo = new FileInfo(firstFile);
            FileInfo secondFileInfo = new FileInfo(secondFile);

            if (firstFileInfo.Length != secondFileInfo.Length)
            {
                return false;
            }

            // Files with same length - compare content
            using FileStream fs1 = File.OpenRead(firstFile);
            using FileStream fs2 = File.OpenRead(secondFile);
            byte[] one = new byte[bytesToRead];
            byte[] two = new byte[bytesToRead];

            int bytesRead1;
            int bytesRead2;

            do
            {
                bytesRead1 = fs1.Read(one, 0, bytesToRead);
                bytesRead2 = fs2.Read(two, 0, bytesToRead);

                if (bytesRead1 != bytesRead2)
                    return false;

                for (int i = 0; i < bytesRead1; i++)
                {
                    if (one[i] != two[i])
                        return false;
                }
            } while (bytesRead1 > 0);

            return true;
        }
    }
}
