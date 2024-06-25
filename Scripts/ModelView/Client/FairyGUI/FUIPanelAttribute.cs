using System;

namespace ET.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FUIPanelAttribute: BaseAttribute
    {
        public PanelId PanelId
        {
            get;
        }

        public PanelInfo PanelInfo
        {
            get;
        }

        public FUIPanelAttribute(PanelId panelId, string packageName, string componentName)
        {
            this.PanelId = panelId;
            this.PanelInfo = new PanelInfo() { PanelId = panelId, PackageName = packageName, ComponentName = componentName };
        }
    }
}