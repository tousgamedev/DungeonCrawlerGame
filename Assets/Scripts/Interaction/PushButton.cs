using System.Collections.Generic;
using UnityEngine;

public class PushButton : MonoBehaviour, IInteractable
{
    public HashSet<IObserver> Observers { get; set; } = new();

    private void Awake()
    {
        gameObject.layer = Layers.InteractableLayer;
    }

    public void OnInteract()
    {
        AlertObservers();
    }

    public void AlertObservers()
    {
        foreach (IObserver observer in Observers)
        {
            observer.Alert();
        }
    }

    public void RegisterObserver(IObserver observer)
    {
        Observers.Add(observer);
    }

    public void DeregisterObserver(IObserver observer)
    {
        Observers.Remove(observer);
    }
}
