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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityPackageAssistant
{
    public class UnionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Union<,>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var args = objectType.GetGenericArguments();
            var t1 = args[0];
            var t2 = args[1];

            var jToken = JToken.Load(reader);

            try
            {
                var value1 = jToken.ToObject(t1, serializer);
                return Activator.CreateInstance(objectType, value1);
            }
            catch
            {
                // ignored
            }

            try
            {
                var value2 = jToken.ToObject(t2, serializer);
                return Activator.CreateInstance(objectType, value2);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var unionType = value!.GetType();
            var item1 = unionType.GetProperty("Item1")!.GetValue(value);
            var item2 = unionType.GetProperty("Item2")!.GetValue(value);

            if (item1 != null)
                serializer.Serialize(writer, item1);
            else if (item2 != null)
                serializer.Serialize(writer, item2);
            else
                writer.WriteNull();
        }
    }
}
