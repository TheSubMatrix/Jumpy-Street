using UnityEngine;
using UnityEngine.Events;

public interface IPickup
{
    UnityEvent OnPickup { get; }
}
