namespace SlopperEditor.UndoSystem;

/// <summary>
/// Does and undoes a particular action.
/// </summary>
public abstract class UndoableAction
{
    /// <summary>
    /// The name of this action.
    /// </summary>
    public readonly string Name;

    protected UndoableAction(string name) => Name = name;

    /// <summary>
    /// Does the action.
    /// </summary>
    public abstract void Do();
    /// <summary>
    /// Undoes the action.
    /// </summary>
    public abstract void Undo();
}