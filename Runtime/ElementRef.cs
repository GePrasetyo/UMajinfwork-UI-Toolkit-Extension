using System;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    /// <summary>
    /// A reference to an element of type <typeparamref name="T"/>, for element types with no ref class of their own, such
    /// as your own controls (<c>ElementRef&lt;HealthGauge&gt;</c>). The Inspector lists only elements of that type.
    /// </summary>
    [Serializable]
    public class ElementRef<T> : UITElementRef<T> where T : VisualElement {
    }
}
