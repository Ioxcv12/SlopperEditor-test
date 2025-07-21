using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Text;

namespace SlopperEditor.SceneRender;

public class SceneDisplay : UIElement
{
    SceneDisplaySettings? _settings;
    UIElement? _display;
    readonly Editor _editor;

    public SceneDisplay(Editor editor)
    {
        _editor = editor;
        _editor.OpenSceneChanged += OnSceneChange;
        OnSceneChange(_editor.OpenScene);
    }

    protected override void OnDestroyed()
    {
        _editor.OpenSceneChanged -= OnSceneChange;
    }

    void OnSceneChange(Scene? scene)
    {
        _settings?.Destroy();
        _display?.Destroy();

        if (_editor.UndoQueue != null)
            _editor.UndoQueue.OnQueueChanged += () => _settings?.Update(scene);

        if (scene == null)
        {
            UIChildren.Add(_display = new());
            _display.UIChildren.Add(new TextBox("No scene currently open.")
            {
                Horizontal = Alignment.Middle,
                Vertical = Alignment.Middle,
                LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f),
                Scale = 1
            });
            return;
        }

        ImageRectangle rect = new();
        UIChildren.Add(_display = rect);

        _editor.FloatingWindowHolder.UIChildren.Add(_settings = new(rect));
        _settings.Update(scene);
    }
}