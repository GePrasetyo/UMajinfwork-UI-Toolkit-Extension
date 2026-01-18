using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    /// <summary>
    /// Custom property drawer for all UITElementRef types.
    /// Shows UIDocument field and a filtered dropdown of elements matching the target type.
    /// </summary>
    [CustomPropertyDrawer(typeof(UITElementRef), true)]
    public class UITElementRefDrawer : PropertyDrawer {
        private const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var documentProp = property.FindPropertyRelative("document");
            var elementNameProp = property.FindPropertyRelative("elementName");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lineHeight);
            Rect contentRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, lineHeight);

            // Draw the label
            EditorGUI.LabelField(labelRect, label);

            // Calculate rects for document and dropdown
            float halfWidth = (contentRect.width - Spacing) / 2f;
            Rect documentRect = new Rect(contentRect.x, contentRect.y, halfWidth, lineHeight);
            Rect dropdownRect = new Rect(contentRect.x + halfWidth + Spacing, contentRect.y, halfWidth, lineHeight);

            // Draw UIDocument field
            EditorGUI.PropertyField(documentRect, documentProp, GUIContent.none);

            // Get the target type from the field's actual type
            Type targetType = GetTargetElementType(fieldInfo.FieldType);
            UIDocument doc = documentProp.objectReferenceValue as UIDocument;

            if (doc == null) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Popup(dropdownRect, 0, new[] { "(assign UIDocument)" });
                EditorGUI.EndDisabledGroup();
            }
            else if (doc.rootVisualElement == null) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Popup(dropdownRect, 0, new[] { "(enter Play mode)" });
                EditorGUI.EndDisabledGroup();
            }
            else {
                // Get all elements of the target type
                List<string> elementNames = new List<string> { "(none)" };
                CollectElementsOfType(doc.rootVisualElement, targetType, elementNames);

                // Find current selection index
                string currentName = elementNameProp.stringValue;
                int selectedIndex = 0;
                if (!string.IsNullOrEmpty(currentName)) {
                    int foundIndex = elementNames.IndexOf(currentName);
                    if (foundIndex >= 0) {
                        selectedIndex = foundIndex;
                    }
                }

                // Draw dropdown
                EditorGUI.BeginChangeCheck();
                int newIndex = EditorGUI.Popup(dropdownRect, selectedIndex, elementNames.ToArray());
                if (EditorGUI.EndChangeCheck()) {
                    if (newIndex == 0) {
                        elementNameProp.stringValue = "";
                    }
                    else {
                        elementNameProp.stringValue = elementNames[newIndex];
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Gets the target VisualElement type from the UITElementRef generic type.
        /// </summary>
        private Type GetTargetElementType(Type refType) {
            // Check if it's a generic UITElementRef<T>
            Type current = refType;
            while (current != null) {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(UITElementRef<>)) {
                    return current.GetGenericArguments()[0];
                }
                current = current.BaseType;
            }

            // Fallback: try to get TargetType from the type itself via reflection
            // This handles cases where we have a concrete class like ButtonRef
            try {
                var instance = Activator.CreateInstance(refType) as UITElementRef;
                if (instance != null) {
                    return instance.TargetType;
                }
            }
            catch {
                // Ignore activation errors
            }

            // Default fallback to VisualElement
            return typeof(VisualElement);
        }

        /// <summary>
        /// Recursively collects all elements of the specified type.
        /// </summary>
        private void CollectElementsOfType(VisualElement root, Type targetType, List<string> names) {
            foreach (var child in root.Children()) {
                // Check if this element matches the target type
                if (targetType.IsAssignableFrom(child.GetType())) {
                    string name = child.name;
                    if (!string.IsNullOrEmpty(name) && !names.Contains(name)) {
                        names.Add(name);
                    }
                }

                // Recurse into children
                if (child.childCount > 0) {
                    CollectElementsOfType(child, targetType, names);
                }
            }
        }
    }
}
