using System.Collections.Generic;
using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    [SerializeField] private TurnGaugeController turnGauge;
    [SerializeField] private BattlefieldController battlefield;

    public void OnBattleUpdate(float deltaTime)
    {
        turnGauge.OnBattleUpdate(deltaTime);
        battlefield.OnBattleUpdate(deltaTime);
    }

    public void SetBattleVisuals(List<BattleUnit> enemies)
    {
        turnGauge.AddUnits(enemies);
        battlefield.AddEnemies(enemies);
    }

    public void Pause()
    {
        turnGauge.ShowPauseIcon();
    }

    public void Unpause()
    {
        turnGauge.ShowPauseIcon(false);
    }
}