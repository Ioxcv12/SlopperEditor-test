namespace SlopperEditor.UndoSystem;

/// <summary>
/// Contains undo/redo information.
/// </summary>
public class UndoQueue
{
    UndoableAction?[] _undoStack;
    int _currentAction = -1;

    public UndoQueue(int capacity = 128)
    {
        _undoStack = new UndoableAction[int.Max(capacity, 2)];
    }

    /// <summary>
    /// Calls Do() on the action, and adds it to the queue.
    /// </summary>
    /// <param name="action">The action to do.</param>
    public void DoAction(UndoableAction action)
    {
        action.Do();

        _currentAction++;
        ClearUndoneActions(_currentAction);
        if (_currentAction >= _undoStack.Length)
        {
            _currentAction--;
            Array.Copy(_undoStack, 1, _undoStack, 0, _undoStack.Length - 1);
        }
        _undoStack[_currentAction] = action;
    }

    /// <summary>
    /// Undoes the last action on the queue.
    /// </summary>
    public void UndoAction()
    {
        if (_currentAction < 0)
            return;

        _undoStack[_currentAction]?.Undo();
        _currentAction--;
    }

    /// <summary>
    /// Redoes the last undone action on the queue.
    /// </summary>
    public void RedoAction()
    {
        int c = _currentAction + 1;
        if (c >= _undoStack.Length)
            return;

        var act = _undoStack[c];
        if (act != null)
        {
            act.Do();
            _currentAction++;
        }
    }

    void ClearUndoneActions(int start)
    {
        if ((uint)start >= _undoStack.Length)
            return;
        if (_undoStack[start] == null)
            return;

        _undoStack[start] = null;
        ClearUndoneActions(start + 1);
    }
}