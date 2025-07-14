using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SlopperEditor.UI;
using SlopperEngine.Core.SceneComponents;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Layout;
using SlopperEngine.Windowing;

namespace SlopperEditor;

public class Editor : SceneObject
{
    /// <summary>
    /// Gets called when a new assembly gets loaded (and caches should be invalidated).
    /// </summary>
    public static event Action? OnNewAssemblyLoaded;

    static void Main(string[] args)
    {
        MainContext.Instance.Load += static () =>
        {
            Vector2i mainWindowSize = new(600, 400);
            var mainScene = Scene.CreateEmpty();

            var renderer = new UIRenderer();
            mainScene.Renderers.Add(renderer);
            mainScene.Components.Add(new UpdateHandler());

            renderer.Resize(mainWindowSize);

            UIElement mainUI = new();
            var layout = new LinearArrangedLayout();
            layout.ChildAlignment = Alignment.Middle;
            layout.IsLayoutHorizontal = false;
            mainUI.Layout.Value = layout;
            mainUI.Children.Add(new Toolbar());

            var win = Window.Create(new(
                mainWindowSize,
                Title: "SlopperEditor"
            ));
            win.KeepProgramAlive = true;
            win.WindowTexture = renderer.GetOutputTexture();
            win.Scene = mainScene;
            win.FramebufferResize += (FramebufferResizeEventArgs args) =>
            {
                renderer.Resize(args.Size);
                win.WindowTexture = renderer.GetOutputTexture();
            };
        };
        MainContext.MultithreadedFrameUpdate = false;
        MainContext.Instance.UpdateFrequency = 24;
        MainContext.Instance.Run();
    }
}
