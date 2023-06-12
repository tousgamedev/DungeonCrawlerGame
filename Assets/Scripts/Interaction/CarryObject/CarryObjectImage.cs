using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CarryObjectImage : MonoBehaviour, IObserver
{
    [SerializeField] private Image image;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 offset;

    private RectTransform canvasRectTransform;

    private void Awake()
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

    private void OnEnable()
    {
        RegisterObserver();
    }

    private void Update()
    {
        MoveImageToCursor();
    }

    private void MoveImageToCursor()
    {
        if (image.sprite == null)
            return;
        
        // TODO: Change this to be input agnostic
        Vector2 cursorPosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, cursorPosition, null,
            out Vector2 localCursorPos);
        Vector2 finalPosition = localCursorPos + offset;

        image.rectTransform.localPosition = finalPosition;
    }

    private void SetImageSprite()
    {
        image.sprite = CarryObjectData.Instance.CarriedObject.CarrySprite;
        MoveImageToCursor();
        image.enabled = true;
    }

    private void ClearImageSprite()
    {
        image.enabled = false;
        image.sprite = null;
    }

    public void Alert()
    {
        if (CarryObjectData.Instance.CarriedObject != null)
        {
            SetImageSprite();
        }
        else
        {
            ClearImageSprite();
        }
    }

    public void RegisterObserver()
    {
        CarryObjectData.Instance.RegisterObserver(this);
    }

    public void DeregisterObserver()
    {
        CarryObjectData.Instance.DeregisterObserver(this);
    }

    private void OnDisable()
    {
        DeregisterObserver();
    }

    public string GetName() => gameObject.name;
}