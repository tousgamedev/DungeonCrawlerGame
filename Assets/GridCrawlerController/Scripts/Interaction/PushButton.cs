using System;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : InteractionObject
{
    private readonly HashSet<IInteractableObserver> observers = new();

    private void Awake()
    {
        Initialize();
    }

    public override void OnInteract()
    {
        AlertObservers();
    }

    public override void AlertObservers()
    {
        foreach (IInteractableObserver observer in observers)
        {
            observer.Alert();
        }
    }

    public override void RegisterObserver(IInteractableObserver observer)
    {
        observers.Add(observer);
    }

    public override void DeregisterObserver(IInteractableObserver observer)
    {
        observers.Remove(observer);
    }
}
