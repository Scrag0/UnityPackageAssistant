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
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Core.ServerRequests;
using NuGet.Versioning;
using Verdaccio.CustomModels;
using Verdaccio.Models;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UnityPackageAssistant
{
    internal interface INPMWebRequests
    {
        UniTask<string> AddUser(string registryUrl, string username, string password, string email, Action<string> onSuccess, Action<string> onError);
        UniTask<string> Login(string registryUrl, string username, string password, Action<string> onSuccess, Action<string> onError, string email = "");
        UniTask<bool> PublishPackage(string registryUrl, string authToken, UnityPackage package, Action<string> onSuccess, Action<string> onError);
        UniTask<bool> UnpublishPackage(string registryUrl, string authToken, UnityManifest manifest, SemanticVersion version, Action<string> onSuccess, Action<string> onError);
        UniTask<bool> DeletePackage(string registryUrl, string authToken, UnityManifest manifest, Action<string> onSuccess, Action<string> onError);
        UniTask<UnityManifest> GetManifest(string registryUrl, string authToken, PackageInfo packageInfo, Action<string> onSuccess, Action<string> onError);
        UniTask<byte[]> GetLatestTarball(UnityManifest manifest, string authToken, Action<string> onSuccess, Action<string> onError);
        UniTask<TarballOperationResult> GetLatestTarballData(string registryUrl, string authToken, PackageInfo packageInfo, Action<string> onSuccess, Action<string> onError);
        UniTask<TarballOperationResult> GetLatestTarballData(string authToken, UnityManifest manifest, Action<string> onSuccess, Action<string> onError);
        UniTask<TarballOperationResult> GetTarballData(string authToken, UnityVersion package, Action<string> onSuccess, Action<string> onError);
    }

    internal class NPMWebRequests : INPMWebRequests
    {
        private const string kContentType = "application/json";

        private readonly IServerRequestHandler _requestHandler = new ServerRequestHandler();

        public async UniTask<string> AddUser(string registryUrl, string username, string password, string email, Action<string> onSuccess, Action<string> onError)
        {
            var result = await Login(registryUrl, username, password, onSuccess, onError, email);
            return result;
        }

        public async UniTask<string> Login(string registryUrl, string username, string password, Action<string> onSuccess, Action<string> onError, string email = "")
        {
            var credentials = new UserCredentials
            {
                Name = username,
                Password = password,
                Email = email,
            };

            registryUrl = ClearUrl(registryUrl);

            string basicToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            string json = JsonConvert.SerializeObject(credentials);
            string url = $"{registryUrl}/-/user/org.couchdb.user:{username}";

            var headers = new[]
            {
                new WebRequestHeader
                {
                    Key = HttpRequestHeader.ContentType.ToString(),
                    Value = kContentType
                },
                new WebRequestHeader
                {
                    Key = HttpRequestHeader.Accept.ToString(),
                    Value = kContentType
                },
                new WebRequestHeader
                {
                    Key = HttpRequestHeader.Authorization.ToString(),
                    Value = $"Basic {basicToken}"
                },
            };

            var result = await _requestHandler.Put(url, json, null, headers);

            if (TryProcessResponse(result, out _))
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<AuthResponse>(result.Value);
                    onSuccess?.Invoke("Login successful");
                    return response.token;
                }
                catch (Exception ex)
                {
                    DebugLogError($"Failed to parse auth response: {ex.Message}");
                    onError?.Invoke($"Failed to parse auth response: {ex.Message}");
                }
            }
            else
            {
                DebugLogError($"Login failed with status code {result.Code}: {result.Value}");
                onError?.Invoke($"Login failed with status code {result.Code}: {result.Value}");
            }

            return string.Empty;
        }

        public async UniTask<bool> PublishPackage(string registryUrl, string authToken, UnityPackage package, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                DebugLogError("Not authenticated. Call Login() or AddUser() first.");
                onError?.Invoke("Not authenticated. Call Login() or AddUser() first.");
                return false;
            }

            registryUrl = ClearUrl(registryUrl);

            try
            {
                var publicationManifest = await CreatePackagePublication(registryUrl, package);
                var packageName = publicationManifest.Name;
                var version = publicationManifest.GetLatestDistTag();

                var headers = new[]
                {
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.ContentType.ToString(),
                        Value = kContentType,
                    },
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.Accept.ToString(),
                        Value = kContentType,
                    },
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.Authorization.ToString(),
                        Value = $"Bearer {authToken}",
                    }
                };

                string url = $"{registryUrl}/{packageName}";

                var serializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                };
                string publishJson = JsonConvert.SerializeObject(publicationManifest, serializerSettings);

                var result = await _requestHandler.Put(url, publishJson, null, headers);

                if (TryProcessResponse(result, out _))
                {
                    DebugLog($"Successfully published {packageName}@{version}");
                    onSuccess?.Invoke($"Successfully published {packageName}@{version}");
                    return true;
                }

                DebugLogError($"Failed to publish package with status code {result.Code}: {result.Value}");
                onError?.Invoke($"Failed to publish package with status code {result.Code}: {result.Value}");
                return false;
            }
            catch (Exception ex)
            {
                DebugLogError($"Error publishing package: {ex.Message}\n{ex.StackTrace}");
                onError?.Invoke($"Error publishing package: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async UniTask<bool> UnpublishPackage(string registryUrl, string authToken, UnityManifest manifest, SemanticVersion version, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                DebugLogError("Not authenticated. Call Login() or AddUser() first.");
                onError?.Invoke("Not authenticated. Call Login() or AddUser() first.");
                return false;
            }

            registryUrl = ClearUrl(registryUrl);

            try
            {
                string packageName = manifest.Name;
                string versionString = version.ToNormalizedString();

                var unpublishManifest = manifest.GetUnpublishManifest();
                unpublishManifest.Versions.Remove(versionString);
                unpublishManifest.Time.Remove(versionString);

                var latestVersion = unpublishManifest.GetLatestDistTag();
                if (latestVersion == versionString)
                {
                    var newLatestVersion = unpublishManifest
                        .Versions
                        .Select(x => x.Value.Version)
                        .OrderByDescending(x => x)
                        .FirstOrDefault();
                    if (newLatestVersion != null)
                    {
                        unpublishManifest.SetLatestDistTag(newLatestVersion.ToNormalizedString());
                    }
                }

                var headers = new[]
                {
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.ContentType.ToString(),
                        Value = kContentType,
                    },
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.Accept.ToString(),
                        Value = kContentType,
                    },
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.Authorization.ToString(),
                        Value = $"Bearer {authToken}",
                    },
                };

                // TODO: Maybe create separate method to delete tarball
                string urlDelete = $"{registryUrl}/{packageName}/-/{packageName}-{versionString}.tgz/-rev/{manifest.Rev}";

                var resultDeleteTarball = await _requestHandler.Delete(urlDelete, null, headers);

                if (!TryProcessResponse(resultDeleteTarball, out _))
                {
                    DebugLogError($"Failed to unpublish package tarball with status code {resultDeleteTarball.Code}: {resultDeleteTarball.Value}");
                    onError?.Invoke($"Failed to unpublish package tarball with status code {resultDeleteTarball.Code}: {resultDeleteTarball.Value}");
                    return false;
                }

                string url = $"{registryUrl}/{packageName}/-rev/{manifest.Rev}";

                var serializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                };
                string unpublishJson = JsonConvert.SerializeObject(unpublishManifest, serializerSettings);

                var result = await _requestHandler.Put(url, unpublishJson, null, headers);

                if (TryProcessResponse(result, out _))
                {
                    DebugLog($"Successfully published {packageName}@{versionString}");
                    onSuccess?.Invoke($"Successfully published {packageName}@{versionString}");
                    return true;
                }

                DebugLogError($"Failed to unpublish package with status code {result.Code}: {result.Value}");
                onError?.Invoke($"Failed to unpublish package with status code {result.Code}: {result.Value}");
                return false;
            }
            catch (Exception ex)
            {
                DebugLogError($"Error unpublishing package: {ex.Message}\n{ex.StackTrace}");
                onError?.Invoke($"Error unpublishing package: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async UniTask<bool> DeletePackage(string registryUrl, string authToken, UnityManifest manifest, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                DebugLogError("Not authenticated. Call Login() or AddUser() first.");
                onError?.Invoke("Not authenticated. Call Login() or AddUser() first.");
                return false;
            }

            registryUrl = ClearUrl(registryUrl);

            try
            {
                var packageName = manifest.Name;
                var revision = manifest.Rev;

                var headers = new[]
                {
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.ContentType.ToString(),
                        Value = kContentType,
                    },
                    new WebRequestHeader
                    {
                        Key = HttpRequestHeader.Authorization.ToString(),
                        Value = $"Bearer {authToken}",
                    }
                };

                string url = $"{registryUrl}/{packageName}/-rev/{revision}";

                var result = await _requestHandler.Delete(url, null, headers);

                if (TryProcessResponse(result, out _))
                {
                    DebugLog($"Successfully unpublished {packageName}");
                    onSuccess?.Invoke($"Successfully unpublished {packageName}");
                    return true;
                }

                DebugLogError($"Failed to unpublishing package with status code {result.Code}: {result.Value}");
                onError?.Invoke($"Failed to unpublishing package with status code {result.Code}: {result.Value}");
                return false;
            }
            catch (Exception ex)
            {
                DebugLogError($"Error unpublishing package: {ex.Message}\n{ex.StackTrace}");
                onError?.Invoke($"Error unpublishing package: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async UniTask<UnityManifest> GetManifest(string registryUrl, string authToken, PackageInfo packageInfo, Action<string> onSuccess, Action<string> onError)
        {
            registryUrl = ClearUrl(registryUrl);
            var packageName = packageInfo.name;
            string packageInfoUrl = $"{registryUrl}/{packageName}";

            var headers = new[]
            {
                new WebRequestHeader
                {
                    Key = HttpRequestHeader.Authorization.ToString(),
                    Value = $"Bearer {authToken}",
                },
            };

            var result = await _requestHandler.Get(packageInfoUrl, null, headers);

            if (TryProcessResponse(result, out _))
            {
                string manifestText = result.Value;
                UnityManifest info = JsonConvert.DeserializeObject<UnityManifest>(manifestText);
                DebugLog($"Successfully fetched package info for {info.Name}");
                onSuccess?.Invoke($"Successfully fetched package info for {info.Name}");
                return info;
            }

            DebugLogError($"Failed to fetching package info with status code {result.Code}: {result.Value}");
            onError?.Invoke($"Failed to fetching package info with status code {result.Code}: {result.Value}");
            return null;
        }

        public async UniTask<byte[]> GetLatestTarball(UnityManifest manifest, string authToken, Action<string> onSuccess, Action<string> onError)
        {
            if (!manifest.TryGetLatestVersion(out UnityVersion latestVersion))
            {
                DebugLogError($"Latest version not found for package {manifest.Name}");
                return null;
            }

            var result = await GetTarball(latestVersion, authToken, onSuccess, onError);
            return result;
        }

        /// <remarks>
        /// Latest version theoretically will always have a value
        /// </remarks>
        public async UniTask<TarballOperationResult> GetLatestTarballData(string registryUrl, string authToken, PackageInfo packageInfo, Action<string> onSuccess, Action<string> onError)
        {
            var result = new TarballOperationResult();
            registryUrl = ClearUrl(registryUrl);

            UniTaskCompletionSource tcs = new();
            var manifestSuccess = false;
            var errorMessage = string.Empty;
            var manifest = await GetManifest(registryUrl, authToken, packageInfo, OnGetManifestSuccess, OnGetManifestFail);
            await tcs.Task;
            if (!manifestSuccess)
            {
                onError?.Invoke(errorMessage);
                result.Success = false;
                result.Data = null;
                result.LatestVersion = null;
                return result;
            }

            var tarball = await GetLatestTarball(manifest, authToken, onSuccess, onError);

            result.Success = true;
            result.Data = tarball;
            var latestDistTag = manifest.GetLatestDistTag();
            result.LatestVersion = SemanticVersion.Parse(latestDistTag);
            return result;

            void OnGetManifestSuccess(string message)
            {
                manifestSuccess = true;
                tcs.TrySetResult();
            }

            void OnGetManifestFail(string message)
            {
                manifestSuccess = false;
                errorMessage = message;
                tcs.TrySetResult();
            }
        }

        public async UniTask<TarballOperationResult> GetLatestTarballData(string authToken, UnityManifest manifest, Action<string> onSuccess, Action<string> onError)
        {
            var result = new TarballOperationResult();

            var tarball = await GetLatestTarball(manifest, authToken, onSuccess, onError);

            result.Success = true;
            result.Data = tarball;
            var latestDistTag = manifest.GetLatestDistTag();
            result.LatestVersion = SemanticVersion.Parse(latestDistTag);
            return result;
        }

        public async UniTask<TarballOperationResult> GetTarballData(string authToken, UnityVersion package, Action<string> onSuccess, Action<string> onError)
        {
            var result = new TarballOperationResult();

            var tarball = await GetTarball(package, authToken, onSuccess, onError);

            result.Success = true;
            result.Data = tarball;
            result.LatestVersion = package.Version;
            return result;
        }

        private async UniTask<byte[]> GetTarball(UnityVersion package, string authToken, Action<string> onSuccess, Action<string> onError)
        {
            string tarballUrl = package.Dist.Tarball;

            var headers = new[]
            {
                new WebRequestHeader
                {
                    Key = HttpRequestHeader.Authorization.ToString(),
                    Value = $"Bearer {authToken}",
                },
            };

            var result = await _requestHandler.Get(tarballUrl, null, headers);

            if (TryProcessResponse(result, out _))
            {
                var tarball = result.ValueRaw;
                DebugLog($"Successfully fetched package tarball info of {package.Name}");
                onSuccess?.Invoke($"Successfully fetched package tarball of {package.Name}");
                return tarball;
            }

            DebugLogError($"Failed to fetching package tarball with status code {result.Code}: {result.Value}");
            onError?.Invoke($"Failed to fetching package tarball with status code {result.Code}: {result.Value}");
            return null;
        }

        private bool TryProcessResponse(WebResponseResult response, out string message)
        {
            switch (response.Code)
            {
                case 200:
                case 201:
                case 202:
                case 204:
                    message = "Success";
                    return true;

                default:
                    message = "Failed";
                    return false;
            }
        }

        private string ClearUrl(string url)
        {
            return url.TrimEnd('/');
        }

        private void DebugLog(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        private void DebugLogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        private async Task<UnityManifest> CreatePackagePublication(string registryUrl, UnityPackage package)
        {
            try
            {
                if (package == null)
                {
                    DebugLogError("Failed to find package info");
                    return null;
                }

                if (!package.TryGetLocalAssetPath(out string assetPath))
                {
                    return null;
                }

                // Extract key information
                string packageName = package.Name;
                string version = package.Version.ToNormalizedString();
                string description = package.Description ?? string.Empty;
                string tarballName = $"{packageName}-{version}.tgz";

                // Read tarball as byte array and calculate checksums
                byte[] tarballBytes = await Tarball.GetBytes(assetPath);
                string base64Data = Convert.ToBase64String(tarballBytes);
                string sha1 = CalculateSHA1(tarballBytes);
                string sha512 = CalculateSHA512(tarballBytes);

                // Create the registry URL for the tarball
                string tarballRegistryPath = $"{packageName}/-/{tarballName}";
                tarballRegistryPath = tarballRegistryPath.TrimStart('/');
                string tarballUri = $"{registryUrl}/{tarballRegistryPath}";

                // Create strongly-typed objects for the publication manifest
                var publishManifest = new UnityManifest
                {
                    Id = packageName,
                    Name = packageName,
                    Description = description,
                };
                publishManifest.SetLatestDistTag(version);

                // Create version info
                var versionInfo = new UnityVersion(package);
                versionInfo.Id = $"{packageName}@{version}";
                versionInfo.Dist = new Dist
                {
                    Integrity = sha512,
                    Shasum = sha1,
                    Tarball = tarballUri,
                };

                string readmeFile = GetReadmeFilename(assetPath);
                string readme = null;
                if (readmeFile != null)
                {
                    var readmeFilePath = Path.Combine(assetPath, readmeFile);
                    readme = await File.ReadAllTextAsync(readmeFilePath);
                }

                if (!string.IsNullOrWhiteSpace(readme))
                {
                    versionInfo.Readme = readme;
                    versionInfo.ReadmeFilename = readmeFile;
                    publishManifest.Readme = readme;
                }

                publishManifest.Versions.Add(version, versionInfo);

                publishManifest.Attachments = new Attachments
                {
                    {
                        tarballName, new AttachmentItem
                        {
                            ContentType = "application/octet-stream",
                            Length = tarballBytes.Length,
                            Data = base64Data,
                        }
                    },
                };

                return publishManifest;
            }
            catch (Exception ex)
            {
                DebugLogError($"Error creating package publication: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        private string GetReadmeFilename(string packageFolderPath)
        {
            // Common readme filenames
            string[] readmeNames = new[] { "README.md", "README", "README.txt", "Readme.md", "readme.md" };

            for (int i = 0, j = readmeNames.Length; i < j; i++)
            {
                var readmeName = readmeNames[i];
                string path = Path.Combine(packageFolderPath, readmeName);
                if (File.Exists(path))
                {
                    return readmeName;
                }
            }

            return null;
        }

        private string CalculateSHA1(byte[] data)
        {
            using SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        private string CalculateSHA512(byte[] data)
        {
            using SHA512 sha512 = SHA512.Create();
            byte[] hashBytes = sha512.ComputeHash(data);
            var base64String = Convert.ToBase64String(hashBytes);
            return $"sha512-{base64String}";
        }
    }
}
