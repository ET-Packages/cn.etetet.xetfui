using FairyGUI;

namespace ET.Client
{
    [EntitySystemOf(typeof(FUIEntity))]
    [FriendOf(typeof(FUIEntity))]
    public static partial class FUIEntitySystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.FUIEntity self)
        {
            self.PanelCoreData = self.AddChild<PanelCoreData>();
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Client.FUIEntity self)
        {
            self.PanelCoreData?.Dispose();
            self.PanelId = PanelId.Invalid;
            if (self.GComponent != null)
            {
                self.GComponent.Dispose();
                self.GComponent = null;
            }
            
            self.IsUsingStack = false;
        }

        public static void SetPanelType(this FUIEntity self, UIPanelType panelType)
        {
            self.panelType = panelType;
            self.SetRoot(FUIRootHelper.GetTargetRoot(self.Root(), panelType));
        }

        public static UIPanelType GetPanelType(this FUIEntity self)
        {
            return self.panelType;
        }
        
        public static void SetRoot(this FUIEntity self, GComponent rootGComponent)
        {
            if (self.GComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} GComponent is null!!!");
                return;
            }
            if (rootGComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} rootGComponent is null!!!");
                return;
            }
            rootGComponent.AddChild(self.GComponent);
        }
    }
}