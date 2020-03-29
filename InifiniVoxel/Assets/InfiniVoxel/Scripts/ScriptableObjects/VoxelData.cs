using Unity.Mathematics;
using UnityEngine;

namespace InfiniVoxel.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VoxelData", menuName = "InfiniVoxel/VoxelData", order = 1)]
    public class VoxelData : ScriptableObject
    {
        [SerializeField]
        private bool m_isTransparent;
        [SerializeField]
        private int m_type;
        [SerializeField]
        private Material m_material;
        public Material Material
        {
            get { return m_material; }
        }

        [SerializeField]
        [HideInInspector]
        private float2 m_topUV;
        public float2 TopUV
        {
            get { return m_topUV; }
            set { m_topUV = value; }
        }
        
        [SerializeField]
        [HideInInspector]
        private float2 m_bottomUV;
        public float2 BottomUV
        {
            get { return m_bottomUV; }
            set { m_bottomUV = value; }
        }
        
        [SerializeField]
        [HideInInspector]
        private float2 m_northUV;
        public float2 NorthUV
        {
            get { return m_northUV; }
            set { m_northUV = value; }
        }
        
        [SerializeField]
        [HideInInspector]
        private float2 m_southUV;
        public float2 SouthUV
        {
            get { return m_southUV; }
            set { m_southUV = value; }
        }
        
        [SerializeField]
        [HideInInspector]
        private float2 m_eastUV;
        public float2 EastUV
        {
            get { return m_eastUV; }
            set { m_eastUV = value; }
        }
        
        [SerializeField]
        [HideInInspector]
        private float2 m_westUV;
        public float2 WestUV
        {
            get { return m_westUV; }
            set { m_westUV = value; }
        }

        [SerializeField]
        private Sprite m_topSprite;
        public Sprite TopSprite => m_topSprite;
        [SerializeField]
        private Sprite m_bottomSprite;
        public Sprite BottomSprite => m_bottomSprite;
        [SerializeField]
        private Sprite m_northSprite;
        public Sprite NorthSprite => m_northSprite;
        [SerializeField]
        private Sprite m_southSprite;
        public Sprite SouthSprite => m_southSprite;
        [SerializeField] 
        private Sprite m_westSprite;
        public Sprite WestSprite => m_westSprite;
        [SerializeField]
        private Sprite m_eastSprite;
        public Sprite EastSprite => m_eastSprite;
        
        
        public VoxelConcurrent ToVoxel()
        {
            return new VoxelConcurrent(m_isTransparent, NorthUV, SouthUV, EastUV, WestUV, TopUV, BottomUV);
        }
    }

    public struct VoxelConcurrent
    {
        private int m_isTransparent;
        public int IsTransparent
        {
            get { return m_isTransparent; }
        }
        
        private float2 m_northUV;
        public float2 NorthUV
        {
            get { return m_northUV; }
        }

        private float2 m_southUV;
        public float2 SouthUV
        {
            get { return m_southUV; }
        }
        
        private float2 m_eastUV;
        public float2 EastUV
        {
            get { return m_eastUV; }
        }
        
        private float2 m_westUV;
        public float2 WestUV
        {
            get { return m_westUV; }
        }
        
        private float2 m_topUV;
        public float2 TopUV
        {
            get { return m_topUV; }
        }
        
        private float2 m_bottomUV;
        public float2 BottomUV
        {
            get { return m_bottomUV; }
        }

        public VoxelConcurrent(bool isTransparent, float2 northUV, float2 southUV, float2 eastUV, float2 westUV,
            float2 topUV, float2 bottomUV)
        {
            m_isTransparent = isTransparent ? 1 : 0;
            m_northUV = northUV;
            m_southUV = southUV;
            m_westUV = westUV;
            m_eastUV = eastUV;
            m_topUV = topUV;
            m_bottomUV = bottomUV;
        }
    }
}


