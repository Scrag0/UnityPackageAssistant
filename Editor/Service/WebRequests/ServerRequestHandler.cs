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

using System.Collections.Generic;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace Core.ServerRequests
{
    public interface IServerRequestHandler
    {
        UniTask<WebResponseResult> Get(string url, Progress progress = null, params WebRequestHeader[] headers);
        UniTask<WebResponseResult> Post(string url, string body, Progress progress = null, params WebRequestHeader[] headers);
        UniTask<WebResponseResult> Put(string url, string body, Progress progress = null, params WebRequestHeader[] headers);
        UniTask<WebResponseResult> Delete(string url, Progress progress = null, params WebRequestHeader[] headers);
    }

    public class ServerRequestHandler : IServerRequestHandler
    {
        private const string kContentType = "application/json";

        private readonly List<UnityWebRequest> _currentRequests = new();

        public async UniTask<WebResponseResult> Get(string url, Progress progress = null, params WebRequestHeader[] headers)
        {
            var request = UnityWebRequest.Get(url);
            for (int i = 0, j = headers.Length; i < j; i++)
            {
                var header = headers[i];
                request.SetRequestHeader(header.Key, header.Value);
            }

            await WebRequest(request, progress);
            WebResponseResult result = new WebResponseResult()
            {
                Code = System.Convert.ToInt32(request.responseCode),
            };

            if (request.result == UnityWebRequest.Result.Success)
            {
                result.Value = request.downloadHandler.text;
                result.ValueRaw = request.downloadHandler.data;
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Error request [{0}, {1}]", url, request.error);
            }

            request.Dispose();
            return result;
        }

        public async UniTask<WebResponseResult> Post(string url, string body, Progress progress = null, params WebRequestHeader[] headers)
        {
            WebResponseResult result = new WebResponseResult();
            var request = UnityWebRequest.Post(url, body, kContentType);

            for (int i = 0, j = headers.Length; i < j; i++)
            {
                var header = headers[i];
                request.SetRequestHeader(header.Key, header.Value);
            }

            await WebRequest(request, progress);
            result.Code = System.Convert.ToInt32(request.responseCode);
            result.Value = request.downloadHandler.text;
            result.ValueRaw = request.downloadHandler.data;

            request.Dispose();
            return result;
        }

        public async UniTask<WebResponseResult> Put(string url, string body, Progress progress = null, params WebRequestHeader[] headers)
        {
            var request = UnityWebRequest.Put(url, body);
            request.uploadHandler.contentType = kContentType;

            for (int i = 0, j = headers.Length; i < j; i++)
            {
                var header = headers[i];
                request.SetRequestHeader(header.Key, header.Value);
            }

            await WebRequest(request, progress);

            WebResponseResult result = new WebResponseResult();
            result.Code = System.Convert.ToInt32(request.responseCode);
            result.Value = request.downloadHandler.text;
            result.ValueRaw = request.downloadHandler.data;

            request.Dispose();
            return result;
        }

        public async UniTask<WebResponseResult> Delete(string url, Progress progress = null, params WebRequestHeader[] headers)
        {
            var request = UnityWebRequest.Delete(url);

            for (int i = 0, j = headers.Length; i < j; i++)
            {
                var header = headers[i];
                request.SetRequestHeader(header.Key, header.Value);
            }

            await WebRequest(request, progress);

            WebResponseResult result = new WebResponseResult();
            result.Code = System.Convert.ToInt32(request.responseCode);

            if (request.result == UnityWebRequest.Result.Success)
            {
                result.Value = request.downloadHandler?.text;
                result.ValueRaw = request.downloadHandler?.data;
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Error request [{0}, {1}]", url, request.error);
            }

            request.Dispose();
            return result;
        }

        private async UniTask WebRequest(UnityWebRequest request
                                         , Progress progress)
        {
            await UniTask.WaitUntil(() => UnityEngine.Caching.ready);

            if (progress != null)
            {
                var sentRequest = request.SendWebRequest();
                _currentRequests.Add(request);

                while (!request.isDone)
                {
                    progress(request.downloadProgress);
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }

                progress(1f);
            }
            else
            {
                try
                {
                    var handler = await request.SendWebRequest();
                }
                catch (UnityWebRequestException exception)
                {
                    UnityEngine.Debug.LogWarning($"Request exception was handled!\n{exception}");
                }

            }

            if (_currentRequests.Contains(request))
            {
                _currentRequests.Remove(request);
            }
        }
    }

    public struct WebResponseResult
    {
        public int Code;
        public string Value;
        public byte[] ValueRaw;
    }

    public struct WebRequestHeader
    {
        public string Key;
        public string Value;
    }

    public delegate void Progress(float progress);
}
