
using System;

namespace ET.Client
{
    public interface IHide
    {
    }

    public interface IHideSystem : ISystemType
    {
        void Run(Entity o);
    }

    [EntitySystem]
    public abstract class HideSystem<T> : SystemObject, IHideSystem where T : Entity, IHide
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IHideSystem);
        }

        public void Run(Entity o)
        {
            this.Hide((T)o);
        }

        protected abstract void Hide(T self);
    }
}