using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    [Serializable]
    public class ProgressBarRef : UITElementRef<ProgressBar> {
        public ProgressBar ProgressBar => Element;

        public float Value {
            get => Element?.value ?? 0f;
            set { var el = Element; if (el != null) el.value = value; }
        }

        public float LowValue {
            get => Element?.lowValue ?? 0f;
            set { var el = Element; if (el != null) el.lowValue = value; }
        }

        public float HighValue {
            get => Element?.highValue ?? 100f;
            set { var el = Element; if (el != null) el.highValue = value; }
        }

        public string Title {
            get => Element?.title;
            set { var el = Element; if (el != null) el.title = value; }
        }

        public void SetNormalizedValue(float normalized) {
            var el = Element;
            if (el == null) return;
            float range = el.highValue - el.lowValue;
            el.value = el.lowValue + (range * Mathf.Clamp01(normalized));
        }
    }
}
