using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;

namespace FUIEditor
{
    public static class FUIPanelIdSpawner
    {
        public static void SpawnPanelId()
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}public enum PanelId");
            sb.AppendLine($"{GetTabs(1)}{{");

            sb.AppendLine($"{GetTabs(2)}Invalid = 0,");

            foreach (ComponentInfo componentInfo in FUICodeSpawner.MainPanelComponentInfos)
            {
                sb.AppendLine($"{GetTabs(2)}{componentInfo.NameWithoutExtension},");
            }
            
            sb.AppendLine($"{GetTabs(2)}// <last line>");
            
            sb.AppendLine($"{GetTabs(1)}}}"); 
            sb.AppendLine("}");
            
            string filePath = "{0}/PanelId.cs".Fmt(FUICodeSpawner.FUIAutoGenDir);
            
            // 创建目录
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        public static void SpawnPanelId(List<ComponentInfo> componentInfos)
        {
            string filePath = "{0}/PanelId.cs".Fmt(FUICodeSpawner.FUIAutoGenDir);
            if (!File.Exists(filePath))
            {
                SpawnPanelId();
                return;
            }
            
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            
            string content = File.ReadAllText(filePath);

            foreach (ComponentInfo componentInfo in componentInfos)
            {
                if (componentInfo.PanelType != PanelType.Main)
                {
                    continue;
                }

                string newline = $"{GetTabs(2)}{componentInfo.NameWithoutExtension},\n";
                if (content.Contains(newline))
                {
                    continue;
                }
                    
                int index = content.IndexOf($"{GetTabs(2)}// <last line>", StringComparison.Ordinal);
                if (index == -1)
                {
                    SpawnPanelId();
                    return;
                }
                    
                content = content.Insert(index, newline);
            }
            
            File.WriteAllText(filePath, content);

        }
    }
}