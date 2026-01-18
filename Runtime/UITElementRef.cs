using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    /// <summary>
    /// Abstract base class for UI Toolkit element references.
    /// Stores UIDocument and element name, resolves at runtime.
    /// </summary>
    [Serializable]
    public abstract class UITElementRef {
        [SerializeField] internal UIDocument document;
        [SerializeField] internal string elementName;

        protected VisualElement cachedElement;
        protected VisualElement cachedRoot;

        public UIDocument Document => document;
        public string ElementName => elementName;
        public bool IsResolved => cachedElement != null && !IsCacheStale();
        public abstract Type TargetType { get; }

        public void ClearCache() {
            cachedElement = null;
            cachedRoot = null;
        }

        /// <summary>
        /// Checks if the cached element is stale (document destroyed or rootVisualElement changed).
        /// </summary>
        protected bool IsCacheStale() {
            if (!document) return true;
            return !ReferenceEquals(cachedRoot, document.rootVisualElement);
        }

        /// <summary>
        /// Resolves the element. Returns cached if valid, otherwise queries fresh.
        /// </summary>
        protected VisualElement ResolveInternal() {
            if (!document) {
                cachedElement = null;
                cachedRoot = null;
                return null;
            }

            var root = document.rootVisualElement;
            if (root == null) return null;

            if (cachedElement != null && ReferenceEquals(cachedRoot, root)) {
                return cachedElement;
            }

            cachedRoot = root;
            cachedElement = string.IsNullOrEmpty(elementName) ? null : root.Q(elementName);
            return cachedElement;
        }
    }

    /// <summary>
    /// Generic typed element reference with strongly-typed access.
    /// </summary>
    [Serializable]
    public abstract class UITElementRef<T> : UITElementRef where T : VisualElement {
        private T cachedTypedElement;

        public override Type TargetType => typeof(T);

        /// <summary>
        /// The resolved element. Returns null if not found.
        /// </summary>
        public T Element {
            get {
                var element = ResolveInternal();
                if (!ReferenceEquals(element, cachedTypedElement)) {
                    cachedTypedElement = element as T;
                }
                return cachedTypedElement;
            }
        }

        public T Resolve() => Element;

        public new void ClearCache() {
            base.ClearCache();
            cachedTypedElement = null;
        }
    }
}
