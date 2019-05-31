/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using System.Linq;
using InfiniVoxel.Attributes;

namespace InfiniVoxel
{
    [Flags]
    public enum BootStrapType
    {
        Default = (1 << 0),
        InfiniVoxel = (1 << 1)
    }

    public class CustomWorldInitializer : MonoBehaviour
    {
        [SerializeField]
        [EnumFlagsAttribute]
        private BootStrapType m_bootstrapType;
        private CustomWorldBootStrap m_bootStrapper;

        private Dictionary<BootStrapType, World> m_worlds = new Dictionary<BootStrapType, World>();

        private void Awake()
        {
            //CustomWorldBootStrap.Bootstrap(m_bootstrapType, m_worlds);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }
    }

    public class CustomWorldBootStrap : ICustomBootstrap
    {

        public static void Bootstrap(BootStrapType type, Dictionary<BootStrapType, World> worlds)
        {
            var values = Enum.GetValues(typeof(BootStrapType)).Cast<BootStrapType>();
            foreach(var value in values)
            {
                if (type.HasFlag(value))
                {
                    DefaultWorldInitialization.Initialize(value.ToString(), false);
                    worlds.Add(type, World.Active);
                }
            }

        }

        public List<Type> Initialize(List<Type> systems)
        {
            List<Type> bootstrap = new List<Type>();
            World world = World.Active;

            bool isDefaultWorld = world.Name == BootStrapType.Default.ToString() ? true : false;

            if (!isDefaultWorld)
            {
                var systemsToBoostrap = systems.Where((t) => t.Name.Contains(world.Name));
                bootstrap.AddRange(systemsToBoostrap.ToArray());
            }
            else
            {
                return systems;
            }

            return bootstrap;
        }
    }
}*/