using InfiniVoxel.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.ScriptableObjects;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.Brush
{
    public class VoxelBrush : MonoBehaviour
    {
        private float m_brushLength = 999.0f;

        private VoxelWorld m_voxelWorld;
        
        [SerializeField]
        private VoxelData m_brushTile;

        // Start is called before the first frame update
        void Start()
        {
            m_voxelWorld = FindObjectOfType<VoxelWorld>();
        }

        // Update is called once per frame
        void Update()
        {
            float3 cameraPos = Camera.main.transform.position;
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float3 toPos = cameraPos + (float3)mouseRay.direction * m_brushLength;

            /*if (Input.GetMouseButtonDown(0))
            {
                var hit = Landfill.Raycast.ResultAsHit(World.All[0],cameraPos, toPos);
                if (hit.SurfaceNormal.Equals(float3.zero))
                {
                    Debug.LogError("Hit Surface Normal is Zero!");
                }
                else
                {
                    Debug.Log("Hit Something!");
                    m_voxelWorld.PlaceVoxel(hit.Position + hit.SurfaceNormal * 0.5f, new Buffers.Voxel { DatabaseIndex = 1 });
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                var hit = Landfill.Raycast.ResultAsHit(World.All[0],cameraPos, toPos);
                if (hit.SurfaceNormal.Equals(float3.zero))
                {
                    Debug.LogError("Hit Surface Normal is Zero!");
                }
                else
                {
                    Debug.Log("Hit Something!");
                    m_voxelWorld.PlaceVoxel(hit.Position - hit.SurfaceNormal * 0.1f, new Buffers.Voxel { DatabaseIndex = 0});
                }
            }*/
        }

        private void OnDrawGizmos()
        {
            float3 cameraPos = Camera.main.transform.position;
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(cameraPos, 0.2f);

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float3 toPos = cameraPos + (float3)mouseRay.direction * m_brushLength;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(toPos, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(cameraPos, toPos);
        }
    }
}

