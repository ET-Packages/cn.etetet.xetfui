using System;
using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSpawner
    {
        public static void SpawnSubPanel(string packageName, ComponentInfo componentInfo)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;

            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string componentName = componentInfo.NameWithoutExtension;
            string nameSpace = componentInfo.NameSpace;
            string filePath = "{0}/{1}.cs".Fmt(fileDir, componentName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For SubComponent {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}[ChildOf]");
            sb.AppendLine($"{GetTabs(1)}public class {componentName}: Entity, IAwake<FUI_{componentName}>");
            sb.AppendLine($"{GetTabs(1)}{{");
            
            // 子组件
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
                sb.AppendLine($"{GetTabs(2)}public {comName} {variableInfo.VariableName} {{get; set;}}");
            });
            
            sb.AppendLine($"{GetTabs(2)}public FUI_{componentName} FUI{componentName} {{ get; set; }}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
            
            File.WriteAllText(filePath, sb.ToString());
        }
        
        public static void SpawnPanel(string packageName, ComponentInfo componentInfo)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            
            string nameSpace = componentInfo.NameSpace;
            string panelName = componentInfo.NameWithoutExtension;
            
            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For Panel {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}[ComponentOf(typeof(FUIEntity))]");
            sb.AppendLine($"{GetTabs(1)}[FUIPanel(PanelId.{panelName}, \"{packageName}\", \"{panelName}\")]");
            sb.AppendLine($"{GetTabs(1)}public class {panelName}: Entity, IAwake");
            sb.AppendLine($"{GetTabs(1)}{{");
            
            // 子组件
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
                sb.AppendLine($"{GetTabs(2)}public {comName} {variableInfo.VariableName} {{get; set;}}\n");
            });
            
            sb.AppendLine($"{GetTabs(2)}private FUI_{panelName} _fui{panelName};");
            sb.AppendLine();
            sb.AppendLine($"{GetTabs(2)}public FUI_{panelName} FUI{panelName}");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}get => _fui{panelName} ??= (FUI_{panelName})this.GetParent<FUIEntity>().GComponent;");
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

    }
}