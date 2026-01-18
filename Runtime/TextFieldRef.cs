using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class TextFieldRef : UITElementRef<TextField> {
        private EventCallback<ChangeEvent<string>> registeredCallback;

        public TextField TextField => Element;

        public string Value {
            get => Element?.value;
            set { var el = Element; if (el != null) el.value = value; }
        }

        /// <summary>
        /// Registers a value changed callback that fires when the user changes the input.
        /// Only one callback is supported - calling again replaces the previous one.
        /// Use RemoveOnValueChanged() to unregister, or access Element directly for multiple listeners.
        /// </summary>
        public void OnValueChanged(Action<string> callback) {
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

        public string Placeholder {
            set { var el = Element; if (el != null) el.textEdition.placeholder = value; }
        }

        public bool IsReadOnly {
            get => Element?.isReadOnly ?? false;
            set { var el = Element; if (el != null) el.isReadOnly = value; }
        }
    }
}
