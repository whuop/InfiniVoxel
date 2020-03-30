using System;
using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections.Generic;
using System.Linq;
using InfiniVoxel.ScriptableObjects;
using InfiniVoxel.Systems;
using SharpNoise;
using SharpNoise.Modules;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace InfiniVoxel.MonoBehaviours
{
    //[ExecuteInEditMode]
    public class VoxelWorld : MonoBehaviour
    {
        [SerializeField]
        private VoxelDatabase m_voxelDatabase;
        public VoxelDatabase VoxelDatabase
        {
            get { return m_voxelDatabase; }
        }
        [SerializeField]
        private int m_chunkWidth = 16;
        public int ChunkWidth
        {
            get { return m_chunkWidth; }
        }
        [SerializeField]
        private int m_chunkHeight = 16;
        public int ChunkHeight
        {
            get { return m_chunkHeight; }
        }
        [SerializeField]
        private int m_chunkDepth = 16;
        public int ChunkDepth
        {
            get { return m_chunkDepth; }
        }
        
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

            if (!Application.isPlaying)
            {
                DefaultWorldInitialization.DefaultLazyEditModeInitialize();
                m_world = World.All[0];
            }
            else
            {
                m_world = World.All[0];
            }
            
        }

        // Start is called before the first frame update
        void Start()
        {
            m_voxelDatabase.Initialize();

            m_world.GetOrCreateSystem<ChunkTriangulationSystem>().VoxelDatabase = m_voxelDatabase;
            m_world.GetOrCreateSystem<ChunkApplyMeshGameObjectSystem>().ChunkMaterial = m_chunkMaterial;
            
            CHUNK_WIDTH = m_chunkWidth;
            CHUNK_HEIGHT = m_chunkHeight;
            CHUNK_DEPTH = m_chunkDepth;

            InitializeEmptyVoxelBuffer();
        }

        private void OnDestroy()
        {
            //    Destroy all persistent native arrays to avoid memory leaks.
            m_emptyChunk.Dispose();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                ScriptBehaviourUpdateOrder.UpdatePlayerLoop(m_world);
            }
        }

        private void GenerateWorld()
        {
            Perlin perlin = new Perlin();
            Perlin dirtPerlin = new Perlin();
            
            perlin.Frequency = 0.01;

            dirtPerlin.Frequency = 0.001;
            dirtPerlin.Seed = 5;
            
            
            for (int x = 0; x < 64; x++)
            {
                for (int z = 0; z < 64; z++)
                {
                    int stone = (int)(perlin.GetValue((double)x + 0.1, 0.1, (double)z + 0.1) * 32) + 32;
                    int dirt = (int) (dirtPerlin.GetValue((double) x + 0.1, 0.1, (double) z + 0.1) * 32) + 32;

                    bool lastDirt = true;
                    for (int y = 0; y < 64; y++)
                    {
                        if (y <= stone)
                        {
                            lastDirt = false;
                            PlaceVoxel(x,y,z, new Voxel(){ DatabaseIndex = 3 });
                        }
                        else if (y <= dirt)
                        {
                            lastDirt = true;
                            PlaceVoxel(x,y,z, new Voxel(){ DatabaseIndex = 1 });
                        }
                        else
                        {
                            if (lastDirt)
                            {
                                PlaceVoxel(x,y - 1,z, new Voxel(){ DatabaseIndex = 2 });
                            }
                            PlaceVoxel(x,y,z, new Voxel(){ DatabaseIndex = 0 });
                            lastDirt = false;
                        }
                        
                    }
                }
            }
        }

        public void LoadChunk(ChunkIndex chunkIndex)
        {
            if (m_createdChunks.ContainsKey(chunkIndex))
                return;
            
            Entity chunkEntity = CreateChunk(chunkIndex, false);

            var voxels = m_world.EntityManager.GetBuffer<Voxel>(chunkEntity);

            float cX = (float) (chunkIndex.X * ChunkWidth);
            float cY = (float) (chunkIndex.Y * ChunkHeight);
            float cZ = (float) (chunkIndex.Z * ChunkDepth);
            
            Perlin perlin = new Perlin();
            Perlin dirtPerlin = new Perlin();
            
            perlin.Frequency = 0.01;

            dirtPerlin.Frequency = 0.001;
            dirtPerlin.Seed = 5;
            
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkDepth; z++)
                {
                    //    Position used for data generation
                    float3 voxelPos = new float3(
                        cX + x + 0.01f,
                        1.01f,
                        cZ + z + 0.01f
                        );
                    
                    
                    int stone = (int)(perlin.GetValue((double)voxelPos.x, 0.1, (double)voxelPos.z) * 6) + 6;
                    int dirt = (int) (dirtPerlin.GetValue((double)voxelPos.x, 0.1, (double)voxelPos.z) * 6) + 6;

                    bool lastDirt = true;
                    
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        int flatIndex = IndexUtils.ToFlatIndex(x, y, z, ChunkWidth, ChunkDepth);
                        
                        if (y <= stone)
                        {
                            lastDirt = false;
                            m_emptyChunk[flatIndex] = new Voxel() { DatabaseIndex = 3 };
                        }
                        else if (y <= dirt)
                        {
                            lastDirt = true;
                            m_emptyChunk[flatIndex] = new Voxel() { DatabaseIndex = 1 };
                        }
                        else
                        {
                            if (lastDirt)
                            {
                                m_emptyChunk[flatIndex] = new Voxel() { DatabaseIndex = 2 };
                            }
                            m_emptyChunk[flatIndex] = new Voxel() { DatabaseIndex = 0 };
                            lastDirt = false;
                        }
                    }
                }
            }
            
            voxels.AddRange(m_emptyChunk);
            if (!m_world.EntityManager.HasComponent<TriangulateChunk>(chunkEntity))
                m_world.EntityManager.AddComponentData(chunkEntity, new TriangulateChunk());
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
                        m_emptyChunk[flatIndex] = new Voxel { DatabaseIndex = 0 };
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
                        voxels[IndexUtils.ToFlatIndex(x, y, z, m_chunkWidth, m_chunkDepth)] =
                            new Voxel {DatabaseIndex = 2};
                    }
                }
            }

            if (!entityManager.HasComponent<TriangulateChunk>(entity))
                entityManager.AddComponentData(entity, new TriangulateChunk());
        }

        public Entity CreateChunk(ChunkIndex index, bool initializeWithEmptyChunk = true)
        {
            if (m_createdChunks.ContainsKey(index))
            {
                return m_createdChunks[index];
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

            if (initializeWithEmptyChunk)
            {
                var voxelBuffer = entityManager.GetBuffer<Voxel>(entity);
                voxelBuffer.AddRange(m_emptyChunk);
            }
            
            //  Calculate world position for chunk, based on chunk index (pos clamped to chunk values), num voxels in each dimension and the scale of each voxel.
            float3 pos = new float3
            {
                x = index.X * m_chunkWidth * m_voxelScale,
                y = index.Y * m_chunkHeight * m_voxelScale,
                z = index.Z * m_chunkDepth * m_voxelScale
            };

            entityManager.AddComponentData(entity, new Translation { Value = pos });
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

