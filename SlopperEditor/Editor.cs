using OpenTK.Mathematics;
using SlopperEditor.UndoSystem;
using SlopperEngine.Core.SceneComponents;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Text;
using SlopperEngine.Windowing;

namespace SlopperEditor;

/// <summary>
/// Represents a slopperengine scene editor.
/// </summary>
public class Editor
{
    /// <summary>
    /// Gets called when a new assembly gets loaded (and caches should be invalidated).
    /// </summary>
    public static event Action? OnNewAssemblyLoaded;

    /// <summary>
    /// Gets called when a new scene gets opened.
    /// </summary>
    public event Action<Scene?>? OpenSceneChanged;

    /// <summary>
    /// Gets called when a new object gets selected.
    /// </summary>
    public event Action<SceneObject?>? SelectedObjectChanged;

    /// <summary>
    /// Gets or sets the current scene being edited. Setting will wipe all undo history.
    /// </summary>
    public Scene? OpenScene
    {
        get => _openScene;
        set
        {
            _openScene = value;
            UndoQueue = new();
            OpenSceneChanged?.Invoke(value);
        }
    }
    Scene? _openScene;

    public UndoQueue? UndoQueue { get; private set; }

    public SceneObject? SelectedObject
    {
        get => _selectedObject;
        set
        {
            UndoQueue?.DoAction(new SelectAction(value, _selectedObject, this));
            _selectedObject = value;
            SelectedObjectChanged?.Invoke(value);
        }
    }
    SceneObject? _selectedObject;

    static void Main()
    {
        MainContext.Instance.Load += static () => new Editor();
        MainContext.MultithreadedFrameUpdate = false;
        MainContext.Instance.UpdateFrequency = 24;
        MainContext.Instance.Run();
    }

    Editor()
    {
        Vector2i mainWindowSize = new(1200, 800);
        var mainScene = Scene.CreateEmpty();

        var renderer = new UIRenderer();
        mainScene.Renderers.Add(renderer);
        mainScene.Components.Add(new UpdateHandler());

        renderer.Resize(mainWindowSize);

        UIElement mainUI = new(new(0, 0, 1, 1));
        var noOpenScene = new TextBox("No scene currently open.");
        noOpenScene.Horizontal = Alignment.Middle;
        noOpenScene.Vertical = Alignment.Middle;
        noOpenScene.LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f);
        noOpenScene.Scale = 1;
        mainUI.UIChildren.Add(noOpenScene);
        mainUI.UIChildren.Add(new Toolbar.Toolbar(this));
        mainUI.UIChildren.Add(new Hierarchy.HierarchyWindow(this));
        mainScene.Children.Add(mainUI);

        var win = Window.Create(new(
            mainWindowSize,
            Title: "SlopperEditor"
        ));
        win.KeepProgramAlive = true;
        win.WindowTexture = renderer.GetOutputTexture();
        win.Scene = mainScene;
        win.FramebufferResize += args =>
        {
            renderer.Resize(args.Size);
            win.WindowTexture = renderer.GetOutputTexture();
        };

        OnNewAssemblyLoaded?.Invoke();
    }
    
    private class SelectAction : UndoableAction
    {
        public readonly SceneObject? PreviousSelected;
        public readonly SceneObject? Selected;

        readonly Editor _editor;

        public SelectAction(SceneObject? newlySelected, SceneObject? previousSelected, Editor editor) : base(newlySelected == null ? $"Deselect {previousSelected}" : $"Select {newlySelected}")
        {
            PreviousSelected = previousSelected;
            Selected = newlySelected;
            _editor = editor;
        }

        public override void Do()
        {
            _editor._selectedObject = Selected;
            _editor.SelectedObjectChanged?.Invoke(Selected);
        }

        public override void Undo()
        {
            _editor._selectedObject = PreviousSelected;
            _editor.SelectedObjectChanged?.Invoke(PreviousSelected);
        }
    }
}
