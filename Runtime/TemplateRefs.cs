using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Majinfwork.UI {
    /// <summary>
    /// A UXML template and references to elements inside it, for UI made from a template instead of living in one
    /// document: list rows and items, or a template placed with &lt;Instance&gt; in several documents. Subclass it with ref
    /// fields; in the Inspector each ref's dropdown lists the template's elements of its type. One set of refs serves
    /// every instance: read an element of one instance with <c>ref.In(instance)</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// [Serializable]
    /// public class PlayerRow : TemplateRefs {
    ///     public LabelRef name;
    ///     public ButtonRef kick;
    /// }
    ///
    /// [SerializeField] private PlayerRow playerRow;
    ///
    /// VisualElement row = playerRow.Instantiate();
    /// playerRow.name.In(row).text = "Ana";
    /// playerRow.kick.In(row).clicked += OnKick;
    /// </code>
    /// </example>
    [Serializable]
    public abstract class TemplateRefs {
        [Tooltip("The UXML template the refs below point into.")]
        [SerializeField] internal VisualTreeAsset template;

        public VisualTreeAsset Template => template;

        /// <summary>A new instance of the template (null without one).</summary>
        public TemplateContainer Instantiate() => template != null ? template.Instantiate() : null;
    }
}
