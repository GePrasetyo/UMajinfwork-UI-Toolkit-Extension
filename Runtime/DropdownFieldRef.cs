using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class DropdownFieldRef : UITElementRef<DropdownField> {
        private EventCallback<ChangeEvent<string>> registeredCallback;

        public DropdownField Dropdown => Element;

        public string Value {
            get => Element?.value;
            set { var el = Element; if (el != null) el.value = value; }
        }

        public int Index {
            get => Element?.index ?? -1;
            set { var el = Element; if (el != null) el.index = value; }
        }

        /// <summary>
        /// Registers a value changed callback. Only one callback supported - calling again replaces previous.
        /// Access Element directly for multiple listeners.
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

        public void SetChoices(List<string> choices) {
            var el = Element;
            if (el != null) el.choices = choices;
        }
    }
}
