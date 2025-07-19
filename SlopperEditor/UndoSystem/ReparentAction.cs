using SlopperEngine.SceneObjects;

namespace SlopperEditor.UndoSystem;

/// <summary>
/// Parents, Reparents, or Removes a SceneObject.
/// </summary>
public class ReparentAction : UndoableAction
{
    /// <summary>
    /// The object that had its parent changed.
    /// </summary>
    public readonly SceneObject ReparentedObject;
    /// <summary>
    /// The container the object originated from. Null if the object was not previously in the scene.
    /// </summary>
    public readonly SceneObject.ChildContainer? OriginalContainer;
    /// <summary>
    /// The container the object was added to. Null if the object was removed.
    /// </summary>
    public readonly SceneObject.ChildContainer? TargetContainer;

    public ReparentAction(SceneObject toReparent, SceneObject.ChildContainer? target) :
        base(toReparent.ParentContainer == null ? "Add SceneObject " + toReparent.GetType().Name :
            target == null ? "Remove SceneObject " + toReparent.GetType().Name : 
            $"Reparent SceneObject " + toReparent.GetType().Name)
    {
        ReparentedObject = toReparent;
        TargetContainer = target;
        OriginalContainer = toReparent.ParentContainer;
    }

    public override void Do()
    {
        if (TargetContainer != null)
            TargetContainer.TryAdd(ReparentedObject);
        else ReparentedObject.Remove();
    }

    public override void Undo()
    {
        if (OriginalContainer != null)
            OriginalContainer.TryAdd(ReparentedObject);
        else ReparentedObject.Remove();
    }
}