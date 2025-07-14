using SlopperEngine.Windowing;

namespace SlopperEditor;

class Editor
{
    static void Main(string[] args)
    {
		MainContext.Instance.Load += () =>
		{
			var win = Window.Create(new(
				new(600, 400),
				Title: "SlopperEditor"
			));
			win.KeepProgramAlive = true;
		};
		MainContext.Instance.Run();
        Console.WriteLine("Hello, World!");
    }
}
