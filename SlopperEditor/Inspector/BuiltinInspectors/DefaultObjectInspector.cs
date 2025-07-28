using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Inspector.BuiltinInspectors;

/// <summary>
/// Shows objects that do not have a custom inspector.
/// </summary>
public class DefaultObjectInspector : IMemberInspectorHandler
{
    public Type GetInspectedType() => typeof(object);
    public UIElement CreateInspectorElement(ValueMember value, object target, InspectorWindow owner, Editor editor)
    {
        var res = new TextBox(value.GetValue(target)?.ToString() ?? "Null", owner.Style.ForegroundWeak, owner.Style.BackgroundWeak)
        {
            Horizontal = Alignment.Max,
            Scale = 1
        };
        owner.OnObjectChanged += () => res.Text = value.GetValue(target)?.ToString() ?? "Null";
        return res;
    }
}