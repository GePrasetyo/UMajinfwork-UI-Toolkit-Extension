using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class VisualElementRef : UITElementRef<VisualElement> {
        public VisualElement VisualElement => Element;

        public void Show() {
            var el = Element;
            if (el != null) el.style.display = DisplayStyle.Flex;
        }

        public void Hide() {
            var el = Element;
            if (el != null) el.style.display = DisplayStyle.None;
        }

        public bool Visible {
            get => Element?.style.display != DisplayStyle.None;
            set {
                var el = Element;
                if (el != null) el.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public void AddClass(string className) => Element?.AddToClassList(className);
        public void RemoveClass(string className) => Element?.RemoveFromClassList(className);
        public void ToggleClass(string className) => Element?.ToggleInClassList(className);
    }
}
