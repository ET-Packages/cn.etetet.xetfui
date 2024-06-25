
using System;
using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        /// <summary>
        /// 创建一个新界面。适用于会创建多个副本的情况。
        /// </summary>
        public static async ETTask<FUIEntity> CreatePanelAsync<T>(this FUIComponent self, PanelId panelId, long id = 0) where T: Entity, IAwake, new()
        {
            FUIEntity fuiEntity = await self.CreateFUIEntityAsync<T>(panelId, id);
            self.SetPanelVisible(fuiEntity);

            return fuiEntity;
        }
        
        private static async ETTask<FUIEntity> CreateFUIEntityAsync<T>(this FUIComponent self, PanelId panelId, long id = 0) where T: Entity, IAwake, new()
        {
            CoroutineLock coroutineLock = null;
            try
            {
                coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoadingPanels, (int)id);
                
                FUIEntity fuiEntity = null;
                if (id != 0)
                {
                    fuiEntity = self.AddChildWithId<FUIEntity>(id, true);
                }
                else
                {
                    fuiEntity = self.AddChild<FUIEntity>(true);
                }
                fuiEntity.PanelId = panelId;
                
                bool isSuccess = await self.LoadFUIEntitysAsync<T>(fuiEntity);
                if (isSuccess)
                {
                    return fuiEntity;
                }

                fuiEntity.Dispose();
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }
        
        /// <summary>
        /// 异步加载
        /// </summary>
        private static async ETTask<bool> LoadFUIEntitysAsync<T>(this FUIComponent self, FUIEntity fuiEntity) where T: Entity, IAwake, new()
        {
            if (!FUIEventComponent.Instance.TryGetPanelInfo<T>(out PanelInfo panelInfo))
            {
                return false;
            }
            
            // 创建组件
            fuiEntity.GComponent = await self.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);
            if (fuiEntity.GComponent == null)
            {
                return false;
            }
            
            // 设置根节点
            fuiEntity.SetRoot(self.GetTargetRoot(self.Root(), fuiEntity.PanelCoreData.panelType));

            Entity component = fuiEntity.AddComponent<T>();
            fuiEntity.Component = component;

            // 记录fuiEntity
            if (!self.AllPanelsDict.TryGetValue(fuiEntity.PanelId, out var list))
            {
                list = new List<long>();
                self.AllPanelsDict[fuiEntity.PanelId] = list;
            }
            list.Add(fuiEntity.Id);
            
            self.IdToEntity[fuiEntity.Id] = fuiEntity;

            return true;
        }
        
        private static async ETTask<GComponent> CreateObjectAsync(this FUIComponent self, string packageName, string componentName)
        {
            return (await self.Scene().GetComponent<FUIAssetComponent>().CreateObjectAsync(packageName, componentName)).asCom;
        }
        
        public static GComponent GetTargetRoot(this FUIComponent self, Scene root, UIPanelType type)
        {
            if (type == UIPanelType.Normal)
            {
                return self.NormalGRoot;
            }
            else if (type == UIPanelType.Fixed)
            {
                return self.FixedGRoot;
            }
            else if (type == UIPanelType.PopUp)
            {
                return self.PopUpGRoot;
            }
            else if (type == UIPanelType.Other)
            {
                return self.OtherGRoot;
            }

            Log.Error("uiroot type is error: " + type.ToString());
            return null;
        }
    }
}