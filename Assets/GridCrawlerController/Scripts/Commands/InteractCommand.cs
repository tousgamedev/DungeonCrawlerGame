using UnityEditor;
using UnityEngine;

public class InteractCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;

    private readonly LayerMask interactionLayer = 1 << 3;
    public InteractCommand(GridCrawlerController controller)
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
        var target = hit.collider.gameObject.GetComponent<InteractionObject>();
        if (target != null)
        {
            target.OnInteract();
        }
    }
}