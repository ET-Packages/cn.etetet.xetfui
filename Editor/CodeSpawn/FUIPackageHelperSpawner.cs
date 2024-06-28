using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FUIEditor
{
    public static class FUIPackageHelperSpawner
    {
        public static void GenerateMappingFile()
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            string filePath = $"{FUICodeSpawner.FUIAutoGenDir}/UIPackageMapping.cs";
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FairyGUI.Dynamic;");
            sb.AppendLine();
            sb.AppendLine("namespace ET.Client");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}[EnableClass]");
            sb.AppendLine($"{GetTabs(1)}public sealed class UIPackageMapping : IUIPackageHelper");
            sb.AppendLine($"{GetTabs(1)}{{");
            sb.AppendLine($"{GetTabs(2)}private readonly Dictionary<string, string> m_PackageIdToNameMap = new()");
            sb.AppendLine($"{GetTabs(2)}{{");

            foreach (PackageInfo packageInfo in FUICodeSpawner.PackageInfos.Values)
            {
                sb.AppendLine($"{GetTabs(3)}{{\"{packageInfo.Id}\", \"{packageInfo.Name}\"}},");
            }
            
            sb.AppendLine($"{GetTabs(3)}// <last line>");
            
            sb.AppendLine($"{GetTabs(2)}}};");
            sb.AppendLine();

            sb.AppendLine($"{GetTabs(2)}public string GetPackageNameById(string id)");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}return m_PackageIdToNameMap.TryGetValue(id, out var packageName) ? packageName : null;");
            sb.AppendLine($"{GetTabs(2)}}}");
            
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
            
            File.WriteAllText(filePath, sb.ToString());
        }

        public static void GenerateMappingFile(PackageInfo packageInfo)
        {
            string filePath = "Assets/Scripts/Codes/ModelView/Client/Demo/FUIAutoGen/UIPackageMapping.cs";
            if (!File.Exists(filePath))
            {
                GenerateMappingFile();
                return;
            }
            
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            string oldContent = File.ReadAllText(filePath);

            string newline = $"\"{packageInfo.Id}\", \"{packageInfo.Name}\"";
            if (oldContent.Contains(newline))
            {
                // 已经有了
                return;
            }
            
            // 如果 Package 的 PackgeId 变了，则删掉以前的记录
            if (oldContent.Contains(packageInfo.Name))
            {
                Regex regex = new Regex($"{GetTabs(3)}{{\"[^\"]*\", \"{packageInfo.Name}\"}},\n");
                oldContent = regex.Replace(oldContent, "");
            }

            int index = oldContent.IndexOf($"{GetTabs(3)}// <last line>", StringComparison.Ordinal);
            if (index == -1)
            {
                GenerateMappingFile();
                return;
            }
            
            newline = $"{GetTabs(3)}{{{newline}}},\n";

            string content = oldContent.Insert(index, newline);
            File.WriteAllText(filePath, content);
        }
    }
}