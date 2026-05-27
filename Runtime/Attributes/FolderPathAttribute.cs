using UnityEditor;
using UnityEngine;
using System.IO;

namespace Nomnom.UnityProjectPatcher.Attributes {
    public sealed class FolderPathAttribute: PropertyAttribute {
        public readonly bool getRelativePath;
        
        public FolderPathAttribute(bool getRelativePath = true) {
            this.getRelativePath = getRelativePath;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathAttributeDrawer: PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var folderPathAttribute = attribute as FolderPathAttribute;
            if (folderPathAttribute == null) {
                return;
            }
            
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUILayout.HelpBox("The FolderPath Attribute can only be attached to a string", MessageType.Error);
                return;
            }
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginHorizontal();
            property.stringValue = EditorGUILayout.TextField(label, property.stringValue);
            if(GUILayout.Button("Browse", GUILayout.Width(70))) {
                var path = EditorUtility.OpenFolderPanel("Select Folder", "", "");
                if (!string.IsNullOrEmpty(path)) {
                    if (folderPathAttribute.getRelativePath) {
                        property.stringValue = path.Substring(Application.dataPath.Length - "Assets".Length);
                    } else {
                        property.stringValue = path;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}