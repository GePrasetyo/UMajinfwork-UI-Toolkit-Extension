using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class SliderIntRef : UITElementRef<SliderInt> {
        private EventCallback<ChangeEvent<int>> registeredCallback;

        public SliderInt Slider => Element;

        public int Value {
            get => Element?.value ?? 0;
            set { var el = Element; if (el != null) el.value = value; }
        }

        /// <summary>
        /// Registers a value changed callback. Only one callback supported - calling again replaces previous.
        /// Access Element directly for multiple listeners.
        /// </summary>
        public void OnValueChanged(Action<int> callback) {
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

        public int LowValue {
            get => Element?.lowValue ?? 0;
            set { var el = Element; if (el != null) el.lowValue = value; }
        }

        public int HighValue {
            get => Element?.highValue ?? 100;
            set { var el = Element; if (el != null) el.highValue = value; }
        }

        public string Label {
            get => Element?.label;
            set { var el = Element; if (el != null) el.label = value; }
        }
    }
}
