using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class InteractCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;

    public InteractCommand(DungeonCrawlerController controller)
    {
        crawlerController = controller;
    }

    public void Execute()
    {
        Camera main = Camera.main;
        if (main == null)
            return;

        // TODO: Make this input agnostic
        Vector3 direction = main.ScreenPointToRay(Input.mousePosition).direction;
        if (CarryObjectData.Instance.CarriedObject == null)
        {
            PickUpObject(main.transform.position, direction);
        }
        else
        {
            ReleaseObject(main.transform.position, direction);
        }
    }

    private void PickUpObject(Vector3 cameraPosition, Vector3 direction)
    {
        if (!Physics.Raycast(cameraPosition, direction, out RaycastHit hit, InputManager.InteractionRange,
                Layers.InteractableMask))
            return;

        GameObject target = hit.collider.gameObject;
        if (!target.TryGetComponent(out IInteractable interactable))
            return;

        interactable.OnInteract();
        target.transform.SetParent(crawlerController.gameObject.transform);
        target.transform.localPosition = Vector3.zero;
    }

    private void ReleaseObject(Vector3 cameraPosition, Vector3 direction)
    {
        if (CanPlaceAtLocation(cameraPosition, direction, out Vector3 position))
        {
            CarryObjectData.Instance.CarriedObject.OnPlace(position);
            return;
        }

        Transform controller = crawlerController.transform;
        Vector3 dropPosition = controller.position + controller.forward * CarryObjectData.ObjectDropOffset;

        // TODO: Make input agnostic
        if (Input.mousePosition.y > Screen.height * .5f)
        {
            CarryObjectData.Instance.CarriedObject.OnThrow(dropPosition, crawlerController.transform.forward);
        }
        else
        {
            CarryObjectData.Instance.CarriedObject.OnDrop(dropPosition);
        }
    }

    private static bool CanPlaceAtLocation(Vector3 cameraPosition, Vector3 direction, out Vector3 position)
    {
        position = Vector3.negativeInfinity;
        if (!Physics.Raycast(cameraPosition, direction, out RaycastHit hit, InputManager.InteractionRange,
                Layers.IgnorePlayerMask) || !Utilities.IsNormalAlignedWithUp(hit.normal, CarryObjectData.MaxPlacementAngle))
            return false;
        position = hit.point;
        return true;
    }
}