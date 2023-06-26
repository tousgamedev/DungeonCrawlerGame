using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private BattleUIController uiController;
    
    private BattleStateBase currentState;
    private readonly OutOfBattleState stateOutOfBattle = new();
    private BattleStartState stateStart = new();
    private BattleTickState stateTick = new();
    private BattleAwaitingInputState stateAwaitingInput = new();
    private BattlePerformingActionState statePerformingAction = new();
    private BattleVictoryState stateVictory = new();
    private BattleDefeatState stateDefeat = new();

    private readonly List<Character> playerParty = new();
    private readonly List<Character> enemyParty = new();

    private void Awake()
    {
        if (Instance != null || Instance != this)
        {
            Utilities.Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentState = stateOutOfBattle;
    }

    

    public void CreateEnemies(EncounterGroupScriptableObject group)
    {
        foreach (EnemyScriptableObject enemy in group.Enemies)
        {
            var newEnemy = new Character(enemy); 
            enemyParty.Add(newEnemy);
        }
        
        
    }
}