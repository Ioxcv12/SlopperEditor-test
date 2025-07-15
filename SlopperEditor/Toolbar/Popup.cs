using OpenTK.Mathematics;
using SlopperEngine.Core;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;

namespace SlopperEditor.Toolbar;

/// <summary>
/// A popup that can contain a set of UIElements.
/// </summary>
public class Popup : SceneObject
{
	/// <summary>
	/// Contains the children for the popup.
	/// </summary>
	public readonly UIElement UIContainer;

	public Popup(bool startHidden)
	{
		UIContainer = new PopupContainer();
		if (!startHidden)
			Children.Add(UIContainer);
	}

	/// <summary>
	/// Shows the popup.
	/// </summary>
	public void Show(Vector2 position)
	{
        position += Vector2.One;
        position *= 0.5f;
        UIContainer.LocalShape = new(position, position);
		Children.Add(UIContainer);
	}

	/// <summary>
	/// Hides the popup.
	/// </summary>
	public void Hide()
	{
		UIContainer.Remove();
	}

    class PopupContainer : UIElement
    {
        public bool Hovered { get; private set; }

        protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 100, 100);

        [OnInputUpdate]
        void InputUpdate(InputUpdateArgs args)
        {
            Hovered = LastGlobalShape.ContainsInclusive(args.NormalizedMousePosition * 2 - Vector2.One);
        }
	}
}
