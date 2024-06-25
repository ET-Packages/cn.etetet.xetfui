using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSystemSpawner
    {
        public static void SpawnPanelSystem(string packageName, ComponentInfo componentInfo, VariableInfo variableInfo = null)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;

            string panelName = componentInfo.NameWithoutExtension;
            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            StringBuilder sb = new StringBuilder();
            string filePath = "{0}/{1}System.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                return;
            }
            
            Debug.Log("Spawn Code For PanelSystem {0}".Fmt(filePath));
            
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");

            sb.AppendLine($"{GetTabs(1)}[EntitySystemOf(typeof({panelName}))]");
            sb.AppendLine($"{GetTabs(1)}[FriendOf(typeof({panelName}))]");
            sb.AppendLine($"{GetTabs(1)}public static partial class {panelName}System");
            sb.AppendLine($"{GetTabs(1)}{{");

            if (variableInfo != null)
            {
                sb.AppendLine($"{GetTabs(2)}[EntitySystem]");
                sb.AppendLine($"{GetTabs(2)}private static void Awake(this {panelName} self, {variableInfo.TypeName} fui{panelName})");
                sb.AppendLine($"{GetTabs(2)}{{");
                sb.AppendLine($"{GetTabs(3)}self.FUI{panelName} = fui{panelName};");
            }
            else
            {
                sb.AppendLine($"{GetTabs(2)}[EntitySystem]");
                sb.AppendLine($"{GetTabs(2)}private static void Awake(this {panelName} self)");
                sb.AppendLine($"{GetTabs(2)}{{");
            }
            
            // 子组件
            List<string> subComNames = new List<string>();
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported)
                {
                    return;
                }

                if (variableInfo.ComponentInfo?.PanelType != PanelType.Common)
                {
                    return;
                }

                int index = variableInfo.TypeName.IndexOf("FUI_", StringComparison.Ordinal);
                string comName = variableInfo.TypeName.Substring(index + 4);
                subComNames.Add(variableInfo.VariableName);
                sb.AppendLine($"{GetTabs(3)}self.{variableInfo.VariableName} = self.AddChild<{comName}, {variableInfo.TypeName}>(self.FUI{panelName}.{variableInfo.VariableName}, true);");
            });
            
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine();

            sb.AppendLine($"{GetTabs(1)}}}");
            sb.Append("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}