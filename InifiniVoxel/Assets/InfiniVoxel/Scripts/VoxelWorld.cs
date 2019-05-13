using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace InfiniVoxel
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

        // Start is called before the first frame update
        void Start()
        {
            CHUNK_WIDTH = m_chunkWidth;
            CHUNK_HEIGHT = m_chunkHeight;
            CHUNK_DEPTH = m_chunkDepth;

            CreateChunk(new ChunkIndex { X = 0, Y = 0, Z = 0 });
            /*CreateChunk(new ChunkIndex { X = 1, Y = 0, Z = 0 });
            CreateChunk(new ChunkIndex { X = -1, Y = 0, Z = 0 });
            CreateChunk(new ChunkIndex { X = 0, Y = 0, Z = 1 });
            CreateChunk(new ChunkIndex { X = 0, Y = 0, Z = -1 });*/

            //PlaceVoxel(new Vector3(0, 15, 3), new Voxel { Transparent = 0, Type = 3 });
        }

        public void CreateChunk(ChunkIndex index)
        {
            if (m_createdChunks.ContainsKey(index))
            {
                Debug.LogError("Could not create chunk with index: " + index.X + "/" + index.Y + "/" + index.Z);
                return;
            }

            var entityManager = World.Active.EntityManager;
            var entity = entityManager.CreateEntity();
            m_createdChunks.Add(index, entity);
            entityManager.SetName(entity, string.Format("Chunk_{0}_{1}_{2}", index.X, index.Y, index.Z));
            entityManager.AddBuffer<Voxel>(entity);
            entityManager.AddBuffer<Vertex>(entity);
            entityManager.AddBuffer<Index>(entity);
            entityManager.AddBuffer<UV0>(entity);
            entityManager.AddBuffer<Buffers.Color>(entity);
            entityManager.AddComponentData(entity, index);
            entityManager.AddComponentData(entity, new ChunkScale { Value = m_voxelScale });

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

            var voxels = entityManager.GetBuffer<Voxel>(entity);
            //  Set up voxel test data
            for (int x = 0; x < m_chunkWidth; x++)
            {
                for (int y = 0; y < m_chunkHeight; y++)
                {
                    for (int z = 0; z < m_chunkDepth; z++)
                    {
                        voxels.Add(new Voxel { Transparent = 0, Type = UnityEngine.Random.Range(0, 2)});
                        /*if (x > m_chunkWidth / 2 && x < m_chunkWidth * 0.75 &&
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
                        }*/
                    }
                }
            }
            entityManager.AddComponentData(entity, new TriangulateChunk());
        }

        public void PlaceVoxel(float3 worldPosition, Voxel voxel)
        {
            var world = World.Active;
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
            Debug.LogFormat("Chunk {0}:{1}:{2}", chunkIndex.x, chunkIndex.y, chunkIndex.z);

            ChunkIndex cIndex = new ChunkIndex { X = chunkIndex.x, Y = chunkIndex.y, Z = chunkIndex.z };
            if (!m_createdChunks.ContainsKey(cIndex))
            {
                Debug.LogError("Could not find chunk with index: " + cIndex.ToString());
                return;
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

            Debug.LogFormat("Voxel {0}:{1}:{2}", localPos.x, localPos.y, localPos.z);

            //  Set the supplied voxel at the voxel index of the chunk.
            int flatVoxelIndex = voxelIndex.x + m_chunkWidth * (voxelIndex.y + m_chunkDepth * voxelIndex.z);
            Debug.LogFormat("VoxelIndex {0}", flatVoxelIndex);

            Entity chunkEntity = m_createdChunks[cIndex];
            var voxelBuffer = world.EntityManager.GetBuffer<Voxel>(chunkEntity);
            voxelBuffer[flatVoxelIndex] = voxel;

            //  Trigger retriangulation.
            if (!world.EntityManager.HasComponent<TriangulateChunk>(chunkEntity))
            {
                world.EntityManager.AddComponentData(chunkEntity, new TriangulateChunk());
            }
        }
    }
}

