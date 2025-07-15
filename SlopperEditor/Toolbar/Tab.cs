using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Text;

namespace SlopperEditor.Toolbar;

public abstract class Tab : BaseButton
{
    protected readonly Popup options;
    readonly ColorRectangle _background;
    readonly TextBox _textRenderer;

    protected Tab(string text)
    {
        options = new(true);
        Children.Add(options);

        _background = new(new(0,0,1,1), Style.ForegroundWeak);
        UIChildren.Add(_background);

        _textRenderer = new(text);
        _textRenderer.Horizontal = Alignment.Middle;
        _textRenderer.Vertical = Alignment.Middle;
        _textRenderer.LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f);
        _textRenderer.Scale = 1;
        UIChildren.Add(_textRenderer);

        LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void ShowOptions()
    {
        options.Show(LastGlobalShape.Min);
    }

    public void HideOptions()
    {
        options.Hide();
    }

    protected override void OnPressed(MouseButton button)
    {
        _background.Color = Style.Tint;
        _textRenderer.TextColor = Style.ForegroundStrong;
        if (button == MouseButton.Left)
            ShowOptions();
    }

    protected override void OnAnyRelease(MouseButton button) { }

    protected override void OnAllButtonsReleased()
    {
        _background.Color = Style.ForegroundStrong;
        _textRenderer.TextColor = Style.Tint;
    }

    protected override void OnMouseEntry()
    {
        OnAllButtonsReleased();
    }

    protected override void OnMouseExit()
    {
        _background.Color = Style.ForegroundWeak;
        _textRenderer.TextColor = Style.Tint;
    }

    protected override UIElementSize GetSizeConstraints() => _textRenderer.LastSizeConstraints;
}