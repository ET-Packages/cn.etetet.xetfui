using System;
using System.Collections.Generic;

namespace ET.Client
{
    public struct PanelInfo
    {
        public PanelId PanelId;
    
        public string PackageName;
    
        public string ComponentName;
    }
    
    [CodeProcess]
    public class FUIEventComponent : Singleton<FUIEventComponent>, ISingletonAwake
    {
        private readonly Dictionary<PanelId, PanelInfo> PanelIdToInfoDict = new();
        private readonly Dictionary<Type, PanelInfo> PanelTypeToInfoDict = new();

        public void Awake()
        {
            this.PanelIdToInfoDict.Clear();
            this.PanelTypeToInfoDict.Clear();
            
            foreach (Type v in CodeTypes.Instance.GetTypes(typeof(FUIPanelAttribute)))
            {
                FUIPanelAttribute attr = v.GetCustomAttributes(typeof(FUIPanelAttribute), false)[0] as FUIPanelAttribute;
                this.PanelIdToInfoDict.Add(attr.PanelId, attr.PanelInfo);
                this.PanelTypeToInfoDict.Add(v, attr.PanelInfo);
            }
        }

        protected override void Destroy()
        {
            this.PanelIdToInfoDict.Clear();
            this.PanelTypeToInfoDict.Clear();
        }

        public bool TryGetPanelInfo<T>(out PanelInfo panelInfo)
        {
            Type type = typeof(T);
            if (this.PanelTypeToInfoDict.TryGetValue(type, out panelInfo))
            {
                return true;
            }
            
            Log.Error($"panelId : {type.FullName} does not have any panelInfo");
            return false;
        }

        public bool TryGetPanelInfo(PanelId panelId, out PanelInfo panelInfo)
        {
            if (this.PanelIdToInfoDict.TryGetValue(panelId, out panelInfo))
            {
                return true;
            }
            
            Log.Error($"panelId : {panelId} does not have any panelInfo");
            return false;
        }
    }
}