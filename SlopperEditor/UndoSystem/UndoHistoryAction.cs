using OpenTK.Mathematics;
using SlopperEditor.UI;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Layout;
using SlopperEngine.UI.Text;

namespace SlopperEditor.UndoSystem;

public class UndoHistoryAction : UIElement
{
    public readonly UndoableAction RepresentedAction;

    public bool Undone
    {
        get => _undone;
        set
        {
            _undone = value;
            if (value)
            {
                _text.BackgroundColor = Style.BackgroundWeak;
                _colorBand.Color = _antiHighlight;
            }
            else
            {
                _text.BackgroundColor = Style.ForegroundWeak;
                _colorBand.Color = Style.Highlight;
            }
        }
    }
    bool _undone;

    readonly ColorRectangle _colorBand;
    readonly TextBox _text;
    Color4 _antiHighlight;

    public UndoHistoryAction(UndoableAction representedAction) : base(new(0, 0, 1, 0))
    {
        Vector4 inverse = Vector4.One - (Vector4)Style.Highlight;
        _antiHighlight = new(inverse.X, inverse.Y, inverse.Z, Style.Highlight.A);

        Layout.Value = DefaultLayouts.DefaultHorizontal;
        RepresentedAction = representedAction;
        Spacer colorBandHolder = new()
        {
            MinWidth = 3,
            LocalShape = new(0, 0, 0, 1),
            GrowDirectionX = Alignment.Max
        };
        colorBandHolder.UIChildren.Add(_colorBand = new(new(0, 0, 1, 1), Style.Highlight));
        UIChildren.Add(colorBandHolder);

        _text = new(representedAction.Name, Style.Tint, Style.ForegroundWeak)
        {
            Horizontal = Alignment.Max,
            Vertical = Alignment.Middle,
            LocalShape = new(0, 0.5f, 0, 0.5f),
            Scale = 1,
        };
        UIChildren.Add(_text);
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, default, 90, 16);
}