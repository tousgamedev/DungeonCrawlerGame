using UnityEngine;

public class InteractCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;

    public InteractCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }

    public void Execute()
    {
        Camera main = Camera.main;
        if (main == null)
            return;

        // TODO: Make this input agnostic
        Vector3 direction = main.ScreenPointToRay(Input.mousePosition).direction;
        HandleInteraction(main.transform.position, direction);
    }

    private void HandleInteraction(Vector3 position, Vector3 direction)
    {
        if (TargetOnInteractableLayer(position, direction, out RaycastHit hit))
        {
            GameObject target = hit.collider.gameObject;
            if (target.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract();
            }
            else if (CarryObjectData.Instance.CarriedObject == null)
            {
                PickUpObject(target);
            }
        }
        else if (CanDropCarriedItem())
        {
            ReleaseObject(position, direction);
        }
    }

    private bool TargetOnInteractableLayer(Vector3 origin, Vector3 direction, out RaycastHit hit)
    {
        return Physics.Raycast(origin, direction, out hit, InputManager.InteractionRange,
            Layers.InteractableMask);
    }

    private void PickUpObject(GameObject target)
    {
        if (!target.TryGetComponent(out ICarriable carriable))
            return;

        carriable.OnPickup();
        target.transform.SetParent(stateMachine.gameObject.transform);
        target.transform.localPosition = Vector3.zero;
    }

    private bool CanDropCarriedItem()
    {
        return CarryObjectData.Instance.CarriedObject != null && stateMachine.IsInIdleState;
    }

    private void ReleaseObject(Vector3 cameraPosition, Vector3 direction)
    {
        if (CanPlaceAtLocation(cameraPosition, direction, out Vector3 position))
        {
            CarryObjectData.Instance.CarriedObject.OnPlace(position);
            return;
        }

        Transform controller = stateMachine.transform;
        Vector3 dropPosition = controller.position + controller.forward * CarryObjectData.ObjectDropOffset;

        // TODO: Make input agnostic
        if (Input.mousePosition.y > Screen.height * .5f)
        {
            CarryObjectData.Instance.CarriedObject.OnThrow(dropPosition, stateMachine.transform.forward);
        }
        else
        {
            CarryObjectData.Instance.CarriedObject.OnDrop(dropPosition);
        }
    }

    private static bool CanPlaceAtLocation(Vector3 cameraPosition, Vector3 direction, out Vector3 position)
    {
        position = Vector3.negativeInfinity;
        if (!Physics.Raycast(cameraPosition, direction, out RaycastHit hit, InputManager.InteractionRange, Layers.IgnorePlayerMask) ||
            !Utilities.IsNormalAlignedWithUp(hit.normal, CarryObjectData.MaxPlacementAngle))
            return false;

        position = hit.point;
        return true;
    }
}