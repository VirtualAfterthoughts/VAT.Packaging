using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public class ContentCreationWizard : EditorWindow {
        public Address _address = Address.EMPTY;
        public Package _package = null;
        public ContentType _type = ContentType.SPAWNABLE_CONTENT;
        public string _title = "My Content";
        public Object _mainAsset = null;

        public static void Initialize(Package package) {
            ContentCreationWizard window = (ContentCreationWizard)EditorWindow.GetWindow(typeof(ContentCreationWizard), true, "Content Creator");
            window._package = package;
            window.Show();
        }

        public void OnGUI() {
            // Header
            EditorGUILayout.LabelField("Content Settings", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Draw options
            EditorGUILayout.TextField("Address", _address);
            _package = EditorGUILayout.ObjectField("Package", _package, typeof(Package), false) as Package;
            _type = (ContentType)EditorGUILayout.EnumPopup("Content Type", _type);
            _title = EditorGUILayout.TextField("Content Title", _title);
            _mainAsset = EditorGUILayout.ObjectField("Main Asset", _mainAsset, typeof(Object), false);

            // Recreate address
            _address = Address.BuildAddress(_package.Author, _package.Title, _title);

            // Verify content creation
            if (!Internal_ValidateContentSettings())
                return;

            // Spacing
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Allow content creation
            if (GUILayout.Button("Create Content", GUILayout.Width(200))) {
                Internal_CreateContent();
                Close();
            }
        }
        
        private bool Internal_ValidateContentSettings() {
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Errors", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            bool isValid = true;

            var errorStyle = new GUIStyle(EditorStyles.boldLabel);
            errorStyle.normal.textColor = Color.red;

            if (_mainAsset == null) {
                EditorGUILayout.LabelField("Missing Main Asset!", errorStyle);
                isValid = false;
            }
            else {
                switch (_type) {
                    default:
                    case ContentType.SPAWNABLE_CONTENT:
                        if (_mainAsset is not GameObject) {
                            EditorGUILayout.LabelField("Main Asset is not a GameObject!", errorStyle);
                            isValid = false;
                        }
                        break;
                    case ContentType.LEVEL_CONTENT:
                        if (_mainAsset is not SceneAsset) {
                            EditorGUILayout.LabelField("Main Asset is not a Scene!", errorStyle);
                            isValid = false;
                        }
                        break;
                    case ContentType.AVATAR_CONTENT:
                        if (_mainAsset is not GameObject) {
                            EditorGUILayout.LabelField("Main Asset is not a GameObject!", errorStyle);
                            isValid = false;
                        }
                        break;
                    case ContentType.SCRIPTABLE_OBJECT_CONTENT:
                        if (_mainAsset is not ScriptableObject) {
                            EditorGUILayout.LabelField("Main Asset is not a Scriptable Object!", errorStyle);
                            isValid = false;
                        }
                        break;
                    case ContentType.AUDIO_CLIP_CONTENT:
                        if (_mainAsset is not AudioClip) {
                            EditorGUILayout.LabelField("Main Asset is not an Audio Clip!");
                            isValid = false;
                        }
                        break;
                }
            }

            if (isValid) {
                EditorGUILayout.LabelField("No issues found!");
            }

            return isValid;
        }

        private void Internal_CreateContent() {
            Content content = Content.Create(_type.GetContentType());
            content.Title = _title;
            content.Package = _package;
            content.Address = _address;
            content.SetAsset(_mainAsset);
            
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_package));
            var filePath = $"{path}/_{_title}.asset";

            if (AssetDatabase.LoadAllAssetsAtPath(filePath).Length > 0)
                AssetDatabase.DeleteAsset(filePath);

            AssetDatabase.CreateAsset(content, filePath);
            _package.Contents.Add(content);

            _package.OnValidate();

            content.ForceSerialize();
            _package.ForceSerialize();

            AssetPackager.EditorForceRefresh();

            // Show file in editor
            Selection.SetActiveObjectWithContext(content, content);

        }
    }
}
