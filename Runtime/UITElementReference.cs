using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majingari.UI {
    /// <summary>
    /// This is not working for now
    /// </summary>
    [Serializable]
    public class UITElementReference {
        [SerializeField] internal UIDocument uiDocument;
    }

    [Serializable]
    public class SampleUIT : UITElementReference {
        public Button testButton;
    }
}