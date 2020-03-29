using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Material = Unity.Physics.Material;

namespace InfiniVoxel.Systems
{
    [UpdateAfter(typeof(ChunkTriangulationSystem))]
    public class ChunkApplyMeshSystem : ComponentSystem
    {
        private EntityQuery m_query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<Vertex>(), 
                ComponentType.ReadOnly<Index>(), 
                ComponentType.ReadOnly<Buffers.Color>(), 
                ComponentType.ReadWrite<ApplyMesh>(), 
                ComponentType.Exclude<TriangulateChunk>());
        }
        
        protected override void OnUpdate()
        {
            var entities = m_query.ToEntityArray(Unity.Collections.Allocator.TempJob);

            for(int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var vertexBuffer = EntityManager.GetBuffer<Vertex>(entity);
                var indexBuffer = EntityManager.GetBuffer<Index>(entity);
                var UV0Buffer = EntityManager.GetBuffer<UV0>(entity);

                Vector3[] vertices = new Vector3[vertexBuffer.Length];
                int[] indices = new int[indexBuffer.Length];
                Vector2[] uv0 = new Vector2[UV0Buffer.Length/* * 2*/];
                
                NativeArray<float3> colliderVertices = new NativeArray<float3>(vertexBuffer.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                NativeArray<int3> colliderIndices = new NativeArray<int3>(indexBuffer.Length / 3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                int longest = Mathf.Max(vertices.Length, indices.Length);
                longest = Mathf.Max(longest, UV0Buffer.Length);

                int l = 0;
                for(int j = 0; j < longest; j++)
                {
                    if (j < vertices.Length)
                    {
                        vertices[j] = vertexBuffer[j].Value;
                        colliderVertices[j] = vertexBuffer[j].Value;
                    }
                    if (j < indices.Length)
                        indices[j] = indexBuffer[j].value;
                    if (j < UV0Buffer.Length)
                    {
                        uv0[j] = new Vector2(UV0Buffer[j].Value.x, UV0Buffer[j].Value.y);
                        //uv0[j] = new Vector2(UV0Buffer[j].Value.width, UV0Buffer[j].Value.height);
                    }

                    l += 2;
                }

                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var mesh = renderMesh.mesh;
                mesh.Clear();

                mesh.SetVertices(new List<Vector3>(vertices));
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                mesh.SetUVs(0, new List<Vector2>(uv0));
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();

                EntityManager.SetComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });

                vertexBuffer.Clear();
                indexBuffer.Clear();
                UV0Buffer.Clear();
                
                for (int k = 0; k < indices.Length; k += 3)
                {
                    colliderIndices[k / 3] = new int3()
                    {
                        x = indices[k],
                        y = indices[k+1],
                        z = indices[k+2]
                    };
                }

                
                //  Create physics collider
                var physicsFilter = Unity.Physics.CollisionFilter.Default;
                var physicsMaterial = Unity.Physics.Material.Default;
                var colliders = Unity.Physics.MeshCollider.Create(colliderVertices, colliderIndices, physicsFilter, physicsMaterial);

                if (EntityManager.HasComponent<PhysicsCollider>(entity))
                {
                    EntityManager.SetComponentData(entity, new PhysicsCollider { Value = colliders });
                }
                else
                {
                    EntityManager.AddComponentData(entity, new PhysicsCollider { Value = colliders });
                }

                EntityManager.SetSharedComponentData(entity, renderMesh);
                EntityManager.RemoveComponent<ApplyMesh>(entity);
                
                //    Clean up native arrays to avoid memory leaks.
                colliderVertices.Dispose();
                colliderIndices.Dispose();
                
                Debug.Log($"Applied Mesh and Collider for Entity {entity.Index}");
            }
            entities.Dispose();
        }
    }

}

