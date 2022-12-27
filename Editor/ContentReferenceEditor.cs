using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

namespace VAT.Packaging.Editor
{
    /// <summary>
    /// Draws a reference to a content asset, which can be toggled between showing the address or the asset itself.
    /// </summary>
    [CustomPropertyDrawer(typeof(ContentReference), true)]
    public class ContentReferenceEditor : PropertyDrawer {
        public Content selectedContent = null;
        public bool isDrawingAddress = false;

        private bool _hasRan = false;
        private bool _hadValue = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var addressProperty = property.FindPropertyRelative("_address").FindPropertyRelative("_id");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;
            if (isDrawingAddress) {
                string result = EditorGUI.TextField(position, label, addressProperty.stringValue);
                addressProperty.stringValue = result;
            }
            else {
                selectedContent = EditorGUI.ObjectField(position, label, selectedContent, typeof(Content), false) as Content;
                if (selectedContent)
                    addressProperty.stringValue = selectedContent.Address.ID;
                else if (!_hasRan && AssetPackager.IsReady) {
                    AssetPackager.Instance.TryGetContent(addressProperty.stringValue, out selectedContent);
                    _hasRan = true;
                }
                else if (_hadValue) {
                    addressProperty.stringValue = Address.EMPTY;
                }

                _hadValue = selectedContent != null;
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(addressProperty);
            position.x -= position.width;
            isDrawingAddress = EditorGUI.Toggle(position, isDrawingAddress);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
