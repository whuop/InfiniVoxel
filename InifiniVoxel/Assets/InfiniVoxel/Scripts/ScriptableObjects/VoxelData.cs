using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.Buffers;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VoxelData", menuName = "InfiniVoxel/VoxelData", order = 1)]
    public class VoxelData : ScriptableObject
    {
        [SerializeField]
        private bool m_isTransparent;
        [SerializeField]
        private int m_type;
        [SerializeField]
        private Material m_material;
        
        [SerializeField]
        private float2 NorthUV;
        [SerializeField]
        private float2 SouthUV;
        [SerializeField]
        private float2 EastUV;
        [SerializeField]
        private float2 WestUV;
        [SerializeField]
        private float2 TopUV;
        [SerializeField]
        private float2 BottomUV;
        
        public VoxelConcurrent ToVoxel()
        {
            return new VoxelConcurrent(m_isTransparent, NorthUV, SouthUV, EastUV, WestUV, TopUV, BottomUV);
        }
    }

    public struct VoxelConcurrent
    {
        private int m_isTransparent;
        public int IsTransparent
        {
            get { return m_isTransparent; }
        }
        
        private float2 m_northUV;
        public float2 NorthUV
        {
            get { return m_northUV; }
        }

        private float2 m_southUV;
        public float2 SouthUV
        {
            get { return m_southUV; }
        }
        
        private float2 m_eastUV;
        public float2 EastUV
        {
            get { return m_eastUV; }
        }
        
        private float2 m_westUV;
        public float2 WestUV
        {
            get { return m_westUV; }
        }
        
        private float2 m_topUV;
        public float2 TopUV
        {
            get { return m_topUV; }
        }
        
        private float2 m_bottomUV;
        public float2 BottomUV
        {
            get { return m_bottomUV; }
        }

        public VoxelConcurrent(bool isTransparent, float2 northUV, float2 southUV, float2 eastUV, float2 westUV,
            float2 topUV, float2 bottomUV)
        {
            m_isTransparent = isTransparent ? 1 : 0;
            m_northUV = northUV;
            m_southUV = southUV;
            m_westUV = westUV;
            m_eastUV = eastUV;
            m_topUV = topUV;
            m_bottomUV = bottomUV;
        }
    }
}


