using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator, IDisposable
{
    private VisualElement _target;

    public event Action DroppedAction;

    public DragAndDropManipulator(VisualElement target)
    {
        _target = target;
        _target.AddManipulator(this);
    }

    public void Dispose()
    {
        _target.RemoveManipulator(this);
    }

    protected override void RegisterCallbacksOnTarget()
    {
        // Register a callback when the user presses the pointer down.
        _target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        // Register callbacks for various stages in the drag process.
        _target.RegisterCallback<DragEnterEvent>(OnDragEnter);
        _target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
        _target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _target.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        // Unregister all callbacks that you registered in RegisterCallbacksOnTarget().
        _target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        _target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
        _target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
        _target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
    }

    private void OnDragPerform(DragPerformEvent _)
    {
        if(DragAndDrop.paths.Length > 0)
        {
            Debug.Log($"Droped {String.Join(", ", DragAndDrop.paths)}");
            DroppedAction?.Invoke();
        }
    }

    private void OnDragUpdate(DragUpdatedEvent _)
    {
        if (DragAndDrop.paths.Length > 0)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragLeave(DragLeaveEvent _)
    {
    }

    private void OnDragEnter(DragEnterEvent _)
    {
    }

    private void OnPointerDown(PointerDownEvent _)
    {
    }
}
