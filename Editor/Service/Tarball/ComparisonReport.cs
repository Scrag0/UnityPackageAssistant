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
using System.Text;

namespace UnityPackageAssistant
{
    public class ComparisonReport
    {
        public ComparisonResult Result { get; set; }
        public List<string> DetailedDifferences { get; set; } = new();
        public List<string> DifferentFiles { get; set; } = new();

        public string GetFormattedReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Unity GTZ Package Comparison Report");
            sb.AppendLine("=================================");
            sb.AppendLine($"Overall result: {Result}");
            sb.AppendLine();

            if (Result == ComparisonResult.Identical)
            {
                sb.AppendLine("The packages are identical.");
                return sb.ToString();
            }

            sb.AppendLine("Differences found:");
            sb.AppendLine("-----------------");
            foreach (var difference in DetailedDifferences)
            {
                sb.AppendLine("- " + difference);
            }

            sb.AppendLine();
            sb.AppendLine("Files with differences:");
            sb.AppendLine("---------------------");
            if (DifferentFiles.Count > 0)
            {
                foreach (var file in DifferentFiles)
                {
                    sb.AppendLine("- " + file);
                }
            }
            else
            {
                sb.AppendLine("No file differences detected (metadata differences only).");
            }

            return sb.ToString();
        }
    }
}
