using System;
using SlopperEditor.UI;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Inspector;

/// <summary>
/// A window that can inspect a certain object.
/// </summary>
public class InspectorWindow : UIElement
{
    public event Action? OnObjectChanged;

    Editor _editor;

    public InspectorWindow(object toInspect, Editor editor) : base(new(0.4f, 0.3f, 0.6f, 0.7f))
    {
        Layout.Value = new LinearArrangedLayout
        {
            Padding = default,
            IsLayoutHorizontal = false,
            StartAtMax = true,
        };

        _editor = editor;
        editor.UndoQueue!.OnQueueChanged += OnObjectChanged;

        FloatingWindowHeader header = new(this, "Inspector - " + toInspect.GetType().Name);
        UIChildren.Add(header);
        ScrollableArea content = new();
        UIChildren.Add(content);
        content.Layout.Value = DefaultLayouts.DefaultVertical;

        foreach (var mems in ReflectionCache.GetPublicMembers(toInspect.GetType()))
        {
            var span = mems.Span;
            if (span.Length < 1)
                continue;

            if (toInspect.GetType() != span[0].DeclaringType)
                content.UIChildren.Add(new TextBox(span[0].DeclaringType.Name, Style.Tint, default)
                {
                    LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f),
                    Scale = 1
                });
            var nameValues = new Spacer
            {
                LocalShape = new(0, 1, 1, 1),
                MinHeight = 16,
            };
            var names = new Spacer
            {
                LocalShape = new(0, 0, 0.5f, 1),
                ScissorRegion = new(0, float.NegativeInfinity, 1, 1),
            };
            names.Layout.Value = DefaultLayouts.DefaultVertical;
            var values = new Spacer
            {
                LocalShape = new(0.5f, 0, 1, 1),
                ScissorRegion = new(0, float.NegativeInfinity, float.PositiveInfinity, 1),
            };
            values.Layout.Value = DefaultLayouts.DefaultVertical;
            content.UIChildren.Add(nameValues);
            nameValues.UIChildren.Add(names);
            nameValues.UIChildren.Add(values);

            foreach (var mem in span)
            {
                var value = ReflectionCache.GetMemberInspectorHandler(mem.MemberType).CreateInspectorElement(mem, toInspect, this, editor);
                values.UIChildren.Add(value);
                names.UIChildren.Add(new InspectorName(mem.Name, value));
            }
        }
    }

    protected override void OnDestroyed()
    {
        _editor.UndoQueue!.OnQueueChanged -= OnObjectChanged;
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Middle, Alignment.Middle, 100, 100);
}