using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfiniVoxel.ScriptableObjects
{
    [System.Serializable]
    public class VoxelMesh : ScriptableObject
    {
        [SerializeField, HideInInspector]
        public int Width;
        [SerializeField, HideInInspector]
        public int Height;
        [SerializeField, HideInInspector]
        public int Depth;

        [SerializeField]
        public int[] Voxels;
    }
}

