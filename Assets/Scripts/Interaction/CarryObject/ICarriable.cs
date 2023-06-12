using UnityEngine;

public interface ICarriable
{
    public void OnPickup();
    public void OnPlace(Vector3 position);
    public void OnDrop(Vector3 position);
    public void OnThrow(Vector3 position, Vector3 direction);
}
