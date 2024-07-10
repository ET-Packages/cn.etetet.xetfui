using System.Collections.Generic;
using System.IO;
using FairyGUI;
using FairyGUIEditor;
using FUIEditor;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public class FUICodeSpawnEditor: EditorWindow
    {
        private bool loaded = false;
        private readonly List<string> packageNameList = new();
        private string[] packageNames = {};
        private int packageIndex = 0;

        private static FUICodeSpawnConfig _fuiCodeSpawnConfig;
        private static FUICodeSpawnConfig fuiCodeSpawnConfig
        {
            get
            {
                if (_fuiCodeSpawnConfig == null)
                {
                    _fuiCodeSpawnConfig = LoadSettingData<FUICodeSpawnConfig>();
                }
                return _fuiCodeSpawnConfig;
            }
        } 
        
        [MenuItem("ET/XET/FairyGUI 代码导出", false, 2)]
        public static void ShowWindow()
        {
            GetWindow<FUICodeSpawnEditor>();
        }
        
        /// <summary>
        /// 加载相关的配置文件
        /// </summary>
        private static TSetting LoadSettingData<TSetting>() where TSetting : ScriptableObject
        {
            var settingType = typeof(TSetting);
            var guids = AssetDatabase.FindAssets($"t:{settingType.Name}");
            if (guids.Length == 0)
            {
                Debug.LogWarning($"Create new {settingType.Name}.asset");
                var setting = ScriptableObject.CreateInstance<TSetting>();
                string filePath = $"Assets/{settingType.Name}.asset";
                AssetDatabase.CreateAsset(setting, filePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return setting;
            }
            else
            {
                if (guids.Length != 1)
                {
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        Debug.LogWarning($"Found multiple file : {path}");
                    }
                    throw new System.Exception($"Found multiple {settingType.Name} files !");
                }

                string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var setting = AssetDatabase.LoadAssetAtPath<TSetting>(filePath);
                return setting;
            }
        }
        
        private void OnGUI()
        {
            LoadPackages();
            GUILayout.Label("");
            GUILayout.Label("FairyGUI 代码导出工具");
            GUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            try
            {
                if (GUILayout.Button("选择 FairyGUI 项目根目录", GUILayout.Width(150)))
                {
                    string path = EditorUtility.OpenFolderPanel("选择 FairyGUI 项目根目录", fuiCodeSpawnConfig.FGUIProjectDir, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        fuiCodeSpawnConfig.FGUIProjectDir = path;
                        EditorUtility.SetDirty(fuiCodeSpawnConfig);
                        AssetDatabase.SaveAssets();
                    }
                }

                fuiCodeSpawnConfig.FGUIProjectDir = GUILayout.TextField(fuiCodeSpawnConfig.FGUIProjectDir);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            try
            {
                if (GUILayout.Button("选择导出到哪个包里", GUILayout.Width(150)))
                {
                    string defaultPath = string.IsNullOrEmpty(fuiCodeSpawnConfig.PackageDir) ? "Packages" : fuiCodeSpawnConfig.PackageDir;
                    string path = EditorUtility.OpenFolderPanel("选择导出到哪个包里", defaultPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        fuiCodeSpawnConfig.PackageDir = path;
                        EditorUtility.SetDirty(fuiCodeSpawnConfig);
                        AssetDatabase.SaveAssets();
                    }
                }

                fuiCodeSpawnConfig.PackageDir = GUILayout.TextField(fuiCodeSpawnConfig.PackageDir);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            try
            {
                packageIndex = EditorGUILayout.Popup("选择要导出的包名", packageIndex, packageNames, GUILayout.Width(300f));
            
                if (GUILayout.Button("FUI代码生成"))
                {
                    if (string.IsNullOrEmpty(fuiCodeSpawnConfig.PackageDir))
                    {
                        ShowNotification(new GUIContent("请设置要导出到哪个包里！"));
                        return;
                    }
                    
                    if (!Directory.Exists($"{fuiCodeSpawnConfig.PackageDir}"))
                    {
                        ShowNotification(new GUIContent("请检查包的路径是否正确！"));
                        return;
                    }
                    
                    ReloadPackages();
                    
                    string fuiAutoGenDir = $"{fuiCodeSpawnConfig.PackageDir}/Scripts/ModelView/Client/FUIAutoGen";
                    string modelViewCodeDir = $"{fuiCodeSpawnConfig.PackageDir}/Scripts/ModelView/Client/FUI";
                    string hotfixViewCodeDir = $"{fuiCodeSpawnConfig.PackageDir}/Scripts/HotfixView/Client/FUI";
                    
                    FUICodeSpawner.SetPath(fuiAutoGenDir, modelViewCodeDir, hotfixViewCodeDir);
                    
                    if (packageIndex == 0)
                    {
                        FUICodeSpawner.FUICodeSpawn(fuiCodeSpawnConfig.FGUIProjectDir, packageNames);
                    }
                    else
                    {
                        FUICodeSpawner.FUICodeSpawn(fuiCodeSpawnConfig.FGUIProjectDir, packageNames[packageIndex], packageNames);
                    }
            
                    ShowNotification(new GUIContent("FUI代码生成成功！"));
                }
            
                // 导出新包后，刷新包名。
                if (GUILayout.Button("刷新"))
                {
                    ReloadPackages();
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void ReloadPackages()
        {
            if (!Application.isPlaying)
            {
                loaded = false;
                LoadPackages();
            }
            else
            {
                EditorUtility.DisplayDialog("FairyGUI", "Cannot run in play mode.", "OK");
            }
        }
        
        private void LoadPackages()
        {
            if (Application.isPlaying || loaded)
            {
                return;
            }
            loaded = true;
            
            EditorToolSet.ReloadPackages();
            
            packageNameList.Clear();
            packageNameList.Add("全部导出");
            List<UIPackage> pkgs = UIPackage.GetPackages();
            int cnt = pkgs.Count;
            for (int i = 0; i < cnt; i++)
            {
                packageNameList.Add(pkgs[i].name);
            }

            packageNames = packageNameList.ToArray();
        }
    }
}