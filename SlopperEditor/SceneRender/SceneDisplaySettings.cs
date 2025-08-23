using System.Collections.Generic;
using SlopperEditor.UI;
using SlopperEngine.Core;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Style;

namespace SlopperEditor.SceneRender;

/// <summary>
/// Shows settings for the SceneDisplay.
/// </summary>
public class SceneDisplaySettings : UIElement
{
    readonly ImageRectangle _output;
    readonly ScrollableArea _area;
    ProxySceneRenderer? _currentRenderer;

    public SceneDisplaySettings(ImageRectangle output) : base(new(0.8f, 0.7f, 1, 0.9f))
    {
        UIChildren.Add(new FloatingWindowHeader(this, "Scene rendering settings", false));
        UIChildren.Add(_area = new());
        _output = output;
        Layout.Value = DefaultLayouts.PackedVertical;
        _area.Layout.Value = DefaultLayouts.DefaultVertical;
    }

    /// <summary>
    /// Should get called when the scene gets changed.
    /// </summary>
    public void Update(Scene? scene)
    {
        if (scene == null)
        {
            while (_area.UIChildren.Count > 0)
                _area.UIChildren[0].Destroy();
            return;
        }

        if (_area.UIChildren.Count == scene.Renderers.Count)
            return;

        Dictionary<SceneRenderer, AvailableRenderer> currentChildren = new();
        for (int i = 0; i < _area!.UIChildren.Count; i++)
        {
            var child = _area.UIChildren[i] as AvailableRenderer;
            if (child == null) continue;
            currentChildren.Add(child.RepresentedRenderer, child);
            child.Remove();
            i--;
        }

        foreach (var rend in scene.Renderers.All)
        {
            if (currentChildren.TryGetValue(rend, out var ui))
            {
                currentChildren.Remove(rend);
                _area.UIChildren.Add(ui);
            }
            else
                _area.UIChildren.Add(new AvailableRenderer(rend, this));
        }

        foreach (var leftOver in currentChildren.Values)
            leftOver.Destroy();

        if (_currentRenderer == null)
        {
            foreach (var ch in _area.UIChildren.All)
            {
                if (ch is not AvailableRenderer rend)
                    continue;

                SelectRenderer(rend.RepresentedRenderer);
                break;
            }
        }
    }

    public void SelectRenderer(SceneRenderer renderer)
    {
        if (_currentRenderer?.RepresentedRenderer == renderer)
            return;

        for (int i = 0; i < _area!.UIChildren.Count; i++)
        {
            var child = _area.UIChildren[i] as AvailableRenderer;
            if (child == null) continue;
            child.Style = child.RepresentedRenderer == renderer ? DefaultStyles.Selected : BasicStyle.DefaultStyle;
        }

        _currentRenderer?.Destroy();
        Scene!.Renderers.Add(_currentRenderer = new(renderer));
        _currentRenderer.OnResize += () => _output.Texture = _currentRenderer.GetOutputTexture();
        _output.Texture = _currentRenderer.GetOutputTexture();
    }

    [OnFrameUpdate]
    void FrameUpdate(FrameUpdateArgs args)
    {
        _currentRenderer?.RenderRepresented(args);
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Middle, Alignment.Middle, 100, 100);
}