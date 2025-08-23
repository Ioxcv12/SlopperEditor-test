using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SlopperEngine.Core;
using SlopperEngine.UI.Base;

namespace SlopperEditor.UI;

/// <summary>
/// Allows a UIElement to be dragged.
/// </summary>
public sealed class DragElementHitbox : UIElement
{
    /// <summary>
    /// How to treat the element when it gets dragged out of its parent's bounds.
    /// </summary>
    public OutOfBoundsMode OobMode;

    /// <summary>
    /// Gets called when the hitbox gets clicked/
    /// </summary>
    public event Action? OnDragStart;

    readonly UIElement _toDrag;
    bool _dragging = false;
    Vector2 _dragOffset;

    public DragElementHitbox(UIElement toDrag, OutOfBoundsMode oobMode = default)
    {
        _toDrag = toDrag;
        OobMode = oobMode;
    }

    protected override void HandleEvent(ref MouseEvent e)
    {
        if (e.Type != MouseEventType.PressedButton)
            return;

        if (e.PressedButton != MouseButton.Left)
            return;

        OnDragStart?.Invoke();
        _dragging = true;
        _dragOffset = _toDrag.LastGlobalShape.Center - e.NDCPosition;
        e.Use();
    }

    [OnInputUpdate]
    void InputUpdate(InputUpdateArgs args)
    {
        if (!_dragging) return;

        UIElement? parent = _toDrag.Parent as UIElement;
        Box2 centerPositionBounds = new(-1, -1, 1, 1);
        if (parent != null)
            centerPositionBounds = parent.LastGlobalShape;
        centerPositionBounds.Size -= _toDrag.LastGlobalShape.Size;

        if (args.MouseState.IsButtonReleased(MouseButton.Left))
            _dragging = false;

        var desiredNDCPos = args.NormalizedMousePosition * 2 - Vector2.One + _dragOffset;
        if (OobMode == OutOfBoundsMode.Always || (!_dragging && OobMode == OutOfBoundsMode.AfterMouseReleased))
            desiredNDCPos = Vector2.Clamp(desiredNDCPos, centerPositionBounds.Min, centerPositionBounds.Max);

        var diff = desiredNDCPos - _toDrag.LastGlobalShape.Center;
        // the double halving here is because NDC is double size of local position
        // this is not remotely confusing
        if (parent != null)
            diff *= parent.LastGlobalShape.HalfSize;

        _toDrag.LocalShape.Center += diff * 0.5f;
    }

    public enum OutOfBoundsMode
    {
        /// <summary>
        /// There are no OOB checks done.
        /// </summary>
        Allowed = -1,

        /// <summary>
        /// The element gets moved back into its parent's bounds after the mouse is released.
        /// </summary>
        AfterMouseReleased = 0,

        /// <summary>
        /// The element will never be moved out of its parent's bounds.
        /// </summary>
        Always,
    }
}