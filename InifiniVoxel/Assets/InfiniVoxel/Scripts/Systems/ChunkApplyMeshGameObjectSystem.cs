using InfiniVoxel.Buffers;
using InfiniVoxel.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace InfiniVoxel.Systems
{
    [UpdateAfter(typeof(ChunkTriangulationSystem))]
    public class ChunkApplyMeshGameObjectSystem : ComponentSystem
    {
        private EntityQuery m_query;

        private ChunkGameObjectPoolSystem m_poolableSystem;

        private Material m_chunkMaterial;
        public Material ChunkMaterial
        {
            get { return m_chunkMaterial; }
            set { m_chunkMaterial = value; }
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<Vertex>(), 
                ComponentType.ReadOnly<Index>(), 
                ComponentType.ReadOnly<Buffers.Color>(), 
                ComponentType.ReadWrite<ApplyMeshGameObject>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.Exclude<TriangulateChunk>());

            m_poolableSystem = World.GetOrCreateSystem<ChunkGameObjectPoolSystem>();
        }

        
        protected override void OnUpdate()
        {
            var entities = m_query.ToEntityArray(Unity.Collections.Allocator.TempJob);
            var positions = m_query.ToComponentDataArray<Translation>(Allocator.TempJob);
            
            for(int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var position = positions[i];
                var vertexBuffer = EntityManager.GetBuffer<Vertex>(entity);
                var indexBuffer = EntityManager.GetBuffer<Index>(entity);
                var UV0Buffer = EntityManager.GetBuffer<UV0>(entity);

                Vector3[] vertices = new Vector3[vertexBuffer.Length];
                int[] indices = new int[indexBuffer.Length];
                Vector2[] uv0 = new Vector2[UV0Buffer.Length];

                int longest = Mathf.Max(vertices.Length, indices.Length);
                longest = Mathf.Max(longest, UV0Buffer.Length);

                for(int j = 0; j < longest; j++)
                {
                    if (j < vertices.Length)
                    {
                        vertices[j] = vertexBuffer[j].Value;
                    }
                    if (j < indices.Length)
                        indices[j] = indexBuffer[j].value;
                    if (j < UV0Buffer.Length)
                    {
                        uv0[j] = new Vector2(UV0Buffer[j].Value.x, UV0Buffer[j].Value.y);
                    }
                }

                var poolable = m_poolableSystem.GetChunkPoolable();
                var mesh = poolable.MeshFilter.sharedMesh;

                poolable.GameObject.transform.position = position.Value;
                
                poolable.MeshRenderer.sharedMaterial = m_chunkMaterial;
                
                mesh.Clear();

                mesh.SetVertices(new List<Vector3>(vertices));
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                mesh.SetUVs(0, new List<Vector2>(uv0));
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();

                poolable.MeshCollider.sharedMesh = mesh;
                
                vertexBuffer.Clear();
                indexBuffer.Clear();
                UV0Buffer.Clear();
                
                EntityManager.RemoveComponent<ApplyMeshGameObject>(entity);
            }
            
            
            entities.Dispose();
            positions.Dispose();
            
        }
    }

}

