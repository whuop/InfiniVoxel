using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace InfiniVoxel.Components
{
    public struct ChunkIndex : IComponentData
    {
        public int X;
        public int Y;
        public int Z;

        public override bool Equals(object obj)
        {
            ChunkIndex other = (ChunkIndex)obj;
            if (this.X == other.X && 
                this.Y == other.Y && 
                this.Z == other.Z)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format("ChunkIndex[{0}:{1}:{2}]", X, Y, Z);
        }
    }
}

