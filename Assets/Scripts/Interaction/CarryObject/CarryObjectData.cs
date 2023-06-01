using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CarryObjectData : MonoBehaviour
{
    public static CarryObjectData Instance { get; private set; }
    public CarryObject CarriedObject { get; private set; }
    public Action OnCarriedObjectDataUpdated;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#endif
            Destroy(gameObject);
        }
    }
    
    public void SetCarriedObject(CarryObject carryObject)
    {
        CarriedObject = carryObject;
        OnCarriedObjectDataUpdated.Invoke();
    }

    public void ClearCarriedObject()
    {
        CarriedObject = null;
        OnCarriedObjectDataUpdated.Invoke();
    }
}