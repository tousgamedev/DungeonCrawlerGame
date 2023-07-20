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
        enemyImage.enabled = false;
    }

    private void OnEnable()
    {
        BattleEvents.OnEnemyUnitDeath += HideEnemy;
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

    public void HideEnemy(BattleUnit unit)
    {
        if (unit == null || unit != battleUnit)
            return;
        
        enemyImage.enabled = false;
    }

    public void SelectEnemy()
    {
        BattleEvents.OnTargetSelection?.Invoke(battleUnit);
    }

    private void OnDisable()
    {
        BattleEvents.OnEnemyUnitDeath -= HideEnemy;
    }
}
