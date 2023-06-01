using UnityEngine;

public class InteractCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;

    private readonly LayerMask interactionLayer = 1 << 3;
    public InteractCommand(DungeonCrawlerController controller)
    {
        crawlerController = controller;
    }

    public void Execute()
    {
        Camera main = Camera.main;
        if (main == null) return;
        
        Vector3 targetPosition = main.ScreenPointToRay(Input.mousePosition).direction;
        if (!Physics.Raycast(main.transform.position, targetPosition, out RaycastHit hit,
                InputManager.Instance.InteractionRange, interactionLayer)) return;
        
        Debug.DrawRay(main.transform.position, targetPosition, Color.yellow, 2f);
        var target = hit.collider.gameObject.GetComponent<IInteractable>();
        target?.OnInteract();
    }
}