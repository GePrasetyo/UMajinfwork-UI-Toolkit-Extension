using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Majingari.UI {
    [CustomPropertyDrawer(typeof(UITElementReference), true)]
    public class UITElementDrawer : PropertyDrawer {
        private List<Type> elements = new List<Type>();
        private List<FieldInfo> fieldsInstance = new List<FieldInfo>();
        private Dictionary<Type, string[]> typeAndElementNames = new Dictionary<Type, string[]>();
        private Dictionary<Type, BindableElement[]> typeAndElements = new Dictionary<Type, BindableElement[]>();
        private UITElementReference reference;
        private UIDocument uiDocument;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            reference = fieldInfo.GetValue(property.serializedObject.targetObject) as UITElementReference;

            GetAllFields(reference);
            if (fieldsInstance.Count == 0) {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            if (!property.isExpanded) {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
            position.y += EditorGUI.GetPropertyHeight(property);
            position.height = EditorGUIUtility.singleLineHeight;

            for (int x = 0; x < fieldsInstance.Count; x++) {
                int selectedIndex = 0;
                Rect dropDownRect = position;
                dropDownRect.height = EditorGUIUtility.singleLineHeight;
                dropDownRect.xMin += 15f;
                position.y += dropDownRect.height + EditorGUIUtility.standardVerticalSpacing;

                if(typeAndElements[fieldsInstance[x].FieldType].Length == 0) {
                    EditorGUI.LabelField(dropDownRect, $"{fieldsInstance[x].Name} (not found)");
                    continue;
                }

                var aaaaaa = fieldsInstance[x].GetValue(reference);
                if (aaaaaa != null) {
                    selectedIndex = Array.IndexOf(typeAndElementNames[fieldsInstance[x].FieldType], (fieldsInstance[x].GetValue(reference) as BindableElement).name);
                    
                    if(selectedIndex < 0) {
                        SetupValue(0, x);
                    }
                }
                else {
                    SetupValue(0, x);
                }

                selectedIndex = EditorGUI.Popup(dropDownRect, fieldsInstance[x].Name, selectedIndex, typeAndElementNames[fieldsInstance[x].FieldType]);
                if (EditorGUI.EndChangeCheck()) {
                    if (selectedIndex >= 0) {
                        SetupValue(selectedIndex, x);
                    }
                }
            }

            void SetupValue(int _index, int x) {
                var tttt = typeAndElements[fieldsInstance[x].FieldType][_index];
                SerializedProperty aaa = property.FindPropertyRelative(fieldsInstance[x].Name);
                aaa.managedReferenceValue = tttt;
                //fieldsInstance[x].SetValue(reference, tttt);
                //property.serializedObject.targetObject.
                bool lalalal = property.serializedObject.ApplyModifiedProperties();
                Debug.LogError(lalalal);
            }

            //SerializedProperty aaa = property.serializedObject.FindProperty(fieldInfo.Name);
            //aaa.objectReferenceValue = reference;
            //Debug.LogError(aaa);
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.isExpanded) {
                return EditorGUI.GetPropertyHeight(property) + (fieldsInstance.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
            }
            else {
                return EditorGUI.GetPropertyHeight(property);
            }
        }

        private void GetAllFields(UITElementReference UIT) {
            if (UIT.uiDocument == null || UIT.uiDocument.rootVisualElement == null) {
                Reset();
                return;
            }
            if(UIT.uiDocument != uiDocument) {
                Reset();
            }

            uiDocument = UIT.uiDocument;


            List<Type> _elements = new List<Type>();
            List<FieldInfo> _fieldsInstance = new List<FieldInfo>();
            var fields = UIT.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var b in fields) {
                if (!b.FieldType.IsSubclassOf(typeof(BindableElement)))
                    continue;

                _fieldsInstance.Add(b);

                if (!_elements.Contains(b.FieldType)) {
                    CheckDocument(uiDocument, b.FieldType, out List<BindableElement> elements,  out List<string> listItems);
                    typeAndElements[b.FieldType] = elements.ToArray();
                    typeAndElementNames[b.FieldType] = listItems.ToArray();
                }
            }

            elements = _elements;
            fieldsInstance = _fieldsInstance;
        }

        private void CheckDocument(UIDocument document, Type type, out List<BindableElement> elements, out List<string> listItems) {
            listItems = new List<string>();
            elements = new List<BindableElement> ();
            GetAllElements(document.rootVisualElement, type, ref elements, ref listItems);
        }

        private void GetAllElements(VisualElement root, Type type, ref List<BindableElement> elements, ref List<string> listItems) {
            for (int x = 0; x < root.childCount; x++) {
                if (root.ElementAt(x).childCount > 0) {
                    GetAllElements(root.ElementAt(x), type, ref elements, ref listItems);
                }

                if (root.ElementAt(x).contentContainer.GetType() == type) {
                    elements.Add(root.ElementAt(x).contentContainer as BindableElement);
                    listItems.Add(root.ElementAt(x).contentContainer.name);
                }
            }
        }

        private void Reset() {
            elements.Clear();
            fieldsInstance.Clear();
            typeAndElementNames.Clear();
            typeAndElements.Clear();
        }
    }
}