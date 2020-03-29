using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.Buffers;
using InfiniVoxel.MonoBehaviours;
using InfiniVoxel.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace InfiniVoxel.Editor.Inspectors
{
    [CustomEditor(typeof(VoxelData))]
    public class VoxelDataInspector : UnityEditor.Editor
    {
        private GameObject m_previewGameObject = null;
        private UnityEditor.Editor m_previewEditor = null;
        
        public override void OnInspectorGUI()
        {
            var voxelData = (VoxelData) target;
            
            /*Rect rect = new Rect(0, 0, 64, 64);
            if (voxelData.Material != null && voxelData.Material.GetTexture("_BlockAtlas") != null)
                EditorGUI.DrawPreviewTexture(rect, voxelData.Material.GetTexture("_BlockAtlas"), voxelData.Material);
            */
            DrawDefaultInspector();

            /*if (m_previewGameObject == null)
            {
                m_previewGameObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/InfiniVoxel/VoxelPreview.prefab"));

                var voxelWorld = m_previewGameObject.GetComponent<VoxelWorld>();

                int index = voxelWorld.VoxelDatabase.GetIndexFromVoxel(voxelData);
                
                m_previewGameObject.GetComponent<VoxelWorld>().PlaceVoxel(0, 0, 0, new Voxel{ DatabaseIndex = index });
                m_previewEditor = UnityEditor.Editor.CreateEditor(m_previewGameObject);
            }*/
            
            //m_previewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(500, 500), EditorStyles.whiteLabel);
        }

    }

}

