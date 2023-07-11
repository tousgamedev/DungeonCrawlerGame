using System.Collections.Generic;
using UnityEngine;

public class BattleActionController : MonoBehaviour
{
    [SerializeField] private UnitActionScriptableObject itemAction;
    [SerializeField] private GameObject battleActionPrefab;
    [SerializeField] private GameObject emptyActionPrefab;

    private readonly List<GameObject> buttonList = new();
    private readonly Dictionary<UnitActionScriptableObject, BattleAction> actionList = new();

    private int actionListSize;
    
    public void InitializeActions(List<UnitActionScriptableObject> skills)
    {
        actionListSize = PlayerPartyManager.Instance.MaxBaseActionCommands;
        var count = 0;
        foreach (UnitActionScriptableObject skill in skills)
        {
            if(skill == null)
                continue;
            
            CreateActionButton(skill);
            
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
            battleAction.InitializeAction(unitAction);
            actionList.Add(unitAction, battleAction);
        }
        
        buttonList.Add(actionItem);
    }

    private void CreateEmptyAction()
    {
        GameObject emptyItem = Instantiate(emptyActionPrefab, transform);
        buttonList.Add(emptyItem);
    }

    public void EnableActions()
    {
        foreach (KeyValuePair<UnitActionScriptableObject, BattleAction> action in actionList)
        {
            action.Value.gameObject.SetActive(true);
            action.Value.EnableAction();
        }
    }

    public void HideAllExcept(UnitActionScriptableObject selectedAction)
    {
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
    
    public void DisableActions()
    {
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
}
