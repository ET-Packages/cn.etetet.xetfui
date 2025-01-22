
namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        // 关闭界面，然后弹出 popCount 个界面并关闭。
        public static void ClosePanelAndPop(this FUIComponent self, Entity entity, int popCount)
        {
            FUIEntity fuiEntity = entity.GetParent<FUIEntity>();
            self.SetPanelHide(fuiEntity);
            self.UnLoadPanel(fuiEntity);

            EntityRef<FUIEntity> preFuiEntity = null;
            for (int i = 0; i < popCount; i++)
            {
                if(!self.HidePanelsStack.TryPop(out preFuiEntity))
                {
                    break;
                }
                
                self.SetPanelHide(preFuiEntity);
                self.UnLoadPanel(preFuiEntity);
            }
             
            if(self.HidePanelsStack.TryPop(out preFuiEntity))
            {
                self.SetPanelVisible(preFuiEntity);
            }
        }
        
        public static void ClosePanel<T>(this FUIComponent self) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId);
        }
        
        public static void ClosePanel<T, P1>(this FUIComponent self, P1 p1) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId, p1);
        }
        
        public static void ClosePanel<T, P1, P2>(this FUIComponent self, P1 p1, P2 p2) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId, p1, p2);
        }
        
        public static void ClosePanel<T, P1, P2, P3>(this FUIComponent self, P1 p1, P2 p2, P3 p3) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId, p1, p2, p3);
        }
        
        public static void ClosePanel<T, P1, P2, P3, P4>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId, p1, p2, p3, p4);
        }
        
        public static void ClosePanel<T, P1, P2, P3, P4, P5>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId, p1, p2, p3, p4, p5);
        }
        
        public static void ClosePanel(this FUIComponent self, long entityId)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(entityId);
            self.ClosePanel(fuiEntity.PanelId);
        }
        
        public static void ClosePanel(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId);
            self.UnLoadPanel(fuiEntity);
        }
        
        public static void ClosePanel<P1>(this FUIComponent self, PanelId panelId, P1 p1)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId, p1);
            self.UnLoadPanel(fuiEntity);
        }
        
        public static void ClosePanel<P1, P2>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId, p1, p2);
            self.UnLoadPanel(fuiEntity);
        }
        
        public static void ClosePanel<P1, P2, P3>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId, p1, p2, p3);
            self.UnLoadPanel(fuiEntity);
        }
        
        public static void ClosePanel<P1, P2, P3, P4>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId, p1, p2, p3, p4);
            self.UnLoadPanel(fuiEntity);

        }
        
        public static void ClosePanel<P1, P2, P3, P4, P5>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.HidePanel(fuiEntity.PanelId, p1, p2, p3, p4, p5);
            self.UnLoadPanel(fuiEntity);
        }


        /// 卸载指定的UI窗口实例
        private static void UnLoadPanel(this FUIComponent self, FUIEntity fuiEntity, bool isDispose = true)
        {
            if (fuiEntity == null)
            {
                return;
            }

            FUIEntitySystemSingleton.Instance.BeforeUnload(fuiEntity.Component);
            if (fuiEntity.IsPreLoad)
            {
                fuiEntity.GComponent.Dispose();
                fuiEntity.GComponent = null;
            }

            if (isDispose)
            {
                if (self.AllPanelsDict.TryGetValue(fuiEntity.PanelId, out var list))
                {
                    list.Remove(fuiEntity.Id);
                }

                self.IdToEntity.Remove(fuiEntity.Id);
                
                fuiEntity.Dispose();
            }
        }
        
        public static void CloseAllPanel(this FUIComponent self)
        {
            foreach (var kv in self.IdToEntity)
            {
                FUIEntity fuiEntity = kv.Value;
                if (fuiEntity == null || fuiEntity.IsDisposed)
                {
                    continue;
                }

                self.SetPanelHide(fuiEntity);
                self.UnLoadPanel(fuiEntity, false);
                fuiEntity.Dispose();
            }

            foreach (var kv in self.AllPanelsDict)
            {
                kv.Value.Clear();
            }
            self.AllPanelsDict.Clear();
            
            self.IdToEntity.Clear();
            self.VisiblePanelCache.Clear();
            self.HidePanelsStack.Clear();
        }
    }
}