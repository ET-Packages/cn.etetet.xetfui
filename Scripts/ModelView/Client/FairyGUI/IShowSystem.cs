
using System;

namespace ET.Client
{
    public interface IShow
    {
    }
    
    public interface IShow<A>
    {
    }
    
    public interface IShow<A, B>
    {
    }
    
    public interface IShow<A, B, C>
    {
    }
    
    public interface IShow<A, B, C, D>
    {
    }
    
    public interface IShow<A, B, C, D, E>
    {
    }
    
    public interface IShowSystem: ISystemType
    {
        void Run(Entity o);
    }
	
    public interface IShowSystem<A>: ISystemType
    {
        void Run(Entity o, A a);
    }
    
    public interface IShowSystem<A, B>: ISystemType
    {
        void Run(Entity o, A a, B b);
    }
    
    public interface IShowSystem<A, B, C>: ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }
    
    public interface IShowSystem<A, B, C, D>: ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }
    
    public interface IShowSystem<A, B, C, D, E>: ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d, E e);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T> : SystemObject, IShowSystem where T: Entity, IShow
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem);
        }

        void IShowSystem.Run(Entity o)
        {
            this.Show((T)o);
        }

        protected abstract void Show(T self);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T, A> : SystemObject, IShowSystem<A> where T: Entity, IShow<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem<A>);
        }

        void IShowSystem<A>.Run(Entity o, A a)
        {
            this.Show((T)o, a);
        }

        protected abstract void Show(T self, A a);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T, A, B> : SystemObject, IShowSystem<A, B> where T: Entity, IShow<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem<A, B>);
        }

        void IShowSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.Show((T)o, a, b);
        }

        protected abstract void Show(T self, A a, B b);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T, A, B, C> : SystemObject, IShowSystem<A, B, C> where T: Entity, IShow<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem<A, B, C>);
        }

        void IShowSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.Show((T)o, a, b, c);
        }

        protected abstract void Show(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T, A, B, C, D> : SystemObject, IShowSystem<A, B, C, D> where T: Entity, IShow<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem<A, B, C, D>);
        }

        void IShowSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.Show((T)o, a, b, c, d);
        }

        protected abstract void Show(T self, A a, B b, C c, D d);
    }
    
    [EntitySystem]
    public abstract class ShowSystem<T, A, B, C, D, E> : SystemObject, IShowSystem<A, B, C, D, E> where T: Entity, IShow<A, B, C, D, E>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IShowSystem<A, B, C, D, E>);
        }

        void IShowSystem<A, B, C, D, E>.Run(Entity o, A a, B b, C c, D d, E e)
        {
            this.Show((T)o, a, b, c, d, e);
        }

        protected abstract void Show(T self, A a, B b, C c, D d, E e);
    }
}