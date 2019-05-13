using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.Buffers
{
    public struct Vertex : IBufferElementData
    {
        public float3 Value;
    }

    public struct Color : IBufferElementData
    {
        public UnityEngine.Color Value;
    }

    public struct Index : IBufferElementData
    {
        public int value;
    }
}

