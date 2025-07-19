using SlopperEditor.UI;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.Hierarchy;

/// <summary>
/// Shows the hierarchy of the current scene.
/// </summary>
public class HierarchyWindow : UIElement
{
    readonly Editor _editor;
    HierarchyObject? _root;
    ScrollableArea? _area;

    public HierarchyWindow(Editor editor) : base(new(0, 0.1f, 0.2f, 0.9f))
    {
        _editor = editor;
        OnSceneChange(editor.OpenScene);
        _editor.OpenSceneChanged += OnSceneChange;
        Layout.Value = new LinearArrangedLayout
        {
            IsLayoutHorizontal = false,
            StartAtMax = true,
            Padding = default
        };
    }

    void OnSceneChange(Scene? newScene)
    {
        if (_editor.UndoQueue != null)
            _editor.UndoQueue.OnQueueChanged += CheckHierarchy;

        _root?.Destroy();
        _area?.Destroy();

        if (newScene != null)
        {
            UIChildren.Add(new FloatingWindowHeader(this, "Hierarchy", false));
            UIChildren.Add(_area = new(new(0, 0, 1, 1)));
            _area.UIChildren.Add(_root = new(newScene, _editor));
        }
    }

    void CheckHierarchy()
    {
        _root?.Update();
    }
}