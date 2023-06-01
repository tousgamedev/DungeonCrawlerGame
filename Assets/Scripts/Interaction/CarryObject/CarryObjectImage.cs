using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CarryObjectImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 offset;


    private RectTransform canvasRectTransform;

    private void Start()
    {
        if (image == null && !TryGetComponent(out image))
        {
            Debug.LogError("CarriedObjectImage field 'image' is unassigned!");
        }

        if (canvas == null || !canvas.TryGetComponent(out canvasRectTransform))
        {
            Debug.LogError("CarriedObjectImage field 'canvas' is unassigned!");
        }

        if (image.sprite == null)
        {
            image.enabled = false;
        }
    }

    private void Update()
    {
        MoveImageToCursor();
    }

    private void MoveImageToCursor()
    {
        if (image.sprite == null)
            return;
        
        Vector2 cursorPosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, cursorPosition, null,
            out Vector2 localCursorPos);
        Vector2 finalPosition = localCursorPos + offset;

        image.rectTransform.localPosition = finalPosition;
    }

    public void SetImageSprite(Sprite sprite)
    {
        image.sprite = sprite;
        MoveImageToCursor();
        image.enabled = true;
    }

    public void ClearImageSprite()
    {
        image.enabled = false;
        image.sprite = null;
    }
}