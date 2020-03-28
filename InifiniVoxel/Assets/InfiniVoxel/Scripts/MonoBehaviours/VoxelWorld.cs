using System;
using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace InfiniVoxel.MonoBehaviours
{
    
    public class VoxelWorld : MonoBehaviour
    {
        [SerializeField]
        private int m_chunkWidth = 16;
        [SerializeField]
        private int m_chunkHeight = 16;
        [SerializeField]
        private int m_chunkDepth = 16;
        [SerializeField]
        private bool m_receiveShadows = true;
        [SerializeField]
        private int m_subMesh = 0;
        [SerializeField]
        private UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        [SerializeField]
        private float m_voxelScale = 1.0f;

        public static int CHUNK_WIDTH;
        public static int CHUNK_HEIGHT;
        public static int CHUNK_DEPTH;
        [SerializeField]
        private UnityEngine.Material m_chunkMaterial;

        private Dictionary<ChunkIndex, Entity> m_createdChunks = new Dictionary<ChunkIndex, Entity>();

        private NativeArray<Voxel> m_emptyChunk;

        private World m_world;

        private void Awake()
        {
            m_emptyChunk = new NativeArray<Voxel>(m_chunkWidth * m_chunkHeight * m_chunkDepth, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            
            m_world = World.All[0];
        }

        // Start is called before the first frame update
        void Start()
        {
            CHUNK_WIDTH = m_chunkWidth;
            CHUNK_HEIGHT = m_chunkHeight;
            CHUNK_DEPTH = m_chunkDepth;

            var chunkIndex = new ChunkIndex { X = 0, Y = 0, Z = 0 };
            var chunkEntity = CreateChunk(chunkIndex);
            InitializeChunkData(chunkEntity);
            InitializeEmptyVoxelBuffer();
        }

        private void OnDestroy()
        {
            //    Destroy all persistent native arrays to avoid memory leaks.
            m_emptyChunk.Dispose();
        }

        public void CreateFromVoxelArray(Voxel[] voxels, Texture2D materialTexture, int chunkWidth, int chunkHeight, int chunkDepth)
        {
            CHUNK_WIDTH = chunkWidth;
            CHUNK_HEIGHT = chunkHeight;
            CHUNK_DEPTH = chunkDepth;

            m_chunkWidth = chunkWidth;
            m_chunkHeight = chunkHeight;
            m_chunkDepth = chunkDepth;

            for(int x = 0; x < chunkWidth; x++)
            {
                for(int y = 0; y < chunkHeight; y++)
                {
                    for(int z = 0; z < chunkDepth; z++)
                    {
                        int flatIndex = IndexUtils.ToFlatIndex(x, y, z, m_chunkWidth, m_chunkDepth);

                        PlaceVoxel(x, y, z, voxels[flatIndex]);
                    }
                }
            }
        }

        private void InitializeEmptyVoxelBuffer()
        {
            m_emptyChunk = new NativeArray<Voxel>(m_chunkWidth * m_chunkHeight * m_chunkWidth, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for(int x = 0; x <  m_chunkWidth; x++)
            {
                for (int y = 0; y < m_chunkHeight; y++)
                {
                    for (int z = 0; z < m_chunkDepth; z++)
                    {
                        int flatIndex = IndexUtils.ToFlatIndex(x, y, z, m_chunkWidth, m_chunkDepth);
                        m_emptyChunk[flatIndex] = new Voxel { Transparent = 1, Type = 0 };
                    }
                }
            }
        }

        public void InitializeChunkData(Entity entity)
        {
            var entityManager = m_world.EntityManager;
            var voxels = entityManager.GetBuffer<Voxel>(entity);
            //  Set up voxel test data
            for (int x = 0; x < m_chunkWidth; x++)
            {
                for (int y = 0; y < m_chunkHeight; y++)
                {
                    for (int z = 0; z < m_chunkDepth; z++)
                    {
                        //voxels.Add(new Voxel { Transparent = 0, Type = UnityEngine.Random.Range(0, 2)});
                        if (x > m_chunkWidth / 2 && x < m_chunkWidth * 0.75 &&
                        y > m_chunkHeight / 2 && y < m_chunkHeight * 0.75 &&
                        z > m_chunkHeight / 2 && z < m_chunkHeight * 0.75)
                        {
                            voxels.Add(new Voxel { Type = 1, Transparent = 0 });
                        }
                        else if (x == 0)
                        {
                            voxels.Add(new Voxel { Type = 2, Transparent = 0 });
                        }
                        else
                        {
                            voxels.Add(Voxel.Null);
                        }
                    }
                }
            }

            if (!entityManager.HasComponent<TriangulateChunk>(entity))
                entityManager.AddComponentData(entity, new TriangulateChunk());
        }

        public Entity CreateChunk(ChunkIndex index)
        {
            if (m_createdChunks.ContainsKey(index))
            {
                Debug.LogError("Could not create chunk with index: " + index.X + "/" + index.Y + "/" + index.Z);
                return default(Entity);
            }

            var entityManager = m_world.EntityManager;
            var entity = entityManager.CreateEntity();
            m_createdChunks.Add(index, entity);
            entityManager.SetName(entity, string.Format("Chunk_{0}_{1}_{2}", index.X, index.Y, index.Z));
            entityManager.AddBuffer<ChunkModification>(entity);
            entityManager.AddBuffer<Voxel>(entity);
            entityManager.AddBuffer<Vertex>(entity);
            entityManager.AddBuffer<Index>(entity);
            entityManager.AddBuffer<UV0>(entity);
            entityManager.AddBuffer<Buffers.Color>(entity);
            entityManager.AddComponentData(entity, index);
            entityManager.AddComponentData(entity, new ChunkScale { Value = m_voxelScale });

            var voxelBuffer = entityManager.GetBuffer<Voxel>(entity);
            voxelBuffer.AddRange(m_emptyChunk);

            //  Calculate world position for chunk, based on chunk index (pos clamped to chunk values), num voxels in each dimension and the scale of each voxel.
            float3 pos = new float3
            {
                x = index.X * m_chunkWidth * m_voxelScale,
                y = index.Y * m_chunkHeight * m_voxelScale,
                z = index.Z * m_chunkDepth * m_voxelScale
            };

            entityManager.AddComponentData(entity, new Translation { Value = pos });
            entityManager.AddComponentData(entity, new LocalToWorld { Value = float4x4.identity });
            entityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });


            //  Create the renderer
            entityManager.AddSharedComponentData(entity, new RenderMesh { castShadows = m_shadowCastingMode, layer = 0, receiveShadows = m_receiveShadows, subMesh = m_subMesh, material = m_chunkMaterial, mesh = new UnityEngine.Mesh() });
            var aabb = new AABB() { Center = pos, Extents = new float3(100, 100, 100)};
            entityManager.AddComponentData(entity, new RenderBounds() { Value = aabb});
            return entity;
        }

        public void PlaceVoxel(int x, int y, int z, Voxel voxel)
        {
            var world = m_world;
            if (world == null)
            {
                Debug.LogError("Could not PlaceVoxel, no active ECS World found!");
                return;
            }

            Vector3Int chunkIndex = new Vector3Int(
                    x / m_chunkWidth,
                    y / m_chunkHeight,
                    z / m_chunkDepth
                );

            ChunkIndex cIndex = new ChunkIndex { X = chunkIndex.x, Y = chunkIndex.y, Z = chunkIndex.z };
            if (!m_createdChunks.ContainsKey(cIndex))
            {
                //  If a chunk doesnt exist, create it.
                CreateChunk(cIndex);
            }

            Vector3Int voxelIndex = new Vector3Int(
                x - (cIndex.X * m_chunkWidth),
                y - (cIndex.Y * m_chunkHeight),
                z - (cIndex.Z * m_chunkDepth)
                );

            //  Set the supplied voxel at the voxel index of the chunk
            int flatVoxelIndex = IndexUtils.ToFlatIndex(voxelIndex.x, voxelIndex.y, voxelIndex.z, m_chunkWidth, m_chunkDepth);

            Entity chunkEntity = m_createdChunks[cIndex];
            var voxelBuffer = world.EntityManager.GetBuffer<ChunkModification>(chunkEntity);

            voxelBuffer.Add(new ChunkModification
            {
                FlatVoxelIndex = flatVoxelIndex,
                Voxel = voxel
            });
        }

        public void PlaceVoxel(float3 worldPosition, Voxel voxel)
        {
            var world = m_world;
            if (world == null)
            {
                Debug.LogError("Could not PlaceVoxel, no active ECS World found!");
                return;
            }
                
            //  Create a Chunk position ( ChunkIndex ) from the worldPosition.
            Vector3Int chunkIndex = new Vector3Int(
                    Mathf.FloorToInt(worldPosition.x / m_chunkWidth ),
                    Mathf.FloorToInt(worldPosition.y / m_chunkHeight),
                    Mathf.FloorToInt(worldPosition.z / m_chunkDepth)
                );

            ChunkIndex cIndex = new ChunkIndex { X = chunkIndex.x, Y = chunkIndex.y, Z = chunkIndex.z };
            if (!m_createdChunks.ContainsKey(cIndex))
            {
                //  If a chunk doesn't exist. Create it.
                CreateChunk(cIndex);
            }

            //  Create a Voxel position ( Chunk voxel buffer index ) from world position and chunk position.
            //  Remove chunk position from world position making it a position local to that chunk.
            float3 localPos = new float3
            {
                x = worldPosition.x - (chunkIndex.x * m_chunkWidth),
                y = worldPosition.y - (chunkIndex.y * m_chunkHeight),
                z = worldPosition.z - (chunkIndex.z * m_chunkDepth)
            };

            Vector3Int voxelIndex = new Vector3Int(
                Mathf.FloorToInt(localPos.x),
                Mathf.FloorToInt(localPos.y),
                Mathf.FloorToInt(localPos.z)
                );

            //  Set the supplied voxel at the voxel index of the chunk.
            int flatVoxelIndex = IndexUtils.ToFlatIndex(voxelIndex.x, voxelIndex.y, voxelIndex.z, m_chunkWidth, m_chunkDepth);

            Entity chunkEntity = m_createdChunks[cIndex];
            var voxelBuffer = world.EntityManager.GetBuffer<ChunkModification>(chunkEntity);

            voxelBuffer.Add(new ChunkModification
            {
                FlatVoxelIndex = flatVoxelIndex,
                Voxel = voxel
            });
        }
    }
}

