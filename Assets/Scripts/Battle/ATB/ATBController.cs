using System;
using UnityEngine;
using UnityEngine.UI;

public class ATBController : MonoBehaviour
{
    [SerializeField] private Image atbBar;
    [SerializeField] [Range(0,1f)] private float actionCutoff;
    [SerializeField] private GameObject atbIconPrefab;

    private float barLength;
    private float waitTurnLength;
    private float waitActionLength;

    private void Awake()
    {
        barLength = atbBar.rectTransform.rect.width;
        waitTurnLength = barLength * actionCutoff;
        waitActionLength = barLength - waitTurnLength;
    }
}