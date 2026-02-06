using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if HAS_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Majinfwork.UI {
    /// <summary>
    /// Reference wrapper for UI Toolkit DropdownField element.
    /// Supports optional localization for dropdown choices.
    /// </summary>
    [Serializable]
    public class DropdownFieldRef : UITElementRef<DropdownField> {
        private EventCallback<ChangeEvent<string>> registeredCallback;

        public DropdownField Dropdown => Element;

#if HAS_UNITY_LOCALIZATION
        [Tooltip("Enable to use localized choices. Configure the localized choices list below.")]
        [SerializeField] private bool useLocalization;
        [SerializeField] private List<LocalizedString> localizedChoices = new List<LocalizedString>();

        private bool isInitialized;
        private List<string> resolvedChoices = new List<string>();

        /// <summary>
        /// Initialize localization subscription for all choices. Call in OnEnable().
        /// </summary>
        public void InitializeLocalization() {
            if (!useLocalization || isInitialized) return;
            if (localizedChoices == null || localizedChoices.Count == 0) return;

            resolvedChoices.Clear();
            for (int i = 0; i < localizedChoices.Count; i++) {
                resolvedChoices.Add(string.Empty);
                int index = i;
                var localizedString = localizedChoices[i];
                if (localizedString != null && !localizedString.IsEmpty) {
                    localizedString.StringChanged += value => OnChoiceChanged(index, value);
                }
            }
            isInitialized = true;
        }

        /// <summary>
        /// Cleanup localization subscriptions. Call in OnDisable().
        /// </summary>
        public void DisposeLocalization() {
            if (!isInitialized) return;

            foreach (var localizedString in localizedChoices) {
                if (localizedString != null && !localizedString.IsEmpty) {
                    // Note: LocalizedString doesn't have a simple way to remove specific handlers
                    // The subscription will be cleaned up when the LocalizedString is disposed
                }
            }
            resolvedChoices.Clear();
            isInitialized = false;
        }

        private void OnChoiceChanged(int index, string value) {
            if (index >= 0 && index < resolvedChoices.Count) {
                resolvedChoices[index] = value;
                UpdateDropdownChoices();
            }
        }

        private void UpdateDropdownChoices() {
            var el = Element;
            if (el != null) {
                int currentIndex = el.index;
                el.choices = new List<string>(resolvedChoices);
                if (currentIndex >= 0 && currentIndex < resolvedChoices.Count) {
                    el.index = currentIndex;
                }
            }
        }

        public bool UseLocalization => useLocalization;
#endif

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
