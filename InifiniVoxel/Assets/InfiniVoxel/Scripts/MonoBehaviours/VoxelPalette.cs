using InfiniVoxel.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.MonoBehaviours
{
    [CreateAssetMenu(fileName ="VoxelPalette", menuName ="InfiniVoxel/")]
    public class VoxelPalette : ScriptableObject
    {
        [SerializeField]
        private List<int> m_keys = new List<int>();
        [SerializeField]
        private List<Voxel> m_values = new List<Voxel>();
    }
    /*
    public struct VoxelPrototype
    {
        public UnityEngine.Color Color;
        public float Shininess;
        public float Metalness;
    }*/

}

