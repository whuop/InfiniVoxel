using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
/*
namespace InfiniVoxel.Editor.Inspectors
{
    [CustomEditor(typeof(InfiniVoxel.MonoBehaviours.VoxelPalette))]
    public class VoxelPaletteInspector : UnityEditor.Editor
    {
        private VisualElement m_rootElement;
        private VisualTreeAsset m_modulesVisualTree;

        public void OnEnable()
        {
            //  Hierarchy
            m_rootElement = new VisualElement();
            m_modulesVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/InfiniVoxel/Editor/UI/VoxelPaletteEditor.uxml");

            //  Styles
            var stylesheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/InfiniVoxel/Editor/UI/VoxelPaletteStyles.uss");
            m_rootElement.styleSheets.Add(stylesheet);
        }

        public void OnDisable()
        {
            
        }

        public override VisualElement CreateInspectorGUI()
        {
            //  Reset root element and reuse
            var root = m_rootElement;
            root.Clear();

            //  Turn the UXML into a VisualElement hierarchy under the root
            m_modulesVisualTree.CloneTree(root);

            //  Create Module previews
            root.Query(classes: new string[] { "voxel-preview" }).
                ForEach((preview) =>
                {
                    preview.Add(CreatePaletteUI(preview.name));
                });

            return root;
        }

        private VisualElement CreatePaletteUI(string moduleName)
        {
            //  Reset ui stuff here

            var imguiContainer = new IMGUIContainer(() =>
            {
                //var editor = 
            });

            imguiContainer.AddToClassList("palette-viewport");

            return imguiContainer;
        }
    }
}

*/