using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace InfiniVoxel.Buffers
{
    [System.Serializable]
    public struct Voxel : IBufferElementData
    {
        public int DatabaseIndex;
        public int Side;

        public static Voxel Null = new Voxel
        {
            DatabaseIndex = 0,
            Side = -999
        };

        public override bool Equals(object obj)
        {
            Voxel other = (Voxel)obj;
            return other.DatabaseIndex == this.DatabaseIndex;
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
            hashCode = hashCode * -1521134295 + DatabaseIndex.GetHashCode();
            return hashCode;
        }
    }
}


