using System;
using System.Collections.Generic;
using System.IO;
using ET;
using FairyGUI.Utils;
using UnityEditor;
using UnityEngine;

namespace FUIEditor
{
    public enum ObjectType
    {
        None,
        graph,
        group,
        image,
        loader,
        loader3D,
        movieclip,
        textfield,
        textinput,
        richtext,
        list
    }
    
    public enum ComponentType
    {
        None,
        Component,
        Button,
        ComboBox, // 下拉框
        Label,
        ProgressBar,
        ScrollBar,
        Slider,
        Tree
    }
    
    public static class FUICodeSpawner
    {
        // 名字空间
        public static string NameSpace = "ET.Client";
        
        // 类名前缀
        public static string ClassNamePrefix = "FUI_";
        
        // 代码生成路径
        public static string FUIAutoGenDir;
        public static string ModelViewCodeDir;
        public static string HotfixViewCodeDir;

        // 不生成使用默认名称的成员
        public static readonly bool IgnoreDefaultVariableName = true;
        
        public static readonly Dictionary<string, PackageInfo> PackageInfos = new Dictionary<string, PackageInfo>();

        public static readonly Dictionary<string, ComponentInfo> ComponentInfos = new Dictionary<string, ComponentInfo>();
        
        // PackageName: PackageId
        public static readonly Dictionary<string, string> PackageNameToId = new Dictionary<string, string>();
        
        // PackageName: List<ComponentInfo>
        public static readonly MultiMap<string, ComponentInfo> PackageComponentInfos = new MultiMap<string, ComponentInfo>();
        
        public static readonly List<ComponentInfo> MainPanelComponentInfos = new List<ComponentInfo>();
        
        public static readonly MultiDictionary<string, string, ComponentInfo> ExportedComponentInfos = new MultiDictionary<string, string, ComponentInfo>();

        private static readonly HashSet<string> ExtralExportURLs = new HashSet<string>();

        public static void SetPath(string fuiAutoGenDir, string modelViewCodeDir, string hotfixViewCodeDir)
        {
            FUIAutoGenDir = fuiAutoGenDir;
            ModelViewCodeDir = modelViewCodeDir;
            HotfixViewCodeDir = hotfixViewCodeDir;
        }
        
        public static string GetTabs(int count)
        {
            if (count == 0)
                return string.Empty;
            var res = "";
            for (var i = 0; i < count; i++)
                res += "    ";
            return res;
        }
        
        public static void FUICodeSpawn(string fguiProjectDir, string[] packageNames)
        {
            ParseAndSpawnCode(fguiProjectDir, packageNames);

            AssetDatabase.Refresh();
        }
        
        public static void FUICodeSpawn(string fguiProjectDir, string packageName, string[] packageNames)
        {
            ParseAndSpawnCode(fguiProjectDir, packageName, packageNames);

            AssetDatabase.Refresh();
        }

        private static void ParseAndSpawnCode(string fguiProjectDir, string[] packageNames)
        {
            ParseAllPackages(fguiProjectDir, packageNames);
            AfterParseAllPackages();
            SpawnCode();
        }
        
        private static void ParseAndSpawnCode(string fguiProjectDir, string packageName, string[] packageNames)
        {
            ParseAllPackages(fguiProjectDir, packageNames);
            AfterParseAllPackages();
            SpawnCode(PackageNameToId[packageName]);
        }

        private static void ParseAllPackages(string fguiProjectDir, string[] packageNames)
        {
            PackageInfos.Clear();
            ComponentInfos.Clear();
            MainPanelComponentInfos.Clear();
            ExportedComponentInfos.Clear();
            ExtralExportURLs.Clear();
            PackageNameToId.Clear();
            PackageComponentInfos.Clear();

            string fuiAssetsDir = $"{fguiProjectDir}/assets";
            for (int index = 1; index < packageNames.Length; index++)
            {
                string packageName = packageNames[index];
                string packageDir = $"{fuiAssetsDir}/{packageName}";
                PackageInfo packageInfo = ParsePackage(packageDir);
                if (packageInfo == null)
                {
                    continue;
                }

                if (PackageInfos.TryGetValue(packageInfo.Id, out PackageInfo info))
                {
                    Debug.LogError($"packageDirOld:{info.Path}\npackDirNew:{packageDir}");
                    break;
                }

                PackageInfos.Add(packageInfo.Id, packageInfo);
            }
        }

        private static PackageInfo ParsePackage(string packageDir)
        {
            PackageInfo packageInfo = new PackageInfo();

            packageInfo.Path = packageDir;
            packageInfo.Name = Path.GetFileName(packageDir);

            string packageXmlPath = $"{packageDir}/package.xml";
            if (!File.Exists(packageXmlPath))
            {
                Log.Warning($"{packageXmlPath} 不存在！");
                return null;
            }
            
            XML xml = new XML(File.ReadAllText($"{packageDir}/package.xml"));
            packageInfo.Id = xml.GetAttribute("id");

            if (xml.elements[0].name != "resources")
            {
                throw new Exception("package.xml 格式不对！");
            }
            
            PackageNameToId.Add(packageInfo.Name, packageInfo.Id);

            foreach (XML element in xml.elements[0].elements)
            {
                if (element.name != "component")
                {
                    continue;
                }
                
                PackageComponentInfo packageComponentInfo = new PackageComponentInfo();
                packageComponentInfo.Id = element.GetAttribute("id");
                packageComponentInfo.Name = element.GetAttribute("name");
                packageComponentInfo.Path = "{0}{1}{2}".Fmt(packageDir, element.GetAttribute("path"), packageComponentInfo.Name);
                packageComponentInfo.Exported = element.GetAttribute("exported") == "true";
                
                packageInfo.PackageComponentInfos.Add(packageComponentInfo.Name, packageComponentInfo);

                ComponentInfo componentInfo = ParseComponent(packageInfo, packageComponentInfo);
                string key = "{0}/{1}".Fmt(componentInfo.PackageId, componentInfo.Id);
                ComponentInfos.Add(key, componentInfo);
                
                PackageComponentInfos.Add(packageInfo.Id, componentInfo);

                if (componentInfo.PanelType == PanelType.Main)
                {
                    MainPanelComponentInfos.Add(componentInfo);    
                }
            }

            return packageInfo;
        }

        private static ComponentInfo ParseComponent(PackageInfo packageInfo, PackageComponentInfo packageComponentInfo)
        {
            ComponentInfo componentInfo = new ComponentInfo();
            componentInfo.PackageId = packageInfo.Id;
            componentInfo.Id = packageComponentInfo.Id;
            componentInfo.Name = packageComponentInfo.Name;
            componentInfo.NameWithoutExtension = Path.GetFileNameWithoutExtension(packageComponentInfo.Name);
            componentInfo.Url = "ui://{0}{1}".Fmt(packageInfo.Id, packageComponentInfo.Id);
            componentInfo.Exported = packageComponentInfo.Exported;
            componentInfo.ComponentType = ComponentType.Component;

            XML xml = new XML(File.ReadAllText(packageComponentInfo.Path));

            if (xml.attributes.TryGetValue("extention", out string typeName))
            {
                ComponentType type = EnumHelper.FromString<ComponentType>(typeName);
                if (type == ComponentType.None)
                {
                    Debug.LogError("{0}类型没有处理！".Fmt(typeName));
                }
                else
                {
                    componentInfo.ComponentType = type;
                }
            }
            else if (xml.attributes.TryGetValue("remark", out string remark))
            {
                if (Enum.TryParse(remark, out PanelType panelType))
                {
                    componentInfo.PanelType = panelType;
                }
            }

            foreach (XML element in xml.elements)
            {
                if (element.name == "displayList")
                {
                    componentInfo.DisplayList = element.elements;
                }
                else if (element.name == "controller")
                {
                    componentInfo.ControllerList.Add(element);
                }
                else if (element.name == "transition")
                {
                    componentInfo.TransitionList.Add(element);
                }
                else if (element.name == "relation")
                { 
                    
                }
                else if (element.name == "customProperty")
                { 
                    
                }
                else
                {
                    if (element.name == "ComboBox" && componentInfo.ComponentType == ComponentType.ComboBox)
                    {
                        ExtralExportURLs.Add(element.GetAttribute("dropdown"));
                    }
                }
            }

            return componentInfo;
        }
        
        // 检查哪些组件可以导出。需要在 ParseAllPackages 后执行，因为需要有全部 package 的信息。
        private static void AfterParseAllPackages()
        {
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.CheckCanExport(ExtralExportURLs, IgnoreDefaultVariableName);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.SetVariableInfoTypeName();
                
                if (componentInfo.NeedExportClass)
                {
                    ExportedComponentInfos.Add(componentInfo.PackageId, componentInfo.Id, componentInfo);
                }
            }
        }
        private static void SpawnCode(string packageId)
        {
            var componentInfoList = PackageComponentInfos[packageId];
            foreach (ComponentInfo componentInfo in componentInfoList)
            {
                FUIComponentSpawner.SpawnComponent(componentInfo);
            }

            var curPackageInfo = PackageInfos[packageId];
            FUIPackageHelperSpawner.GenerateMappingFile(curPackageInfo);
            FUIPanelIdSpawner.SpawnPanelId(PackageComponentInfos[curPackageInfo.Id]);
            FUIBinderSpawner.SpawnFUIBinder(packageId);

            foreach (ComponentInfo componentInfo in componentInfoList)
            {
                if (componentInfo.PanelType == PanelType.Main)
                {
                    PackageInfo packageInfo = PackageInfos[componentInfo.PackageId];
                
                    SpawnSubPanelCode(componentInfo);

                    FUIPanelSpawner.SpawnPanel(packageInfo.Name, componentInfo);
                    FUIPanelSystemSpawner.SpawnPanelSystem(packageInfo.Name, componentInfo);
                }
            }
        }
        
        private static void SpawnCode()
        {
            if (Directory.Exists(FUIAutoGenDir)) 
            {
                Directory.Delete(FUIAutoGenDir, true);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                FUIComponentSpawner.SpawnComponent(componentInfo);
            }

            FUIPackageHelperSpawner.GenerateMappingFile();
            FUIPanelIdSpawner.SpawnPanelId();
            FUIBinderSpawner.SpawnFUIBinder();

            foreach (var kv in ComponentInfos)
            {
                ComponentInfo componentInfo = kv.Value;
                if (componentInfo.PanelType == PanelType.Main)
                {
                    PackageInfo packageInfo = PackageInfos[componentInfo.PackageId];
                    
                    SpawnSubPanelCode(componentInfo);
                    FUIPanelSpawner.SpawnPanel(packageInfo.Name, componentInfo);
                    FUIPanelSystemSpawner.SpawnPanelSystem(packageInfo.Name, componentInfo);
                }
            }
        }

        private static void SpawnSubPanelCode(ComponentInfo componentInfo)
        {
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported || variableInfo.ComponentInfo == null)
                {
                    return;
                }
                
                string subPackageName = PackageInfos[variableInfo.PackageId].Name;

                FUIPanelSpawner.SpawnSubPanel(subPackageName, variableInfo.ComponentInfo);
                FUIPanelSystemSpawner.SpawnPanelSystem(subPackageName, variableInfo.ComponentInfo, variableInfo);
                
                SpawnSubPanelCode(variableInfo.ComponentInfo);
            });
        }
    }
}











