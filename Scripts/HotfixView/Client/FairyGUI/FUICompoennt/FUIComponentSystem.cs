using FairyGUI;

namespace ET.Client
{
    [EntitySystemOf(typeof(FUIComponent))]
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        [EntitySystem]
        public static void Awake(this FUIComponent self)
        {
            self.GRoot = GRoot.inst;
            
            self.NormalGRoot = new GComponent();
            self.NormalGRoot.gameObjectName = "NormalGRoot";
            GRoot.inst.AddChild(self.NormalGRoot);
            
            self.PopUpGRoot = new GComponent();
            self.PopUpGRoot.gameObjectName = "PopUpGRoot";
            GRoot.inst.AddChild(self.PopUpGRoot);
            
            self.FixedGRoot = new GComponent();
            self.FixedGRoot.gameObjectName = "FixedGRoot";
            GRoot.inst.AddChild(self.FixedGRoot);
            
            self.OtherGRoot = new GComponent();
            self.OtherGRoot.gameObjectName = "OtherGRoot";
            GRoot.inst.AddChild(self.OtherGRoot);
            
            FUIBinder.BindAll();
        }
        
        [EntitySystem]
        public static void Destroy(this FUIComponent self)
        {
            self.CloseAllPanel();
        }
        
        public static void Restart(this FUIComponent self)
        {
            self.CloseAllPanel();
            
            FUIBinder.BindAll();
        }
    }
}