public class BattleTickState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        CheckBattleEndConditions();
        ProcessPartyTicks(deltaTime);
        ProcessEnemyTicks(deltaTime);
        battleManager.UpdateBattleUI();
        CheckActionQueue();
    }

    private void CheckBattleEndConditions()
    {
        if (battleManager.IsPlayerPartyDefeated)
        {
            battleManager.SwitchToStateDefeat();
        }
        else if (battleManager.IsEnemyPartyDefeated)
        {
            battleManager.SwitchToStateVictory();
        }
    }
    
    private void ProcessPartyTicks(float deltaTime)
    {
        if (battleManager.IsPartyMemberTurnReady)
        {
            battleManager.PopPartyTurnReadyQueue();
            battleManager.SwitchToStateActionSelection();
            return;
        }

        foreach (BattleUnit unit in PlayerPartyManager.Instance.PlayerParty)
        {
            unit.UpdateTicks(deltaTime);
            if (unit.IsTurnReady && !unit.IsActionSelected)
            {
                battleManager.QueueTurnReadyPartyMember(unit);
            }

            if (unit.IsActionReady)
            {
                battleManager.QueueActionReadyUnit(unit);
            }
        }
    }
    
    private void ProcessEnemyTicks(float deltaTime)
    {
        foreach (BattleUnit unit in battleManager.EnemyParty)
        {
            unit.UpdateTicks(deltaTime);
            if (unit.IsTurnReady && !unit.IsActionSelected)
            {
                // TODO: Create better enemy skill/target selection
                BattleUnit target = PlayerPartyManager.Instance.SelectRandomPartyMember();
                UnitActionScriptableObject unitAction = unit.Actions.SelectRandomAction();
                unit.PrepareAction(unitAction, target, battleManager.SwitchToStateTick);
            }

            if (unit.IsActionReady)
            {
                battleManager.QueueActionReadyUnit(unit);
            }
        }
    }

    private void CheckActionQueue()
    {
        if (battleManager.IsActionReady)
        {
            battleManager.SwitchToStateExecuteAction();
        }
    }
    
    public override void OnStateExit()
    {
    }
}
