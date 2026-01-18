using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class LabelRef : UITElementRef<Label> {
        public Label Label => Element;

        public string Text {
            get => Element?.text;
            set { var el = Element; if (el != null) el.text = value; }
        }
    }
}
