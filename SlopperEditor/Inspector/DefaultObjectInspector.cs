using System.Diagnostics.CodeAnalysis;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Inspector;

public class DefaultObjectInspector : IInspectorValue
{
    public Type GetInspectedType() => typeof(object);
    public bool TryCreateInspectorElement(ValueMember value, object target, InspectorWindow owner, Editor editor, [NotNullWhen(true)] out UIElement? inspectorElement)
    {
        var res = new TextBox(value.GetValue(target)?.ToString() ?? "Null", owner.Style.ForegroundWeak, owner.Style.BackgroundWeak)
        {
            Horizontal = Alignment.Max,
            Scale = 1
        };
        inspectorElement = res;
        owner.OnObjectChanged += () => res.Text = value.GetValue(target)?.ToString() ?? "Null";
        return true;
    }
}