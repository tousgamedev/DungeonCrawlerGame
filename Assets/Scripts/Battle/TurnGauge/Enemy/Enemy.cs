using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Image enemyImage;

    private void Awake()
    {
        Utilities.AssignComponentOrDestroyObject(gameObject, out enemyImage);
    }

    public void SetEnemySprite(Sprite sprite)
    {
        enemyImage.sprite = sprite;
    }
}
