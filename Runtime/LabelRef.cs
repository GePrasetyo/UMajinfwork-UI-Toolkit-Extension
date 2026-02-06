using System;
using UnityEngine;
using UnityEngine.UIElements;
#if HAS_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Majinfwork.UI {
    /// <summary>
    /// Reference wrapper for UI Toolkit Label element.
    /// Supports optional localization for dynamic text with arguments.
    /// For static localized text, use UXML binding: text="@TableName/Key"
    /// </summary>
    [Serializable]
    public class LabelRef : UITElementRef<Label> {
        public Label Label => Element;

#if HAS_UNITY_LOCALIZATION
        [Tooltip("Enable for dynamic localized text with arguments. For static text, use UXML @Table/Key syntax instead.")]
        [SerializeField] private bool useLocalization;
        [SerializeField] private LocalizedString localizedString;

        private bool isInitialized;

        /// <summary>
        /// Set arguments for dynamic localized text (e.g., "Score: {0}" with score value).
        /// Automatically updates the label text.
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
        /// Only needed when useLocalization is enabled in Inspector.
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

        public string Text {
            get => Element?.text;
            set { var el = Element; if (el != null) el.text = value; }
        }
    }
}
