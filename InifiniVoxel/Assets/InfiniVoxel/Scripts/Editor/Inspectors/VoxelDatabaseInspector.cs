using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace InfiniVoxel.Editor.Inspectors
{
    [CustomEditor(typeof(VoxelDatabase))]
    public class VoxelDatabaseInspector : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load All Voxels"))
            {
                FindAllVoxelTypes((VoxelDatabase)target);
            }

            DrawDefaultInspector();
        }
        
        public void FindAllVoxelTypes(VoxelDatabase database)
        {
            var voxels = AssetDatabase.FindAssets("t:VoxelData");

            database.ClearVoxels();
            
            Debug.Log("Searching for Voxels");
            foreach (var voxelGuid in voxels)
            {
                Debug.Log(voxelGuid);
                var assetPath = AssetDatabase.GUIDToAssetPath(voxelGuid);
                var voxelData = AssetDatabase.LoadAssetAtPath<VoxelData>(assetPath);
                database.AddVoxel(voxelData);
            }
            
            EditorUtility.SetDirty(database);
        }
    }

}

