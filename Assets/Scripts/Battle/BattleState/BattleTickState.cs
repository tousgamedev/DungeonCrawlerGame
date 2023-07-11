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
        battleManager.UIController.OnBattleUpdate();
    }

    private void CheckBattleEndConditions()
    {
        if (battleManager.IsPlayerPartyDefeated)
        {
            battleManager.SwitchToState(BattleState.Defeat);
        }
        else if (battleManager.IsEnemyPartyDefeated)
        {
            battleManager.SwitchToState(BattleState.Victory);
        }
    }
    
    private void ProcessPartyTicks(float deltaTime)
    {
        foreach (BattleUnit unit in PlayerPartyManager.Instance.PlayerParty)
        {
            unit.UpdateTicks(deltaTime);
        }
    }
    
    private void ProcessEnemyTicks(float deltaTime)
    {
        foreach (BattleUnit unit in battleManager.EnemyParty)
        {
            unit.UpdateTicks(deltaTime);
            if (unit.TickHandler.IsTurnReady && !unit.Actions.IsActionSelected)
            {
                // TODO: Create better enemy skill/target selection
                BattleUnit target = PlayerPartyManager.Instance.SelectRandomPartyMember();
                UnitActionScriptableObject unitAction = unit.Actions.SelectRandomAction();
                unit.PrepareAction(unitAction, target);
            }
        }
    }
    
    public override void OnStateExit()
    {
    }
}
