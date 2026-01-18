using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class ButtonRef : UITElementRef<Button> {
        public Button Button => Element;

        public void OnClick(Action callback) {
            var el = Element;
            if (el != null) el.clicked += callback;
        }

        public void RemoveOnClick(Action callback) {
            var el = Element;
            if (el != null) el.clicked -= callback;
        }

        public void SetEnabled(bool enabled) {
            var el = Element;
            if (el != null) el.SetEnabled(enabled);
        }

        public string Text {
            get => Element?.text;
            set { var el = Element; if (el != null) el.text = value; }
        }
    }
}
