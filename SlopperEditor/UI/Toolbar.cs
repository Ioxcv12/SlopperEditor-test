using SlopperEngine.Core;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Display;

namespace SlopperEditor.UI;

/// <summary>
/// The top toolbar of the editor.
/// </summary>
public class Toolbar : UIElement
{
    readonly Editor _editor;
	readonly ColorRectangle _background;
    
	public Toolbar(Editor editor) : base(new(0, 1, 1, 1))
    {
        _editor = editor;
        UIChildren.Add(_background = new(new(0, 0, 1, 1), Style.BackgroundStrong));
    }

	protected override void OnStyleChanged()
	{
		_background.Color = Style.BackgroundStrong;
	}

	protected override UIElementSize GetSizeConstraints() => new(default, Alignment.Min, 1, 20);
}
