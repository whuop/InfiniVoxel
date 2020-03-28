using InfiniVoxel.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
/*
namespace InfiniVoxel.MonoBehaviours
{
    [System.Serializable]
    public struct VoxelPrototype
    {
        [SerializeField]
        public float2[] SideUvs;
        [SerializeField]
        public bool Transparent;
        [SerializeField]
        public int Type;

        public Voxel ToVoxel()
        {
            return new Voxel { Transparent = Transparent ? 1 : 0, Type = Type }; 
        }

        public float2 GetUVs(int side)
        {
            return SideUvs[side];
        }
    }

    [CreateAssetMenu(fileName ="VoxelPalette", menuName ="InfiniVoxel/")]
    public class VoxelPalette : ScriptableObject
    {
        [SerializeField]
        private Texture2D m_texture;
        public Texture2D Texture { get { return m_texture; } }
        [SerializeField]
        private Material m_material;
        public Material Material { get { return m_material; } set { m_material = value; } }
        [SerializeField]
        private float2 m_elementSize;
        public float2 ElementSize { get { return m_elementSize; } }

        [SerializeField]
        private int m_elementCountX;
        public int ElementCountX { get { return m_elementCountX; } }
        [SerializeField]
        private int m_elementCountY;
        public int ElementCountY { get { return m_elementCountY; } }

        [SerializeField]
        private VoxelPrototype[] m_voxels;

        public void Initialize(Material material, Texture2D texture, int pixelsPerVoxel = 1)
        {
            m_texture = texture;
            m_material = material;

            int textureWidth = texture.width;
            int textureHeight = texture.height;

            float2 elementSize = new float2(
                1.0f / (float)textureWidth,
                1.0f / (float)textureHeight
                );
            m_elementSize = elementSize;

            m_elementCountX = (int)((float)textureWidth / elementSize.x);
            m_elementCountY = (int)((float)textureHeight / elementSize.y);

            m_voxels = new VoxelPrototype[m_elementCountX * m_elementCountY];
            for(int x = 0; x < m_elementCountX; x++)
            {
                for(int y = 0; y < m_elementCountY; y++)
                {
                    int index = IndexUtils.ToFlatIndex(x, y, m_elementCountX);
                    var voxel = new VoxelPrototype
                    {
                        SideUvs = new float2[6],
                        Transparent = false,
                        Type = index
                    };

                    for(int i = 0; i < 6; i++)
                    {
                        voxel.SideUvs[i] = new float2(m_elementSize.x * x, m_elementSize.y * y);
                    }
                    m_voxels[index] = voxel;
                }
            }
        }

        public float2 GetVoxelSideUVs(int type, int side)
        {
            return m_voxels[type].GetUVs(side);
        }

        public Voxel GetVoxelByType(int type)
        {
            return m_voxels[type].ToVoxel();
        }

        public VoxelPrototype GetVoxelPrototypeByType(int type)
        {
            return m_voxels[type];
        }
    }

    
}

*/