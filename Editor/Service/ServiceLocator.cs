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
using UnityEngine;

namespace UnityPackageAssistant
{
    internal static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new();

        internal static void Register<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
            Debug.Log($"Registered service of type {typeof(T).Name}");
        }

        internal static T Get<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        internal static bool TryGet<T>(out T service) where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = default;
            return false;
        }

        internal static void Clear()
        {
            _services.Clear();
        }
    }

    internal interface IService
    { }
}
