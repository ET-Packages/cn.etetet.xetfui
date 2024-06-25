using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;

namespace FUIEditor
{
    public static class FUIBinderSpawner
    {
        public static void SpawnFUIBinder()
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;

            List<PackageInfo> ExportedPackageInfos = new List<PackageInfo>();
            foreach (var kv in FUICodeSpawner.ExportedComponentInfos)
            {
                string packageId = kv.Key;
                Dictionary<string, ComponentInfo> componentInfos = kv.Value;

                PackageInfo packageInfo = FUICodeSpawner.PackageInfos[packageId];
                ExportedPackageInfos.Add(packageInfo);
                
                FUIBinderSpawner.SpawnCodeForPanelBinder(packageInfo, componentInfos);
            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
    
            sb.AppendLine("");
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}public static class FUIBinder");
            sb.AppendLine($"{GetTabs(1)}{{");
            sb.AppendLine($"{GetTabs(2)}public static void BindAll()");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}UIObjectFactory.Clear();");
            sb.AppendLine($"{GetTabs(3)}");

            foreach (PackageInfo packageInfo in ExportedPackageInfos)
            {
                sb.AppendLine($"{GetTabs(3)}{packageInfo.Name}Binder.BindAll();");
            }
    
            sb.AppendLine($"{GetTabs(3)}// <last line>");
            
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
    
            string filePath = $"{FUICodeSpawner.FUIAutoGenDir}/FUIBinder.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
        
        public static void SpawnFUIBinderFirstTime(PackageInfo packageInfo, Dictionary<string, ComponentInfo> componentInfos)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;

       
            FUIBinderSpawner.SpawnCodeForPanelBinder(packageInfo, componentInfos);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
            
            sb.AppendLine("");
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}public static class FUIBinder");
            sb.AppendLine($"{GetTabs(1)}{{");
            sb.AppendLine($"{GetTabs(2)}public static void BindAll()");
            sb.AppendLine($"{GetTabs(2)}{{");
            sb.AppendLine($"{GetTabs(3)}UIObjectFactory.Clear();");
            sb.AppendLine();

            sb.AppendLine($"{GetTabs(3)}{packageInfo.Name}Binder.BindAll();");
    
            sb.AppendLine($"{GetTabs(3)}// <last line>");
            
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
    
            string filePath = $"{FUICodeSpawner.FUIAutoGenDir}/FUIBinder.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        public static void SpawnFUIBinder(string packageId)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;

            // 生成对应包的 Binder 文件
            PackageInfo packageInfo = FUICodeSpawner.PackageInfos[packageId];
            if (!FUICodeSpawner.ExportedComponentInfos.TryGetDic(packageId, out var componentInfos))
            {
                // 如果包里没有可导出组件，会到这里。
                Log.Info($"packageId: {packageId} 没有可导出组件");
                return;
            }
            
            FUIBinderSpawner.SpawnCodeForPanelBinder(packageInfo, componentInfos);

            string filePath = $"{FUICodeSpawner.FUIAutoGenDir}/FUIBinder.cs";
            string oldContent = string.Empty;

            if (File.Exists(filePath))
            {
                oldContent = File.ReadAllText(filePath);
            }

            string newline = $"{GetTabs(3)}{packageInfo.Name}Binder.BindAll();\n";
            
            if (oldContent.Contains(newline))
            {
                // 已经有了
                return;
            }
            
            int index = oldContent.IndexOf($"{GetTabs(3)}// <last line>", StringComparison.Ordinal);
            if (index == -1)
            {
                SpawnFUIBinderFirstTime(packageInfo, componentInfos);
                return;
            }
            
            string content = oldContent.Insert(index, newline);
            
            File.WriteAllText(filePath, content);
        }
        
        public static void SpawnCodeForPanelBinder(PackageInfo packageInfo, Dictionary<string, ComponentInfo> componentInfos)
        {
            Func<int, string> GetTabs = FUICodeSpawner.GetTabs;
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine($"{GetTabs(1)}[EnableClass]");
            sb.AppendFormat($"{GetTabs(1)}public class {packageInfo.Name}Binder");
            sb.AppendLine();
            sb.AppendLine($"{GetTabs(1)}{{");
            sb.AppendLine($"{GetTabs(2)}public static void BindAll()");
            sb.AppendLine($"{GetTabs(2)}{{");

            foreach (ComponentInfo componentInfo in componentInfos.Values)
            {
                sb.AppendFormat($"{GetTabs(3)}UIObjectFactory.SetPackageItemExtension({{0}}.{{1}}.URL, typeof({{0}}.{{1}}));", componentInfo.NameSpace, componentInfo.ComponentTypeName);
                sb.AppendLine();
            }
            
            sb.AppendLine($"{GetTabs(2)}}}");
            sb.AppendLine($"{GetTabs(1)}}}");
            sb.AppendLine("}");
            
            string dir = "{0}/{1}".Fmt(FUICodeSpawner.FUIAutoGenDir, packageInfo.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}Binder.cs".Fmt(dir, packageInfo.Name);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}