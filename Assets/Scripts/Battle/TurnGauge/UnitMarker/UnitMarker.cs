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

    public void AssignCharacterIcon(Sprite sprite)
    {
        characterIcon.sprite = sprite;
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