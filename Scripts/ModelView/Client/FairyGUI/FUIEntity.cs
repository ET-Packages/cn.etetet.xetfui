using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [ChildOf]
    public class FUIEntity : Entity, IAwake, IDestroy
    {
        public bool Visible
        {
            get
            {
                return this.GComponent.visible;
            }

            set
            {
                this.GComponent.visible = value;
            }
        }
        
        public bool IsPreLoad
        {
            get
            {
                return this.GComponent != null;
            }
        }
        
        public PanelId PanelId
        {
            get
            {
                if (this.panelId == PanelId.Invalid)
                {
                    Log.Error("panel id is " + PanelId.Invalid);
                }
                return this.panelId;
            }
            set { this.panelId = value; }
        }
      
        private PanelId panelId = PanelId.Invalid;

        public GComponent GComponent { get; set; }

        public PanelCoreData PanelCoreData { get; set; }

        public SystemLanguage Language { get; set; }

        public bool IsUsingStack { get; set; }

        public EntityRef<Entity> Component { get; set; }
    }
}