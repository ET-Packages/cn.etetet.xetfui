
using System;

namespace ET.Client
{
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        public static async ETTask<FUIEntity> ShowPanelAsync<T>(this FUIComponent self) where T: Entity, IAwake, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component);
            return fuiEntity;
        }
        
        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1>(this FUIComponent self, P1 p1) where T: Entity, IAwake, IShow<P1>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1);
            return fuiEntity;
        }
        
        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2>(this FUIComponent self, P1 p1, P2 p2) where T: Entity, IAwake, IShow<P1, P2>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2);
            return fuiEntity;
        }
        
        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3>(this FUIComponent self, P1 p1, P2 p2, P3 p3) where T: Entity, IAwake, IShow<P1, P2, P3>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3);
            return fuiEntity;
        }
        
        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3, P4>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4) where T: Entity, IAwake, IShow<P1, P2, P3, P4>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4);
            return fuiEntity;
        }
        
        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3, P4, P5>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T: Entity, IAwake, IShow<P1, P2, P3, P4, P5>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();

            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4, p5);
            return fuiEntity;
        }
        
        /// <summary>
        /// 显示一个界面，如果没有则创建。适用于一种 PanelId 只会有一个 Panel 的情况。
        /// </summary>
        private static async ETTask<FUIEntity> InnerShowPanelAsync<T>(this FUIComponent self, PanelId panelId) where T: Entity, IAwake, new()
        {
            if (!self.ShowingPanels.Add(panelId))
            {
                return null;
            }

            var fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            if (fuiEntity != null)
            {
                self.SetPanelVisible(fuiEntity);
                self.ShowingPanels.Remove(panelId);
                return fuiEntity;
            }

            fuiEntity = await self.CreatePanelAsync<T>(panelId);
            self.ShowingPanels.Remove(panelId);
            return fuiEntity;
        }
        
        /// <summary>
        /// 显示指定的界面，这个界面由 FUIComponentSystem.CreatePanelAsync() 创建
        /// </summary>
        public static T ShowPanelWithId<T>(this FUIComponent self, long id) where T : Entity, IShow
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel);

            return panel;
        }
        
        public static T ShowPanelWithId<T, P1>(this FUIComponent self, long id, P1 p1) where T : Entity, IShow<P1>
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel, p1);

            return panel;
        }
        
        public static T ShowPanelWithId<T, P1, P2>(this FUIComponent self, long id, P1 p1, P2 p2) where T : Entity, IShow<P1, P2>
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel, p1, p2);

            return panel;
        }
        
        public static T ShowPanelWithId<T, P1, P2, P3>(this FUIComponent self, long id, P1 p1, P2 p2, P3 p3) where T : Entity, IShow<P1, P2, P3>
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel, p1, p2, p3);

            return panel;
        }
        
        public static T ShowPanelWithId<T, P1, P2, P3, P4>(this FUIComponent self, long id, P1 p1, P2 p2, P3 p3, P4 p4) where T : Entity, IShow<P1, P2, P3, P4>
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel, p1, p2, p3, p4);

            return panel;
        }
        
        public static T ShowPanelWithId<T, P1, P2, P3, P4, P5>(this FUIComponent self, long id, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T : Entity, IShow<P1, P2, P3, P4, P5>
        {
            T panel = self.InnerShowPanel<T>(id);
            
            FUIEntitySystemSingleton.Instance.Show(panel, p1, p2, p3, p4, p5);

            return panel;
        }

        private static T InnerShowPanel<T>(this FUIComponent self, long id) where T : Entity
        {
            T panel = self.GetPanelLogic<T>(id);
            if (panel == null)
            {
                return null;
            }
            
            FUIEntity fuiEntity = panel.GetParent<FUIEntity>();
            if (fuiEntity == null)
            {
                return null;
            }
            
            self.SetPanelVisible(fuiEntity);

            return panel;
        }
    }
}