using System;
using System.Collections.Generic;

namespace SlopperEditor.UndoSystem;

/// <summary>
/// Contains undo/redo information.
/// </summary>
public class UndoQueue
{
    /// <summary>
    /// Mostly useful for equality check when adding to the last action, for example, when setting the same float several times in a row.
    /// </summary>
    public UndoableAction? LastAction => (uint)_currentAction < _undoStack.Length ? _undoStack[_currentAction] : null;

    /// <summary>
    /// Gets called when an UndoableAction gets added to the stack.
    /// </summary>
    public event Action? OnQueueChanged;

    UndoableAction?[] _undoStack;
    int _currentAction = -1;

    public UndoQueue(int capacity = 128)
    {
        _undoStack = new UndoableAction[int.Max(capacity, 2)];
    }

    /// <summary>
    /// Gets all actions in the stack, from earliest to latest.
    /// </summary>
    public IEnumerable<(UndoableAction action, bool actionDone)> GetActions()
    {
        for (int i = 0; i < _undoStack.Length; i++)
        {
            var act = _undoStack[i];
            if (act == null) break;
            yield return (act, i <= _currentAction);
        }
    } 

    /// <summary>
    /// Adds a completed UndoableAction to the queue.
    /// </summary>
    /// <param name="action">The action to add to the queue. Do() or an equivalent action should already have been called.</param>
    public void DoAction(UndoableAction action)
    {
        _currentAction++;
        ClearUndoneActions(_currentAction);
        if (_currentAction >= _undoStack.Length)
        {
            _currentAction--;
            Array.Copy(_undoStack, 1, _undoStack, 0, _undoStack.Length - 1);
        }
        _undoStack[_currentAction] = action;
        OnQueueChanged?.Invoke();
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
        OnQueueChanged?.Invoke();
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
        OnQueueChanged?.Invoke();
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