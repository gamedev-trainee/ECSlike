﻿using System.Collections.Generic;

namespace ECSlike
{
    public class ComponentManager
    {
        private Dictionary<System.Type, int> m_components = new Dictionary<System.Type, int>();
        private Dictionary<System.Type, System.Func<IComponentConfig, IComponent>> m_componentCreators = new Dictionary<System.Type, System.Func<IComponentConfig, IComponent>>();

        public ComponentManager(ComponentRegisterData[] componentRegisters)
        {
            int count = componentRegisters.Length;
            m_components.EnsureCapacity(count);
            for (int i = 0; i < count; i++)
            {
                m_components.Add(componentRegisters[i].componentType, i + 1);
                if (componentRegisters[i].configType != null && componentRegisters[i].componentCreator != null)
                {
                    m_componentCreators.Add(componentRegisters[i].configType, componentRegisters[i].componentCreator);
                }
            }
        }

        internal IComponent createComponent(IComponentConfig config)
        {
            System.Func<IComponentConfig, IComponent> creator;
            if (m_componentCreators.TryGetValue(config.GetType(), out creator))
            {
                return creator.Invoke(config);
            }
            return null;
        }

        internal int getComponentID(System.Type type)
        {
            int componentID;
            if (m_components.TryGetValue(type, out componentID))
            {
                return componentID;
            }
            UnityEngine.Debug.LogErrorFormat("unregister component type: {0}", type.Name);
            return 0;
        }
    }
}
