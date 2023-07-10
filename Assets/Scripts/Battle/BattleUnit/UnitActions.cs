using System.Collections.Generic;
using UnityEngine;

public class UnitActions
{
    public List<UnitActionScriptableObject> ActionList { get; private set; }

    public UnitActions(UnitBaseScriptableObject unit)
    {
        ActionList = unit.ActionList;
        
        if (ActionList == null || ActionList.Count == 0)
        {
            LogHelper.Report($"Check actions assigned to {unit.Name}", LogType.Warning, LogGroup.Battle);
        }
    }
    
    public UnitActionScriptableObject SelectRandomAction()
    {
        int index = Random.Range(0, ActionList.Count);
        return ActionList[index];
    }
}
