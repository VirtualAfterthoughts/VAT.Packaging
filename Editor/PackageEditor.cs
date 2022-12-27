using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(Package))]
    [CanEditMultipleObjects]
    public class PackageEditor : UnityEditor.Editor {
        private SerializedProperty _title;
        private SerializedProperty _description;
        private SerializedProperty _author;
        private SerializedProperty _internal;
        private SerializedProperty _contents;

        private void OnEnable() {
            _title = serializedObject.FindProperty("_title");
            _description = serializedObject.FindProperty("_description");
            _author = serializedObject.FindProperty("_author");
            _internal = serializedObject.FindProperty("_internal");
            _contents = serializedObject.FindProperty("_contents");

            var package = serializedObject.targetObject as Package;
            package.OnValidate();
            package.ForceSerialize();
        }

        public override void OnInspectorGUI() {
            var package = serializedObject.targetObject as Package;

            serializedObject.Update();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_title);
            EditorGUILayout.PropertyField(_description);
            EditorGUILayout.PropertyField(_author);
            EditorGUILayout.PropertyField(_internal);

            // Draw content list and content buttons
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_contents);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Add Content", GUILayout.Width(120))) {
                ContentCreationWizard.Initialize(package);
            }

            // Space and header
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Exporting Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));
            GUILayout.Space(20);

            // Draw build buttons
            if (GUILayout.Button("Pack for PC", GUILayout.Width(120))) {
                ExternalAssetPacker.PackPackage(package, BuildTarget.StandaloneWindows64);
            }

            // Draw exporting buttons
            if (GUILayout.Button("Export as JSON", GUILayout.Width(120))) {
                PackageTools.ExportPackage(package);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
