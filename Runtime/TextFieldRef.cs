using System;
using UnityEngine;
using UnityEngine.UIElements;
#if HAS_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Majinfwork.UI {
    /// <summary>
    /// Reference wrapper for UI Toolkit TextField element.
    /// Supports optional localization for placeholder text with arguments.
    /// For static localized placeholder, use UXML binding or set in code.
    /// </summary>
    [Serializable]
    public class TextFieldRef : UITElementRef<TextField> {
        private EventCallback<ChangeEvent<string>> registeredCallback;

        public TextField TextField => Element;

#if HAS_UNITY_LOCALIZATION
        [Tooltip("Enable for dynamic localized placeholder with arguments.")]
        [SerializeField] private bool useLocalization;
        [SerializeField] private LocalizedString localizedPlaceholder;

        private bool isInitialized;

        /// <summary>
        /// Set arguments for dynamic localized placeholder text.
        /// </summary>
        public object[] PlaceholderArguments {
            set {
                if (localizedPlaceholder != null) {
                    localizedPlaceholder.Arguments = value;
                }
            }
        }

        /// <summary>
        /// Initialize localization subscription. Call in OnEnable().
        /// </summary>
        public void InitializeLocalization() {
            if (!useLocalization || isInitialized) return;
            if (localizedPlaceholder == null || localizedPlaceholder.IsEmpty) return;

            localizedPlaceholder.StringChanged += OnLocalizedPlaceholderChanged;
            isInitialized = true;
        }

        /// <summary>
        /// Cleanup localization subscription. Call in OnDisable().
        /// </summary>
        public void DisposeLocalization() {
            if (!isInitialized) return;

            localizedPlaceholder.StringChanged -= OnLocalizedPlaceholderChanged;
            isInitialized = false;
        }

        private void OnLocalizedPlaceholderChanged(string value) {
            Placeholder = value;
        }

        public bool UseLocalization => useLocalization;
#endif

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
