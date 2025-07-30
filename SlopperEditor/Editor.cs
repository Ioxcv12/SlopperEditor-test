using OpenTK.Mathematics;
using SlopperEditor.UndoSystem;
using SlopperEngine.Core;
using SlopperEngine.Core.SceneComponents;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
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

    /// <summary>
    /// The undo/redo system.
    /// </summary>
    public UndoQueue? UndoQueue { get; private set; }

    /// <summary>
    /// A UIElement that holds floating windows in the correct order.
    /// </summary>
    public readonly UIElement FloatingWindowHolder;

    readonly EditorAssetHandler _assetHandler;

    static void Main()
    {
        MainContext.Instance.Load += static () => new Editor();
        MainContext.MultithreadedFrameUpdate = false;
        MainContext.Instance.UpdateFrequency = 60;
        MainContext.Instance.Run();
    }

    Editor()
    {
        Assets.Instance = _assetHandler = new();

        Vector2i mainWindowSize = new(1200, 800);
        var mainScene = Scene.CreateEmpty();

        var renderer = new UIRenderer();
        mainScene.Renderers.Add(renderer);
        mainScene.Components.Add(new UpdateHandler());

        renderer.Resize(mainWindowSize);

        UIElement mainUI = new(new(0, 0, 1, 1));
        mainUI.UIChildren.Add(new SceneRender.SceneDisplay(this));
        mainUI.UIChildren.Add(new Toolbar.Toolbar(this));
        mainUI.UIChildren.Add(FloatingWindowHolder = new());
        FloatingWindowHolder.UIChildren.Add(new Hierarchy.HierarchyWindow(this));
        FloatingWindowHolder.UIChildren.Add(new UndoHistory(this));

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
}
