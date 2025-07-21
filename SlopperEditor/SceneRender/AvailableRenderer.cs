using SlopperEngine.Rendering;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;

namespace SlopperEditor.SceneRender;

public class AvailableRenderer : UIElement
{
    public readonly SceneRenderer RepresentedRenderer;

    readonly TextButton _button;

    public AvailableRenderer(SceneRenderer represented, SceneDisplaySettings owner) : base(default)
    {
        RepresentedRenderer = represented;
        _button = new TextButton(represented.GetType().Name);
        _button.OnButtonPressed += _ => owner.SelectRenderer(RepresentedRenderer);
        UIChildren.Add(_button);
    }

    protected override void OnStyleChanged()
    {
        _button.Style = Style;
    }
}