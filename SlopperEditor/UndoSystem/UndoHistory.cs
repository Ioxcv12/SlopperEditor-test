using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEditor.UI;
using SlopperEngine.Core;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.UndoSystem;

/// <summary>
/// Displays the undo queue.
/// </summary>
public class UndoHistory : UIElement
{
    readonly Editor _editor;
    ScrollableArea? _area;

    public UndoHistory(Editor editor) : base(new(0.8f, 0.1f, 1, 0.3f))
    {
        Layout.Value = new LinearArrangedLayout
        {
            IsLayoutHorizontal = false,
            StartAtMax = true,
            Padding = default
        };
        _editor = editor;
        editor.OpenSceneChanged += OnSceneChange;
        OnSceneChange(editor.OpenScene);
    }

    protected override void OnDestroyed()
    {
        _editor.OpenSceneChanged -= OnSceneChange;
    }

    void OnSceneChange(Scene? newScene)
    {
        if (_editor.UndoQueue != null)
            _editor.UndoQueue.OnQueueChanged += CheckUndoList;

        _area?.Destroy();

        if (newScene != null)
        {
            UIChildren.Add(new FloatingWindowHeader(this, "Undo History", false));
            UIChildren.Add(_area = new(new(0, 0, 1, 1)));
            var layout = DefaultLayouts.DefaultVertical;
            _area.Layout.Value = layout;
        }
    }

    void CheckUndoList()
    {
        // the lion does not concern themself with performance
        Dictionary<UndoableAction, UndoHistoryAction> currentChildren = new();
        for (int i = 0; i < _area!.UIChildren.Count; i++)
        {
            var child = _area.UIChildren[i] as UndoHistoryAction;
            if (child == null) continue;
            currentChildren.Add(child.RepresentedAction, child);
            child.Remove();
            i--;
        }

        foreach ((var act, bool isCompleted) in _editor.UndoQueue!.GetActions())
        {
            if (currentChildren.TryGetValue(act, out var rep))
            {
                currentChildren.Remove(act);
                _area.UIChildren.Add(rep);
                rep.Undone = !isCompleted;
            }
            else
                _area.UIChildren.Add(new UndoHistoryAction(act));
        }

        foreach (var leftOver in currentChildren.Values)
            leftOver.Destroy();
    }

    [OnInputUpdate]
    void InputUpdate(InputUpdateArgs args)
    {
        foreach (var j in args.TextInputEvents)
        {
            if (!j.AnyControlheld) continue;
            if (j.CharacterIsAsUnicode) continue;
            if (j.CharacterAsKey != Keys.Z) continue;

            if (j.AnyShiftHeld)
                _editor.UndoQueue?.RedoAction();
            else _editor.UndoQueue?.UndoAction();
        }
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Middle, Alignment.Middle, 100, 100);
}