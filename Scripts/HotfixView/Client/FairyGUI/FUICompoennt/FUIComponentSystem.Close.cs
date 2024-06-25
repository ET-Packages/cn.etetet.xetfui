
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
        
        public static void ClosePanel(this FUIComponent self, long entityId)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(entityId);
            self.ClosePanel(fuiEntity);
        }

        public static void ClosePanel(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            self.ClosePanel(fuiEntity);
        }
        
        public static void ClosePanel(this FUIComponent self, Entity entity)
        {
            FUIEntity fuiEntity = entity.GetParent<FUIEntity>();
            self.ClosePanel(fuiEntity);
        }
                
        /// 关闭指定的UI窗口，会Unload资源。
        public static void ClosePanel(this FUIComponent self, FUIEntity fuiEntity)
        {
            self.InnerHidePanel(fuiEntity);
            self.UnLoadPanel(fuiEntity);
            Log.Info("<color=magenta>## close panel without Pop ##</color>");
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