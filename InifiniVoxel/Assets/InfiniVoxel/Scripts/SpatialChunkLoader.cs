using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.Components;
using InfiniVoxel.MonoBehaviours;
using UnityEngine;

namespace InfiniVoxel
{
    public class SpatialChunkLoader : MonoBehaviour
    {
        [SerializeField]
        private VoxelWorld m_voxelWorld;
        public VoxelWorld World
        {
            get { return m_voxelWorld; }
            set { m_voxelWorld = value; }
        }

        [SerializeField]
        private Vector3Int m_loadArea = new Vector3Int(10, 1, 10);
        
        private Vector3Int m_chunkPosition = new Vector3Int(-999,-999, -999);
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var chunkPosition = CalculateChunkPosition();

            if (chunkPosition != m_chunkPosition)
            {
                LoadChunks(chunkPosition);
                //m_voxelWorld.LoadChunk(new ChunkIndex(){ X = chunkPosition.x, Y = chunkPosition.y, Z = chunkPosition.z});
                //Debug.Log("Loaded chunk: " + chunkPosition);
            }
            
            m_chunkPosition = chunkPosition;
        }

        private Vector3Int CalculateChunkPosition()
        {
            int x = (int) (transform.position.x / World.ChunkWidth);
            int y = (int) (transform.position.y / World.ChunkHeight);
            int z = (int) (transform.position.z / World.ChunkDepth);
            
            Vector3Int chunkPos = new Vector3Int(x,y,z);
            return chunkPos;
        }

        private void LoadChunks(Vector3Int chunkPosition)
        {
            for (int x = 0; x < m_loadArea.x; x++)
            {
                for (int z = 0; z < m_loadArea.z; z++)
                {
                    for (int y = 0; y < m_loadArea.y; y++)
                    {
                        ChunkIndex chunkToLoad = new ChunkIndex()
                        {
                            X = chunkPosition.x - (m_loadArea.x / 2) + x,
                            Y = chunkPosition.y - (m_loadArea.y / 2) + y,
                            Z = chunkPosition.z - (m_loadArea.z / 2) + z
                        };
                        
                        m_voxelWorld.LoadChunk(chunkToLoad);
                    }
                }
            }
        }
    }
}


