using OpenTK.Mathematics;
using SlopperEditor.UI;
using SlopperEngine.Core.SceneComponents;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
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

    static void Main(string[] args)
    {
        MainContext.Instance.Load += static () => new Editor();
        MainContext.MultithreadedFrameUpdate = false;
        MainContext.Instance.UpdateFrequency = 24;
        MainContext.Instance.Run();
    }

    Editor()
    {
        OnNewAssemblyLoaded?.Invoke();

        Vector2i mainWindowSize = new(600, 400);
        var mainScene = Scene.CreateEmpty();

        var renderer = new UIRenderer();
        mainScene.Renderers.Add(renderer);
        mainScene.Components.Add(new UpdateHandler());

        renderer.Resize(mainWindowSize);

        UIElement mainUI = new(new(0,0,1,1));
        var noOpenScene = new TextBox("No scene currently open.");
        noOpenScene.Horizontal = Alignment.Middle;
        noOpenScene.Vertical = Alignment.Middle;
        noOpenScene.LocalShape = new(0.5f, 0.5f, 0.5f, 0.5f);
        noOpenScene.Scale = 1;
        mainUI.UIChildren.Add(noOpenScene);
        mainUI.UIChildren.Add(new Toolbar(this));
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
    }
}
