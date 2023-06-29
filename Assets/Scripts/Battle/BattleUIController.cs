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

    public void SetEnemyBattleVisuals(List<BattleUnit> enemies)
    {
        foreach (BattleUnit enemy in enemies)
        {
            enemy.Initialize();
            turnGauge.AddUnit(enemy);
            battlefield.AddUnit(enemy);
        }
    }

    public void SetHeroBattleVisuals(List<BattleUnit> heroes)
    {
        foreach (BattleUnit hero in heroes)
        {
            hero.Initialize();
            turnGauge.AddUnit(hero);
        }
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