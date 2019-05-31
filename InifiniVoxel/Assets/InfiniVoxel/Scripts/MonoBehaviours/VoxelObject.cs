using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using InfiniVoxel.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace InfiniVoxel.MonoBehaviours
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class VoxelObject : MonoBehaviour
    {
        [SerializeField]
        private VoxelMesh m_mesh;
        [SerializeField]
        private Vector3Int m_chunkSize;

        private GameObjectEntity m_goEntity;

        // Start is called before the first frame update
        void Start()
        {
            m_goEntity = GetComponent<GameObjectEntity>();
        }

        private void PrepareChunks(VoxelMesh mesh)
        {
            int numChunksWide = Mathf.CeilToInt((float)mesh.Width / (float)m_chunkSize.x);
            int numChunksHigh = Mathf.CeilToInt((float)mesh.Height / (float)m_chunkSize.y);
            int numChunksDeep = Mathf.CeilToInt((float)mesh.Depth / (float)m_chunkSize.z);
            
            for(int x = 0; x < numChunksWide; x++)
            {
                for(int y = 0; y < numChunksHigh; y++)
                {
                    for(int z = 0; z < numChunksDeep; z++)
                    {
                        CreateChunk(x,y,z, m_chunkSize.x, m_chunkSize.y, m_chunkSize.z);
                    }
                }
            }
        }

        private Entity CreateChunk(int x, int y, int z, int width, int depth, int height)
        {
            var entityManager = m_goEntity.EntityManager;

            var entity = entityManager.CreateEntity();
            entityManager.SetName(entity, this.name + "_" + x + "_" + y + "_" + z);
            entityManager.AddBuffer<ChunkModification>(entity);
            entityManager.AddBuffer<Voxel>(entity);
            entityManager.AddBuffer<Vertex>(entity);
            entityManager.AddBuffer<Index>(entity);
            entityManager.AddBuffer<UV0>(entity);
            entityManager.AddBuffer<Buffers.Color>(entity);
            entityManager.AddComponentData(entity, new ChunkIndex { X = x, Y = y, Z = z });
            entityManager.AddComponentData(entity, new ChunkScale { Value = 1.0f });

            float m_voxelScale = 1.0f;
            //  Calculate world position for chunk, based on chunk index (pos clamped to chunk values), num voxels in each dimension and the scale of each voxel.
            float3 pos = new float3
            {
                x = x * width * m_voxelScale,
                y = y * height * m_voxelScale,
                z = z * depth * m_voxelScale
            };

            entityManager.AddComponentData(entity, new Translation { Value = pos });
            entityManager.AddComponentData(entity, new LocalToWorld { Value = float4x4.identity });
            entityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });

            //  Create the renderer
            //entityManager.AddSharedComponentData(entity, new RenderMesh { castShadows = UnityEngine.Rendering.ShadowCastingMode.On, layer = 0, receiveShadows = true, subMesh = 0, material = m_chunkMaterial, mesh = new UnityEngine.Mesh() });

            return entity;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


