using OpenTK.Mathematics;
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
	public UIElement UIContainer => _container;

    /// <summary>
    /// Whether or not the popup is currently shown.
    /// </summary>
    public bool Shown => UIContainer.InScene;

    readonly PopupContainer _container;

	public Popup(bool startHidden)
    {
        _container = new();
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
        _container.CheckCount = 0;
	}

	/// <summary>
	/// Hides the popup.
	/// </summary>
	public void Hide()
	{
		UIContainer.Remove();
	}

    /// <summary>
    /// Whether or not the popup is hovered over.
    /// </summary>
    public bool Hovered(Vector2 normalizedMousePos) => _container.Hovered(normalizedMousePos);

    class PopupContainer : UIElement
    {
        public int CheckCount;

        protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 100, 100);

        public bool Hovered(Vector2 normalizedMousePos)
        {
            CheckCount++;
            return CheckCount < 2 || LastChildrenBounds.ContainsInclusive(normalizedMousePos);
        } 
	}
}
