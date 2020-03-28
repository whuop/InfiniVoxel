using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ChunkTriangulationSystem : JobComponentSystem
    {
        private struct Job : IJobChunk
        {
            private const int NORTH = 1;
            private const int SOUTH = -NORTH;
            private const int EAST = 2;
            private const int WEST = -EAST;
            private const int TOP = 3;
            private const int BOTTOM = -TOP;

            [ReadOnly]
            public ArchetypeChunkEntityType EntityType;
            [ReadOnly]
            public ArchetypeChunkBufferType<Voxel> VoxelType;
            [ReadOnly]
            public ArchetypeChunkComponentType<Components.ChunkScale> ChunkScaleType;

            public NativeArray<VoxelConcurrent> ConcurrentVoxels;

            public ArchetypeChunkBufferType<Vertex> VertexType;
            public ArchetypeChunkBufferType<Index> IndexType;
            public ArchetypeChunkBufferType<UV0> UV0Type;

            public EntityCommandBuffer.Concurrent CommandBuffer;

            public int ChunkWidth;
            public int ChunkHeight;
            public int ChunkDepth;

            private void AddQuad(
                              float3 v1,
                              float3 v2,
                              float3 v3,
                              float3 v4,
                              int width,
                              int height,
                              Voxel voxel,
                              bool backFace,
                              float scale,
                              DynamicBuffer<Vertex> vertices,
                              DynamicBuffer<Index> indices,
                              DynamicBuffer<UV0> UVs)
            {
                NativeArray<Vertex> voxelVertices = new NativeArray<Vertex>(4, Allocator.Temp);
                NativeArray<Index> voxelIndices = new NativeArray<Index>(6, Allocator.Temp);
                NativeArray<UV0> voxelUVs = new NativeArray<UV0>(4, Allocator.Temp);

                voxelVertices[0] = new Vertex { Value = v1 * scale };
                voxelVertices[1] = new Vertex { Value = v2 * scale };
                voxelVertices[2] = new Vertex { Value = v3 * scale };
                voxelVertices[3] = new Vertex { Value = v4 * scale };

                int indexOffset = vertices.Length;
                if (backFace)
                {
                    voxelIndices[0] = new Index { value = indexOffset + 2};
                    voxelIndices[1] = new Index { value = indexOffset + 1 };
                    voxelIndices[2] = new Index { value = indexOffset };

                    voxelIndices[3] = new Index { value = indexOffset };
                    voxelIndices[4] = new Index { value = indexOffset + 3 };
                    voxelIndices[5] = new Index { value = indexOffset + 2 };
                }
                else
                {
                    voxelIndices[0] = new Index { value = indexOffset };
                    voxelIndices[1] = new Index { value = indexOffset + 1 };
                    voxelIndices[2] = new Index { value = indexOffset + 2 };

                    voxelIndices[3] = new Index { value = indexOffset + 2 };
                    voxelIndices[4] = new Index { value = indexOffset + 3 };
                    voxelIndices[5] = new Index { value = indexOffset };

                }

                //float tileSize = (1.0f / 16.0f);
                VoxelConcurrent currentVoxel = ConcurrentVoxels[voxel.DatabaseIndex];
                
                for(int i = 0; i < voxelUVs.Length; i++)
                {
                    float2 uv = float2.zero;
                    switch (voxel.Side)
                    {
                        case TOP:
                            uv = currentVoxel.TopUV;
                            break;
                        case BOTTOM:
                            uv = currentVoxel.BottomUV;
                            break;
                        case WEST:
                            uv = currentVoxel.WestUV;
                            break;
                        case EAST:
                            uv = currentVoxel.EastUV;
                            break;
                        case NORTH:
                            uv = currentVoxel.NorthUV;
                            break;
                        case SOUTH:
                            uv = currentVoxel.SouthUV;
                            break;
                    }
                    Debug.Log($"Set UV {uv}");
                    voxelUVs[i] = new UV0 { Value = uv };
                    /*if (voxel.Type == 1)
                    {
                        float2 uv = new float2(0, tileSize * 15);
                        switch(voxel.Side)
                        {
                            case TOP:
                                uv.x = tileSize * 0;
                                break;
                            case BOTTOM:
                                uv.x = tileSize * 2;
                                break;
                            case WEST:
                            case EAST:
                            case NORTH:
                            case SOUTH:
                                uv.x = tileSize * 3;
                                break;
                        }
                        voxelUVs[i] = new UV0 { Value = uv };
                    }
                    else if (voxel.Type == 2)
                    {
                        float2 uv = new float2(0, tileSize * 15 );
                        voxelUVs[i] = new UV0 { Value = uv };
                    }
                    else if (voxel.Type == 3)
                    {
                        float2 uv = new float2(0, 0);
                        voxelUVs[i] = new UV0 { Value = uv };
                    }
                    else
                    {
                        voxelUVs[i] = new UV0 { Value = new float2(0, 0) };
                    }*/
                }

                vertices.AddRange(voxelVertices);
                indices.AddRange(voxelIndices);
                UVs.AddRange(voxelUVs);

                voxelVertices.Dispose();
                voxelIndices.Dispose();
                voxelUVs.Dispose();
            }

            private Voxel GetVoxel(DynamicBuffer<Voxel> buffer, int x, int y, int z, int side)
            {
                int flatIndex = x + ChunkWidth * (y + ChunkDepth * z);
                if (flatIndex < 0 || flatIndex >= buffer.Length)
                    return Voxel.Null;

                var voxelface = buffer[flatIndex];
                voxelface.Side = side;
                return voxelface;
            }

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var entities = chunk.GetNativeArray(EntityType);
                var voxelsAccessor = chunk.GetBufferAccessor(VoxelType);
                var scales = chunk.GetNativeArray(ChunkScaleType);

                var vertexAccessor = chunk.GetBufferAccessor(VertexType);
                var indexAccessor = chunk.GetBufferAccessor(IndexType);
                var uv0Accessor = chunk.GetBufferAccessor(UV0Type);
                int[] dims = new int[] { ChunkWidth, ChunkHeight, ChunkDepth };
                
                for(int index = 0; index < entities.Length; index++)
                {
                    var entity = entities[index];
                    var voxels = voxelsAccessor[index];
                    var scale = scales[index];

                    var vertices = vertexAccessor[index];
                    var indices = indexAccessor[index];
                    var uv0 = uv0Accessor[index];

                    /*
                     * These are just working variables for the algorithm - almost all taken 
                     * directly from Mikola Lysenko's javascript implementation.
                     */
                    int i, j, k, l, w, h, u, v, n, side = 0;

                    int[] x = new int[] { 0, 0, 0 };
                    int[] q = new int[] { 0, 0, 0 };
                    int[] du = new int[] { 0, 0, 0 };
                    int[] dv = new int[] { 0, 0, 0 };

                    /*
                     * These are just working variables to hold two faces during comparison.
                     */
                    Voxel voxelFace, voxelFace1;

                    /*
                     * We start with the lesser-spotted boolean for-loop (also known as the old flippy floppy). 
                     * 
                     * The variable backFace will be TRUE on the first iteration and FALSE on the second - this allows 
                     * us to track which direction the indices should run during creation of the quad.
                     * 
                     * This loop runs twice, and the inner loop 3 times - totally 6 iterations - one for each 
                     * voxel face.
                     */
                    for (bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b)
                    {

                        /*
                         * We sweep over the 3 dimensions - most of what follows is well described by Mikola Lysenko 
                         * in his post - and is ported from his Javascript implementation.  Where this implementation 
                         * diverges, I've added commentary.
                         */
                        for (int d = 0; d < 3; d++)
                        {

                            u = (d + 1) % 3;
                            v = (d + 2) % 3;

                            x[0] = 0;
                            x[1] = 0;
                            x[2] = 0;

                            q[0] = 0;
                            q[1] = 0;
                            q[2] = 0;

                            /*
                             * We create a mask - this will contain the groups of matching voxel faces 
                             * as we proceed through the chunk in 6 directions - once for each face.
                             */
                            Voxel[] mask = new Voxel[(dims[u] + 1) * (dims[v] + 1)];

                            q[d] = 1;

                            /*
                             * Here we're keeping track of the side that we're meshing.
                             */
                            if (d == 0) { side = backFace ? WEST : EAST; }
                            else if (d == 1) { side = backFace ? BOTTOM : TOP; }
                            else if (d == 2) { side = backFace ? SOUTH : NORTH; }

                            /*
                             * We move through the dimension from front to back
                             */
                            for (x[d] = -1; x[d] < dims[d];)
                            {

                                /*
                                 * -------------------------------------------------------------------
                                 *   We compute the mask
                                 * -------------------------------------------------------------------
                                 */
                                n = 0;

                                for (x[v] = 0; x[v] < dims[v]; x[v]++)
                                {
                                    for (x[u] = 0; x[u] < dims[u]; x[u]++)
                                    {
                                        /*
                                         * Here we retrieve two voxel faces for comparison.
                                         */
                                        voxelFace = (x[d] >= 0) ? GetVoxel(voxels, x[0], x[1], x[2], side) : Voxel.Null;
                                        voxelFace1 = (x[d] < ChunkWidth - 1) ? GetVoxel(voxels, x[0] + q[0], x[1] + q[1], x[2] + q[2], side) : Voxel.Null;

                                        /*
                                         * Note that we're using the equals function in the voxel face class here, which lets the faces 
                                         * be compared based on any number of attributes.
                                         * 
                                         * Also, we choose the face to add to the mask depending on whether we're moving through on a backface or not.
                                         */

                                        if (voxelFace != Voxel.Null && voxelFace1 != Voxel.Null && voxelFace.Equals(voxelFace1))
                                        {
                                            mask[n++] = Voxel.Null;
                                        }
                                        else if (backFace)
                                        {
                                            //    Todo: Fix transparent part
                                            var voxelFaceData = ConcurrentVoxels[voxelFace.DatabaseIndex];
                                            if (voxelFace == Voxel.Null || voxelFaceData.IsTransparent == 1)
                                                mask[n++] = voxelFace1;
                                            else
                                                mask[n++] = Voxel.Null;
                                        }
                                        else
                                        {
                                            //    Todo: Fix transparent part
                                            var voxelFaceData = ConcurrentVoxels[voxelFace1.DatabaseIndex];
                                            if (voxelFace1 == Voxel.Null || voxelFaceData.IsTransparent == 1)
                                                mask[n++] = voxelFace;
                                            else
                                                mask[n++] = Voxel.Null;
                                        }
                                    }
                                }

                                x[d]++;

                                /*
                                 * Now we generate the mesh for the mask
                                 */
                                n = 0;

                                for (j = 0; j < ChunkHeight; j++)
                                {

                                    for (i = 0; i < ChunkWidth;)
                                    {

                                        if (mask[n] != null)
                                        {
                                            
                                            /*
                                             * We compute the width
                                             */
                                            for (w = 1; i + w < ChunkWidth && mask[n + w] != Voxel.Null && mask[n + w].Equals(mask[n]); w++) { }

                                            /*
                                             * Then we compute height
                                             */
                                            bool done = false;

                                            for (h = 1; j + h < ChunkHeight; h++)
                                            {

                                                for (k = 0; k < w; k++)
                                                {

                                                    if (mask[n + k + h * ChunkWidth] == null || !mask[n + k + h * ChunkWidth].Equals(mask[n])) { done = true; break; }
                                                }

                                                if (done) { break; }
                                            }

                                            /*
                                             * Here we check the "transparent" attribute in the VoxelFace class to ensure that we don't mesh 
                                             * any culled faces.
                                             */
                                            var maskVoxel = ConcurrentVoxels[mask[n].DatabaseIndex];
                                            if (maskVoxel.IsTransparent == 0)
                                            {
                                                /*
                                                 * Add quad
                                                 */
                                                x[u] = i;
                                                x[v] = j;

                                                du[0] = 0;
                                                du[1] = 0;
                                                du[2] = 0;
                                                du[u] = w;

                                                dv[0] = 0;
                                                dv[1] = 0;
                                                dv[2] = 0;
                                                dv[v] = h;

                                                /*
                                                 * And here we call the quad function in order to render a merged quad in the scene.
                                                 * 
                                                 * We pass mask[n] to the function, which is an instance of the VoxelFace class containing 
                                                 * all the attributes of the face - which allows for variables to be passed to shaders - for 
                                                 * example lighting values used to create ambient occlusion.
                                                 */
                                                AddQuad(new float3(x[0], x[1], x[2]),
                                                     new float3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                                     new float3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                                     new float3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                                     w,
                                                     h,
                                                     mask[n],
                                                     backFace,
                                                     scale.Value,
                                                     vertices,indices,uv0);
                                            }

                                            /*
                                             * We zero out the mask
                                             */
                                            for (l = 0; l < h; ++l)
                                            {

                                                for (k = 0; k < w; ++k) { mask[n + k + l * ChunkWidth] = Voxel.Null; }
                                            }

                                            /*
                                             * And then finally increment the counters and continue
                                             */
                                            i += w;
                                            n += w;

                                        }
                                        else
                                        {

                                            i++;
                                            n++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    CommandBuffer.RemoveComponent<TriangulateChunk>(chunkIndex, entity);
                    CommandBuffer.AddComponent(chunkIndex, entity, new ApplyMesh());
                }
            }
        }

        private EntityQuery m_entityQuery;
        private EndSimulationEntityCommandBufferSystem m_barrier;

        private VoxelDatabase m_voxelDatabase;
        public VoxelDatabase VoxelDatabase
        {
            get { return m_voxelDatabase; }
            set { m_voxelDatabase = value; }
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_entityQuery = GetEntityQuery(ComponentType.ReadOnly<ChunkIndex>(), ComponentType.ReadOnly<Voxel>(), ComponentType.ReadWrite<TriangulateChunk>(),
                ComponentType.ReadWrite<Vertex>(), ComponentType.ReadWrite<Index>(), ComponentType.ReadWrite<Buffers.Color>(), ComponentType.Exclude<ApplyMesh>());
            
            m_barrier = this.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var entityType = GetArchetypeChunkEntityType();
            var voxelType = GetArchetypeChunkBufferType<Voxel>(true);
            var scaleType = GetArchetypeChunkComponentType<ChunkScale>(true);
            var vertexType = GetArchetypeChunkBufferType<Vertex>();
            var indexType = GetArchetypeChunkBufferType<Index>();
            var uv0Type = GetArchetypeChunkBufferType<UV0>();

            var execJob = new Job();
            execJob.CommandBuffer = m_barrier.CreateCommandBuffer().ToConcurrent();
            execJob.ConcurrentVoxels = m_voxelDatabase.GetConcurrentVoxels();
            execJob.EntityType = entityType;
            execJob.VoxelType = voxelType;
            execJob.ChunkScaleType = scaleType;
            execJob.ChunkHeight = 16;
            execJob.ChunkWidth = 16;
            execJob.ChunkDepth = 16;
            execJob.VertexType = vertexType;
            execJob.IndexType = indexType;
            execJob.UV0Type = uv0Type;
            var handle = execJob.Schedule(m_entityQuery, inputDeps);
            m_barrier.AddJobHandleForProducer(handle);

            return handle;
        }
    }

}

