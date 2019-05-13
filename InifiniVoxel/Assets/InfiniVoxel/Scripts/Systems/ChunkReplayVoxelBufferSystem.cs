using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace InfiniVoxel.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ChunkReplayVoxelBufferSystem : JobComponentSystem
    {
        private struct Job : IJobChunk
        {
            [ReadOnly]
            public ArchetypeChunkEntityType EntityType;
            public ArchetypeChunkBufferType<Voxel> VoxelType;
            public ArchetypeChunkBufferType<ChunkModification> ModificationType; 

            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var entities = chunk.GetNativeArray(EntityType);
                var voxelsAccessor = chunk.GetBufferAccessor(VoxelType);
                var modificationsAccessor = chunk.GetBufferAccessor(ModificationType);

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    var modifications = modificationsAccessor[i];
                    var voxels = voxelsAccessor[i];

                    if (modifications.Length == 0)
                        continue;

                    for(int j = 0; j < modifications.Length; j++)
                    {
                        var modification = modifications[j];
                        int voxelIndex = modification.FlatVoxelIndex;
                        voxels[voxelIndex] = modification.Voxel;
                    }

                    CommandBuffer.AddComponent(chunkIndex, entity, new TriangulateChunk());
                    modifications.Clear();
                }

            }
        }

        private EntityQuery m_entityQuery;
        private EndInitializationEntityCommandBufferSystem m_barrier;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            m_entityQuery = GetEntityQuery(ComponentType.ReadWrite<Voxel>(), ComponentType.ReadWrite<ChunkModification>(),
                                            ComponentType.Exclude<ApplyMesh>(), ComponentType.Exclude<TriangulateChunk>());
            m_barrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var entities = GetArchetypeChunkEntityType();
            var voxelType = GetArchetypeChunkBufferType<Voxel>(false);
            var chunkModificationType = GetArchetypeChunkBufferType<ChunkModification>(false);

            var job = new Job
            {
                EntityType = entities,
                VoxelType = voxelType,
                ModificationType = chunkModificationType,

                CommandBuffer = m_barrier.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(m_entityQuery, inputDeps);
            m_barrier.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}


