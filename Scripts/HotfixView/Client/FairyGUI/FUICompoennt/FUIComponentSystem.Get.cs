
namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        public static FUIEntity GetFUIEntity(this FUIComponent self, long id)
        {
            if (!self.IdToEntity.TryGetValue(id, out EntityRef<FUIEntity> fuiEntity))
            {
                return null;
            }

            return fuiEntity;
        }
        
        // 获取指定 Id 的界面
        public static T GetPanelLogic<T>(this FUIComponent self, long id, bool needVisible = false) where T : Entity
        {
            if (!self.IdToEntity.TryGetValue(id, out EntityRef<FUIEntity> fuiEntity))
            {
                return null;
            }
            
            return self.GetPanelLogic<T>(fuiEntity, needVisible);
        }

        /// <summary>
        /// 获取第一个类型为 T 的界面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="needVisible">是否要求界面为显示状态</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPanelLogic<T>(this FUIComponent self, bool needVisible = false) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFirstFUIEntityByPanelId(panelId);
            
            return self.GetPanelLogic<T>(fuiEntity, needVisible);
        }

        private static T GetPanelLogic<T>(this FUIComponent self, FUIEntity fuiEntity, bool needVisible = false) where T : Entity
        {
            if (fuiEntity == null)
            {
                // Log.Warning("fuiEntity is null");
                return null;
            }

            if (!fuiEntity.IsPreLoad)
            {
                Log.Warning($"{fuiEntity.PanelId} is not loaded!");
                return null;
            }

            if (needVisible && !fuiEntity.Visible)
            {
                // Log.Warning($"{fuiEntity.PanelId} is need visible state!");
                return null;
            }

            return fuiEntity.GetComponent<T>();
        }
        
        private static PanelId GetPanelIdByGeneric<T>(this FUIComponent self) where T : Entity
        {
            if (FUIEventComponent.Instance.TryGetPanelInfo<T>(out PanelInfo panelInfo))
            {
                return panelInfo.PanelId;
            }

            return PanelId.Invalid;
        }
        
        private static FUIEntity GetFirstFUIEntityByPanelId(this FUIComponent self, PanelId panelId)
        {
            if (!self.AllPanelsDict.TryGetValue(panelId, out var list))
            {
                return null;
            }

            if (list.Count == 0)
            {
                return null;
            }

            long id = list[0];
            self.IdToEntity.TryGetValue(id, out EntityRef<FUIEntity> fuiEntity);

            return fuiEntity;
        }
    }
}