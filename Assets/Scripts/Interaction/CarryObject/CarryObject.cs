using UnityEngine;

public class CarryObject : MonoBehaviour, ICarriable
{
    public Sprite CarrySprite { get; private set; }
    [SerializeField] private Sprite carrySprite;
    
    public void OnPickup()
    {
        CarryObjectData.Instance.SetCarriedObject(this);
        gameObject.SetActive(false);
    }

    public void OnPlace()
    {
        CarryObjectData.Instance.ClearCarriedObject();
        // Raycast from player
        // If raycast hits surface, enable and place.
        // If raycast hits nothing, enable and drop if raycast is in bottom half of screen, through if on top
    }
    
    public void OnDrop()
    {
        CarryObjectData.Instance.ClearCarriedObject();
        // Raycast from player
        // If raycast hits surface, enable and place.
        // If raycast hits nothing, enable and drop if raycast is in bottom half of screen, through if on top
    }
    
    public void OnThrow()
    {
        CarryObjectData.Instance.ClearCarriedObject();
        // Raycast from player
        // If raycast hits surface, enable and place.
        // If raycast hits nothing, enable and drop if raycast is in bottom half of screen, through if on top
    }
}
