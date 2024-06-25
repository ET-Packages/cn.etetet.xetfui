using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;
using FairyGUI.Utils;

namespace FUIEditor
{
    public static class FUIComponentSpawner
    {
        private static readonly List<string> ControllerNames = new List<string>();
        private static readonly List<string> TransitionNames = new List<string>();
        private static readonly Dictionary<string, List<string>> ControllerPageNames = new Dictionary<string, List<string>>();
        public static void SpawnComponent(ComponentInfo componentInfo)
        {
            if (!componentInfo.NeedExportClass)
            {
                return;
            }

            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
                
            GatherController(componentInfo);
            GatherTransition(componentInfo);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine("using FairyGUI.Utils;");
            sb.AppendLine();
            sb.AppendLine($"namespace {componentInfo.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}[EnableClass]");
            sb.AppendLine($"{GetTabs(1)}public partial class {FUICodeSpawner.ClassNamePrefix}{componentInfo.NameWithoutExtension}: {componentInfo.ComponentClassName}");
            sb.AppendLine($"{GetTabs(1)}{{");

            foreach (var kv in ControllerPageNames)
            {
                sb.AppendLine($"{GetTabs(2)}public enum {kv.Key}Page");
                sb.AppendLine($"{GetTabs(2)}{{");
                foreach (string pageName in kv.Value)
                {
                    sb.AppendLine($"{GetTabs(3)}{pageName},");
                }
                sb.AppendLine($"{GetTabs(2)}}}");
                sb.AppendLine();
            }
            
            for (int i = 0; i < ControllerNames.Count; i++)
            {
                if (string.IsNullOrEmpty(ControllerNames[i])) continue;
                sb.AppendLine($"{GetTabs(2)}public Controller {ControllerNames[i]};");
            }
            
            for (int i = 0; i < TransitionNames.Count; i++)
            {
                sb.AppendLine($"{GetTabs(2)}public Transition {TransitionNames[i]};");
            }
            
            // 去掉 typeName 为空的变量
            List<VariableInfo> variableInfos = new List<VariableInfo>();
            for (int i = 0; i < componentInfo.VariableInfos.Count; i++)
            {
                if(!string.IsNullOrEmpty(componentInfo.VariableInfos[i].TypeName))
                {
                    variableInfos.Add(componentInfo.VariableInfos[i]);
                }
            }

            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (!variableInfos[i].IsExported)
                {
                    continue;
                }

                sb.AppendLine($"{GetTabs(2)}public {variableInfos[i].TypeName} {variableInfos[i].VariableName};");
            }

            sb.AppendLine($"{GetTabs(2)}public const string URL = \"{componentInfo.Url}\";");
            sb.AppendLine();

            sb.AppendLine($"{GetTabs(2)}public static {FUICodeSpawner.ClassNamePrefix}{componentInfo.NameWithoutExtension} CreateInstance()");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}return ({FUICodeSpawner.ClassNamePrefix}{componentInfo.NameWithoutExtension})UIPackage.CreateObject(\"{FUICodeSpawner.PackageInfos[componentInfo.PackageId].Name}\", \"{componentInfo.NameWithoutExtension}\");");
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine();
            
            sb.AppendLine($"{GetTabs(2)}public override void ConstructFromXML(XML xml)");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}base.ConstructFromXML(xml);");

            for (int i = 0; i < ControllerNames.Count; i++)
            {
                if (string.IsNullOrEmpty(ControllerNames[i])) continue;
                sb.AppendLine($"{GetTabs(3)}{ControllerNames[i]} = GetControllerAt({i});");
            }
            
            for (int i = 0; i < TransitionNames.Count; i++)
            {
                sb.AppendLine($"{GetTabs(3)}{TransitionNames[i]} = GetTransitionAt({i});");
            }
            
            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (!variableInfos[i].IsExported)
                {
                    continue;
                }
                
                sb.AppendLine($"{GetTabs(3)}{variableInfos[i].VariableName} = ({variableInfos[i].TypeName})GetChildAt({i});");
            }
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");

            string dir = "{0}/{1}".Fmt(FUICodeSpawner.FUIAutoGenDir, FUICodeSpawner.PackageInfos[componentInfo.PackageId].Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}{2}.cs".Fmt(dir, FUICodeSpawner.ClassNamePrefix, componentInfo.NameWithoutExtension);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        private static void GatherTransition(ComponentInfo componentInfo)
        {
            TransitionNames.Clear();

            foreach (XML transitionXML in componentInfo.TransitionList)
            {
                string transitionName = transitionXML.GetAttribute("name");
                if (string.IsNullOrEmpty(transitionName))
                {
                    continue;
                }

                TransitionNames.Add(transitionName);
            }
        }
        
        private static void GatherController(ComponentInfo componentInfo)
        {
            ControllerNames.Clear();
            ControllerPageNames.Clear();
            foreach (XML controllerXML in componentInfo.ControllerList)
            {
                string controllerName = controllerXML.GetAttribute("name");
                if (!CheckControllerName(controllerName, componentInfo.ComponentType))
                {
                    ControllerNames.Add("");
                    continue;
                }

                ControllerNames.Add(controllerName);

                List<string> pageNames = new List<string>();
                string[] pages = controllerXML.GetAttribute("pages").Split(',');
                for (int i = 0; i < pages.Length; i++)
                {
                    string page = pages[i];
                    if (i % 2 == 1)
                    {
                        if (!string.IsNullOrEmpty(page))
                        {
                            pageNames.Add(page);
                        }
                    }
                }

                if (pageNames.Count == pages.Length / 2)
                {
                    ControllerPageNames.Add(controllerName, pageNames);
                }
            }
        }

        private static bool CheckControllerName(string controllerName, ComponentType componentType)
        {
            if (componentType == ComponentType.Button || componentType == ComponentType.ComboBox)
            {
                return controllerName != "button";
            }

            return true;
        }
    }
}