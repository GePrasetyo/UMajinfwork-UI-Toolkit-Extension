using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majingari.UI {
    [CustomPropertyDrawer(typeof(UITElementModel), true)]
    public class UITModelDrawer : PropertyDrawer {
        private List<FieldInfo> fieldsInstance = new List<FieldInfo>();
        private List<string> elementNames = new List<string>();
        
        private UITElementModel model;
        private UIDocument uiDocument;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //EditorGUI.PropertyField(position, property, label, true);
            model = fieldInfo.GetValue(property.serializedObject.targetObject) as UITElementModel;
            var ye = property.FindPropertyRelative("uiDocument");
            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, ye);

            GetAllFields(model);

            if (fieldsInstance.Count == 0) {
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            for (int x = 0; x < fieldsInstance.Count; x++) {
                int selectedIndex = 0;
                //fieldRect.xMin += 15f;
                fieldRect.y += fieldRect.height + EditorGUIUtility.standardVerticalSpacing;

                if (elementNames.Count == 0) {
                    EditorGUI.LabelField(fieldRect, $"{fieldsInstance[x].Name} (element not found in the UI Document)");
                    continue;
                }

                if (fieldsInstance[x].GetValue(model) != null) {
                    selectedIndex = elementNames.IndexOf(fieldsInstance[x].GetValue(model) as string);

                    if (selectedIndex < 0) {
                        SetupValue(0, x);
                    }
                }
                else {
                    SetupValue(0, x);
                }

                selectedIndex = EditorGUI.Popup(fieldRect, fieldsInstance[x].Name, selectedIndex, elementNames.ToArray());
                if (EditorGUI.EndChangeCheck()) {
                    if (selectedIndex >= 0) {
                        SetupValue(selectedIndex, x);
                    }
                }
            }

            void SetupValue(int _index, int x) {
                SerializedProperty propField = property.FindPropertyRelative(fieldsInstance[x].Name);
                propField.stringValue = elementNames[_index];
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property) + (fieldsInstance.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
        }

        private void GetAllFields(UITElementModel UIT) {
            if (UIT.uiDocument == null || UIT.uiDocument.rootVisualElement == null) {
                Reset();
                return;
            }
            if (UIT.uiDocument != uiDocument) {
                Reset();
            }

            uiDocument = UIT.uiDocument;


            List<FieldInfo> _fieldsInstance = new List<FieldInfo>();
            var fields = UIT.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var b in fields) {
                if (!b.FieldType.IsEquivalentTo(typeof(string)))
                    continue;

                _fieldsInstance.Add(b);
            }

            CheckDocument(uiDocument, out List<string> listItems);
            elementNames = listItems;
            fieldsInstance = _fieldsInstance;
        }

        private void CheckDocument(UIDocument document, out List<string> listItems) {
            listItems = new List<string>();
            GetAllElements(document.rootVisualElement, ref listItems);
        }

        private void GetAllElements(VisualElement root, ref List<string> listItems) {
            for (int x = 0; x < root.childCount; x++) {
                if (root.ElementAt(x).childCount > 0) {
                    GetAllElements(root.ElementAt(x), ref listItems);
                }

                if (root.ElementAt(x).contentContainer.GetType().IsSubclassOf(typeof(BindableElement))) {
                    listItems.Add(root.ElementAt(x).contentContainer.name != ""? root.ElementAt(x).contentContainer.name: $"{root.ElementAt(x).contentContainer.GetType().Name} (no name)");
                }
            }
        }

        private void Reset() {
            elementNames.Clear();
            fieldsInstance.Clear();
        }
    }
}