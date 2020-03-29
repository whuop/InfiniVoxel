using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace InfiniVoxel.Systems
{
    public class ChunkPoolable
    {
        public GameObject GameObject;
        public MeshFilter MeshFilter;
        public MeshRenderer MeshRenderer;
        public MeshCollider MeshCollider;
    }
    
    public class ChunkGameObjectPoolSystem : ComponentSystem
    {
        private List<ChunkPoolable> m_freeList = new List<ChunkPoolable>();
        private List<ChunkPoolable> m_takenList = new List<ChunkPoolable>();

        private GameObject m_poolParent;
        
        protected override void OnCreate()
        {
            m_poolParent = new GameObject("PoolParent");
            m_poolParent.transform.position = Vector3.zero;
            IncreasePoolSize(200);
        }

        private void IncreasePoolSize(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var poolableGo = new GameObject("poolable", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                poolableGo.transform.SetParent(m_poolParent.transform, false);
                
                var meshFilter = poolableGo.GetComponent<MeshFilter>();
                var meshRenderer = poolableGo.GetComponent<MeshRenderer>();
                var meshCollider = poolableGo.GetComponent<MeshCollider>();
                
                meshFilter.sharedMesh = new Mesh();
                poolableGo.SetActive(false);

                m_freeList.Add(new ChunkPoolable()
                {
                    GameObject = poolableGo,
                    MeshCollider = meshCollider,
                    MeshFilter = meshFilter,
                    MeshRenderer = meshRenderer
                });
            }
        }

        public ChunkPoolable GetChunkPoolable()
        {
            if (m_freeList.Count == 0)
            {
                IncreasePoolSize(10);
            }

            var poolable = m_freeList[m_freeList.Count - 1];
            m_freeList.RemoveAt(m_freeList.Count - 1);
            m_takenList.Add(poolable);
            poolable.GameObject.SetActive(true);
            return poolable;
        }

        public void ReleasePoolable(ChunkPoolable poolable)
        {
            poolable.GameObject.SetActive(false);
            m_freeList.Add(poolable);
            m_takenList.Remove(poolable);
        }

        protected override void OnUpdate()
        {
        }
    }

}

