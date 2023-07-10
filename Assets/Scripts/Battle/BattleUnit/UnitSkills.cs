using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkills
{
    public List<SkillScriptableObject> SkillList { get; private set; }

    public UnitSkills(UnitBaseScriptableObject unit)
    {
        SkillList = unit.SkillList;
        
        if (SkillList == null || SkillList.Count == 0)
        {
            LogHelper.Report($"Check skills assigned to {unit.Name}", LogType.Warning, LogGroup.Battle);
        }
    }
}
