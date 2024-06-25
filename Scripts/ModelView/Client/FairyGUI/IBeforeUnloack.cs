using System;

namespace ET.Client
{
    public interface IBeforeUnload
    {
    }

    public interface IBeforeUnloadSystem : ISystemType
    {
        void Run(Entity o);
    }

    [EntitySystem]
    public abstract class BeforeUnloadSystem<T> : SystemObject, IBeforeUnloadSystem where T : Entity, IBeforeUnload
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IBeforeUnloadSystem);
        }

        public void Run(Entity o)
        {
            this.Hide((T)o);
        }

        protected abstract void Hide(T self);
    }
}