using System;
using System.Collections.Generic;

namespace ET.Client
{
    [CodeProcess]
    public partial class FUIEntitySystemSingleton: Singleton<FUIEntitySystemSingleton>, ISingletonAwake
    {
        public bool isClicked { get; set; }

        public void Awake()
        {
            
        }

        protected override void Destroy()
        {
            this.isClicked = false;
        }
        
        public void Show(Entity component)
        {
            if (component is not IShow)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Show<P1>(Entity component, P1 p1)
        {
            if (component is not IShow<P1>)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem<P1>));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem<P1> aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component, p1);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Show<P1, P2>(Entity component, P1 p1, P2 p2)
        {
            if (component is not IShow<P1, P2>)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem<P1, P2>));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem<P1, P2> aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component, p1, p2);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Show<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
        {
            if (component is not IShow<P1, P2, P3>)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem<P1, P2, P3>));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem<P1, P2, P3> aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component, p1, p2, p3);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Show<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            if (component is not IShow<P1, P2, P3, P4>)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem<P1, P2, P3, P4>));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem<P1, P2, P3, P4> aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component, p1, p2, p3, p4);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Show<P1, P2, P3, P4, P5>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            if (component is not IShow<P1, P2, P3, P4, P5>)
            {
                return;
            }
            
            List<SystemObject> iShowSystems = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IShowSystem<P1, P2, P3, P4, P5>));
            if (iShowSystems == null)
            {
                return;
            }

            foreach (IShowSystem<P1, P2, P3, P4, P5> aShowSystem in iShowSystems)
            {
                if (aShowSystem == null)
                {
                    continue;
                }

                try
                {
                    aShowSystem.Run(component, p1, p2, p3, p4, p5);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void Hide(Entity component)
        {
            if (component is not IShow)
            {
                return;
            }
            
            List<SystemObject> iHideSystem = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IHideSystem));
            if (iHideSystem == null)
            {
                return;
            }

            foreach (IHideSystem aHideSystem in iHideSystem)
            {
                if (aHideSystem == null)
                {
                    continue;
                }

                try
                {
                    aHideSystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public void BeforeUnload(Entity component)
        {
            if (component is not IShow)
            {
                return;
            }
            
            List<SystemObject> iBeforeUnloadSystem = EntitySystemSingleton.Instance.TypeSystems.GetSystems(component.GetType(), typeof(IBeforeUnloadSystem));
            if (iBeforeUnloadSystem == null)
            {
                return;
            }

            foreach (IBeforeUnloadSystem aBeforeUnloadSystem in iBeforeUnloadSystem)
            {
                if (aBeforeUnloadSystem == null)
                {
                    continue;
                }

                try
                {
                    aBeforeUnloadSystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}