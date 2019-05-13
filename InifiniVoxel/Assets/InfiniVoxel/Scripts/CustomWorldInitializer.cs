using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using System.Linq;
using InfiniVoxel.Attributes;
/*
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

        // Start is called before the first frame update
        void Start()
        {
            CustomWorldBootStrap.Bootstrap(m_bootstrapType, m_worlds);
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
                bootstrap.AddRange(systems.Where((t) => t.Namespace.Contains(world.Name)));
            }
            else
            {
                return systems;
            }

            return bootstrap;
        }
    }
}

    */