using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace InfiniVoxel.ScriptableObjects
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "WorldGenerator", menuName = "InfiniVoxel/World Generator", order = 1)]
    public class WorldGenerator : ScriptableObject
    {

        [SerializeField]
        private int m_seed = 0;
        [SerializeField]
        private float m_frequency = 0.01f;

        private struct Job : IJob
        {
            public int ChunkIndex;
            public NativeArray<ArchetypeChunk> Chunks;
            public ArchetypeChunkEntityType EntityType;
            public ArchetypeChunkBufferType<Voxel> VoxelType;
            public ArchetypeChunkComponentType<ChunkIndex> ChunkIndexType;

            public void Execute()
            {

            }
        }


        public ComponentSystemBase GetGenerationSystem(World world)
        {
            return world.GetOrCreateSystem<WorldGenerationSystem>();
        }

        public struct GenerateVoxels : IComponentData { }

        public class WorldGenerationSystem : ComponentSystem
        {
            private EntityQuery m_query;

            private JobHandle m_handle;
            private EntityCommandBuffer m_entityCommandBuffer;

            private static int CONCURRENT_JOB_COUNT = 10;

            protected override void OnCreate()
            {
                base.OnCreate();
                m_query = GetEntityQuery(ComponentType.ReadOnly<ChunkIndex>(),
                                       ComponentType.ReadWrite<Voxel>(),
                                       ComponentType.ReadWrite<GenerateVoxels>());

                m_handle = new JobHandle();
                m_entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

                
            }

            protected override void OnDestroy()
            {
                base.OnDestroy();
                m_entityCommandBuffer.Dispose();
            }

            protected override void OnUpdate()
            {
                if (m_handle.IsCompleted)
                {
                    m_handle.Complete();

                    m_entityCommandBuffer.Playback(EntityManager);
                    m_entityCommandBuffer.Dispose();
                }
                else
                {
                    return;
                }

                m_entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

                var entityType = GetArchetypeChunkEntityType();
                var chunkIndexType = GetArchetypeChunkComponentType<ChunkIndex>();
                var voxelType = GetArchetypeChunkBufferType<Voxel>();

                var chunks = m_query.CreateArchetypeChunkArray(Allocator.TempJob);

                int jobCount = Mathf.Min(CONCURRENT_JOB_COUNT, chunks.Length);

                NativeArray<JobHandle> handlesToCombine = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);
                for(int i = 0; i < jobCount; i++)
                {
                    int chunkIndex = i;

                    var job = new Job
                    {
                        ChunkIndex = chunkIndex,
                        EntityType = entityType,
                        ChunkIndexType = chunkIndexType,
                        VoxelType = voxelType,
                        Chunks = chunks
                    };

                    var handle = job.Schedule();
                    handlesToCombine[i] = handle;
                }

                m_handle = JobHandle.CombineDependencies(handlesToCombine);
            }
        }
    }
}


