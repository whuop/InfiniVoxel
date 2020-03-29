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
        private List<Sprite> m_sprites = new List<Sprite>();
        
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

        public void Pack()
        {
            Texture2D packedTexture = new Texture2D(512, 512);

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

                offset.x += newOffset.x;
            }
            
            SaveTexture(packedTexture);
            //AssetDatabase.CreateAsset(packedTexture, "Assets/packedTexture.png");
        }

        private void SaveTexture(Texture2D texture)
        {
            string filePath = Application.dataPath + "/packedTexture.png";
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
    }
}


