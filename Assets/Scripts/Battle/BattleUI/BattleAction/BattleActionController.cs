using System.Collections.Generic;
using UnityEngine;

public class BattleActionController : MonoBehaviour
{
    [SerializeField] private SkillScriptableObject itemAction;
    [SerializeField] private GameObject battleActionPrefab;
    [SerializeField] private GameObject emptyActionPrefab;

    private readonly List<GameObject> buttonList = new();
    private readonly List<BattleAction> actionList = new();
    public void InitializeActions(List<SkillScriptableObject> skills)
    {
        int maxActions = PlayerPartyManager.Instance.MaxBaseActionCommands;
        var count = 0;
        foreach (SkillScriptableObject skill in skills)
        {
            if(skill == null)
                continue;
            
            CreateActionButton(skill);
            
            count++;
            if (count == maxActions)
                break;
        }
        
        for (int i = count; i < maxActions; i++)
        {
            CreateEmptyAction();
        }

        CreateActionButton(itemAction);
    }

    private void CreateActionButton(SkillScriptableObject skill)
    {
        GameObject actionItem = Instantiate(battleActionPrefab, transform);
        if (actionItem.TryGetComponent(out BattleAction battleAction))
        {
            battleAction.InitializeAction(skill);
            actionList.Add(battleAction);
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
        foreach (BattleAction action in actionList)
        {
            action.EnableAction();
        }
    }

    public void DisableActions()
    {
        foreach (BattleAction action in actionList)
        {
            action.DisableAction();
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
