using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEditor.UndoSystem;
using SlopperEngine.Core;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.Toolbar;

/// <summary>
/// The top toolbar of the editor.
/// </summary>
public class Toolbar : UIElement
{
    readonly Editor _editor;
	readonly ColorRectangle _background;
    readonly UIElement _foreground;

    public Toolbar(Editor editor) : base(new(0, 1, 1, 1))
    {
        _editor = editor;
        UIChildren.Add(_background = new(new(0, 0, 1, 1), Style.BackgroundStrong));

        UIChildren.Add(_foreground = new());
        var layout = new LinearArrangedLayout();
        layout.IsLayoutHorizontal = true;
        layout.Padding = UISize.FromPixels(new(3, 3));
        layout.ChildAlignment = Alignment.Middle;
        layout.StartAtMax = false;
        _foreground.Layout.Value = layout;

        _foreground.UIChildren.Add(new SceneTab(editor));
    }

	protected override void OnStyleChanged()
	{
		_background.Color = Style.BackgroundStrong;
	}

	protected override UIElementSize GetSizeConstraints() => new(default, Alignment.Min, 1, 20);
}
