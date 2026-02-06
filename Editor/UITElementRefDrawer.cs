using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    /// <summary>
    /// Custom property drawer for all UITElementRef types.
    /// Supports two modes:
    /// - UseReference: Shows UIDocument field + element dropdown (manual assignment)
    /// - UseDocumentHere: Shows only element dropdown (auto-finds UIDocument on same GameObject)
    /// When Unity Localization is installed, also shows localization toggle and LocalizedString field.
    /// </summary>
    [CustomPropertyDrawer(typeof(UITElementRef), true)]
    public class UITElementRefDrawer : PropertyDrawer {
        private const float Spacing = 2f;
        private const float ModeButtonWidth = 20f;
        private const float LocalizeToggleWidth = 20f;

        private static GUIContent refModeIcon;
        private static GUIContent hereModeIcon;

        private static GUIContent RefModeIcon => refModeIcon ??= EditorGUIUtility.IconContent("d_Linked");
        private static GUIContent HereModeIcon => hereModeIcon ??= EditorGUIUtility.IconContent("d_Unlinked");

#if HAS_UNITY_LOCALIZATION
        private static GUIContent localizeIcon;

        private static GUIContent LocalizeIcon {
            get {
                if (localizeIcon == null) {
                    // Try Unity Localization package icon first, fallback to text
                    localizeIcon = EditorGUIUtility.IconContent("Localization Icon");
                    if (localizeIcon == null || localizeIcon.image == null) {
                        localizeIcon = EditorGUIUtility.IconContent("d_Preset.Context");
                    }
                    if (localizeIcon == null || localizeIcon.image == null) {
                        localizeIcon = new GUIContent("L");
                    }
                }
                return localizeIcon;
            }
        }
#endif

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var modeProp = property.FindPropertyRelative("mode");
            var documentProp = property.FindPropertyRelative("document");
            var elementNameProp = property.FindPropertyRelative("elementName");

            var mode = (UITReferenceMode)modeProp.enumValueIndex;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            // Label rect
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lineHeight);
            EditorGUI.LabelField(labelRect, label);

            // Content area (after label)
            float contentX = position.x + EditorGUIUtility.labelWidth;
            float contentWidth = position.width - EditorGUIUtility.labelWidth;

            // Mode toggle button (small icon button on the left)
            Rect modeButtonRect = new Rect(contentX, position.y, ModeButtonWidth, lineHeight);

            var modeIcon = mode == UITReferenceMode.UseReference ? RefModeIcon : HereModeIcon;
            var modeTooltip = mode == UITReferenceMode.UseReference
                ? "Using external UIDocument reference.\nClick to use UIDocument on same GameObject."
                : "Using UIDocument on same GameObject.\nClick to use external reference.";

            if (GUI.Button(modeButtonRect, new GUIContent(modeIcon.image, modeTooltip), EditorStyles.iconButton)) {
                int newMode = mode == UITReferenceMode.UseReference ? 1 : 0;
                modeProp.enumValueIndex = newMode;

                // When switching to UseDocumentHere, auto-populate UIDocument from same GameObject
                if (newMode == (int)UITReferenceMode.UseDocumentHere) {
                    AutoPopulateDocument(property.serializedObject, documentProp);
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            float buttonsWidth = ModeButtonWidth;

#if HAS_UNITY_LOCALIZATION
            // Localization toggle button (next to mode button)
            var useLocalizationProp = property.FindPropertyRelative("useLocalization");
            bool hasLocalization = useLocalizationProp != null;
            bool useLocalization = hasLocalization && useLocalizationProp.boolValue;

            if (hasLocalization) {
                Rect localizeButtonRect = new Rect(contentX + ModeButtonWidth + Spacing, position.y, LocalizeToggleWidth, lineHeight);
                buttonsWidth += LocalizeToggleWidth + Spacing;

                var localizeTooltip = useLocalization
                    ? "Localization enabled.\nClick to disable."
                    : "Localization disabled.\nClick to enable for dynamic text with arguments.";

                var prevColor = GUI.backgroundColor;
                if (useLocalization) {
                    GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);
                }

                var icon = LocalizeIcon;
                var buttonContent = icon.image != null
                    ? new GUIContent(icon.image, localizeTooltip)
                    : new GUIContent("L", localizeTooltip);

                if (GUI.Button(localizeButtonRect, buttonContent, EditorStyles.iconButton)) {
                    useLocalizationProp.boolValue = !useLocalizationProp.boolValue;
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUI.backgroundColor = prevColor;
            }
#endif

            // Remaining content area
            float remainingX = contentX + buttonsWidth + Spacing;
            float remainingWidth = contentWidth - buttonsWidth - Spacing;

            // Get target type
            Type targetType = GetTargetElementType(fieldInfo.FieldType);

            if (mode == UITReferenceMode.UseReference) {
                // Show document field + dropdown
                float halfWidth = (remainingWidth - Spacing) / 2f;
                Rect documentRect = new Rect(remainingX, position.y, halfWidth, lineHeight);
                Rect dropdownRect = new Rect(remainingX + halfWidth + Spacing, position.y, halfWidth, lineHeight);

                EditorGUI.PropertyField(documentRect, documentProp, GUIContent.none);
                var doc = documentProp.objectReferenceValue as UIDocument;

                DrawElementDropdown(dropdownRect, elementNameProp, doc, targetType);
            }
            else {
                // UseDocumentHere - auto-populate if needed, show only dropdown
                Rect dropdownRect = new Rect(remainingX, position.y, remainingWidth, lineHeight);

                // Ensure document is populated from same GameObject
                var doc = documentProp.objectReferenceValue as UIDocument;
                if (doc == null) {
                    doc = AutoPopulateDocument(property.serializedObject, documentProp);
                }

                if (doc == null) {
                    // Show error state - no UIDocument found
                    var prevColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.Popup(dropdownRect, 0, new[] { "(no UIDocument on GameObject)" });
                    EditorGUI.EndDisabledGroup();
                    GUI.backgroundColor = prevColor;
                }
                else {
                    DrawElementDropdown(dropdownRect, elementNameProp, doc, targetType);
                }
            }

#if HAS_UNITY_LOCALIZATION
            // Draw LocalizedString field on second line only when enabled
            if (hasLocalization && useLocalization) {
                DrawLocalizedStringField(position, property, lineHeight, contentX, contentWidth);
            }
#endif

            EditorGUI.EndProperty();
        }

#if HAS_UNITY_LOCALIZATION
        private void DrawLocalizedStringField(Rect position, SerializedProperty property, float lineHeight, float contentX, float contentWidth) {
            float y = position.y + lineHeight + Spacing;

            // Try to find the localized string property (different names for different refs)
            var localizedStringProp = property.FindPropertyRelative("localizedString")
                ?? property.FindPropertyRelative("localizedPlaceholder")
                ?? property.FindPropertyRelative("localizedChoices");

            if (localizedStringProp != null) {
                float propHeight = EditorGUI.GetPropertyHeight(localizedStringProp, true);
                Rect fieldRect = new Rect(contentX, y, contentWidth, propHeight);
                EditorGUI.PropertyField(fieldRect, localizedStringProp, GUIContent.none, true);
            }
        }
#endif

        private UIDocument AutoPopulateDocument(SerializedObject serializedObject, SerializedProperty documentProp) {
            var targetObject = serializedObject.targetObject as Component;
            if (targetObject == null) return null;

            var doc = targetObject.GetComponent<UIDocument>();
            if (doc != null && documentProp.objectReferenceValue != doc) {
                documentProp.objectReferenceValue = doc;
                serializedObject.ApplyModifiedProperties();
            }
            return doc;
        }

        private void DrawElementDropdown(Rect rect, SerializedProperty elementNameProp, UIDocument doc, Type targetType) {
            if (doc == null) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Popup(rect, 0, new[] { "(assign UIDocument)" });
                EditorGUI.EndDisabledGroup();
            }
            else if (doc.rootVisualElement == null) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Popup(rect, 0, new[] { "(open Prefab or enter Play mode)" });
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
                    else {
                        // Element name set but not found - show warning
                        elementNames.Insert(1, $"? {currentName} (not found)");
                        selectedIndex = 1;
                    }
                }

                EditorGUI.BeginChangeCheck();
                int newIndex = EditorGUI.Popup(rect, selectedIndex, elementNames.ToArray());
                if (EditorGUI.EndChangeCheck()) {
                    if (newIndex == 0) {
                        elementNameProp.stringValue = "";
                    }
                    else if (elementNames[newIndex].StartsWith("?")) {
                        // Keep the old value if selecting the "not found" entry
                    }
                    else {
                        elementNameProp.stringValue = elementNames[newIndex];
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = EditorGUIUtility.singleLineHeight;

#if HAS_UNITY_LOCALIZATION
            // Add height for LocalizedString field only when localization is enabled
            var useLocalizationProp = property.FindPropertyRelative("useLocalization");
            if (useLocalizationProp != null && useLocalizationProp.boolValue) {
                // Find the localized string property and get its actual height (handles foldout)
                var localizedStringProp = property.FindPropertyRelative("localizedString")
                    ?? property.FindPropertyRelative("localizedPlaceholder")
                    ?? property.FindPropertyRelative("localizedChoices");

                if (localizedStringProp != null) {
                    height += Spacing + EditorGUI.GetPropertyHeight(localizedStringProp, true);
                }
            }
#endif

            return height;
        }

        private Type GetTargetElementType(Type refType) {
            Type current = refType;
            while (current != null) {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(UITElementRef<>)) {
                    return current.GetGenericArguments()[0];
                }
                current = current.BaseType;
            }

            try {
                var instance = Activator.CreateInstance(refType) as UITElementRef;
                if (instance != null) {
                    return instance.TargetType;
                }
            }
            catch {
                // Ignore activation errors
            }

            return typeof(VisualElement);
        }

        private void CollectElementsOfType(VisualElement root, Type targetType, List<string> names) {
            foreach (var child in root.Children()) {
                if (targetType.IsAssignableFrom(child.GetType())) {
                    string name = child.name;
                    if (!string.IsNullOrEmpty(name) && !names.Contains(name)) {
                        names.Add(name);
                    }
                }

                if (child.childCount > 0) {
                    CollectElementsOfType(child, targetType, names);
                }
            }
        }
    }
}
