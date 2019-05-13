using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using InfiniVoxel.Attributes;

namespace InfiniVoxel.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }


}
