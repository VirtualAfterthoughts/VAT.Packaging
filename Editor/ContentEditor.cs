using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(Content), true)]
    [CanEditMultipleObjects]
    public class ContentEditor : UnityEditor.Editor {
        private SerializedProperty _title;
        private SerializedProperty _description;
        private SerializedProperty _package;

        protected virtual void OnEnable() {
            _title = serializedObject.FindProperty("_title");
            _description = serializedObject.FindProperty("_description");
            _package = serializedObject.FindProperty("_package");

            var content = serializedObject.targetObject as Content;
            content.OnValidate();
            content.ForceSerialize();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_title);
            EditorGUILayout.PropertyField(_description);

            // Draw package without letting it be modified
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_package);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(GameObjectContent), true)]
    [CanEditMultipleObjects]
    public class GameObjectContentEditor : ContentEditor {
        private SerializedProperty _mainGameObject;

        protected override void OnEnable() {
            base.OnEnable();

            _mainGameObject = serializedObject.FindProperty("_mainGameObject");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_mainGameObject);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(LevelContent), true)]
    [CanEditMultipleObjects]
    public class LevelContentEditor : ContentEditor
    {
        public override void OnInspectorGUI()
        {
            var levelContent = serializedObject.targetObject as LevelContent;

            base.OnInspectorGUI();

            levelContent.ForceAsset(EditorGUILayout.ObjectField("Main Scene", levelContent.MainScene.EditorAsset, typeof(SceneAsset), false) as SceneAsset);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
