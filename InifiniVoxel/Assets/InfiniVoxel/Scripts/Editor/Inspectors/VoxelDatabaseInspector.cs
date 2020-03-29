using System.Collections;
using System.Collections.Generic;
using InfiniVoxel.Editor.TexturePacker;
using InfiniVoxel.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace InfiniVoxel.Editor.Inspectors
{
    [CustomEditor(typeof(VoxelDatabase))]
    public class VoxelDatabaseInspector : UnityEditor.Editor
    {

        private struct VoxelSideIDs
        {
            public int Top;
            public int Bottom;
            public int North;
            public int South;
            public int West;
            public int East;
        }
        
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
            
            PackVoxelSprites(database);
        }

        private void PackVoxelSprites(VoxelDatabase database)
        {
            List<VoxelSideIDs> sideIDs = new List<VoxelSideIDs>();
            SpriteTexturePacker texturePacker = new SpriteTexturePacker();
            foreach (var voxel in database.GetVoxels())
            {
                VoxelSideIDs ids = new VoxelSideIDs();
                ids.Top = texturePacker.AddSprite(voxel.TopSprite);
                ids.Bottom = texturePacker.AddSprite(voxel.BottomSprite);
                ids.North = texturePacker.AddSprite(voxel.NorthSprite);
                ids.South = texturePacker.AddSprite(voxel.SouthSprite);
                ids.West = texturePacker.AddSprite(voxel.WestSprite);
                ids.East = texturePacker.AddSprite(voxel.EastSprite);
                sideIDs.Add(ids);
            }
            
            texturePacker.Pack();

            for (int i = 0; i < database.GetVoxels().Count; i++)
            {
                VoxelData voxel = database.GetVoxels()[i];
                VoxelSideIDs id = sideIDs[i];

                voxel.TopUV = texturePacker.GetUVs(id.Top);
                voxel.BottomUV = texturePacker.GetUVs(id.Bottom);
                voxel.NorthUV = texturePacker.GetUVs(id.North);
                voxel.SouthUV = texturePacker.GetUVs(id.South);
                voxel.WestUV = texturePacker.GetUVs(id.West);
                voxel.EastUV = texturePacker.GetUVs(id.East);
                
                EditorUtility.SetDirty(voxel);
            }
        }
    }

}

