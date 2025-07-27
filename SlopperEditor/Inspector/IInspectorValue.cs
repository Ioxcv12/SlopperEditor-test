using System.Diagnostics.CodeAnalysis;
using SlopperEngine.UI.Base;

namespace SlopperEditor.Inspector;

public interface IInspectorValue
{
    /// <summary>
    /// Creates an inspector element for the given ValueMember.
    /// </summary>
    /// <param name="value">The value to inspect on the target.</param>
    /// <param name="target">The object to change the member of.</param>
    /// <param name="inspectorElement">An unparented UIElement that can read or change the target's member.</param>
    /// <returns>Whether or not the element was successfully created.</returns>
    public bool TryCreateInspectorElement(ValueMember value, object target, InspectorWindow owner, Editor editor, [NotNullWhen(true)] out UIElement? inspectorElement);

    /// <summary>
    /// The type this IInspectorValue inspects. Should be constant!
    /// </summary>
    public Type GetInspectedType();
}