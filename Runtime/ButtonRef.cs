using System;
using UnityEngine;
using UnityEngine.UIElements;
#if HAS_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Majinfwork.UI {
    /// <summary>
    /// Reference wrapper for UI Toolkit Button element.
    /// Supports optional localization for dynamic text with arguments.
    /// For static localized text, use UXML binding: text="@TableName/Key"
    /// </summary>
    [Serializable]
    public class ButtonRef : UITElementRef<Button> {
        public Button Button => Element;

#if HAS_UNITY_LOCALIZATION
        [Tooltip("Enable for dynamic localized text with arguments. For static text, use UXML @Table/Key syntax instead.")]
        [SerializeField] private bool useLocalization;
        [SerializeField] private LocalizedString localizedString;

        private bool isInitialized;

        /// <summary>
        /// Set arguments for dynamic localized text.
        /// Automatically updates the button text.
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
