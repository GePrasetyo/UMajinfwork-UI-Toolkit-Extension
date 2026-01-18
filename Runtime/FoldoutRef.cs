using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class FoldoutRef : UITElementRef<Foldout> {
        public Foldout Foldout => Element;

        public bool Value {
            get => Element?.value ?? false;
            set { var el = Element; if (el != null) el.value = value; }
        }

        public string Text {
            get => Element?.text;
            set { var el = Element; if (el != null) el.text = value; }
        }
    }
}
