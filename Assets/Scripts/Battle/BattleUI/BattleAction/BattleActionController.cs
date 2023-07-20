using System.Collections.Generic;
using UnityEngine;

public class BattleActionController : MonoBehaviour
{
    [SerializeField] private UnitActionScriptableObject itemAction;
    [SerializeField] private GameObject battleActionPrefab;
    [SerializeField] private GameObject emptyActionPrefab;

    private readonly List<GameObject> buttonList = new();
    private readonly Dictionary<UnitActionScriptableObject, BattleAction> actionList = new();
    private BattleUnit unit;
    
    private int actionListSize;

    private void OnEnable()
    {
        BattleEvents.OnTurnReady += EnableActions;
        BattleEvents.OnActionSelected += HideAllExcept;
        BattleEvents.OnActionComplete += DisableActions;
        BattleEvents.OnBattleEnd += DisableActions;
    }

    public void InitializeActions(BattleUnit battleUnit)
    {
        unit = battleUnit;
        actionListSize = PlayerUnitManager.Instance.MaxBaseActionCommands;
        var count = 0;
        foreach (UnitActionScriptableObject action in unit.Actions.ActionList)
        {
            if(action == null)
                continue;
            
            CreateActionButton(action);
            
            count++;
            if (count == actionListSize)
                break;
        }
        
        for (int i = count; i < actionListSize; i++)
        {
            CreateEmptyAction();
        }

        CreateActionButton(itemAction);
    }

    private void CreateActionButton(UnitActionScriptableObject unitAction)
    {
        GameObject actionItem = Instantiate(battleActionPrefab, transform);
        if (actionItem.TryGetComponent(out BattleAction battleAction))
        {
            battleAction.InitializeAction(unit, unitAction);
            actionList.Add(unitAction, battleAction);
        }
        
        buttonList.Add(actionItem);
    }

    private void CreateEmptyAction()
    {
        GameObject emptyItem = Instantiate(emptyActionPrefab, transform);
        buttonList.Add(emptyItem);
    }

    private void EnableActions(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        foreach (KeyValuePair<UnitActionScriptableObject, BattleAction> action in actionList)
        {
            action.Value.gameObject.SetActive(true);
            action.Value.EnableAction();
        }
    }

    private void HideAllExcept(BattleUnit battleUnit, UnitActionScriptableObject selectedAction)
    {
        if (battleUnit != unit)
            return;
        
        foreach (KeyValuePair<UnitActionScriptableObject, BattleAction> action in actionList)
        {
            if(action.Key != selectedAction)
            {
                action.Value.gameObject.SetActive(false);
            }
            else
            {
                action.Value.DisableAction();
            }
        }
    }
    
    private void DisableActions(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        foreach (KeyValuePair<UnitActionScriptableObject, BattleAction> action in actionList)
        {
            action.Value.DisableAction();
        }
    }
    
    public void ClearActionList()
    {
        foreach (GameObject actionItem in buttonList)
        {
            Utilities.Destroy(actionItem);
        }
    }

    private void OnDisable()
    {
        BattleEvents.OnTurnReady -= EnableActions;
        BattleEvents.OnActionSelected -= HideAllExcept;
        BattleEvents.OnActionComplete -= DisableActions;
        BattleEvents.OnBattleEnd -= DisableActions;
    }
}
