using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

    public interface IRegistMonobehavior
    {
        public void RegistBehavior(IContainerBuilder builder);
    }

    public abstract class BaseFacade : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] protected IObjectResolver _container;

        public abstract void Initialize();
        public abstract void Dispose();
    }

    public static partial class RegisterExtension
    {
        public static T RegisterByHierarchy<T>(this IContainerBuilder builder, GameObject root, string hierarchyPath) where T : UnityEngine.Component
        {
            T component = null;

            if(root != null)
            {
                component = root.GetHierachyPath<T>(hierarchyPath);
            }
            else
            {
                component = Util.FindChildWithPath(hierarchyPath).GetComponent<T>();
            }

            if(component == null)
            {
                Debug.LogError($"Component is Null. Please Confirm Hierarchy {hierarchyPath}");
                return null;
            }

            if(component is BaseFacade)
            {
                builder.Register(container =>
                    {
                        container.Inject(component);
                        return component;
                    }, Lifetime.Scoped)
                    .AsImplementedInterfaces()
                    .AsSelf();
            }
            else
            {
                
                builder.Register(_ => component, Lifetime.Scoped).AsSelf();
            }


            if(component is IRegistMonobehavior)
            {
                (component as IRegistMonobehavior).RegistBehavior(builder);
            }

            return component;
        }

    }
