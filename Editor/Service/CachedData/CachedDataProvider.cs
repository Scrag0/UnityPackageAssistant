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
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityPackageAssistant
{
    public interface ICachedDataProvider
    {
        void Initialize(string folder);
        void SetData<T>(string key, T value);
        bool TryGetData<T>(string key, TimeSpan maxAge, out T value);
        void ClearCache();
    }

    public class CachedDataProvider : ICachedDataProvider
    {
        private const string kFileName = "KeyValuePairs.json";

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            // ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            NullValueHandling = NullValueHandling.Ignore,
        };

        private readonly JsonSerializerSettings _deserializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        private readonly Queue<object> _savingQueue = new();
        private readonly string _fullFilePath;
        private bool _isSaving;
        private string _mainFolder = "PackageRegistryCache";
        private CachedData _data;

        private string CachePath { get => Path.Combine(Application.dataPath, "..", "Temp", _mainFolder); }
        private string FilePath { get => Path.Combine(CachePath, kFileName); }

        public void Initialize(string folder)
        {
            _mainFolder = folder;
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }

            if (!File.Exists(FilePath))
            {
                _data = new CachedData();
                return;
            }

            var json = File.ReadAllText(FilePath);
            _data = JsonConvert.DeserializeObject<CachedData>(json, _deserializerSettings);
        }

        public void SetData<T>(string key, T value)
        {
            var kvpMapper = _data.KeyValuePairs.Select((kvp, index) => (kvp, index)).FirstOrDefault(mapper => mapper.kvp.Key == key);
            var json = JsonConvert.SerializeObject(value, _serializerSettings);
            var entry = new KeyValuePair();
            entry.Key = key;
            entry.Value = json;
            entry.Timestamp = DateTime.UtcNow.Ticks;

            if (kvpMapper.kvp == null)
            {
                _data.KeyValuePairs.Add(entry);
            }

            _data.KeyValuePairs[kvpMapper.index] = entry;
            Save(_data);
        }

        public bool TryGetData<T>(string key, TimeSpan maxAge, out T value)
        {
            var kvpMapper = _data.KeyValuePairs.Select((kvp, index) => (kvp, index)).FirstOrDefault(mapper => mapper.kvp.Key == key);

            if (kvpMapper.kvp == null)
            {
                value = default;
                return false;
            }

            var entry = kvpMapper.kvp;

            var cacheTime = new DateTime(entry.Timestamp);
            if (DateTime.UtcNow - cacheTime > maxAge)
            {
                value = default;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(entry.Value, _deserializerSettings);
            return true;
        }

        public void ClearCache()
        {
            try
            {
                if (Directory.Exists(CachePath))
                {
                    Directory.Delete(CachePath, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clear cache: {ex.Message}");
            }
        }

        private void Save(CachedData data)
        {
            var copy = data.Clone();
            _savingQueue.Enqueue(copy);
            _ = SaveQueueData();
        }

        private async UniTaskVoid SaveQueueData()
        {
            if (_isSaving)
            {
                return;
            }

            _isSaving = true;
            while (_savingQueue.Count > 0)
            {
                if (_savingQueue.TryDequeue(out object data))
                {
                    await SerializeAndSaveData(FilePath, data);
                }
            }

            _isSaving = false;
        }

        private async UniTask SerializeAndSaveData<TClass>(string pathToFile, TClass data)
        {
            string serializedData = JsonConvert.SerializeObject(data, _serializerSettings);
            await File.WriteAllTextAsync(pathToFile, serializedData);
        }
    }
}
