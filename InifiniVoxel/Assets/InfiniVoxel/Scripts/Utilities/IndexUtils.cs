using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel
{
    public static class IndexUtils
    {
        public static int ToFlatIndex(int x, int y, int z, int indexWidth, int indexDepth)
        {
            int index = x + indexWidth * (y + indexDepth * z);
            return index;
        }

        public static int ToFlatIndex(Vector3Int inputIndex, int indexWidth, int indexDepth)
        {
            int index = inputIndex.x + indexWidth * (inputIndex.y + indexDepth * inputIndex.z);
            return index;
        }

        public static int ToFlatIndex(int x, int y, int indexWidth)
        {
            int index = x * indexWidth + y;
            return index;
        }
    }

}
