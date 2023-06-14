using System.Collections.Generic;
using UnityEngine;

public class CarryObjectData : MonoBehaviour, ISubject
{
    public static CarryObjectData Instance { get; private set; }
    public static int MaxPlacementAngle => Instance.maxPlacementAngle;
    public static float ObjectDropOffset => Instance.objectDropOffset;
    public static float ThrowForce => Instance.throwForce;

    public HashSet<IObserver> Observers { get; set; } = new();
    public CarryObject CarriedObject { get; private set; }

    [SerializeField] private int maxPlacementAngle = 5;
    [SerializeField] private float objectDropOffset = 1.25f;
    [SerializeField] private float throwForce = 20;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterObserver(IObserver observer)
    {
        Observers.Add(observer);
    }
    
    public void SetCarriedObject(CarryObject carryObject)
    {
        CarriedObject = carryObject;
        AlertObservers();
    }

    public void AlertObservers()
    {
        foreach (IObserver observer in Observers)
        {
            observer.Alert();
        }
    }
    
    public void ClearCarriedObject()
    {
        CarriedObject = null;
        AlertObservers();
    }

    public void DeregisterObserver(IObserver observer)
    {
        Observers.Remove(observer);
    }
}