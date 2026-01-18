using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class SliderRef : UITElementRef<Slider> {
        private EventCallback<ChangeEvent<float>> registeredCallback;

        public Slider Slider => Element;

        public float Value {
            get => Element?.value ?? 0f;
            set { var el = Element; if (el != null) el.value = value; }
        }

        /// <summary>
        /// Registers a value changed callback. Only one callback supported - calling again replaces previous.
        /// Access Element directly for multiple listeners.
        /// </summary>
        public void OnValueChanged(Action<float> callback) {
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

        public float LowValue {
            get => Element?.lowValue ?? 0f;
            set { var el = Element; if (el != null) el.lowValue = value; }
        }

        public float HighValue {
            get => Element?.highValue ?? 1f;
            set { var el = Element; if (el != null) el.highValue = value; }
        }

        public string Label {
            get => Element?.label;
            set { var el = Element; if (el != null) el.label = value; }
        }
    }
}
