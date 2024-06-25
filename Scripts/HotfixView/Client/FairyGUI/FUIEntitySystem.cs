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