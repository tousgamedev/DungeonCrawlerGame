using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UnitMarker : MonoBehaviour
{
    public RectTransform RectTransform { get; private set; }
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image pointer;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        HideMarker();
    }

    public void Initialize(float startingPositionX, Sprite turnBarIcon)
    {
        RectTransform.position = new Vector3(startingPositionX, 0, 0);
        characterIcon.sprite = turnBarIcon;
        ShowMarker();
    }

    public void ShowMarker()
    {
        characterIcon.enabled = true;
        pointer.enabled = true;
    }

    public void HideMarker()
    {
        characterIcon.enabled = false;
        pointer.enabled = false;
    }
}