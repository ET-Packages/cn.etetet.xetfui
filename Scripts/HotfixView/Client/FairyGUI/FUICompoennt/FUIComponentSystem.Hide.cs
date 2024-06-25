
namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        /// <summary>
        /// 在指定 UIPanelType 里是否有打开的界面。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="uiPanelType"></param>
        /// <returns></returns>
        public static bool IsAnyPanelVisible(this FUIComponent self, UIPanelType uiPanelType)
        {
            return self.VisiblePanelTypeDict[uiPanelType].Count > 0;
        }
        
        public static bool IsPanelVisible(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            return self.IsPanelVisible(fuiEntity);
        }

        public static bool IsPanelVisible(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
            {
                return false;
            }
            
            return true;
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2>(this FUIComponent self) where T1: Entity where T2: Entity, IAwake, IShow, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component);
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2, P1>(this FUIComponent self, P1 p1) where T1: Entity where T2: Entity, IAwake, IShow<P1>, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1);
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2, P1, P2>(this FUIComponent self, P1 p1, P2 p2) where T1: Entity where T2: Entity, IAwake, IShow<P1, P2>, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2);
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2, P1, P2, P3>(this FUIComponent self, P1 p1, P2 p2, P3 p3) where T1: Entity where T2: Entity, IAwake, IShow<P1, P2, P3>, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3);
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2, P1, P2, P3, P4>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4) where T1: Entity where T2: Entity, IAwake, IShow<P1, P2, P3, P4>, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4);
        }
        
        public static async ETTask HideAndShowPanelStackAsync<T1, T2, P1, P2, P3, P4, P5>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T1: Entity where T2: Entity, IAwake, IShow<P1, P2, P3, P4, P5>, new()
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T1>();
            PanelId showPanelId = self.GetPanelIdByGeneric<T2>();
            FUIEntity hideFUIEntity = self.GetFirstFUIEntityByPanelId(hidePanelId);

            FUIEntity fuiEntity = await self.InnerHideAndShowPanelStackAsync<T2>(hideFUIEntity, showPanelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// 隐藏 hideFUIEntity 界面，然后显示 showPanelId 界面。并将 hidePanelId 界面压入栈中。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hideFUIEntity">要隐藏的界面</param>
        /// <param name="showPanelId">要显示的界面的 PanelId</param>
        private static async ETTask<FUIEntity> InnerHideAndShowPanelStackAsync<T>(this FUIComponent self, FUIEntity hideFUIEntity, PanelId showPanelId) where T: Entity, IAwake, new()
        {
            // 隐藏 hidePanelId
            if (!self.SetPanelHide(hideFUIEntity))
            {
                Log.Warning($"关闭 panelId: {hideFUIEntity.PanelId} 失败！");
            }
            
            // 显示 showPanelId
            FUIEntity showFUIEntity = await self.InnerShowPanelAsync<T>(showPanelId);
            if (showFUIEntity == null)
            {
                Log.Error($"界面 {showPanelId} 创建失败！");
                return null;
            }
            
            // 将 hidePanelId 界面压入栈中
            if (hideFUIEntity.PanelId != PanelId.Invalid)
            {
                self.HidePanelsStack.Push(hideFUIEntity);
                showFUIEntity.IsUsingStack = true;
            }

            return showFUIEntity;
        }
        
        public static void HidePanel(this FUIComponent self, long Id)
        {
            if (!self.IdToEntity.TryGetValue(Id, out EntityRef<FUIEntity> fuiEntityRef))
            {
                return;
            }

            FUIEntity fuiEntity = fuiEntityRef;
            self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
        }
        
        public static void HidePanel(this FUIComponent self, Entity entity)
        {
            FUIEntity fuiEntity = entity.GetParent<FUIEntity>();
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                FUIEntitySystemSingleton.Instance.Show(showFuiEntity.Component);
            }
        }

        public static void HidePanel<T>(this FUIComponent self) where T: Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);

            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                FUIEntitySystemSingleton.Instance.Show(showFuiEntity.Component);
            }        
        }
        
        public static void HidePanel<T, P1>(this FUIComponent self, P1 p1) where T: Entity, IHide
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                Entity component = showFuiEntity.Component;
                if (component is not IShow<P1>)
                {
                    Log.Error("弹出的界面类型不是 IShow<P1>");
                    return;
                }
                
                FUIEntitySystemSingleton.Instance.Show(component, p1);
            }
        }
        
        public static void HidePanel<T, P1, P2>(this FUIComponent self, P1 p1, P2 p2) where T: Entity, IHide
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                Entity component = showFuiEntity.Component;
                if (component is not IShow<P1, P2>)
                {
                    Log.Error("弹出的界面类型不是 IShow<P1, P2>");
                    return;
                }
                
                FUIEntitySystemSingleton.Instance.Show(component, p1, p2);
            }
        }
        
        public static void HidePanel<T, P1, P2, P3>(this FUIComponent self, P1 p1, P2 p2, P3 p3) where T: Entity, IHide
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                Entity component = showFuiEntity.Component;
                if (component is not IShow<P1, P2, P3>)
                {
                    Log.Error("弹出的界面类型不是 IShow<P1, P2, P3>");
                    return;
                }
                
                FUIEntitySystemSingleton.Instance.Show(component, p1, p2, p3);
            }
        }
        
        public static void HidePanel<T, P1, P2, P3, P4>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4) where T: Entity, IHide
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                Entity component = showFuiEntity.Component;
                if (component is not IShow<P1, P2, P3, P4>)
                {
                    Log.Error("弹出的界面类型不是 IShow<P1, P2, P3, P4>");
                    return;
                }
                
                FUIEntitySystemSingleton.Instance.Show(component, p1, p2, p3, p4);
            }
        }
        
        public static void HidePanel<T, P1, P2, P3, P4, P5>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T: Entity, IHide
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            FUIEntity showFuiEntity = self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
            if (showFuiEntity != null)
            {
                Entity component = showFuiEntity.Component;
                if (component is not IShow<P1, P2, P3, P4, P5>)
                {
                    Log.Error("弹出的界面类型不是 IShow<P1, P2, P3, P4, P5>");
                    return;
                }
                
                FUIEntitySystemSingleton.Instance.Show(component, p1, p2, p3, p4, p5);
            }
        }
        
        /// <summary>
        /// 隐藏指定的 UI 窗口。如果之前使用 HideAndShowPanelStackAsync() 显示，则调用 HideAndPopPanelStack()，否则调用 SetPanelHide()。
        /// </summary>
        private static FUIEntity InnerHidePanel(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
            {
                return null;
            }

            if (fuiEntity.IsUsingStack)
            {
                FUIEntity showFuiEntity = self.HideAndPopPanelStack(fuiEntity);
                return showFuiEntity;
            }
            else
            {
                self.SetPanelHide(fuiEntity);
                return null;
            }
        }
        
        private static FUIEntity HideAndPopPanelStack(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (!self.SetPanelHide(fuiEntity))
            {
                Log.Warning($"检测关闭 panelId: {fuiEntity.PanelId} 失败！");
                return null;
            }

            if (self.HidePanelsStack.Count <= 0)
            {
                return null;
            }
        
            // 弹出之前的界面
            FUIEntity preFuiEntity = self.HidePanelsStack.Pop();
            self.SetPanelVisible(preFuiEntity);

            return preFuiEntity; 
        }
        
        private static void SetPanelVisible(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed)
            {
                return;
            }
            
            fuiEntity.GComponent.visible = true;

            self.VisiblePanelTypeDict.Add(fuiEntity.PanelCoreData.panelType, fuiEntity.PanelId);
            
            Log.Info("<color=magenta>### current Navigation panel </color>{0}".Fmt(fuiEntity.PanelId));
        }
        
        private static bool SetPanelHide(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
            {
                return false;
            }
            
            fuiEntity.Visible = false;

            self.VisiblePanelTypeDict.Remove(fuiEntity.PanelCoreData.panelType, fuiEntity.PanelId);
            
            return true;
        }
        
        /// <summary>
        /// 隐藏所有显示的界面，用 VisiblePanelCache 存下现在显示的界面，供之后恢复显示用。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="includeFixed">是否隐藏 Fixed 类型的界面</param>
        public static void HideAllVisiblePanel(this FUIComponent self, bool includeFixed = false)
        {
            self.VisiblePanelCache.Clear();
            foreach (var kv in self.IdToEntity)
            {
                FUIEntity fuiEntity = kv.Value;
                if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
                {
                    continue;
                }
                
                if (fuiEntity.PanelCoreData.panelType == UIPanelType.Fixed && !includeFixed)
                {
                    continue;
                }

                self.VisiblePanelCache.Add(fuiEntity);
                fuiEntity.Visible = false;
            }
        }
        
        /// <summary>
        /// 恢复显示之前隐藏的界面
        /// </summary>
        public static void ReShowAllVisiblePanel(this FUIComponent self)
        {
            foreach (FUIEntity fuiEntity in self.VisiblePanelCache)
            {
                fuiEntity.Visible = true;
            }
            
            self.VisiblePanelCache.Clear();
        }
    }
}