﻿using InfiniVoxel.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
/*
namespace InfiniVoxel.Editor.Importers
{
    

    public struct VoxVoxel
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte ColorIndex;
    }


    [ScriptedImporter(1, "vox")]
    public class VoxImporter : ScriptedImporter
    {
        uint[] DEFAULT_PALETTE = new uint[]{
            0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
            0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
            0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
            0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
            0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
            0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
            0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
            0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
            0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
            0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
            0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
            0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
            0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
            0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
            0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
            0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
        };

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var voxFile = File.ReadAllBytes(ctx.assetPath);
            Debug.Log("Loading vox file");
            using (MemoryStream stream = new MemoryStream(voxFile))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string header = new string(new char[] { reader.ReadChar(), reader.ReadChar(), reader.ReadChar(), reader.ReadChar() });
                Debug.Log("Vox Header: " + header);
                int version = reader.ReadInt32();
                Debug.Log("Vox Version: " + version);

                Vector3Int chunkSize;
                List<VoxVoxel> voxels = null;
                Texture2D paletteTexture = null;

                int i = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string chunkID = new string(new char[] { reader.ReadChar(), reader.ReadChar(), reader.ReadChar(), reader.ReadChar() });
                    
                    Debug.LogFormat("Processing Chunk: {0}", chunkID);
                    switch (chunkID)
                    {
                        case "MAIN":
                            ProcessMainChunk(reader);
                            break;
                        case "SIZE": // Begin new model
                            chunkSize = ProcessSizeChunk(reader);
                            break;
                        case "XYZI": // Finalize new model
                            voxels = ProcessXYZIChunk(reader);
                            break;
                        case "PACK": // Optional
                            ProcessPackChunk(reader);
                            break;
                        case "RGBA": // Optional
                            var paletteColors = ProcessRGBAChunk(reader);
                            paletteTexture = CreateTextureFromColor16x16(paletteColors.ToArray());
                            break;
                        case "MATT": // Optional
                            ProcessMatChunk(reader);
                            break;
                    }
                    i++;
                    if (i > 1000)
                        break;
                }

                //  Create palette
                AssetDatabase.StartAssetEditing();
                string texturePathWithExt = Path.Combine(Application.dataPath, "Palette.png");
                string texturePath = Path.Combine("Assets", "Palette.png");
                File.WriteAllBytes(texturePathWithExt, paletteTexture.EncodeToPNG());
                AssetDatabase.StopAssetEditing();

                Texture2D referencedPaletteTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                Material paletteMat = new Material(Shader.Find("Diffuse"));

                AssetDatabase.StartAssetEditing();
                var palette = ScriptableObject.CreateInstance<VoxelPalette>();
                AssetDatabase.CreateAsset(palette, "Assets/Palette.asset");
                palette.Initialize(paletteMat, referencedPaletteTexture, 1);
                EditorUtility.SetDirty(palette);
                AssetDatabase.Refresh();
                AssetDatabase.StopAssetEditing();

                
                //  Process voxels, process colors
                GameObject voxelModelGO = new GameObject(ctx.assetPath);
                VoxelWorld world = voxelModelGO.AddComponent<InfiniVoxel.MonoBehaviours.VoxelWorld>();
                
                //  Create voxels from Vox Voxels
                for(int j = 0; j < voxels.Count; j++)
                {
                    VoxVoxel vox = voxels[j];
                    
                }


                //world.CreateFromVoxelArray()

                //ProcessChunk(chunkID, chunkByteSize, childrenChunkCount);
            }
        }

        private void ProcessMainChunk(BinaryReader reader)
        {
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();

            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);
        }

        private Vector3Int ProcessSizeChunk(BinaryReader reader)
        {
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();
            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);

            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            Debug.LogFormat("X:{0} Y:{1} Z:{2}", x, y, z);
            return new Vector3Int(x, y, z);
        }

        private List<Color32> ProcessRGBAChunk(BinaryReader reader)
        {
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();
            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);

            List<Color32> palette = new List<Color32>();

            for(int i = 0; i < 256; i++)
            {
                uint rgba = reader.ReadUInt32();

                Color32 color = new Color32();
                color.r = (byte)((rgba >> 0)  & 0xFF);
                color.g = (byte)((rgba >> 8)  & 0xFF);
                color.b = (byte)((rgba >> 16) & 0xFF);
                color.a = (byte)((rgba >> 24) & 0xFF);

                palette.Add(color);
            }

            return palette;
        }

        private Texture2D CreateTextureFromColor16x16(Color32[] colors)
        {
            Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false, false);
            texture.name = "PaletteTexture";
            texture.SetPixels32(colors);
            texture.Apply();
            return texture;
        }

        private void ProcessMatChunk(BinaryReader reader)
        {
            Debug.Log("Processing MATT");
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();
            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);
        }

        private void ProcessPackChunk(BinaryReader reader)
        {
            Debug.Log("Processing PACK");
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();
            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);
        }

        private List<VoxVoxel> ProcessXYZIChunk(BinaryReader reader)
        {
            int chunkContentSize = reader.ReadInt32();
            int childrenContentSizeTotal = reader.ReadInt32();
            Debug.LogFormat("Chunk Content Size: {0} Children total content size: {1}", chunkContentSize, childrenContentSizeTotal);

            int numVoxels = reader.ReadInt32();
            Debug.LogFormat("Num Voxels: " + numVoxels);

            List<VoxVoxel> voxels = new List<VoxVoxel>();
            for(int i = 0; i < numVoxels; i++)
            {
                voxels.Add(new VoxVoxel
                {
                    X = reader.ReadByte(),
                    Y = reader.ReadByte(),
                    Z = reader.ReadByte(),
                    ColorIndex = reader.ReadByte()
                });
            }
            return voxels;
        }

        private void ProcessChunk(string chunkID, int chunkByteSize, int childrenChunkCount)
        {
            Debug.LogFormat("Processing Chunk {0} with ChunkByteSize {1} and ChildrenChunkCount {2}", chunkID, chunkByteSize, childrenChunkCount);
            if (chunkID == "MAIN")
            {
                Debug.Log("Processing main chunk");
            }
            else if (chunkID == "SIZE")
            {
                Debug.Log("Processing size");
            }
            else if (chunkID == "XYZI")
            {
                Debug.Log("Processing XYZI");
            }
            //  Optional, only present if there are more than 1 voxel models.
            else if (chunkID == "PACK")
            {
                Debug.Log("Processing PACK");
            }
        }
    }
}
*/

