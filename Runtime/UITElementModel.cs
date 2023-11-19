using System;
using UnityEngine.UIElements;

namespace Majingari.UI {
    /// <summary>
    /// To get the button reference use string, due to limitation of Unity not allowing reference directly to the visual element
    /// Only contains string name of element.
    /// </summary>
    [Serializable]
    public class UITElementModel {
        public UIDocument uiDocument;
    }

    [Serializable]
    public class SampleUITModel : UITElementModel {
        public string testButton;
        public string testButtonA;
        public string testProgress;
    }
}