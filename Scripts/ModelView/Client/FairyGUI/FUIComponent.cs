using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class FUIComponent : Entity, IAwake, IDestroy
    {
        public GComponent GRoot{ get; set; }
        public GComponent NormalGRoot{ get; set; }
        public GComponent PopUpGRoot{ get; set; }
        public GComponent FixedGRoot{ get; set; }
        public GComponent OtherGRoot{ get; set; }
        
        public Dictionary<PanelId, List<long>> AllPanelsDict = new();
        
        /// 当前打开的各个类型的界面
        public MultiMapSet<UIPanelType, PanelId> VisiblePanelTypeDict = new();
        
        /// 隐藏所有界面时临时的存储
        public List<EntityRef<FUIEntity>> VisiblePanelCache = new();
        
        public Stack<EntityRef<FUIEntity>> HidePanelsStack = new();

        public Dictionary<long, EntityRef<FUIEntity>> IdToEntity = new();
        
        // 记录正在显示的界面，避免多次打开同一种界面
        public HashSet<PanelId> ShowingPanels = new();
    }
}