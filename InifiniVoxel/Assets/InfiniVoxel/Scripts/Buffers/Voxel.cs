using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace InfiniVoxel.Buffers
{
    [System.Serializable]
    public struct Voxel : IBufferElementData
    {
        public int Type;
        public int Transparent;
        public int Side;

        public static Voxel Null = new Voxel
        {
            Transparent = 1,
            Type = -999,
            Side = -999
        };

        public override bool Equals(object obj)
        {
            Voxel other = (Voxel)obj;
            return other.Type == this.Type && other.Transparent == this.Transparent;
        }

        public static bool operator ==(Voxel a, Voxel b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Voxel A, Voxel B)
        {
            return !A.Equals(B);
        }

        public override int GetHashCode()
        {
            var hashCode = -1019560780;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Transparent.GetHashCode();
            return hashCode;
        }
    }
}


