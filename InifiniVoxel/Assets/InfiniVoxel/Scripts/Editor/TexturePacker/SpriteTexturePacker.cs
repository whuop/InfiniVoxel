using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace InfiniVoxel.Editor.TexturePacker
{
    public class SpriteTexturePacker
    {
        private string m_outputPath = Application.dataPath + "/InfiniVoxel/Generated/";
        
        private List<Sprite> m_sprites = new List<Sprite>();
        private List<float2> m_spriteRectMapping = new List<float2>();
        
        public SpriteTexturePacker()
        {
            
        }

        public int AddSprite(Sprite sprite)
        {
            if (m_sprites.Contains(sprite))
            {
                return m_sprites.IndexOf(sprite);
            }
            
            m_sprites.Add(sprite);
            return m_sprites.Count - 1;
        }

        public float2 GetUVs(int spriteIndex)
        {
            return m_spriteRectMapping[spriteIndex];
        }

        public void Pack()
        {
            Texture2D packedTexture = new Texture2D(256, 256);
            int2 offset = new int2(0,0);

            for (int i = 0; i < m_sprites.Count; i++)
            {
                Sprite sprite = m_sprites[i];
                int2 newOffset = new int2(offset.x + (int)sprite.textureRect.width,
                                        offset.y + (int)sprite.textureRect.height);

                var pixels = sprite.texture.GetPixels((int) sprite.textureRect.x,
                    (int) sprite.textureRect.y,
                    (int) sprite.textureRect.width,
                    (int) sprite.textureRect.height);
                
                packedTexture.SetPixels(offset.x, offset.y,
                    (int)sprite.textureRect.width, (int)sprite.textureRect.height, 
                    pixels);


                float2 tileSize = new float2(packedTexture.width / sprite.textureRect.width,
                    packedTexture.height / sprite.textureRect.height);
                float2 tileSizeNormalized = new float2(1.0f / tileSize.x, 1.0f / tileSize.y);
                Debug.Log("Tile Size: " + tileSizeNormalized);
                
                float2 uvPosition = new float2(offset.x / tileSize.x, offset.y / tileSize.y);
                Debug.Log("UV Position: " + uvPosition);
                
                float2 uv = new float2(uvPosition.x * tileSizeNormalized.x, uvPosition.y * tileSizeNormalized.y);
                Debug.Log("UV: " + uv);
                
                m_spriteRectMapping.Add(uv);
                offset.x = newOffset.x;
            }
            
            SaveTexture(packedTexture);
            //NormalizeUVs();
        }

        private void SaveTexture(Texture2D texture)
        {
            Directory.CreateDirectory(m_outputPath);
            string filePath = m_outputPath + "packedTexture.png";
            Debug.Log("Saving to path: " + filePath);
            byte[] bytes = texture.EncodeToPNG();
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            for (int i = 0; i < bytes.Length; i++) {
                writer.Write(bytes[i]);
            }
            writer.Close();
            stream.Close();
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private void NormalizeUVs()
        {
            string localPath = "Assets/InfiniVoxel/Generated/packedTexture.png";
            var packedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(localPath);
            
            for (int i = 0; i < m_spriteRectMapping.Count; i++)
            {
                float2 uvs = m_spriteRectMapping[i];

                uvs.x = uvs.x / packedTexture.width;
                uvs.y = uvs.y / packedTexture.height;
                //uvs.width = uvs.width / packedTexture.width;
                //uvs.height = uvs.height / packedTexture.height;

                m_spriteRectMapping[i] = uvs;
            }
        }

    }
}


