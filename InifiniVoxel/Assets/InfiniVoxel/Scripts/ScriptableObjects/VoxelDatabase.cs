using System;
using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.Buffers;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace InfiniVoxel.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VoxelDatabase", menuName = "InfiniVoxel/Voxel Database", order = 2)]
    public class VoxelDatabase : ScriptableObject
    {
        [SerializeField]
        private List<VoxelData> m_voxels;

        private NativeArray<VoxelConcurrent> m_concurrentVoxels;

        public void Initialize()
        {
            CreateConcurrentVoxels();
            Debug.Log($"Awoke Voxel Database");
        }

        public int GetIndexFromVoxel(VoxelData data)
        {
            for (int i = 0; i < m_voxels.Count; i++)
            {
                if (data == m_voxels[i])
                    return i;
            }

            return -1;
        }

        private void CreateConcurrentVoxels()
        {
            m_concurrentVoxels = new NativeArray<VoxelConcurrent>(m_voxels.Count, Allocator.Persistent);
            for (int i = 0; i < m_voxels.Count; i++)
            {
                m_concurrentVoxels[i] = m_voxels[i].ToVoxel();
            }
        }

        public List<VoxelData> GetVoxels()
        {
            return m_voxels;
        }

        public NativeArray<VoxelConcurrent> GetConcurrentVoxels()
        {
            return m_concurrentVoxels;
        }

        public void Dispose()
        {
            m_concurrentVoxels.Dispose();
        }

        public void AddVoxel(VoxelData voxelData)
        {
            m_voxels.Add(voxelData);
        }

        public void ClearVoxels()
        {
            m_voxels.Clear();
        }
    }
}

