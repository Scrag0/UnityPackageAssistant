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

using NuGet.Versioning;
using Verdaccio.CustomModels;

namespace UnityPackageAssistant
{
    public sealed class AvailablePackageListView : BasePackageListView<AvailablePackageElement>
    {
        private IUnpublishHandler _inputHandler;

        public void SetInputHandler(IUnpublishHandler handler)
        {
            _inputHandler = handler;
        }

        protected override void OnBindItem(AvailablePackageElement item, int index)
        {
            base.OnBindItem(item, index);
            item.OnUnpublishClicked += OnPackageUnpublishClicked;
        }

        protected override void OnUnbindItem(AvailablePackageElement item, int index)
        {
            base.OnUnbindItem(item, index);
            item.OnUnpublishClicked -= OnPackageUnpublishClicked;
        }

        private void OnPackageUnpublishClicked(UnityManifest manifest, SemanticVersion version)
        {
            _inputHandler.UnpublishPackage(manifest, version);
        }
    }
}
