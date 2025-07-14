using OpenTK.Mathematics;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;

namespace SlopperEditor.UI;

/// <summary>
/// A popup that can contain a set of UIElements.
/// </summary>
public class Popup : SceneObject
{
	/// <summary>
	/// Contains the children for the popup.
	/// </summary>
	public readonly UIElement UIContainer;

	public Popup(Vector2 globalPosition, bool startHidden)
	{
		UIContainer = new PopupContainer(globalPosition);
		if (!startHidden)
			Children.Add(UIContainer);
	}

	/// <summary>
	/// Hides the popup.
	/// </summary>
	public void Hide()
	{
		UIContainer.Remove();
	}

	/// <summary>
	/// Shows the popup.
	/// </summary>
	public void Show()
	{
		Children.Add(UIContainer);
	}

	class PopupContainer(Vector2 position) : UIElement(new(position, position))
	{
		protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 100, 100);
	}
}
