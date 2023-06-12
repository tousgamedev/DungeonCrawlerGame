using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CarryObject : MonoBehaviour, IInteractable, ICarriable
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

        if (Utilities.FindGround(carryTransform.position, out RaycastHit hit, 1f, Layers.IgnoreInteractableMask))
        {
            SetOnSurface(hit.point);
        }
    }

    public void SetInteractionLayer()
    {
        gameObject.layer = Layers.Interactable;
    }

    public void OnInteract()
    {
        if (CarryObjectData.Instance.CarriedObject != null)
            return;

        OnPickup();
    }

    public void OnPickup()
    {
        CarryObjectData.Instance.SetCarriedObject(this);
        carryRigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void OnPlace(Vector3 position)
    {
        CarryObjectData.Instance.ClearCarriedObject();
        SetOnSurface(position);
        carryTransform.SetParent(null);
        gameObject.SetActive(true);
    }

    public void OnDrop(Vector3 position)
    {
        CarryObjectData.Instance.ClearCarriedObject();
        carryTransform.SetParent(null);
        carryTransform.position = position;
        gameObject.SetActive(true);
    }

    public void OnThrow(Vector3 position, Vector3 direction)
    {
        CarryObjectData.Instance.ClearCarriedObject();
        carryTransform.SetParent(null);
        carryTransform.position = position;
        gameObject.SetActive(true);
        carryRigidbody.AddForce(direction * CarryObjectData.ThrowForce, ForceMode.Impulse);
    }

    private void SetOnSurface(Vector3 position)
    {
        float offset = carryCollider.size.y * .5f;
        position.y += offset;
        carryTransform.position = position;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layers.Player)
            return;

        Vector3 collisionForce = other.impulse / Time.fixedDeltaTime;
        carryRigidbody.AddForce(-collisionForce, ForceMode.Impulse);
    }
}