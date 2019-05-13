using InfiniVoxel.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace InfiniVoxel.Components
{
    public struct ChunkModification : IBufferElementData
    {
        public int FlatVoxelIndex;
        public Voxel Voxel;
    }
}


