using System;
using SlopperEngine.UI.Base;

namespace SlopperEditor.Inspector;

/// <summary>
/// Creates a UIElement that allows the editing of a value in an object.
/// </summary>
public interface IMemberInspectorHandler
{
    /// <summary>
    /// Creates an inspector element for the given ValueMember.
    /// </summary>
    /// <param name="value">The value to inspect on the target.</param>
    /// <param name="target">The object to change the member of.</param>
    /// <param name="inspectorElement">An unparented UIElement that can read or change the target's member.</param>
    /// <returns>Whether or not the element was successfully created.</returns>
    public UIElement CreateInspectorElement(ValueMember value, object target, InspectorWindow owner, Editor editor);

    /// <summary>
    /// The type this IMemberInspectorHandler inspects. Should be constant and superclasses should always be handled!
    /// </summary>
    public Type GetInspectedType();
}