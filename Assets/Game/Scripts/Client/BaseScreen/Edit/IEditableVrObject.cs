using System;

public interface IEditableVrObject
{
    event Action OnPointerDown;
    event Action OnDrag;
    event Action OnPointerUp;
}