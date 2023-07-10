using UnityEngine;
using UnityEngine.UI;

public class BattlefieldEnemy : MonoBehaviour
{
    private Image enemyImage;
    // TODO: Get rid of this for world space implementation
    private Button testButton;
    private BattleUnit battleUnit;
    
    private void Awake()
    {
        Utilities.AssignComponentOrDestroyObject(gameObject, out enemyImage);
        // TODO: Get rid of this for world space implementation
        Utilities.AssignComponentOrDestroyObject(gameObject, out testButton);
        HideEnemy();
    }

    public void InitializeEnemy(BattleUnit unit)
    {
        battleUnit = unit;
        enemyImage.sprite = battleUnit.BattleIcon;
    }

    public void ShowEnemy()
    {
        enemyImage.enabled = true;
    }

    public void HideEnemy()
    {
        enemyImage.enabled = false;
    }

    public void SelectEnemy()
    {
        BattleManager.Instance.SelectEnemy(battleUnit);
    }
}
