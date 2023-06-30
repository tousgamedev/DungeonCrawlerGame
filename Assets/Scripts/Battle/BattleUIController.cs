using System.Collections.Generic;
using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    [SerializeField] private TurnGaugeController turnGauge;
    [SerializeField] private BattlefieldController battlefield;

    public void OnBattleUpdate()
    {
        turnGauge.OnBattleUpdate();
        battlefield.OnBattleUpdate();
    }

    public void SetEnemyBattleVisuals(List<BattleUnit> enemies)
    {
        foreach (BattleUnit enemy in enemies)
        {
            turnGauge.AddUnit(enemy);
            battlefield.AddUnit(enemy);
        }
    }

    public void SetHeroBattleVisuals(BattleUnit hero)
    {
        turnGauge.AddUnit(hero);
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