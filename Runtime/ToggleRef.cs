using System;
using UnityEngine;
using UnityEngine.UIElements;
#if HAS_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Majinfwork.UI {
    /// <summary>
    /// Reference wrapper for UI Toolkit Toggle element.
    /// Supports optional localization for label text with arguments.
    /// For static localized text, use UXML binding: text="@TableName/Key"
    /// </summary>
    [Serializable]
    public class ToggleRef : UITElementRef<Toggle> {
        private EventCallback<ChangeEvent<bool>> registeredCallback;

        public Toggle Toggle => Element;

#if HAS_UNITY_LOCALIZATION
        [Tooltip("Enable for dynamic localized text with arguments. For static text, use UXML @Table/Key syntax instead.")]
        [SerializeField] private bool useLocalization;
        [SerializeField] private LocalizedString localizedString;

        private bool isInitialized;

        /// <summary>
        /// Set arguments for dynamic localized text.
        /// </summary>
        public object[] Arguments {
            set {
                if (localizedString != null) {
                    localizedString.Arguments = value;
                }
            }
        }

        /// <summary>
        /// Initialize localization subscription. Call in OnEnable().
        /// </summary>
        public void InitializeLocalization() {
            if (!useLocalization || isInitialized) return;
            if (localizedString == null || localizedString.IsEmpty) return;

            localizedString.StringChanged += OnLocalizedStringChanged;
            isInitialized = true;
        }

        /// <summary>
        /// Cleanup localization subscription. Call in OnDisable().
        /// </summary>
        public void DisposeLocalization() {
            if (!isInitialized) return;

            localizedString.StringChanged -= OnLocalizedStringChanged;
            isInitialized = false;
        }

        private void OnLocalizedStringChanged(string value) {
            Text = value;
        }

        public bool UseLocalization => useLocalization;
#endif

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
