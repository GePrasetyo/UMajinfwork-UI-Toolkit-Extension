using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class ToggleRef : UITElementRef<Toggle> {
        private EventCallback<ChangeEvent<bool>> registeredCallback;

        public Toggle Toggle => Element;

        public bool Value {
            get => Element?.value ?? false;
            set { var el = Element; if (el != null) el.value = value; }
        }

        /// <summary>
        /// Registers a value changed callback. Only one callback supported - calling again replaces previous.
        /// Access Element directly for multiple listeners.
        /// </summary>
        public void OnValueChanged(Action<bool> callback) {
            var el = Element;
            if (el == null) return;

            if (registeredCallback != null) {
                el.UnregisterValueChangedCallback(registeredCallback);
            }

            registeredCallback = evt => callback?.Invoke(evt.newValue);
            el.RegisterValueChangedCallback(registeredCallback);
        }

        public void RemoveOnValueChanged() {
            var el = Element;
            if (el != null && registeredCallback != null) {
                el.UnregisterValueChangedCallback(registeredCallback);
                registeredCallback = null;
            }
        }

        public string Text {
            get => Element?.text;
            set { var el = Element; if (el != null) el.text = value; }
        }
    }
}
