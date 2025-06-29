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

namespace UnityPackageAssistant
{
    public sealed class InstallablePackageListView : BasePackageListView<InstallablePackageElement>
    {
        private IPublishHandler _inputHandler;

        public void SetInputHandler(IPublishHandler handler)
        {
            _inputHandler = handler;
        }

        protected override void OnBindItem(InstallablePackageElement item, int index)
        {
            base.OnBindItem(item, index);
            item.OnPublishClicked += OnPackagePublishClicked;
        }

        protected override void OnUnbindItem(InstallablePackageElement item, int index)
        {
            base.OnUnbindItem(item, index);
            item.OnPublishClicked -= OnPackagePublishClicked;
        }

        private void OnPackagePublishClicked(UnityVersionExtended package, SemanticVersion version)
        {
            _inputHandler.PublishPackage(package, version);
        }
    }
}
