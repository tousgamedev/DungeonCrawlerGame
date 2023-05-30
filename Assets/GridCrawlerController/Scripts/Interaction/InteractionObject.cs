using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    private const int InteractableLayer = 3;

    public virtual void Initialize()
    {
        gameObject.layer = InteractableLayer;
    }
    
    public abstract void OnInteract();
    public abstract void AlertObservers();
    public abstract void RegisterObserver(IInteractableObserver observer);
    public abstract void DeregisterObserver(IInteractableObserver observer);

}
