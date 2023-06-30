using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    private Image enemyImage;

    private void Awake()
    {
        Utilities.AssignComponentOrDestroyObject(gameObject, out enemyImage);
        HideEnemy();
    }

    public void SetEnemySprite(Sprite sprite)
    {
        enemyImage.sprite = sprite;
    }

    public void ShowEnemy()
    {
        enemyImage.enabled = true;
    }

    public void HideEnemy()
    {
        enemyImage.enabled = false;
    }
}
