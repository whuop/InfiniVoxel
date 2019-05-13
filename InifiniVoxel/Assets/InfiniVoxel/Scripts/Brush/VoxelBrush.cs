using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.Brush
{
    public class VoxelBrush : MonoBehaviour
    {
        private float m_brushLength = 999.0f;

        private VoxelWorld m_voxelWorld;

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

            var hit = Landfill.Raycast.ResultAsHit(cameraPos, toPos);

            if (hit.SurfaceNormal.Equals(float3.zero))
            {
                Debug.Log("No Collision!");
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_voxelWorld.PlaceVoxel(hit.Position + hit.SurfaceNormal * 0.1f, new Buffers.Voxel { Transparent = 0, Type = 3 });
                    Debug.Log("Adding Voxels");
                }
                if (Input.GetMouseButtonDown(1))
                {
                    m_voxelWorld.PlaceVoxel(hit.Position - hit.SurfaceNormal * 0.1f, new Buffers.Voxel { Transparent = 1, Type = 0 });
                    Debug.Log("Removing Voxels");
                }
                    
                //Debug.Log("Collision Normal: " + hit.SurfaceNormal);
            }
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

