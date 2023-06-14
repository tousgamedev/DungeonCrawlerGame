using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CarryObject : MonoBehaviour, ICarriable
{
    public Sprite CarrySprite => carrySprite;
    
    [SerializeField] private Sprite carrySprite;

    private BoxCollider carryCollider;
    private Rigidbody carryRigidbody;
    private Transform carryTransform;

    private void Awake()
    {
        SetInteractionLayer();
        Utilities.AssignComponentOrDestroyObject(gameObject, out carryCollider);
        Utilities.AssignComponentOrDestroyObject(gameObject, out carryRigidbody);
        Utilities.AssignComponentOrDestroyObject(gameObject, out carryTransform);

        SetupRigidbody();
        
        if (Utilities.FindGround(carryTransform.position, out RaycastHit hit, 1f, Layers.IgnoreInteractableMask))
        {
            GetSurfacePosition(hit.point);
        }
    }

    public void SetInteractionLayer()
    {
        gameObject.layer = Layers.Interactable;
    }

    private void SetupRigidbody()
    {
        carryRigidbody.freezeRotation = true;
    }
    
    public void OnPickup()
    {
        if (CarryObjectData.Instance.CarriedObject != null)
            return;
        
        CarryObjectData.Instance.SetCarriedObject(this);
        carryRigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void OnPlace(Vector3 position)
    {
        Vector3 setPosition = GetSurfacePosition(position);
        ReleaseCarriedObject(setPosition);
    }

    public void OnDrop(Vector3 position)
    {
        ReleaseCarriedObject(position);
    }

    public void OnThrow(Vector3 position, Vector3 direction)
    {
        ReleaseCarriedObject(position);
        carryRigidbody.AddForce(direction * CarryObjectData.ThrowForce, ForceMode.Impulse);
    }

    private Vector3 GetSurfacePosition(Vector3 position)
    {
        float offset = carryCollider.size.y * .5f;
        position.y += offset;
        return position;
    }

    private void ReleaseCarriedObject(Vector3 position)
    {
        CarryObjectData.Instance.ClearCarriedObject();
        carryTransform.SetParent(null);
        carryTransform.position = position;
        gameObject.SetActive(true);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layers.Player)
            return;

        Vector3 collisionForce = other.impulse / Time.fixedDeltaTime;
        carryRigidbody.AddForce(-collisionForce, ForceMode.Impulse);
    }
}