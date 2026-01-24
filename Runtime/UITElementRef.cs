using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    public enum UITReferenceMode {
        UseReference,
        UseDocumentHere
    }

    /// <summary>
    /// Abstract base class for UI Toolkit element references.
    /// Supports two modes:
    /// - UseReference: Manually assign UIDocument from any GameObject
    /// - UseDocumentHere: Auto-assign UIDocument from the same GameObject (set by editor)
    /// Can also be configured via code using Bind() methods.
    /// </summary>
    [Serializable]
    public abstract class UITElementRef {
        [SerializeField] internal UITReferenceMode mode;
        [SerializeField] internal UIDocument document;
        [SerializeField] internal string elementName;

        protected VisualElement cachedElement;
        protected VisualElement cachedRoot;

        public UITReferenceMode Mode => mode;
        public UIDocument Document => document;
        public string ElementName => elementName;
        public bool IsResolved => cachedElement != null && !IsCacheStale();
        public abstract Type TargetType { get; }

        /// <summary>
        /// Binds this ref to a document and element name. Element will be queried on next access.
        /// </summary>
        public void Bind(UIDocument doc, string name) {
            document = doc;
            elementName = name;
            cachedElement = null;
            cachedRoot = null;
        }

        /// <summary>
        /// Binds this ref to query from a root element directly.
        /// </summary>
        public void Bind(VisualElement root, string name) {
            document = null;
            elementName = name;
            cachedRoot = root;
            cachedElement = string.IsNullOrEmpty(name) ? null : root?.Q(name);
        }

        /// <summary>
        /// Directly binds a resolved element, bypassing query.
        /// </summary>
        public void BindDirect(VisualElement element) {
            document = null;
            elementName = null;
            cachedElement = element;
            cachedRoot = element?.parent;
        }

        public void ClearCache() {
            cachedElement = null;
            cachedRoot = null;
        }

        protected bool IsCacheStale() {
            // No document = either manually bound or not configured
            // If we have a cached element, trust it (manually bound)
            if (!document) return cachedElement == null;
            return !ReferenceEquals(cachedRoot, document.rootVisualElement);
        }

        protected VisualElement ResolveInternal() {
            // If no document but we have cached element, return it (manually bound)
            if (!document) {
                return cachedElement;
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

        /// <summary>
        /// Binds this ref to query from a root element directly. Type-safe version.
        /// </summary>
        public new void Bind(VisualElement root, string name) {
            document = null;
            elementName = name;
            cachedRoot = root;
            cachedElement = string.IsNullOrEmpty(name) ? null : root?.Q<T>(name);
            cachedTypedElement = cachedElement as T;
        }

        /// <summary>
        /// Directly binds a resolved element, bypassing query. Type-safe version.
        /// </summary>
        public void BindDirect(T element) {
            document = null;
            elementName = null;
            cachedElement = element;
            cachedTypedElement = element;
            cachedRoot = element?.parent;
        }

        public new void ClearCache() {
            base.ClearCache();
            cachedTypedElement = null;
        }
    }
}
