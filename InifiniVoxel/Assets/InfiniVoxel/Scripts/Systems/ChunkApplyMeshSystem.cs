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

namespace InfiniVoxel.Systems
{
    [UpdateAfter(typeof(ChunkTriangulationSystem))]
    public class ChunkApplyMeshSystem : ComponentSystem
    {
        private EntityQuery m_query;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            m_query = GetEntityQuery(ComponentType.ReadOnly<Vertex>(), ComponentType.ReadOnly<Index>(), ComponentType.ReadOnly<Buffers.Color>(), ComponentType.ReadWrite<ApplyMesh>(), ComponentType.Exclude<TriangulateChunk>());
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
                Vector2[] uv0 = new Vector2[UV0Buffer.Length];

                float3[] colliderVertices = new float3[vertexBuffer.Length];

                int longest = Mathf.Max(vertices.Length, indices.Length);
                longest = Mathf.Max(longest, UV0Buffer.Length);

                for(int j = 0; j < longest; j++)
                {
                    if (j < vertices.Length)
                    {
                        vertices[j] = vertexBuffer[j].Value;
                        colliderVertices[j] = vertexBuffer[j].Value;
                    }
                    if (j < indices.Length)
                        indices[j] = indexBuffer[j].value;
                    if (j < uv0.Length)
                        uv0[j] = UV0Buffer[j].Value;
                }

                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var mesh = renderMesh.mesh;
                mesh.Clear();

                Debug.LogErrorFormat("VertCount:{0} UVCount:{1}", vertices.Length, uv0.Length);

                if (vertices.Length != uv0.Length)
                    Debug.LogError("Mismatch Vert/UV. Vert Count: " + vertices.Length + " UV count: " + uv0.Length);

                mesh.SetVertices(new List<Vector3>(vertices));
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                //mesh.SetTriangles(indices, 0);
                mesh.SetUVs(0, new List<Vector2>(uv0));
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();

                EntityManager.SetComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });

                vertexBuffer.Clear();
                indexBuffer.Clear();
                UV0Buffer.Clear();

                //  Create physics collider
                var colliders = Unity.Physics.MeshCollider.Create(colliderVertices, indices, null, null);

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
            }
            entities.Dispose();
        }
    }

}

