using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SlopperEditor.UI;
using SlopperEngine.Core.SceneComponents;
using SlopperEngine.Rendering;
using SlopperEngine.SceneObjects;
using SlopperEngine.Windowing;

namespace SlopperEditor;

public class Editor : SceneObject
{
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

			mainScene.Children.Add(new Toolbar());

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
